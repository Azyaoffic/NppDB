using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Kbg.NppPluginNET.PluginInfrastructure;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using NppDB.Comm;
using NppDB.Core;
using NppDB.MSAccess;
using NppDB.PostgreSQL;
using NppDB.Properties;
using Formatting = Newtonsoft.Json.Formatting;
using PromptPreferences = NppDB.Core.PromptPreferences;

namespace NppDB
{
    public static class StringExtension
    {
        public static string RemoveSuffix(this string value, string suffix)
        {
            return value.EndsWith(suffix) ? value.Substring(0, value.Length - suffix.Length) : value;
        }
    }

    public class NppDbPlugin : PluginBase, INppDbCommandHost
    {
        private const string PLUGIN_NAME = "NppDB";
        private string _nppDbPluginDir;
        private string _nppDbConfigDir;
        private string _cfgPath;
        private string _dbConnsPath;
        private string _languageConfigPath;
        private string _translationsConfigPath;
        private string _promptLibraryPath;
        private string _tutorialPath;
        private static string _staticPromptLibraryPath;
        private static string _staticLanguageCodesPath;
        private static string _staticPromptPreferencesPath;
        private static string _staticBehaviorSettingsPath;
        private bool _isPromptLibraryDisabled = false;
        private FrmDatabaseExplore _frmDbExplorer;
        private static FrmDatabaseExplore _staticFrmDbExplorer;
        private int _cmdFrmDbExplorerIdx = -1;
        private readonly Bitmap _imgDbManager = Resources.DBPPManage16;
        private readonly Bitmap _imgExecute = Resources.IconExecute;
        private readonly Bitmap _imgAnalyze = Resources.IconAnalyze;
        private readonly Bitmap _imgPromptLibrary = Resources.IconPromptLibrary;
        private Icon _tbIcon;
        private readonly Func<IScintillaGateway> _getCurrentEditor = GetGatewayFactory();
        private static readonly Func<IScintillaGateway> _getStaticCurrentEditor = GetGatewayFactory();
        private readonly List<string> _editorErrors = new List<string>();
        private readonly List<Tuple<string, ParserMessage>> _structuredErrorDetails = new List<Tuple<string, ParserMessage>>();
        private Dictionary<ParserMessageType, string> _warningMessages = new Dictionary<ParserMessageType, string>();
        private Dictionary<ParserMessageType, string> _generalTranslations = new Dictionary<ParserMessageType, string>();
        private Control _currentCtr;
        private const int DEFAULT_SQL_RESULT_HEIGHT = 200;
        private const int MIN_SQL_RESULT_HEIGHT = 80;
        private const int MIN_EDITOR_HEIGHT = 50;
        private int _sqlResultHeight = DEFAULT_SQL_RESULT_HEIGHT;
        private bool _isUpdatingResultPos;
        private bool _uiScaleInitialized;
        private float _uiScale = 1f;
        private ParserResult _lastAnalysisResult;
        private string _lastAnalyzedText;
        private SqlDialect _lastUsedDialect;
        private IScintillaGateway _lastEditor;
        private readonly DbPluginMenuBuilder _menuBuilder = new DbPluginMenuBuilder(PLUGIN_NAME);
        
        private bool _showRecreationNotificationsToUsers = false; // TODO: might be good to turn off to confuse users less?


