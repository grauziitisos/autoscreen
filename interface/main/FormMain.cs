﻿//-----------------------------------------------------------------------
// <copyright file="FormMain.cs" company="Gavin Kendall">
//     Copyright (c) 2008-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>The main interface form for setting up sub-forms, showing the interface, hiding the interface, displaying dates in the calendar, and searching for screenshots.</summary>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using Gavin.Kendall.SFTP;

namespace AutoScreenCapture
{
    /// <summary>
    /// The application's main window.
    /// </summary>
    public partial class FormMain : Form
    {
        // Determines if the application has started so it can run any trigger
        // with the ApplicationStartup condition. This is so we don't accidentally
        // run these types of triggers before command line options get a chance to be parsed.
        private bool _appStarted = false;

        // Preview
        bool _preview = false;

        // Determines if we should show or hide the application's interface.
        private bool _initialVisibilitySet = false;

        // The "About Auto Screen Capture" form.
        private FormAbout _formAbout;

        // The "Auto Screen Capture - Help" form.
        private FormHelp _formHelp;

        // The "Email Settings" form.
        private FormEmailSettings _formEmailSettings;

        // The "File Transfer Settings" form.
        private FormFileTransferSettings _formFileTransferSettings;

        // The "Region Select Options" form.
        private FormRegionSelectOptions _formRegionSelectOptions;

        // SFTP client.
        private SftpClient _sftpClient = null;

        // The various forms that are used for modules.
        private FormMacroTag _formMacroTag;
        private FormRegion _formRegion;
        private FormScreen _formScreen;
        private FormEditor _formEditor;
        private FormExternalProgram _formExternalProgram;
        private FormTrigger _formTrigger;
        private FormSchedule _formSchedule;

        // Setup.
        private FormSetup _formSetup;
        private FormSetupWizard _formSetupWizard;

        // Screeshot Properties
        private FormScreenshotMetadata _formScreenshotMetadata = new FormScreenshotMetadata();

        // The form to display when challenging the user for the passphrase in order to unlock the running screen capture session.
        private FormEnterPassphrase _formEnterPassphrase;

        // A small window is shown when the user selects "Show Screen Capture Status" from the system tray icon menu.
        private FormScreenCaptureStatus _formScreenCaptureStatus;

        // The Dynamic Regex Validator tool.
        private FormDynamicRegexValidator _formDynamicRegexValidator = new FormDynamicRegexValidator();

        // Keyboard Shortcuts
        private HotKeyMap _hotKeyMap;
        private string _keyboardShortcutStartScreenCaptureKeyUserSetting;
        private string _keyboardShortcutStopScreenCaptureKeyUserSetting;
        private string _keyboardShortcutCaptureNowArchiveKeyUserSetting;
        private string _keyboardShortcutCaptureNowEditKeyUserSetting;
        private string _keyboardShortcutRegionSelectClipboardKeyUserSetting;
        private string _keyboardShortcutRegionSelectAutoSaveKeyUserSetting;
        private string _keyboardShortcutRegionSelectEditKeyUserSetting;

        // Classes
        private Log _log;
        private Config _config;
        private FileSystem _fileSystem;
        private Security _security;
        private Slideshow _slideShow;
        private DataConvert _dataConvert;
        private ScreenCapture _screenCapture;
        private MacroParser _macroParser;
        private ImageFormatCollection _imageFormatCollection;
        private ScreenshotCollection _screenshotCollection;

        /// <summary>
        /// Threads for background operations.
        /// </summary>
        private BackgroundWorker runScreenshotSearchThread = null;
        private BackgroundWorker runDateSearchThread = null;
        private BackgroundWorker runDeleteSlidesThread = null;
        private BackgroundWorker runFilterSearchThread = null;
        private BackgroundWorker runSaveScreenshotsThread = null;

        /// <summary>
        /// Delegates for the threads.
        /// </summary>
        private delegate void RunSlideSearchDelegate(DoWorkEventArgs e);
        private delegate void RunDateSearchDelegate(DoWorkEventArgs e);
        private delegate void RunTitleSearchDelegate(DoWorkEventArgs e);

        /// <summary>
        /// Default settings used by the command line parser.
        /// </summary>
        private const int CAPTURE_LIMIT_MIN = 0;
        private const int CAPTURE_LIMIT_MAX = 9999;