        static NppDbPlugin()
        {
            AppDomain.CurrentDomain.AssemblyResolve += FindAssembly;
        }
        private static Assembly FindAssembly(object sender, ResolveEventArgs args)
        {
            var logFilePath = Path.Combine(Path.GetTempPath(), "NppDB_ResolveTrace.log");
            try
            {
                File.AppendAllText(logFilePath,
                    $"{DateTime.Now}: Trying to resolve '{args.Name}' requested by '{args.RequestingAssembly?.FullName ?? "Unknown"}'.\r\n");
                var requestedAssemblyName = new AssemblyName(args.Name);
                var pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var assemblyPath = Path.Combine(pluginDirectory, requestedAssemblyName.Name + ".dll");
                File.AppendAllText(logFilePath, $"{DateTime.Now}: Looking for '{assemblyPath}'.\r\n");
                if (File.Exists(assemblyPath))
                {
                    File.AppendAllText(logFilePath, $"{DateTime.Now}: Found at '{assemblyPath}'. Loading...\r\n");
                    var loadedAssembly = Assembly.LoadFrom(assemblyPath);
                    File.AppendAllText(logFilePath,
                        $"{DateTime.Now}: Successfully loaded '{loadedAssembly.FullName}'.\r\n");
                    return loadedAssembly;
                }

                if (requestedAssemblyName.Name.Equals("NppDB.Comm", StringComparison.OrdinalIgnoreCase))
                {
                    File.AppendAllText(logFilePath,
                        $"{DateTime.Now}: '{requestedAssemblyName.Name}.dll' not found in plugin dir. Checking N++ base dir...\r\n");
                    var nppBaseDirectory = Path.GetFullPath(Path.Combine(pluginDirectory, "..", ".."));
                    var commPathInBase = Path.Combine(nppBaseDirectory, "NppDB.Comm.dll");
                    if (File.Exists(commPathInBase))
                    {
                        File.AppendAllText(logFilePath,
                            $"{DateTime.Now}: Found NppDB.Comm.dll at '{commPathInBase}'. Loading...\r\n");
                        var loadedAssembly = Assembly.LoadFrom(commPathInBase);
                        File.AppendAllText(logFilePath,
                            $"{DateTime.Now}: Successfully loaded '{loadedAssembly.FullName}'.\r\n");
                        return loadedAssembly;
                    }

                    File.AppendAllText(logFilePath,
                        $"{DateTime.Now}: NppDB.Comm.dll NOT found at '{commPathInBase}'.\r\n");
                }
                else
                {
                    File.AppendAllText(logFilePath, $"{DateTime.Now}: '{assemblyPath}' does not exist.\r\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now}: EXCEPTION in FindAssembly resolving '{args.Name}': {ex}\r\n");
            }
            File.AppendAllText(logFilePath, $"{DateTime.Now}: Failed to resolve '{args.Name}'. Returning null.\r\n");
            return null;
        }

        #region plugin interface

        public static bool IsUnicode() { return true; }
        public static string PluginDirectoryPath { get; private set; }
        public static IntPtr GetNppHandle() => nppData._nppHandle;
        public void SetInfo(NppData notepadPlusData) { nppData = notepadPlusData; InitPlugin(); }

        public static IntPtr GetFuncsArray(ref int nbF)
        {
            nbF = _funcItems.Items.Count; 
            return _funcItems.NativePointer;
        }
        public uint MessageProc(uint message, IntPtr wParam, IntPtr lParam)
        {
            if (nppData._nppHandle == IntPtr.Zero && message != (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR)
            {
                return 1;
            }
            try
            {
                switch ((Win32.Wm)message)
                {
                    case Win32.Wm.MOVE:
                    case Win32.Wm.MOVING:
                    case Win32.Wm.SIZE:
                    case Win32.Wm.SIZING:
                    case Win32.Wm.ENTER_SIZE_MOVE:
                    case Win32.Wm.EXIT_SIZE_MOVE:
                        UpdateCurrentSqlResult();
                        break;
                    case Win32.Wm.NOTIFY:
                        break;
                    case Win32.Wm.COMMAND:
                        break;
                    default:
                        return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"NppDB Plugin Error in messageProc (Message: {message}):\n{ex}",
                                PLUGIN_NAME + " - Runtime Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
        }
        private IntPtr _ptrPluginName = IntPtr.Zero;

        public IntPtr GetName()
        {
            if (_ptrPluginName == IntPtr.Zero) _ptrPluginName = Marshal.StringToHGlobalUni(PLUGIN_NAME); 
            return _ptrPluginName;
        }
        public void BeNotified(ScNotification nc)
        {
            switch (nc.Header.Code) {
                case (uint)NppMsg.NPPN_TBMODIFICATION: _funcItems.RefreshItems(); SetToolBarIcons(); break;
                case (uint)NppMsg.NPPN_SHUTDOWN: FinalizePlugin(); break;
                case (uint)NppMsg.NPPN_FILECLOSED: CloseSqlResult(nc.Header.IdFrom); break;
                case (uint)NppMsg.NPPN_BUFFERACTIVATED:
                    UpdateCurrentSqlResult();
                    break;
                case (uint)SciMsg.SCN_UPDATEUI:
                    ReadTranslations();
                    break;
                case (uint)SciMsg.SCN_DWELLSTART: ShowTip(nc.Position); break;
                case (uint)SciMsg.SCN_DWELLEND: CloseTip(); break;
                case (uint)NppMsg.NPPN_READY: _menuBuilder.TryRebuildOnce(nppData._nppHandle, _funcItems); break;
            }
        }
        #endregion

        #region initialize and finalize a plugin

        private void InitPlugin() {
             DbServerManager.Instance.NppCommandHost = this;
             var sbCfgPath = new StringBuilder(Win32.MaxPath);
             Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MaxPath, sbCfgPath);
             _nppDbPluginDir = Path.GetDirectoryName(Uri.UnescapeDataString(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath));
             PluginDirectoryPath = _nppDbPluginDir;
             _nppDbConfigDir = Path.Combine(sbCfgPath.ToString(), PLUGIN_NAME);
             if (!Directory.Exists(_nppDbConfigDir))
             {
                 try
                 {
                     Directory.CreateDirectory(_nppDbConfigDir);
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("plugin dir : " + ex.Message); throw;
                 }
             }
             _cfgPath = Path.Combine(_nppDbConfigDir, "config.xml");
             if (File.Exists(_cfgPath))
             {
                 try
                 {
                     Options.Instance.LoadFromXml(_cfgPath);
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("config.xml : " + ex.Message); throw;
                 }
             }
             _dbConnsPath = Path.Combine(_nppDbConfigDir, "dbconnects.xml");
             if (File.Exists(_dbConnsPath))
             {
                 try
                 {
                     DbServerManager.Instance.LoadFromXml(_dbConnsPath);
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("dbconnects.xml : "+ ex.Message); throw;
                 }
             }

             var parent = new DirectoryInfo(sbCfgPath.ToString()).Parent;
             var directoryInfo = parent?.Parent;
             if (directoryInfo != null)
                 _languageConfigPath = Path.Combine(directoryInfo.FullName,
                     "nativeLang.xml");

             try
             {
                 ReadTranslations();
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message); throw;
             }
             
             _staticLanguageCodesPath = Path.Combine(_nppDbConfigDir, "LanguageCodes.csv");
             LoadLanguageDoc();
             
             _staticPromptPreferencesPath = Path.Combine(_nppDbConfigDir, "prompt_preferences.json");
             FrmPromptPreferences.PreferencesFilePath = _staticPromptPreferencesPath;
             if (!File.Exists(_staticPromptPreferencesPath))
             {
                 try
                 {
                     var defaultPreferences = new PromptPreferences
                     {
                         ResponseLanguage = "English",
                         CustomInstructions = ""
                     };
                     
                    var jsonData = JsonConvert.SerializeObject(defaultPreferences, Formatting.None);
                     
                     File.WriteAllText(_staticPromptPreferencesPath, jsonData);
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("Error creating default prompt preferences file: " + ex.Message);
                 }
             }
             
             _staticBehaviorSettingsPath = Path.Combine(_nppDbConfigDir, "behavior_settings.json");
             if (!File.Exists(_staticBehaviorSettingsPath))
             {
                 try
                 {
                     var defaultSettings = new BehaviorSettings
                     {
                         EnableDestructiveSelectInto = false,
                         EnableNewTabCreation = false
                     };
                     
                    var jsonData = JsonConvert.SerializeObject(defaultSettings, Formatting.None);
                    
                    File.WriteAllText(_staticBehaviorSettingsPath, jsonData);
                 } catch (Exception ex)
                 {
                     MessageBox.Show("Error creating default behaviors file: " + ex.Message);
                 }
             }
             
             _promptLibraryPath = Path.Combine(_nppDbConfigDir, "promptLibrary.xml");
             _tutorialPath = Path.Combine(_nppDbConfigDir, "tutorial.md");
             _staticPromptLibraryPath = _promptLibraryPath;
             FrmPromptLibrary.PromptLibraryPath = _promptLibraryPath;
             if (!File.Exists(_promptLibraryPath))
             {
                 // Ask user whether to recreate default prompt library
                 DialogResult result = _showRecreationNotificationsToUsers
                     ? MessageBox.Show(
                         "Prompt Library file not found. Do you want to create a default Prompt Library file?",
                         PLUGIN_NAME,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question)
                     : DialogResult.Yes;
                 

                 if (result == DialogResult.Yes)
                 {

                     try
                     {
                         using (var stream = Assembly.GetExecutingAssembly()
                                    .GetManifestResourceStream("NppDB.Resources.PromptLibraryDefault.xml"))
                         {
                             if (stream == null)
                                 throw new InvalidOperationException(
                                     "Embedded resource `NppDB.Plugin.Resources.PromptLibraryDefault.xml` not found.");

                             using (var file = File.Create(_promptLibraryPath))
                             {
                                 stream.CopyTo(file);
                             }
                         }

                         MessageBox.Show(
                             $"Prompt Library file not found. A default file has been created at:\n{_promptLibraryPath}");
                     }
                     catch (Exception ex)
                     {
                         MessageBox.Show(ex.Message);
                     }
                 }
                 else
                 {
                     // Todo: deprecate disabling
                     // _isPromptLibraryDisabled = true;
                     // MessageBox.Show("Prompt Library file not found. Prompt Library features will be disabled.");
                 }
                 
                 // we'll assume that this is mostly run on the first plugin load,
                 // so we can use this as injection point for first launch tutorial
                 ShowTutorial();
             }

             
             SetCommand(0, "Execute SQL", Execute, new ShortcutKey(false, false, false, Keys.F9));
             SetCommand(1, "Analyze SQL", Analyze, new ShortcutKey(false, false, true, Keys.F9));
             SetCommand(2, "Analyze and Create Prompt", HandleCtrlF9ForAiPrompt, new ShortcutKey(true, false, false, Keys.F9));
             SetCommand(3, "Database Connect Manager", ToggleDbManager, new ShortcutKey(false, false, false, Keys.F10));
             SetCommand(4, "Clear analysis", ClearAnalysis, new ShortcutKey(true, false, true, Keys.F9));
             SetCommand(5, "Open console", OpenConsole);
             SetCommand(6, "About", ShowAbout);

             if (!_isPromptLibraryDisabled)
             {
                 SetCommand(7, "Show Prompt Library", ShowPromptLibrary, new ShortcutKey(true, false, false, Keys.F10));
             }
             SetCommand(8, "Show LLM Response Preferences", ShowPromptPreferences);
             SetCommand(9, "Show Behavior Settings", ShowBehaviorSettings);
             SetCommand(10, "Show AI Prompt Template Editor", ShowAiTemplateEditor);
             SetCommand(11, "Analyze and Create Prompt (Issue at Caret)", AnalyzeAndCreatePromptForIssueAtCaret, new ShortcutKey(false, true, false, Keys.F9));
             SetCommand(12, "Show Tutorial", ShowTutorial, new ShortcutKey(true, false, false, Keys.F11));

             _cmdFrmDbExplorerIdx = 3; 
        }

        private void ReadTranslations()
        {
            string localTranslationsConfigPath;
            try
            {
                var xd = new XmlDocument();
                xd.Load(_languageConfigPath);
                var selectedLocalizationFileNode = xd.SelectSingleNode("/NotepadPlus/Native-Langue/@filename");

                if (selectedLocalizationFileNode != null && !string.IsNullOrEmpty(selectedLocalizationFileNode.Value))
                {
                    var selectedLocalizationFileName = selectedLocalizationFileNode.Value;
                    var iniFileName = selectedLocalizationFileName.RemoveSuffix(".xml") + ".ini";
                    localTranslationsConfigPath = Path.Combine(_nppDbPluginDir, iniFileName);
                }
                else
                {
                    Console.WriteLine($"Warning: Could not find selected language filename in '{_languageConfigPath}'. Fallback will be used.");
                    localTranslationsConfigPath = Path.Combine(_nppDbPluginDir, "english.ini");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception reading language config '{_languageConfigPath}': {ex.Message}. Defaulting to english.ini.");
                localTranslationsConfigPath = Path.Combine(_nppDbPluginDir, "english.ini");
            }

            if (!string.IsNullOrEmpty(_translationsConfigPath) && _translationsConfigPath.Equals(localTranslationsConfigPath))
            {
                return;
            }
            _translationsConfigPath = localTranslationsConfigPath;

            if (string.IsNullOrEmpty(_translationsConfigPath) || !File.Exists(_translationsConfigPath))
            {
                MessageBox.Show($"Translation INI file not found at: {_translationsConfigPath ?? "Path not set"}. Warnings will not be translated.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _warningMessages.Clear();
                _generalTranslations.Clear();
                _generalTranslations[ParserMessageType.WARNING_FORMAT] = "Warning at {0}:{1} : {2}";
                return;
            }

            Console.WriteLine($@"Plugin translations being loaded from: {_translationsConfigPath}");

            var warningTypesToLoad = Enum.GetValues(typeof(ParserMessageType)).Cast<ParserMessageType>().Where(type => type != ParserMessageType.WARNING_FORMAT).ToDictionary(type => type, type => string.Empty);

            var generalTypesToLoad = new Dictionary<ParserMessageType, string>
            {
                { ParserMessageType.WARNING_FORMAT, "Warning at {0}:{1} : {2}" }
            };

            _warningMessages.Clear();
            _generalTranslations.Clear();

            ReadTranslations("Warnings", warningTypesToLoad, ref _warningMessages);
            ReadTranslations("General", generalTypesToLoad, ref _generalTranslations);
        }

        private void ReadTranslations(string sectionName, in Dictionary<ParserMessageType, string> inputDictionary, ref Dictionary<ParserMessageType, string> outputDictionary)
        {
            var codePage = Win32.SendMessage(GetCurrentScintilla(), SciMsg.SCI_GETCODEPAGE, 0, 0).ToInt32();
            foreach (var entry in inputDictionary)
            {
                var iniKeyName = Enum.GetName(typeof(ParserMessageType), entry.Key);

                if (string.IsNullOrEmpty(iniKeyName))
                {
                    Console.WriteLine($"Warning: Could not get name for enum value {entry.Key}. Skipping translation.");
                    continue;
                }

                const int bufferSize = 256;

                var bufferBytes = new byte[bufferSize];

                try
                {
                    Win32.GetPrivateProfileString(
                        sectionName,
                        iniKeyName,
                        entry.Value, bufferBytes,
                        bufferSize,
                        _translationsConfigPath
                    );

                    var translatedText = Encoding.GetEncoding(codePage).GetString(bufferBytes).TrimEnd('\0');

                    outputDictionary[entry.Key] = translatedText;
                }
                catch (ArgumentException argEx)
                {
                    Console.WriteLine(
                        $"Warning: Encoding error for key '{iniKeyName}' with codepage {codePage}. Using default. Error: {argEx.Message}");
                    outputDictionary[entry.Key] = entry.Value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Warning: Failed to read translation for key '{iniKeyName}'. Using default. Error: {ex.Message}");
                    outputDictionary[entry.Key] = entry.Value;
                }
            }
        }

        private const uint STD_OUTPUT_HANDLE = 0xFFFFFFF5; private const int MY_CODE_PAGE = 437;
        /// <summary>
        /// Allocates a new console window for the application and redirects
        /// the standard console output (Console.WriteLine, etc.) to it.
        /// Useful for debugging purposes within the Notepad++ plugin.
        /// </summary>
        private static void OpenConsole()
        {
            try
            {
                Win32.AllocConsole();

                var stdOutHandle = Win32.GetStdHandle(STD_OUTPUT_HANDLE);

                var safeFileHandle = new SafeFileHandle(stdOutHandle, true);

                var fileStream = new FileStream(safeFileHandle, FileAccess.Write);

                var standardOutput = new StreamWriter(fileStream, Encoding.GetEncoding(MY_CODE_PAGE))
                {
                    AutoFlush = true
                };

                Console.SetOut(standardOutput);

                Console.WriteLine("Debug console successfully opened.");
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"Error opening console (IOException): {ioEx.Message}", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening console: {ex.Message}", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sets the toolbar icon for the Database Connect Manager menu item.
        /// This method should be called after Notepad++ has initialized its toolbar,
        /// typically in response to the NPPN_TBMODIFICATION notification.
        /// </summary>
        private void SetToolBarIcons()
        {
            TryAddToolbarIcon(0, Resources.IconExecute);
            TryAddToolbarIcon(1, Resources.IconAnalyze);
            TryAddToolbarIcon(7, Resources.IconPromptLibrary);
            TryAddToolbarIcon(3, Resources.DBPPManage16);
        }

        private void TryAddToolbarIcon(int funcItemIndex, Bitmap bmp)
        {
            if (bmp == null)
            {
                Console.WriteLine($"Toolbar icon bitmap is null for index {funcItemIndex}");
                return;
            }

            if (funcItemIndex < 0 || funcItemIndex >= _funcItems.Items.Count)
            {
                Console.WriteLine("Error: Toolbar icon image resource (_imgMan) is null.");
                return;
            }

            var pTbIcons = IntPtr.Zero;

            try
            {
                var tbIcons = new toolbarIcons
                {
                    hToolbarBmp = bmp.GetHbitmap()
                };

                pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
                Marshal.StructureToPtr(tbIcons, pTbIcons, false);

                Win32.SendMessage(
                    nppData._nppHandle,
                    (uint)NppMsg.NPPM_ADDTOOLBARICON,
                    _funcItems.Items[funcItemIndex]._cmdID,
                    pTbIcons);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting toolbar icon: {ex.Message}",
                                PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (pTbIcons != IntPtr.Zero)
                    Marshal.FreeHGlobal(pTbIcons);
            }
        }

        private void FinalizePlugin()
        {
            if (_ptrPluginName != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_ptrPluginName);
                _ptrPluginName = IntPtr.Zero;
            }

            try
            {
                Options.Instance.SaveToXml(_cfgPath);

                DbServerManager.Instance.SaveToXml(_dbConnsPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving plugin configuration during finalization: " + ex.Message,
                    PLUGIN_NAME, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

        private void Analyze()
        {
            AnalyzeAndExecute(true, true);
        }

        private void Execute()
        {
            AnalyzeAndExecute(false, false);
        }
        
        

        private void AnalyzeAndExecute(bool showFeedbackOnSuccess, bool onlyAnalyze, bool forceFullDocument = false)
        {
            SqlResult attachedResult = null;
            IScintillaGateway editor = null;
            var baseLine = 0;
            var selectionOnly = !forceFullDocument;

            try
            {
                var bufId = GetCurrentBufferId();
                if (bufId == IntPtr.Zero) return;

                editor = _getCurrentEditor();
                if (editor == null)
                {
                    MessageBox.Show(@"Could not get editor instance.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetSqlLang(bufId);

                var currentScintilla = GetCurrentScintilla();

                var textToParse = GetScintillaText(currentScintilla, selectionOnly);
                if (!forceFullDocument && string.IsNullOrWhiteSpace(textToParse))
                {
                    selectionOnly = false;
                    textToParse = GetScintillaText(currentScintilla, false);
                }
                if (string.IsNullOrWhiteSpace(textToParse))
                {
                    ClearAnalysisIndicators(editor);
                    _lastAnalysisResult = null;
                    _lastAnalyzedText = null;
                    _lastUsedDialect = SqlDialect.NONE;
                    _lastEditor = null;
                    return;
                }
                textToParse = textToParse.Replace("\t", "    ");

                if (selectionOnly)
                {
                    var start = editor.GetSelectionStart();
                    baseLine = editor.LineFromPosition(start);
                }

                attachedResult = SQLResultManager.Instance.GetSQLResult(bufId);

                ParserResult analysisResult;
                SqlDialect currentDialect;
                if (attachedResult != null)
                {
                    if (!attachedResult.LinkedDbConnect.IsOpened && !onlyAnalyze)
                    {
                        MessageBox.Show(@"Database connection is closed. Cannot execute.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    currentDialect = attachedResult.LinkedDbConnect.Dialect;

                    var caretPosition = GetCaretPosition(editor);
                    analysisResult = attachedResult.Parse(textToParse, caretPosition);
                    ShowSqlResult(attachedResult);
                }
                else
                {
                    if (!onlyAnalyze)
                    {
                        MessageBox.Show("No database connection attached for execution.\nPlease attach a connection first.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ISqlExecutor chosenExecutorForAnalysisOnly;
                    using (var dialectDlg = new FrmSelectSqlDialect())
                    {
                        IWin32Window owner = Control.FromHandle(nppData._nppHandle);
                        if (dialectDlg.ShowDialog(owner) != DialogResult.OK)
                        {
                            return;
                        }

                        currentDialect = dialectDlg.SelectedDialect;

                        switch (currentDialect)
                        {
                            case SqlDialect.POSTGRE_SQL:
                                chosenExecutorForAnalysisOnly = new PostgreSqlExecutor(null);
                                break;
                            case SqlDialect.MS_ACCESS:
                                chosenExecutorForAnalysisOnly = new MsAccessExecutor(null, null);
                                break;
                            case SqlDialect.NONE:
                            default:
                                MessageBox.Show("No SQL dialect selected for analysis.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                        }
                    }


                    var caretPosition = GetCaretPosition(editor);
                    analysisResult = chosenExecutorForAnalysisOnly.Parse(textToParse, caretPosition);
                }

                if (analysisResult == null)
                {
                    MessageBox.Show("Analysis failed to produce a result.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                     _lastAnalysisResult = null;
                     _lastAnalyzedText = textToParse;
                     _lastUsedDialect = currentDialect;
                     _lastEditor = editor;
                    return;
                }
                
                var hasErrors = analysisResult.Errors?.Any() ?? false;
                var hasWarns = analysisResult.Commands?.Any(c => c != null && ((c.Warnings?.Any() ?? false) || (c.AnalyzeErrors?.Any() ?? false))) ?? false;

                var firstIssueLine = -1;
                if (hasErrors)
                {
                    var firstError = analysisResult.Errors.Where(e => e != null).OrderBy(e => e.StartLine).ThenBy(e => e.StartColumn).FirstOrDefault();
                    if (firstError != null) firstIssueLine = firstError.StartLine;
                }
                else if (hasWarns)
                {
                    var firstWarningMessage = analysisResult.Commands
                        .Where(c => c != null)
                        .SelectMany(c => (c.Warnings ?? Enumerable.Empty<ParserMessage>())
                            .Concat(c.AnalyzeErrors ?? Enumerable.Empty<ParserMessage>()))
                        .Where(m => m != null)
                        .OrderBy(m => m.StartLine)
                        .ThenBy(m => m.StartColumn)
                        .FirstOrDefault();
                    if (firstWarningMessage != null) firstIssueLine = firstWarningMessage.StartLine;
                }
                
                if (showFeedbackOnSuccess || hasErrors || hasWarns)
                {
                    DisplayAnalysisFeedback(editor, analysisResult, baseLine, selectionOnly);
                }
                else
                {
                    ClearAnalysisIndicators(editor);
                    attachedResult?.ClearAnalysisStatus();
                }

                attachedResult?.SetAnalysisStatus(hasErrors, hasWarns, firstIssueLine);
                _lastAnalysisResult = analysisResult;
                _lastAnalyzedText = textToParse;
                _lastUsedDialect = currentDialect;
                _lastEditor = editor;

                if (onlyAnalyze)
                {
                    return;
                }
                if (hasErrors)
                {
                    MessageBox.Show(@"Execution cancelled due to parsing errors. Please fix the errors shown.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (analysisResult.Commands == null || !analysisResult.Commands.Any())
                {
                    MessageBox.Show(@"No valid SQL commands found to execute.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var commandIndex = (analysisResult.EnclosingCommandIndex >= 0 && analysisResult.EnclosingCommandIndex < analysisResult.Commands.Count)
                                    ? analysisResult.EnclosingCommandIndex : 0;
                var commandsToExecute = selectionOnly
                    ? analysisResult.Commands.Where(c => c != null).Select(c => c.Text).ToList()
                    : analysisResult.Commands.Skip(commandIndex).Take(1).Where(c => c != null).Select(c => c.Text).ToList();

                if (commandsToExecute.Any(cmd => !string.IsNullOrWhiteSpace(cmd)))
                {
                    attachedResult.SetError("");
                    attachedResult.Execute(commandsToExecute);
                    attachedResult.ClearAnalysisStatus();
                }
                else
                {
                    MessageBox.Show(@"Could not determine specific command to execute based on selection or caret position.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    attachedResult.ClearAnalysisStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during analysis/execution:\n{ex.Message}\n{ex.StackTrace}", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                attachedResult?.SetError($"Unexpected Error: {ex.Message}");
                if (editor != null) ClearAnalysisIndicators(editor);

                _lastAnalysisResult = null;
                _lastAnalyzedText = null;
                _lastUsedDialect = SqlDialect.NONE;
                _lastEditor = null;
            }
        }

        private void AnalyzeAndCreatePromptForIssueAtCaret()
        {
            AnalyzeAndExecute(true, true, forceFullDocument: true);
            GenerateAiPromptForIssueAtCaret(_lastAnalysisResult, _lastAnalyzedText, _lastUsedDialect, _lastEditor);
        }

        private void GenerateAiPromptForIssueAtCaret(ParserResult analysisResult, string fullQuery, SqlDialect dialect, IScintillaGateway editor)
        {
            if (analysisResult == null || string.IsNullOrEmpty(fullQuery) || editor == null)
            {
                MessageBox.Show("Not enough information to generate AI debug prompt (Initial check failed).", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var caret = GetCaretPosition(editor);
            var caretLine = caret.Line;
            var caretCol = caret.Column;

            bool HasValidLineRange(ParserMessage m)
            {
                if (m == null) return false;
                if (m.StartLine <= 0) return false;
                var stopLine = (m.StopLine > 0) ? m.StopLine : m.StartLine;
                return stopLine >= m.StartLine;
            }

            bool LineOverlaps(ParserMessage m, int startLine, int stopLine)
            {
                if (!HasValidLineRange(m)) return false;
                var mStop = (m.StopLine > 0) ? m.StopLine : m.StartLine;
                return m.StartLine <= stopLine && mStop >= startLine;
            }

            bool ContainsCaret(ParserMessage m)
            {
                if (!HasValidLineRange(m)) return false;

                var mStopLine = (m.StopLine > 0) ? m.StopLine : m.StartLine;
                if (caretLine < m.StartLine || caretLine > mStopLine) return false;

                // Same-line column checks (best-effort; StopColumn is often -1)
                if (caretLine == m.StartLine && m.StartColumn >= 0 && caretCol < m.StartColumn) return false;
                if (caretLine == mStopLine && m.StopColumn >= 0 && caretCol > m.StopColumn) return false;

                return true;
            }

            int LineDistanceToCaret(ParserMessage m)
            {
                if (!HasValidLineRange(m)) return int.MaxValue;

                var mStopLine = (m.StopLine > 0) ? m.StopLine : m.StartLine;
                if (caretLine < m.StartLine) return m.StartLine - caretLine;
                if (caretLine > mStopLine) return caretLine - mStopLine;
                return 0;
            }

            ParsedCommand caretCommand = null;

            if (analysisResult.Commands != null && analysisResult.Commands.Any())
            {
                var candidates = analysisResult.Commands
                    .Where(c => c != null && HasValidLineRange(c))
                    .Where(c =>
                    {
                        var cStop = (c.StopLine > 0) ? c.StopLine : c.StartLine;
                        return caretLine >= c.StartLine && caretLine <= cStop;
                    })
                    .ToList();

                if (candidates.Count == 1)
                {
                    caretCommand = candidates[0];
                }
                else if (candidates.Count > 1)
                {
                    caretCommand = candidates
                        .OrderByDescending(c => ContainsCaret(c))
                        .ThenBy(c => ((c.StopLine > 0) ? c.StopLine : c.StartLine) - c.StartLine)
                        .ThenBy(c => Math.Abs((c.StartColumn >= 0 ? c.StartColumn : 0) - caretCol))
                        .FirstOrDefault();
                }

                if (caretCommand == null
                    && analysisResult.EnclosingCommandIndex >= 0
                    && analysisResult.EnclosingCommandIndex < analysisResult.Commands.Count)
                {
                    caretCommand = analysisResult.Commands[analysisResult.EnclosingCommandIndex];
                }

                if (caretCommand == null)
                {
                    caretCommand = analysisResult.Commands.FirstOrDefault(c => c != null);
                }
            }

            var scopedMessages = new List<ParserMessage>();

            if (caretCommand != null)
            {
                if (caretCommand.Warnings != null)
                    scopedMessages.AddRange(caretCommand.Warnings.Where(w => w != null));

                if (caretCommand.AnalyzeErrors != null)
                    scopedMessages.AddRange(caretCommand.AnalyzeErrors.Where(w => w != null));

                if (analysisResult.Errors != null && HasValidLineRange(caretCommand))
                {
                    var cmdStartLine = caretCommand.StartLine;
                    var cmdStopLine = (caretCommand.StopLine > 0) ? caretCommand.StopLine : caretCommand.StartLine;
                    scopedMessages.AddRange(analysisResult.Errors.Where(e => e != null && LineOverlaps(e, cmdStartLine, cmdStopLine)));
                }
            }
            else
            {
                if (analysisResult.Errors != null)
                    scopedMessages.AddRange(analysisResult.Errors.Where(e => e != null));
            }

            if (!scopedMessages.Any())
            {
                MessageBox.Show("No issues found for the SQL statement at the caret.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var chosenMessage = scopedMessages
                .OrderByDescending(m => m is ParserError)
                .ThenByDescending(ContainsCaret)
                .ThenBy(LineDistanceToCaret)
                .ThenBy(m => (m.StopLine > 0 ? m.StopLine : m.StartLine) - m.StartLine)
                .FirstOrDefault();

            if (chosenMessage == null)
            {
                MessageBox.Show("Could not determine an issue near caret to generate a prompt for.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int SafePosFromLineCol(int line1Based, int col0Based)
                {
                    var line0 = Math.Max(0, line1Based - 1);
                    var lineStart = editor.PositionFromLine(line0);
                    var lineEnd = editor.GetLineEndPosition(line0);

                    var col = Math.Max(0, col0Based);
                    var pos = lineStart + col;
                    if (pos < lineStart) pos = lineStart;
                    if (pos > lineEnd) pos = lineEnd;
                    return pos;
                }

                var startLine = chosenMessage.StartLine > 0 ? chosenMessage.StartLine : caretLine;
                var startCol = chosenMessage.StartColumn >= 0 ? chosenMessage.StartColumn : 0;

                var stopLine = chosenMessage.StopLine > 0 ? chosenMessage.StopLine : startLine;
                var stopCol = chosenMessage.StopColumn >= 0 ? chosenMessage.StopColumn : startCol;

                var startPos = SafePosFromLineCol(startLine, startCol);
                var endPos = SafePosFromLineCol(stopLine, stopCol);

                if (endPos <= startPos)
                {
                    var line0 = Math.Max(0, startLine - 1);
                    startPos = editor.PositionFromLine(line0);
                    endPos = editor.GetLineEndPosition(line0);
                }
                else
                {
                    endPos = Math.Min(editor.GetLength(), endPos + 1);
                }

                editor.SetSel(startPos, endPos);
                editor.GotoPos(startPos);
            }
            catch
            {
                // ignore selection failures
            }

            var commandText = (caretCommand != null && !string.IsNullOrWhiteSpace(caretCommand.Text))
                ? caretCommand.Text
                : fullQuery;

            var translatedMessage = _warningMessages.TryGetValue(chosenMessage.Type, out var translated)
                ? translated
                : chosenMessage.Text;

            if (string.IsNullOrEmpty(translatedMessage))
                translatedMessage = chosenMessage.Text ?? "N/A";

            var issueType = chosenMessage is ParserError ? "Error" : "Warning";

            var issuesDetailedListBuilder = new StringBuilder();
            issuesDetailedListBuilder.AppendLine("    Issue 1:");
            issuesDetailedListBuilder.AppendLine($"      Type: {issueType}");
            issuesDetailedListBuilder.AppendLine($"      Message: \"{translatedMessage}\"");

            if (chosenMessage.StartLine > 0 && chosenMessage.StartColumn >= 0)
                issuesDetailedListBuilder.AppendLine($"      Location: Line {chosenMessage.StartLine}, Column {chosenMessage.StartColumn + 1}");
            else
                issuesDetailedListBuilder.AppendLine("      Location: N/A");

            if (chosenMessage.StartLine > 0)
            {
                var msgLineZeroBased = chosenMessage.StartLine - 1;
                var snippetBuilderForMsg = new StringBuilder();
                var startSnippetLine = Math.Max(0, msgLineZeroBased - 1);
                var endSnippetLine = Math.Min(editor.GetLineCount() - 1, msgLineZeroBased + 1);

                for (var i = startSnippetLine; i <= endSnippetLine; i++)
                {
                    var lineText = editor.GetLine(i);
                    snippetBuilderForMsg.AppendLine($"          {i + 1:D3}: {lineText.TrimEnd('\r', '\n')}");
                }

                var codeSnippetForMsg = snippetBuilderForMsg.ToString().Trim();
                if (string.IsNullOrWhiteSpace(codeSnippetForMsg))
                    codeSnippetForMsg = "        Could not retrieve code snippet.";

                issuesDetailedListBuilder.AppendLine("      Code Context:");
                issuesDetailedListBuilder.AppendLine(codeSnippetForMsg);
            }

            var analysisIssuesWithDetailsListString = issuesDetailedListBuilder.ToString().TrimEnd('\r', '\n');

            string generatedPrompt;
            var dbDialectString = dialect.ToString();

            try
            {
                var templateFilePath = Path.Combine(_nppDbPluginDir, "AIPromptTemplate.txt");

                if (!File.Exists(templateFilePath))
                {
                    MessageBox.Show($"AI prompt template file not found: {templateFilePath}\nUsing default prompt structure.",
                        PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    generatedPrompt = GenerateDefaultAiPromptOptionB(dbDialectString, commandText, analysisIssuesWithDetailsListString);
                    if (string.IsNullOrEmpty(generatedPrompt)) return;
                }
                else
                {
                    var promptTemplate = File.ReadAllText(templateFilePath);

                    generatedPrompt = promptTemplate
                        .Replace("{DATABASE_DIALECT}", dbDialectString)
                        .Replace("{SQL_QUERY}", commandText.Trim())
                        .Replace("{ANALYSIS_ISSUES_WITH_DETAILS_LIST}", analysisIssuesWithDetailsListString);
                }
            }
            catch (Exception exReadTemplate)
            {
                MessageBox.Show($"Error reading or processing AI prompt template: {exReadTemplate.Message}\nFalling back to default prompt structure.",
                    PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);

                generatedPrompt = GenerateDefaultAiPromptOptionB(dbDialectString, commandText, analysisIssuesWithDetailsListString);
                if (string.IsNullOrEmpty(generatedPrompt)) return;
            }

            try
            {
                Clipboard.SetText(generatedPrompt);
                var dialogMessage = "AI debug prompt (issue at caret) copied to clipboard!\n\n" +
                                    "--- Prompt Content: ---\n" +
                                    generatedPrompt;

                MessageBox.Show(dialogMessage, PLUGIN_NAME + " - AI Prompt Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exClipboard)
            {
                MessageBox.Show($"Error copying prompt to clipboard or displaying prompt: {exClipboard.Message}",
                    PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static CaretPosition GetCaretPosition(IScintillaGateway editor)
        {
            if (editor == null)
            {
                return new CaretPosition { Line = 1, Column = 0, Offset = 0 };
            }

            var offset = editor.GetCurrentPos();
            var length = editor.GetLength();
            if (length <= 0)
            {
                offset = 0;
            }
            else
            {
                var atSemicolon = false;
                var semicolonPos = -1;

                if (offset >= 0 && offset < length && editor.GetCharAt(offset) == ';')
                {
                    atSemicolon = true;
                    semicolonPos = offset;
                }
                else if (offset > 0 && editor.GetCharAt(offset - 1) == ';')
                {
                    atSemicolon = true;
                    semicolonPos = offset - 1;
                }

                if (atSemicolon)
                {
                    var i = semicolonPos - 1;
                    while (i >= 0)
                    {
                        var c = (char)editor.GetCharAt(i);
                        if (c == ';' || char.IsWhiteSpace(c))
                        {
                            i--;
                            continue;
                        }
                        break;
                    }

                    offset = i >= 0 ? i : 0;
                }
            }

            return new CaretPosition
            {
                Line = editor.LineFromPosition(offset) + 1,
                Column = editor.GetColumn(offset),
                Offset = offset,
            };
        }


        private static unsafe string GetScintillaText(IntPtr scintillaHnd, bool selectionOnly)
        {
            if (scintillaHnd == IntPtr.Zero) return string.Empty;

            byte[] textBuffer;
            int length;

            var codePage = Win32.SendMessage(scintillaHnd, SciMsg.SCI_GETCODEPAGE, 0, 0).ToInt32();
            if (codePage == 0) codePage = 65001;

            if (selectionOnly)
            {
                length = Win32.SendMessage(scintillaHnd, SciMsg.SCI_GETSELTEXT, 0, 0).ToInt32();
                if (length <= 0) return string.Empty;

                textBuffer = new byte[length + 1];
                fixed (byte* textPtr = textBuffer)
                {
                    Win32.SendMessage(scintillaHnd, SciMsg.SCI_GETSELTEXT, 0, (IntPtr)textPtr);
                }

                length = Array.IndexOf(textBuffer, (byte)0);
                if (length == -1) length = textBuffer.Length -1;
            }
            else
            {
                length = Win32.SendMessage(scintillaHnd, SciMsg.SCI_GETTEXTLENGTH, 0, 0).ToInt32();
                if (length <= 0) return string.Empty;

                textBuffer = new byte[length + 1];
                fixed (byte* textPtr = textBuffer)
                {
                    Win32.SendMessage(scintillaHnd, SciMsg.SCI_GETTEXT, (IntPtr)(length + 1), (IntPtr)textPtr);
                }

                length = Array.IndexOf(textBuffer, (byte)0);
                 if (length == -1) length = textBuffer.Length -1;
            }

            string text;
            try
            {
                text = Encoding.GetEncoding(codePage).GetString(textBuffer, 0, length);
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is ArgumentException || ex is NotSupportedException)
            {
                MessageBox.Show($"Scintilla Text Encoding Error (Codepage: {codePage}): {ex.Message}", @"Encoding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { text = Encoding.UTF8.GetString(textBuffer, 0, length); } catch { text = string.Empty; }
            }
            catch(Exception genEx)
            {
                 MessageBox.Show($"Error getting Scintilla text: {genEx.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 text = string.Empty;
            }

            return text;
        }

        private void DisplayAnalysisFeedback(IScintillaGateway editor, ParserResult parserResult, int baseLine, bool selectionOnly)
        {
            if (editor == null || parserResult == null) return;

            var textLength = editor.GetTextLength();

            editor.AnnotationClearAll();
            editor.SetIndicatorCurrent(20);
            editor.IndicatorClearRange(0, textLength);

            lock (_structuredErrorDetails)
            {
                _structuredErrorDetails.Clear();
            }
            lock (_editorErrors)
            {
                _editorErrors.Clear();
            }

            const int warnStyle = 199;
            const int errorStyle = 200;
            try
            {
                editor.StyleSetFont(warnStyle, "Tahoma"); editor.StyleSetSize(warnStyle, 8);
                editor.StyleSetBack(warnStyle, new Colour(250, 250, 200)); editor.StyleSetFore(warnStyle, new Colour(100, 100, 0));
                editor.StyleSetFont(errorStyle, "Tahoma"); editor.StyleSetSize(errorStyle, 8);
                editor.StyleSetBack(errorStyle, new Colour(255, 220, 220)); editor.StyleSetFore(errorStyle, new Colour(180, 0, 0));
            }
            catch (Exception styleEx) { Console.WriteLine($"Style Setting Error: {styleEx.Message}"); }

            var lineAnnotations = new Dictionary<int, Tuple<int, StringBuilder>>();
            var allMessages = new List<ParserMessage>();

            if (parserResult.Errors != null)
            {
                allMessages.AddRange(parserResult.Errors.Where(e => e != null));
            }
            if (parserResult.Commands != null)
            {
                foreach (var cmd in parserResult.Commands.Where(c => c != null))
                {
                    if (cmd.Warnings != null) allMessages.AddRange(cmd.Warnings.Where(w => w != null));
                    if (cmd.AnalyzeErrors != null) allMessages.AddRange(cmd.AnalyzeErrors.Where(w => w != null));
                }
            }

            var currentErrorIndicatorValue = 0;

            foreach (var msg in allMessages.OrderBy(m => m.StartLine).ThenBy(m => m.StartColumn))
            {
                if (msg.StartLine <= 0 || msg.StartColumn < 0 || msg.StartOffset < 0 || msg.StopOffset < msg.StartOffset) continue;

                var isError = msg is ParserError;
                var style = isError ? errorStyle : warnStyle;
                var prefix = isError ? "Error:" : "Warning:";
                var line = baseLine + msg.StartLine - 1;
                if (line < 0) line = 0;

                var messageKeyText = _warningMessages.TryGetValue(msg.Type, out var translatedMsg) ? translatedMsg : msg.Text;
                if (string.IsNullOrEmpty(messageKeyText)) messageKeyText = msg.Text ?? "Undefined issue";

                var messageText = $"{prefix} (L{msg.StartLine} C{msg.StartColumn + 1}) {messageKeyText?.Replace("\\n", "\n") ?? ""}";

                if (lineAnnotations.TryGetValue(line, out var existingAnnotation))
                {
                    existingAnnotation.Item2.AppendLine().Append(messageText);
                    if (isError && existingAnnotation.Item1 != errorStyle)
                    {
                        lineAnnotations[line] = Tuple.Create(style, existingAnnotation.Item2);
                    }
                }
                else
                {
                    lineAnnotations[line] = Tuple.Create(style, new StringBuilder(messageText));
                }

                lock (_structuredErrorDetails)
                {
                    _structuredErrorDetails.Add(Tuple.Create(messageText, msg));
                }

                if (!isError) continue;
                lock (_editorErrors)
                {
                    _editorErrors.Add(messageText);
                }

                int length;
                int startBytePos;

                if (selectionOnly)
                {
                    var selectionStartPos = editor.GetSelectionStart();
                    startBytePos = selectionStartPos + msg.StartOffset;
                    length = Math.Max(1, msg.StopOffset - msg.StartOffset + 1);
                }
                else
                {
                    startBytePos = msg.StartOffset;
                    length = Math.Max(1, msg.StopOffset - msg.StartOffset + 1);
                }

                if (startBytePos < 0) startBytePos = 0;
                if (startBytePos + length > textLength) length = textLength - startBytePos;
                if (length < 0) length = 0;

                try
                {
                    editor.SetIndicatorCurrent(20);
                    editor.IndicSetStyle(20, IndicatorStyle.SQUIGGLE);
                    editor.IndicSetFore(20, new Colour(255, 0, 0));

                    currentErrorIndicatorValue++;
                    editor.SetIndicatorValue(currentErrorIndicatorValue);

                    if (length > 0)
                    {
                        editor.IndicatorFillRange(startBytePos, length);
                    }
                }
                catch (Exception indicatorEx)
                {
                    Console.WriteLine($"Indicator Error: {indicatorEx.Message} Pos:{startBytePos} Len:{length}");
                }
            }

            foreach (var entry in lineAnnotations.Where(entry => entry.Key >= 0 && entry.Key < editor.GetLineCount()))
            {
                try
                {
                    editor.AnnotationSetStyle(entry.Key, entry.Value.Item1);
                    editor.AnnotationSetText(entry.Key, entry.Value.Item2.ToString());
                }
                catch (Exception annEx) { Console.WriteLine($"Annotation Error Line {entry.Key}: {annEx.Message}"); }
            }

            if (lineAnnotations.Count > 0)
            {
                try { editor.AnnotationSetVisible(AnnotationVisible.BOXED); }
                catch
                {
                }
            }

            try { editor.SetMouseDwellTime(500); }
            catch
            {
            }
        }


        private void ClearAnalysisIndicators(IScintillaGateway editor)
        {
            if (editor == null) editor = _getCurrentEditor();
            if (editor == null) return;
            try
            {
                var textLength = editor.GetTextLength();
                editor.AnnotationClearAll();
                editor.SetIndicatorCurrent(20);
                editor.IndicatorClearRange(0, textLength);
                lock (_editorErrors) _editorErrors.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing analysis indicators: {ex.Message}");
            }
        }
        
        private void ShowTip(Position position)
        {
            var editor = _getCurrentEditor();
            var value = editor.IndicatorValueAt(20, Convert.ToInt32(position.Value));

            string tipText = null;

            lock (_structuredErrorDetails)
            {
                var errorCount = 0;
                foreach (var t in _structuredErrorDetails.Where(t => t.Item2 is ParserError))
                {
                    errorCount++;
                    if (errorCount != value) continue;
                    tipText = t.Item1;
                    break;
                }
            }

            if (tipText == null) return;
            try
            {
                editor.CallTipShow(Convert.ToInt32(position.Value), tipText);
            }
            catch (Exception ex) { Console.WriteLine($"CallTipShow Error: {ex.Message}"); }
        }

        private void CloseTip()
        {
            try
            {
                var editor = _getCurrentEditor();
                editor.CallTipCancel();
            }
            catch (Exception ex) { Console.WriteLine($"CallTipCancel Error: {ex.Message}"); }
        }

        private void ClearAnalysis()
        {
            var editor = _getCurrentEditor();
            ClearAnalysisIndicators(editor);

            var currentBufferId = GetCurrentBufferId();
            if (currentBufferId != IntPtr.Zero)
            {
                var currentResultPanel = SQLResultManager.Instance.GetSQLResult(currentBufferId);
                if (currentResultPanel != null && !currentResultPanel.IsDisposed)
                {
                    currentResultPanel.ClearAnalysisStatus();
                }
            }

            _lastAnalysisResult = null;
            _lastAnalyzedText = null;
        }

        public void SendNppMessage(uint msg, IntPtr wParam, int lParam)
        {
            Win32.SendMessage(nppData._nppHandle, msg, wParam, lParam);
        }

        public object Execute(NppDbCommandType type, object[] parameters)
        {
            try
            {
                switch (type)
                {
                    case NppDbCommandType.ACTIVATE_BUFFER:
                        ActivateBufferId((int)parameters[0]);
                        break;
                    case NppDbCommandType.APPEND_TO_CURRENT_VIEW:
                        AppendToScintillaText(GetCurrentScintilla(), (string)parameters[0]);
                        break;
                    case NppDbCommandType.NEW_FILE:
                        NewFile();
                        break;
                    
                    case NppDbCommandType.SET_SQL_LANGUAGE:
                        if (parameters != null && parameters.Length >= 1)
                        {
                            if (parameters[0] is IntPtr pLang0) SetSqlLang(pLang0);
                            else if (parameters[0] is int pLang1) SetSqlLang(new IntPtr(pLang1));
                            else SetSqlLang(GetCurrentBufferId());
                        }
                        else
                        {
                            SetSqlLang(GetCurrentBufferId());
                        }
                        break;

                    case NppDbCommandType.CREATE_RESULT_VIEW:
                        if (parameters != null && parameters.Length >= 3 && parameters[0] is IntPtr p0 &&
                            parameters[1] is IDbConnect p1 && parameters[2] is ISqlExecutor p2)
                        {
                            SetSqlLang(p0);
                            var ctr = AddSqlResult(p0, p1, p2);

                            if (p0 == GetCurrentBufferId()) UpdateCurrentSqlResult();

                            return ctr;
                        }
                        return null;
                    case NppDbCommandType.DESTROY_RESULT_VIEW:
                        CloseCurrentSqlResult();
                        break;
                    case NppDbCommandType.EXECUTE_SQL:
                         if (parameters != null && parameters.Length >= 2 && parameters[0] is IntPtr pSql0 && parameters[1] is string pSql1)
                             ExecuteSql(pSql0, pSql1);
                         break;
                    case NppDbCommandType.GET_ATTACHED_BUFFER_ID:
                        return GetCurrentAttachedBufferId();
                    case NppDbCommandType.GET_ACTIVATED_BUFFER_ID:
                        return GetCurrentBufferId();
                    case NppDbCommandType.OPEN_FILE_IN_NPP:
                        if (parameters == null || parameters.Length < 1 || !(parameters[0] is string filePath))
                            return false;
                        Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_DOOPEN, 0, filePath);
                        return true;
                    case NppDbCommandType.GET_PLUGIN_DIRECTORY:
                        return _nppDbPluginDir;
                    case NppDbCommandType.GET_PLUGIN_CONFIG_DIRECTORY:
                        return _nppDbConfigDir;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing command {type}: {ex.Message}", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private void SetResultPos(Control control)
        {
            try
            {
                if (_isUpdatingResultPos) return;
                _isUpdatingResultPos = true;

                if (!_uiScaleInitialized && control != null && control.IsHandleCreated)
                {
                    try
                    {
                        using (var g = control.CreateGraphics())
                        {
                            var scale = g.DpiY / 96f;
                            if (scale > 0.1f) _uiScale = scale;
                        }
                    }
                    catch
                    {
                        _uiScale = 1f;
                    }

                    _sqlResultHeight = (int)Math.Round(DEFAULT_SQL_RESULT_HEIGHT * _uiScale);
                    _uiScaleInitialized = true;
                }

                var nppHwnd = nppData._nppHandle;
                var hndScin = GetCurrentScintilla();

                if (hndScin == IntPtr.Zero || nppHwnd == IntPtr.Zero) return;

                if (!Win32.GetClientRect(nppHwnd, out var nppClientRect)) return;
                var nppClientHeight = nppClientRect.Bottom - nppClientRect.Top;

                var hStatusBar = Win32.FindWindowEx(nppHwnd, IntPtr.Zero, "msctls_statusbar32", null);
                var statusBarHeight = 0;
                if (hStatusBar != IntPtr.Zero)
                {
                    if (Win32.GetWindowRect(hStatusBar, out var statusBarRect))
                    {
                        statusBarHeight = statusBarRect.Bottom - statusBarRect.Top;
                    }
                }

                var availableHeight = nppClientHeight - statusBarHeight;

                if (!Win32.GetWindowRect(hndScin, out var scinScreenRect)) return;

                var scinWidth = scinScreenRect.Right - scinScreenRect.Left;
                var scinTopLeftScreen = new Point(scinScreenRect.Left, scinScreenRect.Top);
                Win32.ScreenToClient(nppHwnd, ref scinTopLeftScreen);
                var scinLeft = scinTopLeftScreen.X;
                var scinTop = scinTopLeftScreen.Y;

                var minEditorHeightPx = (int)Math.Round(MIN_EDITOR_HEIGHT * _uiScale);
                var minSqlResultHeightPx = (int)Math.Round(MIN_SQL_RESULT_HEIGHT * _uiScale);
                if (minEditorHeightPx < 10) minEditorHeightPx = 10;
                if (minSqlResultHeightPx < 20) minSqlResultHeightPx = 20;

                var maxResultHeight = availableHeight - scinTop - minEditorHeightPx;
                if (maxResultHeight < minSqlResultHeightPx) maxResultHeight = minSqlResultHeightPx;

                var resultHeight = _sqlResultHeight;
                if (resultHeight < minSqlResultHeightPx) resultHeight = minSqlResultHeightPx;
                if (resultHeight > maxResultHeight) resultHeight = maxResultHeight;
                _sqlResultHeight = resultHeight;

                var resultTop = availableHeight - resultHeight;
                var minResultTop = scinTop + minEditorHeightPx;
                if (resultTop < minResultTop) resultTop = minResultTop;

                var newScinHeight = resultTop - scinTop;
                if (newScinHeight < minEditorHeightPx) newScinHeight = minEditorHeightPx;

                Win32.SetWindowPos(
                    hndScin, IntPtr.Zero,
                    scinLeft, scinTop,
                    scinWidth, newScinHeight,
                    Win32.SetWindowPosFlags.NO_Z_ORDER | Win32.SetWindowPosFlags.NO_ACTIVATE
                );

                Win32.SetWindowPos(
                    control.Handle, IntPtr.Zero,
                    scinLeft, resultTop, scinWidth, resultHeight,
                    Win32.SetWindowPosFlags.SHOW_WINDOW | Win32.SetWindowPosFlags.NO_Z_ORDER | Win32.SetWindowPosFlags.NO_ACTIVATE
                );

                if (!control.Visible) control.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in SetResultPos: {ex.Message}", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isUpdatingResultPos = false;
            }
        }

        private static void ResetViewPos()
        {
            try
            {
                var nppHwnd = nppData._nppHandle;
                var hndScin = GetCurrentScintilla();

                if (hndScin == IntPtr.Zero || nppHwnd == IntPtr.Zero) return;

                if (!Win32.GetClientRect(nppHwnd, out var nppClientRect)) return;
                var nppClientHeight = nppClientRect.Bottom - nppClientRect.Top;

                var hStatusBar = Win32.FindWindowEx(nppHwnd, IntPtr.Zero, "msctls_statusbar32", null);
                var statusBarHeight = 0;
                if (hStatusBar != IntPtr.Zero)
                {
                    if (Win32.GetWindowRect(hStatusBar, out var statusBarRect))
                    {
                         statusBarHeight = statusBarRect.Bottom - statusBarRect.Top;
                    }
                }

                var availableHeight = nppClientHeight - statusBarHeight;

                if (!Win32.GetWindowRect(hndScin, out var scinScreenRect)) return;

                var scinWidth = scinScreenRect.Right - scinScreenRect.Left;
                var scinTopLeftScreen = new Point(scinScreenRect.Left, scinScreenRect.Top);
                Win32.ScreenToClient(nppHwnd, ref scinTopLeftScreen);
                var scinLeft = scinTopLeftScreen.X;
                var scinTop = scinTopLeftScreen.Y;

                var restoredScinHeight = availableHeight - scinTop;
                if (restoredScinHeight < 50) restoredScinHeight = 50;

                Win32.SetWindowPos(
                    hndScin, IntPtr.Zero,
                    scinLeft, scinTop,
                    scinWidth, restoredScinHeight,
                    Win32.SetWindowPosFlags.NO_Z_ORDER | Win32.SetWindowPosFlags.NO_ACTIVATE
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in ResetViewPos: {ex.Message}", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ExecuteSql(IntPtr bufferId, string query)
        {
            var result = SQLResultManager.Instance.GetSQLResult(bufferId);

            if (result == null)
            {
                return;
            }

            result.SetError("");

            result.Execute(new List<string> { query });
        }
        
        private void CloseCurrentSqlResult() { var bufId = GetCurrentBufferId(); CloseSqlResult(bufId); }

        private void CloseSqlResult(IntPtr bufferId)
        {
            var result = SQLResultManager.Instance.GetSQLResult(bufferId);
            if (result == null) return;

            SQLResultManager.Instance.Remove(bufferId);

            var wasCurrentControl = (_currentCtr == result);

            if (wasCurrentControl)
            {
                _currentCtr = null;
            }

            if (result.IsHandleCreated && !result.IsDisposed)
            {
                ResetViewPos();
            }
            
            if (result is SqlResult sqlResult)
            {
                sqlResult.UserResizeRequested -= SqlResult_UserResizeRequested;
            }

            if (result.IsHandleCreated && !result.IsDisposed)
                Win32.DestroyWindow(result.Handle);

            if (!result.IsDisposed)
                result.Dispose();
        }

         private void Disconnect(IDbConnect connection)
         {
             connection.Disconnect(); 
             CloseCurrentSqlResult(); 
             SQLResultManager.Instance.RemoveSQLResults(connection);
         }

         private void Unregister(IDbConnect connection)
         {
             DbServerManager.Instance.Unregister(connection); 
             connection.Disconnect(); 
             CloseCurrentSqlResult(); 
             SQLResultManager.Instance.RemoveSQLResults(connection);
         }

         private void ToggleDbManager()
        {
            try
            {

                if (_frmDbExplorer == null || _frmDbExplorer.IsDisposed)
                {
                    _frmDbExplorer = new FrmDatabaseExplore(this);
                    _staticFrmDbExplorer = _frmDbExplorer;

                    _frmDbExplorer.AddNotifyHandler((ref Message msg) =>
                    {
                        var nc = (ScNotification)Marshal.PtrToStructure(msg.LParam, typeof(ScNotification));
                        if (nc.Header.Code != (uint)DockMgrMsg.DMN_CLOSE) return;
                        Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, _funcItems.Items[_cmdFrmDbExplorerIdx]._cmdID, 0);
                    });

                    _frmDbExplorer.DisconnectHandler = Disconnect;
                    _frmDbExplorer.UnregisterHandler = Unregister;

                    using (var newBmp = new Bitmap(16, 16))
                    {
                        var g = Graphics.FromImage(newBmp);
                        var colorMap = new ColorMap[1];
                        colorMap[0] = new ColorMap
                        {
                            OldColor = Color.Fuchsia,
                            NewColor = Color.FromKnownColor(KnownColor.ButtonFace)
                        };
                        var attr = new ImageAttributes();
                        attr.SetRemapTable(colorMap);
                        g.DrawImage(_imgDbManager, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                        _tbIcon = Icon.FromHandle(newBmp.GetHicon());
                    }

                    var nppTbData = new NppTbData
                    {
                        hClient = _frmDbExplorer.Handle,
                        pszName = _funcItems.Items[_cmdFrmDbExplorerIdx]._itemName,
                        dlgID = _cmdFrmDbExplorerIdx,
                        uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR,
                        hIconTab = (uint)_tbIcon.Handle,
                        pszModuleName = PLUGIN_NAME
                    };

                    var ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(nppTbData));
                    Marshal.StructureToPtr(nppTbData, ptrNppTbData, false);
                    Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, ptrNppTbData);
                    Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, _funcItems.Items[_cmdFrmDbExplorerIdx]._cmdID, 1);
                    Marshal.FreeHGlobal(ptrNppTbData);
                }
                else
                {
                    var nppMsg = NppMsg.NPPM_DMMSHOW; var toggleStatus = 1;
                    if (_frmDbExplorer.Visible)
                    {
                        nppMsg = NppMsg.NPPM_DMMHIDE; toggleStatus = 0;
                    }
                    Win32.SendMessage(nppData._nppHandle, (uint)nppMsg, 0, _frmDbExplorer.Handle);
                    Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, _funcItems.Items[_cmdFrmDbExplorerIdx]._cmdID, toggleStatus);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CRITICAL Error in ToggleDbManager():\nMessage: {ex.Message}\nStackTrace:\n{ex.StackTrace}",
                                PLUGIN_NAME + " - ToggleDbManager CRASH", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _frmDbExplorer = null;
            }
        }
        private static void ShowAbout() { var dlg = new frmAbout(); dlg.ShowDialog(); }

        private static void ShowPromptLibrary()
        {
            LoadPromptLibrary();
            var placeholders = new Dictionary<string, string>();
            SetPlaceholders(placeholders);
            
            var dlg = new FrmPromptLibrary(placeholders);
            dlg.ShowDialog();
        }
        
        private static void ShowPromptPreferences()
        {
            LoadLanguageDoc();
            var dlg = new FrmPromptPreferences(_staticPromptPreferencesPath);
            dlg.ShowDialog();
        }
        
        private static void ShowBehaviorSettings()
        {
            var dlg = new FrmBehaviorSettings(_staticBehaviorSettingsPath);
            dlg.ShowDialog();
        }

        private void ShowAiTemplateEditor()
        {
            var templateFilePath = Path.Combine(_nppDbPluginDir, "AIPromptTemplate.txt");

            var dlg = new FrmAiPromptTemplateEditor(templateFilePath);
            dlg.ShowDialog();
        }

        private static void SetPlaceholders(Dictionary<string, string> placeholders)
        {
            placeholders["selected_sql"] = GetSelectedSql();
            placeholders["dialect"] = GetCurrentDialect();

            var dbContext = _staticFrmDbExplorer?.GetCurrentTemplateContext();
            if (dbContext != null)
            {
                placeholders["table_name"] = dbContext.TableName;
            }
        }
        
        private static string GetSelectedSql()
        {
            var ed = _getStaticCurrentEditor();
            
            var placeholderValue = ed.GetSelectionLength() > 0 
                ? ed.GetSelText()
                : ed.GetText(ed.GetTextLength());

            return placeholderValue;
        }

        private static string GetCurrentDialect()
        {
            var dbContext = _staticFrmDbExplorer?.GetCurrentTemplateContext();
            return dbContext?.Dialect ?? "standard sql";
        }

        private static List<PromptItem> ReadPromptLibraryFromFile(string filePath)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                var results = new List<PromptItem>();

                try
                {
                    xmlDoc.Load(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load XML from file:\n{ex.Message}", PLUGIN_NAME, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return null;
                }

                var root = xmlDoc.DocumentElement;
                if (root == null || !string.Equals(root.Name, "Prompts", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        $"Invalid prompt library format in `{filePath}`. Root element `<Prompts>` not found.",
                        PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return results;
                }
                
                var promptNodes = root.SelectNodes("Prompt");
                if (promptNodes == null)
                {
                    return results;
                }
                
                foreach (XmlNode promptNode in promptNodes)
                {
                    if (promptNode.NodeType != XmlNodeType.Element)
                        continue;

                    var typeAttr = (promptNode as XmlElement)?.GetAttribute("type") ?? string.Empty;

                    var id = promptNode.SelectSingleNode("Id")?.InnerText.Trim() ?? string.Empty;
                    var title = promptNode.SelectSingleNode("Title")?.InnerText.Trim() ?? string.Empty;
                    var description = promptNode.SelectSingleNode("Description")?.InnerText.Trim() ?? string.Empty;
                    var tagsRaw = promptNode.SelectSingleNode("Tags")?.InnerText ?? string.Empty;
                    var text = promptNode.SelectSingleNode("Text")?.InnerText ?? string.Empty;

                    string[] ParseTags(string raw)
                    {
                        if (string.IsNullOrWhiteSpace(raw))
                            return Array.Empty<string>();

                        return raw
                            .Split(new[] { ',', ';', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => (t ?? string.Empty).Trim())
                            .Where(t => !string.IsNullOrWhiteSpace(t))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToArray();
                    }

                    var placeholderList = new List<PromptPlaceholder>();
                    var placeholdersNode = promptNode.SelectSingleNode("Placeholders");
                    if (placeholdersNode != null)
                    {
                        foreach (XmlNode phNode in placeholdersNode.SelectNodes("Placeholder"))
                        {
                            if (phNode is XmlElement phElem)
                            {
                                var name = phElem.GetAttribute("name");
                                
                                var isEditableRaw = phElem.GetAttribute("editable");
                                var isEditable = !string.IsNullOrEmpty(isEditableRaw);
                                
                                var isRichRaw = phElem.GetAttribute("rich");
                                var isRich = !string.IsNullOrEmpty(isRichRaw);

                                if (!string.IsNullOrWhiteSpace(name))
                                {
                                    placeholderList.Add(new PromptPlaceholder
                                    {
                                        Name = name,
                                        IsEditable = isEditable,
                                        IsRichText = isRich
                                    });
                                }
                            }
                        }
                    }

                    var item = new PromptItem
                    {
                        Id = id,
                        Title = title,
                        Description = description,
                        Type = typeAttr,
                        Tags = ParseTags(tagsRaw),
                        Text = text,
                        Placeholders = placeholderList.ToArray()
                    };

                    results.Add(item);
                }
                return results;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error reading prompt library from file:\n{ex.Message}", PLUGIN_NAME, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return null;
            }
        }

        private static void LoadLanguageDoc()
        {
            var langFilePath = _staticLanguageCodesPath;
            var langDict = new Dictionary<string, string>();
            try
            {
                if (!File.Exists(langFilePath))
                {
                    using (var stream = Assembly.GetExecutingAssembly()
                               .GetManifestResourceStream("NppDB.Resources.LanguageCodes.csv"))
                    {
                        if (stream == null)
                            throw new InvalidOperationException(
                                "Embedded resource `NppDB.Plugin.Resources.LanguageCodes.csv` not found.");

                        using (var file = File.Create(langFilePath))
                        {
                            stream.CopyTo(file);
                        }
                    }
                }

                var lines = File.ReadAllLines(langFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { ',' }, 2);
                    if (parts.Length == 2)
                    {
                        var code = parts[0].Trim();
                        var name = parts[1].Trim();
                        if (!langDict.ContainsKey(code))
                        {
                            langDict[code] = name;
                        }
                    }
                }

                FrmPromptPreferences.LanguageCodeDict = langDict;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading language codes from file:\n{ex.Message}", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void LoadPromptLibrary()
        {
            var promptItems = ReadPromptLibraryFromFile(_staticPromptLibraryPath);
            FrmPromptLibrary.SetPrompts(promptItems);
        }

        private void ShowTutorial()
        {
            if (!EnsureTutorialFileExists())
                return;

            Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_DOOPEN, 0, _tutorialPath);
        }
        
        private bool EnsureTutorialFileExists()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_tutorialPath) && File.Exists(_tutorialPath))
                    return true;

                var result = _showRecreationNotificationsToUsers
                    ? MessageBox.Show(
                    "Tutorial file not found. Do you want to recreate the default tutorial file?",
                    PLUGIN_NAME,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                    : DialogResult.Yes;

                if (result != DialogResult.Yes)
                    return false;

                using (var stream = Assembly.GetExecutingAssembly()
                           .GetManifestResourceStream("NppDB.Resources.Tutorial.txt"))
                {
                    if (stream == null)
                    {
                        MessageBox.Show(
                            "Embedded resource `NppDB.Resources.Tutorial.txt` not found.",
                            PLUGIN_NAME,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }

                    using (var file = File.Create(_tutorialPath))
                    {
                        stream.CopyTo(file);
                    }
                }

                return File.Exists(_tutorialPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating default tutorial file:\n{ex.Message}",
                    PLUGIN_NAME,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }


        private void UpdateCurrentSqlResult()
         {
            if (SQLResultManager.Instance == null) return;

            var bufId = GetCurrentBufferId();
            SqlResult targetResult = null;
            
            if (bufId == IntPtr.Zero)
            {
                if (_currentCtr != null && _currentCtr.IsHandleCreated && !_currentCtr.IsDisposed && _currentCtr.Visible)
                {
                    SetResultPos(_currentCtr);
                }
                return;
            }

            if (bufId != IntPtr.Zero)
            {
                targetResult = SQLResultManager.Instance.GetSQLResult(bufId);
                if (targetResult != null && targetResult.IsDisposed)
                {
                    targetResult = null;
                }
            }

            var controlToHide = _currentCtr;
            Control controlToShow = targetResult;

            if (controlToHide == controlToShow)
            {
                if (controlToShow != null && controlToShow.IsHandleCreated && !controlToShow.IsDisposed && controlToShow.Visible)
                {
                    SetResultPos(controlToShow);
                }
                return;
            }

            if (controlToHide != null && controlToHide.IsHandleCreated && !controlToHide.IsDisposed && controlToHide.Visible)
            {
                ResetViewPos();
                controlToHide.Visible = false;
            }

            _currentCtr = controlToShow;

            if (_currentCtr != null && _currentCtr.IsHandleCreated && !_currentCtr.IsDisposed)
            {
                SetResultPos(_currentCtr);
            }
         }
        private Control AddSqlResult(IntPtr bufId, IDbConnect connect, ISqlExecutor sqlExecutor)
        {
            var ctr = SQLResultManager.Instance.CreateSQLResult(bufId, connect, sqlExecutor);

            var ret = Win32.SetParent(ctr.Handle, nppData._nppHandle);
            if (ret == IntPtr.Zero) MessageBox.Show(@"setparent fail");

            ctr.Visible = false;

            if (ctr is SqlResult sqlResult)
            {
                sqlResult.UserResizeRequested -= SqlResult_UserResizeRequested;
                sqlResult.UserResizeRequested += SqlResult_UserResizeRequested;
            }

            return ctr;
        }
        
        private void SqlResult_UserResizeRequested(SqlResult sender, int requestedHeight)
        {
            if (requestedHeight <= 0) return;
            _sqlResultHeight = requestedHeight;

            if (sender == null || sender.IsDisposed || !sender.IsHandleCreated) return;
            if (!ReferenceEquals(sender, _currentCtr)) return;
            if (!sender.Visible) return;

            SetResultPos(sender);
        }

        private void ShowSqlResult(SqlResult control)
        {
            if (control == null || control.IsDisposed) return;

            _currentCtr = control;

            SetResultPos(control);


            if (!control.LinkedDbConnect.IsOpened)
            {
                control.SetError("This database connection is closed. Please connect again.");
            }
        }
        
        static bool NewTabCreateEnable(string staticBehaviorSettingsPath)
        {
            var json = File.ReadAllText(staticBehaviorSettingsPath);
            if (string.IsNullOrWhiteSpace(json)) return false;

            var value = JsonConvert.DeserializeObject<BehaviorSettings>(json);
            return value.EnableNewTabCreation;
        }

        private static void NewFile(bool forceNewTab = false)
        {
            
            if (!forceNewTab && !NewTabCreateEnable(_staticBehaviorSettingsPath)) return;
            
            Win32.SendMessage(nppData._nppHandle, (uint)Win32.Wm.COMMAND, (int)NppMenuCmd.IDM_FILE_NEW, 0);
        }
        private static IntPtr? GetCurrentAttachedBufferId()
        {
            var bufferId = GetCurrentBufferId();

            if (bufferId == IntPtr.Zero)
            {
                return null;
            }

            var result = SQLResultManager.Instance.GetSQLResult(bufferId);

            if (result == null)
            {
                return null;
            }

            return bufferId;
        }
        private static void ActivateBufferId(int bufferId) { 
            Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_ACTIVATEDOC, 0, bufferId);
        }

        private static IntPtr GetCurrentBufferId()
        {
            return Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTBUFFERID, 0, 0);
        }
        

        private static void SetSqlLang(IntPtr bufferId)
        {
            if (bufferId == IntPtr.Zero) return;
            if (nppData._nppHandle == IntPtr.Zero) return;

            var currentLang = Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_GETBUFFERLANGTYPE, bufferId, 0).ToInt32();
            if (currentLang == (int)LangType.L_SQL) return;

            Win32.SendMessage(nppData._nppHandle, (uint)NppMsg.NPPM_SETBUFFERLANGTYPE, bufferId, (int)LangType.L_SQL);
        }

        private static void AppendToScintillaText(IntPtr scintillaHnd, string text, bool forceIgnoreTwoNewLines = false)
        {
            if (scintillaHnd == IntPtr.Zero || string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!forceIgnoreTwoNewLines && !NewTabCreateEnable(_staticBehaviorSettingsPath))
            {
                text = "\n\n" + text;
            }

            var codePage = Win32.SendMessage(scintillaHnd, SciMsg.SCI_GETCODEPAGE, 0, 0).ToInt32();
            if (codePage == 0) codePage = 65001;

            var ptrChars = IntPtr.Zero;

            try
            {
                var bytes = Encoding.GetEncoding(codePage).GetBytes(text);

                ptrChars = Marshal.AllocHGlobal(bytes.Length);

                Marshal.Copy(bytes, 0, ptrChars, bytes.Length);

                Win32.SendMessage(scintillaHnd, SciMsg.SCI_APPENDTEXT, (IntPtr)bytes.Length, ptrChars);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Appending text to Scintilla (SCI_APPENDTEXT) failed.", ex);
            }
            finally
            {
                if (ptrChars != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrChars);
                }
            }
        }

        private void HandleCtrlF9ForAiPrompt()
        {
            Analyze();
            GenerateAiPromptForAllIssues(_lastAnalysisResult, _lastAnalyzedText, _lastUsedDialect, _lastEditor);
        }

        private void GenerateAiPromptForAllIssues(ParserResult analysisResult, string fullQuery, SqlDialect dialect, IScintillaGateway editor)
        {
            if (analysisResult == null || string.IsNullOrEmpty(fullQuery) || editor == null)
            {
                MessageBox.Show("Not enough information to generate AI debug prompt (Initial check failed).", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var allMessagesForPrompt = new List<ParserMessage>();
            if (analysisResult.Errors != null)
            {
                allMessagesForPrompt.AddRange(analysisResult.Errors.Where(e => e != null));
            }
            if (analysisResult.Commands != null)
            {
                allMessagesForPrompt.AddRange(analysisResult.Commands
                    .Where(c => c != null)
                    .SelectMany(c => (c.Warnings ?? Enumerable.Empty<ParserMessage>())
                                      .Concat(c.AnalyzeErrors ?? Enumerable.Empty<ParserMessage>()))
                    .Where(m => m != null)
                );
            }

            if (!allMessagesForPrompt.Any())
            {
                MessageBox.Show("No issues found in the last analysis to generate a prompt for.", PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var issuesDetailedListBuilder = new StringBuilder();
            var issueCounter = 1;
            foreach (var msg in allMessagesForPrompt.OrderBy(m => m.StartLine).ThenBy(m => m.StartColumn))
            {
                var translatedMessage = _warningMessages.TryGetValue(msg.Type, out var translated)
                                           ? translated
                                           : msg.Text;
                if (string.IsNullOrEmpty(translatedMessage))
                {
                    translatedMessage = msg.Text ?? "N/A";
                }
                var issueType = msg is ParserError ? "Error" : "Warning";

                issuesDetailedListBuilder.AppendLine($"    Issue {issueCounter}:");
                issuesDetailedListBuilder.AppendLine($"      Type: {issueType}");
                issuesDetailedListBuilder.AppendLine($"      Message: \"{translatedMessage}\"");
                issuesDetailedListBuilder.AppendLine($"      Location: Line {msg.StartLine}, Column {msg.StartColumn + 1}");

                var msgLineZeroBased = msg.StartLine - 1;
                var snippetBuilderForMsg = new StringBuilder();
                var startSnippetLine = Math.Max(0, msgLineZeroBased - 1);
                var endSnippetLine = Math.Min(editor.GetLineCount() - 1, msgLineZeroBased + 1);

                for (var i = startSnippetLine; i <= endSnippetLine; i++)
                {
                    var lineText = editor.GetLine(i);
                    snippetBuilderForMsg.AppendLine($"          {i + 1:D3}: {lineText.TrimEnd('\r', '\n')}");
                }
                var codeSnippetForMsg = snippetBuilderForMsg.ToString().Trim();
                if (string.IsNullOrWhiteSpace(codeSnippetForMsg)) codeSnippetForMsg = "        Could not retrieve code snippet.";

                issuesDetailedListBuilder.AppendLine("      Code Context:");
                issuesDetailedListBuilder.AppendLine(codeSnippetForMsg);
                if (issueCounter < allMessagesForPrompt.Count)
                {
                    issuesDetailedListBuilder.AppendLine("    --------------------");
                }
                issueCounter++;
            }
            var analysisIssuesWithDetailsListString = issuesDetailedListBuilder.ToString().TrimEnd('\r', '\n');

            string generatedPrompt;
            var dbDialectString = dialect.ToString();

            try
            {
                var templateFilePath = Path.Combine(_nppDbPluginDir, "AIPromptTemplate.txt");

                if (!File.Exists(templateFilePath))
                {
                    MessageBox.Show($"AI prompt template file not found: {templateFilePath}\nUsing default prompt structure.",
                                    PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    generatedPrompt = GenerateDefaultAiPromptOptionB(dbDialectString, fullQuery, analysisIssuesWithDetailsListString);
                    if (string.IsNullOrEmpty(generatedPrompt)) return;
                }
                else
                {
                    var promptTemplate = File.ReadAllText(templateFilePath);

                    generatedPrompt = promptTemplate
                        .Replace("{DATABASE_DIALECT}", dbDialectString)
                        .Replace("{SQL_QUERY}", fullQuery.Trim())
                        .Replace("{ANALYSIS_ISSUES_WITH_DETAILS_LIST}", analysisIssuesWithDetailsListString);
                }
            }
            catch (Exception exReadTemplate)
            {
                MessageBox.Show($"Error reading or processing AI prompt template: {exReadTemplate.Message}\nFalling back to default prompt structure.",
                                PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                generatedPrompt = GenerateDefaultAiPromptOptionB(dbDialectString, fullQuery, analysisIssuesWithDetailsListString);
                if (string.IsNullOrEmpty(generatedPrompt)) return;
            }

            try
            {
                Clipboard.SetText(generatedPrompt);
                var dialogMessage = "AI debug prompt copied to clipboard!\n\n" +
                                    "--- Prompt Content: ---\n" +
                                    generatedPrompt;
                MessageBox.Show(dialogMessage, PLUGIN_NAME + " - AI Prompt Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exClipboard)
            {
                MessageBox.Show($"Error copying prompt to clipboard or displaying prompt: {exClipboard.Message}",
                                PLUGIN_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GenerateDefaultAiPromptOptionB(string dbDialectString, string fullQuery, string analysisIssuesWithDetailsList)
        {
            if (string.IsNullOrEmpty(dbDialectString) || string.IsNullOrEmpty(fullQuery) || string.IsNullOrEmpty(analysisIssuesWithDetailsList))
            {
                Console.WriteLine("GenerateDefaultAiPromptOptionB: Missing essential information to build default prompt.");
                return null;
            }

            var defaultPrompt = $@"**Role**
You are an expert {dbDialectString} SQL developer and troubleshooter.

**Task**
- Diagnose and fix problems in the provided SQL query using the detected issues list.
- For each issue, explain what it means, why it happens, and how to fix it.

**Constraints**
- Use the {dbDialectString} dialect only.
- Preserve the query’s intended result; prefer minimal, targeted changes.
- If critical context is missing (schema, sample data, constraints), state assumptions explicitly.
- Any SQL you propose must be valid and runnable for this dialect.

**Response**
For each issue (in the same order as provided), output:
1) Meaning (1–3 sentences)
2) Likely cause(s) (bullets)
3) Fix (final SQL or snippet in one `sql` code block)
4) Best-practice note (optional, 1–2 bullets)

**Database Dialect**
`{dbDialectString}`

**SQL Query**
```sql
{fullQuery}`
```
**Detected Issues**
```
{analysisIssuesWithDetailsList}
```
";
            return defaultPrompt;
        }
    }
}