        /// <summary>
        /// Constructor for the main form.
        /// </summary>
        public FormMain(Config config)
        {
            _config = config;
            _fileSystem = config.FileSystem;
            _log = config.Log;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                AutoScaleMode = AutoScaleMode.Dpi;
                Font = new Font(Font.Name, 8.25f * 96f / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            }

            InitializeComponent();

            _security = new Security();
            _slideShow = new Slideshow();
            _dataConvert = new DataConvert();

            _hotKeyMap = new HotKeyMap();
            RegisterKeyboardShortcuts();
            _hotKeyMap.KeyPressed += new EventHandler<KeyPressedEventArgs>(hotKey_KeyPressed);

            LoadSettings();

            Text = _config.Settings.ApplicationName;

            // Get rid of the old "slides" directory that may still remain from an old version of the application.
            DeleteSlides();
        }

        /// <summary>
        /// When this form loads we'll need to delete slides and then search for dates and slides.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            bool firstRun = Convert.ToBoolean(_config.Settings.User.GetByKey("FirstRun", _config.Settings.DefaultSettings.FirstRun).Value);

            string welcome = "Welcome to " + _config.Settings.ApplicationName + " (" + _config.Settings.ApplicationVersion + ")";

            HelpMessage(welcome);

            LoadHelpTips();

            ShowInfo();

            SearchFilterValues();
            SearchDates();
            SearchScreenshots();

            // Start the scheduled capture timer.
            timerScheduledCapture.Interval = 1000;
            timerScheduledCapture.Enabled = true;
            timerScheduledCapture.Start();

            // Set this to true so anything that needs to be processed at startup will be done in the
            // first tick of the scheduled capture timer. This is when using -hide and -start command line options
            // so we avoid having to show the interface and/or the system tray icon too early during application startup.
            _appStarted = true;

            // To be figured out later. I want to have a "Setup Wizard" opened on the first run.
            //if (firstRun)
            //{
            //    _formSetupWizard.ShowDialog(this);
            //}
        }

        /// <summary>
        /// Set opacity to the appropriate value and taskbar appearance based on visibility.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            // Annoyingly, WinForms likes to show the main form even if Visible is set to false in the Designer code
            // so we have to make sure we only consider the visibility of the main form if either the methods
            // ShowInterface or HideInterface have been called and therefore the _initialVisibilitySet bool variable
            // has been flagged as true by those methods. This fixes a situation where the user could have no Triggers at all so
            // we want to keep the main form invisible if there are no Triggers to trigger either ShowInterface or HideInterface.
            if (!_initialVisibilitySet)
            {
                return;
            }

            if (Visible)
            {
                Opacity = 100;
                ShowInTaskbar = true;
                Show();
                Focus();
            }
            else
            {
                Opacity = 0;
                ShowInTaskbar = false;
                Hide();
            }

            base.OnVisibleChanged(e);
        }

        /// <summary>
        /// When this form is closing we can either exit the application or just close this window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown ||
                e.CloseReason == CloseReason.ApplicationExitCall)
            {
                DisableStopCapture();
                EnableStartCapture();

                _screenCapture.Count = 0;
                _screenCapture.Running = false;

                HideSystemTrayIcon();

                _log.WriteDebugMessage("Hiding interface on forced application exit because Windows is shutting down");
                HideInterface();

                _log.WriteDebugMessage("Saving screenshots on forced application exit because Windows is shutting down");
                _screenshotCollection.SaveToXmlFile(_config);

                if (runDateSearchThread != null && runDateSearchThread.IsBusy)
                {
                    runDateSearchThread.CancelAsync();
                }

                if (runScreenshotSearchThread != null && runScreenshotSearchThread.IsBusy)
                {
                    runScreenshotSearchThread.CancelAsync();
                }

                _log.WriteMessage("Bye!");

                // Exit.
                Environment.Exit(0);
            }
            else
            {
                _log.WriteDebugMessage("Running triggers of condition type InterfaceClosing");
                RunTriggersOfConditionType(TriggerConditionType.InterfaceClosing);

                // If there isn't a Trigger for "InterfaceClosing" that performs an action
                // then make sure we cancel this event so that nothing happens. We want the user
                // to use a Trigger, and decide what they want to do, when closing the interface window.
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Searches for dates. They should be in the format yyyy-mm-dd.
        /// </summary>
        private void SearchDates()
        {
            _log.WriteDebugMessage("Searching for dates");

            if (runDateSearchThread == null)
            {
                runDateSearchThread = new BackgroundWorker
                {
                    WorkerReportsProgress = false,
                    WorkerSupportsCancellation = true
                };

                runDateSearchThread.DoWork += new DoWorkEventHandler(DoWork_runDateSearchThread);
            }

            if (!runDateSearchThread.IsBusy)
            {
                runDateSearchThread.RunWorkerAsync();
            }

        }

        private void DeleteSlides()
        {
            _log.WriteDebugMessage("Deleting slides directory from old version of application (if needed)");

            if (runDeleteSlidesThread == null)
            {
                runDeleteSlidesThread = new BackgroundWorker
                {
                    WorkerReportsProgress = false,
                    WorkerSupportsCancellation = true
                };

                runDeleteSlidesThread.DoWork += new DoWorkEventHandler(DoWork_runDeleteSlidesThread);
            }

            if (!runDeleteSlidesThread.IsBusy)
            {
                runDeleteSlidesThread.RunWorkerAsync();
            }
        }

        /// <summary>
        /// This thread is responsible for figuring out what days screenshots were taken.
        /// </summary>
        /// <param name="e"></param>
        private void RunDateSearch(DoWorkEventArgs e)
        {
            if (monthCalendar.InvokeRequired)
            {
                monthCalendar.Invoke(new RunDateSearchDelegate(RunDateSearch), new object[] { e });
            }
            else
            {
                if (_screenshotCollection != null)
                {
                    List<string> dates = new List<string>();
                    dates = _screenshotCollection.GetDatesByFilter(comboBoxFilterType.Text, comboBoxFilterValue.Text);

                    DateTime[] boldedDates = new DateTime[dates.Count];

                    for (int i = 0; i < dates.Count; i++)
                    {
                        boldedDates.SetValue(ConvertDateStringToDateTime(dates[i].ToString()), i);
                    }

                    monthCalendar.BoldedDates = boldedDates;
                }
            }
        }

        /// <summary>
        /// This thread is responsible for deleting all the slides remaining from an old version of the application
        /// since we no longer use slides or support the Slideshow module going forward.
        /// </summary>
        /// <param name="e"></param>
        private void RunDeleteSlides(DoWorkEventArgs e)
        {
            _fileSystem.DeleteFilesInDirectory(_fileSystem.SlidesFolder);
        }

        /// <summary>
        /// Shows the interface.
        /// </summary>
        private void ShowInterface()
        {
            try
            {
                if (_initialVisibilitySet && Visible)
                {
                    return;
                }

                _log.WriteDebugMessage("Showing interface");

                string passphrase = _config.Settings.User.GetByKey("Passphrase", _config.Settings.DefaultSettings.Passphrase).Value.ToString();

                if (!string.IsNullOrEmpty(passphrase))
                {
                    _screenCapture.LockScreenCaptureSession = true;

                    if (!_formEnterPassphrase.Visible)
                    {
                        _formEnterPassphrase.Text = "Auto Screen Capture - Enter Passphrase (Show Interface)";
                        _formEnterPassphrase.ShowDialog(this);
                    }
                    else
                    {
                        _formEnterPassphrase.Focus();
                        _formEnterPassphrase.BringToFront();
                    }

                    if (_formEnterPassphrase.DialogResult != DialogResult.OK)
                    {
                        _log.WriteErrorMessage("Passphrase incorrect or not entered. Cannot show interface. Screen capture session has been locked. Interface is now hidden");

                        HideInterface();

                        return;
                    }

                    _screenCapture.LockScreenCaptureSession = false;
                }

                SearchDates();
                SearchScreenshots();

                PopulateLabelList();

                _initialVisibilitySet = true;
                Visible = true;

                Opacity = 100;
                ShowInTaskbar = true;
                Show();
                Focus();

                // If the window is mimimized then show it when the user wants to open the window.
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }

                _log.WriteDebugMessage("Running triggers of condition type InterfaceShowing");
                RunTriggersOfConditionType(TriggerConditionType.InterfaceShowing);
            }
            catch (Exception ex)
            {
                _screenCapture.ApplicationError = true;
                _log.WriteExceptionMessage("FormMain::ShowInterface", ex);
            }
        }

        /// <summary>
        /// Hides the interface.
        /// </summary>
        private void HideInterface()
        {
            try
            {
                _log.WriteDebugMessage("Hiding interface");

                string passphrase = _config.Settings.User.GetByKey("Passphrase", _config.Settings.DefaultSettings.Passphrase).Value.ToString();

                if (!string.IsNullOrEmpty(passphrase))
                {
                    _screenCapture.LockScreenCaptureSession = true;
                }
                else
                {
                    _screenCapture.LockScreenCaptureSession = false;
                }

                _initialVisibilitySet = true;
                Visible = false;

                Opacity = 0;
                ShowInTaskbar = false;
                Hide();

                _log.WriteDebugMessage("Running triggers of condition type InterfaceHiding");
                RunTriggersOfConditionType(TriggerConditionType.InterfaceHiding);
            }
            catch (Exception ex)
            {
                _screenCapture.ApplicationError = true;
                _log.WriteExceptionMessage("FormMain::HideInterface", ex);
            }
        }

        /// <summary>
        /// Runs the date search thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWork_runDateSearchThread(object sender, DoWorkEventArgs e)
        {
            RunDateSearch(e);
        }

        /// <summary>
        /// Runs the delete slides thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWork_runDeleteSlidesThread(object sender, DoWorkEventArgs e)
        {
            RunDeleteSlides(e);
        }

        /// <summary>
        /// Shows or hides the interface depending on if the interface is already visible or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemShowHideInterface_Click(object sender, EventArgs e)
        {
            // This is to check if we're in the weird condition whereby the main form is considered Visible
            // but we haven't set the visibility ourselves yet. We can help the user by setting Visible to false
            // and setting _initialVisibilitySet to true so they don't have to select this option twice on initial load.
            // See OnVisibleChanged method for more information; especially if the user doesn't have any Triggers setup.
            if (!_initialVisibilitySet && Visible)
            {
                Visible = false;
                _initialVisibilitySet = true;
            }

            if (Visible)
            {
                HideInterface();
            }
            else
            {
                ShowInterface();
            }
        }

        /// <summary>
        /// Hides the interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemHideInterface_Click(object sender, EventArgs e)
        {
            HideInterface();
        }

        /// <summary>
        /// Shows a small screen capture status window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemScreenCaptureStatus_Click(object sender, EventArgs e)
        {
            if (!_formScreenCaptureStatus.Visible)
            {
                _formScreenCaptureStatus.Show();
            }
            else
            {
                _formScreenCaptureStatus.Focus();
                _formScreenCaptureStatus.BringToFront();
            }
        }

        /// <summary>
        /// Shows the Dynamic Regex Validator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDynamicRegexValidator_Click(object sender, EventArgs e)
        {
            if (!_formDynamicRegexValidator.Visible)
            {
                _formDynamicRegexValidator.Show();
            }
            else
            {
                _formDynamicRegexValidator.Focus();
                _formDynamicRegexValidator.BringToFront();
            }
        }

        /// <summary>
        /// Shows the "About" window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            if (!_formAbout.Visible)
            {
                _formAbout.Show();
            }
            else
            {
                _formAbout.Focus();
                _formAbout.BringToFront();
            }
        }

        /// <summary>
        /// Shows the "Email Settings" window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemEmailSettings_Click(object sender, EventArgs e)
        {
            ShowInterface();

            if (!_formEmailSettings.Visible)
            {
                _formEmailSettings.ShowDialog(this);
            }
            else
            {
                _formEmailSettings.Focus();
                _formEmailSettings.BringToFront();
            }

            if (_formEmailSettings.DialogResult == DialogResult.OK)
            {
                BuildViewTabPages();
            }
        }

        /// <summary>
        /// Shows the "File Transfer Settings" window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemFileTransferSettings_Click(object sender, EventArgs e)
        {
            ShowInterface();

            if (!_formFileTransferSettings.Visible)
            {
                _formFileTransferSettings.ShowDialog(this);
            }
            else
            {
                _formFileTransferSettings.Focus();
                _formFileTransferSettings.BringToFront();
            }

            if (_formFileTransferSettings.DialogResult == DialogResult.OK)
            {
                BuildViewTabPages();
            }
        }

        private void toolStripMenuItemSetup_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedMenuItem = (ToolStripMenuItem)sender;

            _formSetup.ShowTabPage(selectedMenuItem.Text);

            _formSetup.ShowDialog(this);

            if (_formSetup.DialogResult == DialogResult.OK)
            {
                RegisterKeyboardShortcuts();
            }
        }

        private void toolStripDropDownButtonPreview_Click(object sender, EventArgs e)
        {
            _preview = !_preview;

            _config.Settings.User.SetValueByKey("Preview", _preview);

            ShowScreenshotBySlideIndex();

            SaveSettings();
        }

        private void toolStripMenuItemSetupWizard_Click(object sender, EventArgs e)
        {
            _formSetupWizard.ShowDialog(this);
        }
    }
}