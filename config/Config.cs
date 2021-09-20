﻿//-----------------------------------------------------------------------
// <copyright file="Config.cs" company="Gavin Kendall">
//     Copyright (c) 2008-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>The Auto Screen Capture Configuration File is a special file for specifying the folders and files to be used by Auto Screen Capture.</summary>
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
using System.Text.RegularExpressions;

namespace AutoScreenCapture
{
    /// <summary>
    /// The configuration file is used to define the folders and files that the application will use.
    /// </summary>
    public class Config
    {
        private const string REGEX_SCREENSHOTS_FOLDER = "^ScreenshotsFolder=(?<Path>.+)$";
        private const string REGEX_DEBUG_FOLDER = "^DebugFolder=(?<Path>.+)$";
        private const string REGEX_LOGS_FOLDER = "^LogsFolder=(?<Path>.+)$";

        private const string REGEX_COMMAND_FILE = "^CommandFile=(?<Path>.+)$";
        private const string REGEX_APPLICATION_SETTINGS_FILE = "^ApplicationSettingsFile=(?<Path>.+)$";
        private const string REGEX_SMTP_SETTINGS_FILE = "^SMTPSettingsFile=(?<Path>.+)$";
        private const string REGEX_SFTP_SETTINGS_FILE = "^SFTPSettingsFile=(?<Path>.+)$";
        private const string REGEX_USER_SETTINGS_FILE = "^UserSettingsFile=(?<Path>.+)$";
        private const string REGEX_EDITORS_FILE = "^EditorsFile=(?<Path>.+)$";
        private const string REGEX_EXTERNAL_PROGRAMS_FILE = "^ExternalProgramsFile=(?<Path>.+)$";
        private const string REGEX_REGIONS_FILE = "^RegionsFile=(?<Path>.+)$";
        private const string REGEX_SCREENS_FILE = "^ScreensFile=(?<Path>.+)$";
        private const string REGEX_TRIGGERS_FILE = "^TriggersFile=(?<Path>.+)$";
        private const string REGEX_SCREENSHOTS_FILE = "^ScreenshotsFile=(?<Path>.+)$";
        private const string REGEX_TAGS_FILE = "^TagsFile=(?<Path>.+)$";
        private const string REGEX_SCHEDULES_FILE = "^SchedulesFile=(?<Path>.+)$";

        /// <summary>
        /// The log file to use.
        /// </summary>
        public Log Log { get; set; }

        /// <summary>
        /// The settings for the application or the user.
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// Access to file system methods.
        /// </summary>
        public FileSystem FileSystem { get; set; }

        /// <summary>
        /// A parser for parsing macro tags.
        /// </summary>
        public MacroParser MacroParser { get; set; }

        /// <summary>
        /// A class for handling screen capture operations.
        /// </summary>
        public ScreenCapture ScreenCapture { get; set; }

        /// <summary>
        /// A class for handling configuration of the application.
        /// </summary>
        public Config()
        {

        }

        /// <summary>
        /// Loads the configuration file.
        /// </summary>
        public void Load(FileSystem fileSystem)
        {
            try
            {
                FileSystem = fileSystem;

                Settings = new Settings();
                MacroParser = new MacroParser(Settings);

                string configDirectory = FileSystem.GetDirectoryName(FileSystem.ConfigFile);

                if (!string.IsNullOrEmpty(configDirectory) && !FileSystem.DirectoryExists(configDirectory))
                {
                    FileSystem.CreateDirectory(configDirectory);
                }

                if (!FileSystem.FileExists(FileSystem.ConfigFile))
                {
                    string[] linesToWrite =
                    {
                        "# Auto Screen Capture Configuration File",
                        "# Use this file to tell the application what folders and files it should utilize.",
                        "# Each key-value pair can be the name of a folder or file or a path to a folder or file.",
                        "# If only the folder name is given then it will be parsed as the sub-folder of the folder",
                        "# where the executed autoscreen.exe binary is located.", "",
                        "# This is the folder where screenshots will be stored by default.",
                        "ScreenshotsFolder=" + FileSystem.DefaultScreenshotsFolder, "",
                        "# If any errors are encountered then you will find them in this folder when DebugMode is enabled.",
                        "DebugFolder=" + FileSystem.DefaultDebugFolder, "",
                        "# Logs are stored in this folder when either Logging or DebugMode is enabled.",
                        "LogsFolder=" + FileSystem.DefaultLogsFolder, "",
                        "# This file is monitored by the application for commands issued from the command line while it's running.",
                        "CommandFile=" + FileSystem.DefaultCommandFile, "",
                        "# The application settings (such as DebugMode).",
                        "ApplicationSettingsFile=" + FileSystem.DefaultApplicationSettingsFile, "",
                        "# Your personal settings.",
                        "UserSettingsFile=" + FileSystem.DefaultUserSettingsFile, "",
                        "# SMTP settings for emailing screenshots using an email server.",
                        "SMTPSettingsFile=" + FileSystem.DefaultSmtpSettingsFile, "",
                        "# SFTP settings for uploading screenshots to a file server.",
                        "SFTPSettingsFile=" + FileSystem.DefaultSftpSettingsFile, "",
                        "# References to image editors.",
                        "EditorsFile=" + FileSystem.DefaultEditorsFile, "",
                        "# References to external programs.",
                        "ExternalProgramsFile=" + FileSystem.DefaultExternalProgramsFile, "",
                        "# References to regions.",
                        "RegionsFile=" + FileSystem.DefaultRegionsFile, "",
                        "# References to screens.",
                        "ScreensFile=" + FileSystem.DefaultScreensFile, "",
                        "# References to triggers.",
                        "TriggersFile=" + FileSystem.DefaultTriggersFile, "",
                        "# References to screenshots.",
                        "ScreenshotsFile=" + FileSystem.DefaultScreenshotsFile, "",
                        "# References to tags.",
                        "TagsFile=" + FileSystem.DefaultTagsFile, "",
                        "# References to schedules.",
                        "SchedulesFile=" + FileSystem.DefaultSchedulesFile, ""
                    };

                    FileSystem.WriteToFile(FileSystem.ConfigFile, linesToWrite);
                }

                foreach (string line in FileSystem.ReadFromFile(FileSystem.ConfigFile))
                {
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    string path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_SCREENSHOTS_FOLDER, out path))
                        FileSystem.ScreenshotsFolder = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_DEBUG_FOLDER, out path))
                        FileSystem.DebugFolder = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_LOGS_FOLDER, out path))
                        FileSystem.LogsFolder = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_COMMAND_FILE, out path))
                        FileSystem.CommandFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_APPLICATION_SETTINGS_FILE, out path))
                        FileSystem.ApplicationSettingsFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_SMTP_SETTINGS_FILE, out path))
                        FileSystem.SmtpSettingsFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_SFTP_SETTINGS_FILE, out path))
                        FileSystem.SftpSettingsFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_USER_SETTINGS_FILE, out path))
                        FileSystem.UserSettingsFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_EDITORS_FILE, out path))
                        FileSystem.EditorsFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_EXTERNAL_PROGRAMS_FILE, out path))
                        FileSystem.ExternalProgramsFile = path;
                    
                    if (GetPathAndCreateIfNotFound(line, REGEX_REGIONS_FILE, out path))
                        FileSystem.RegionsFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_SCREENS_FILE, out path))
                        FileSystem.ScreensFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_TRIGGERS_FILE, out path))
                        FileSystem.TriggersFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_SCREENSHOTS_FILE, out path))
                        FileSystem.ScreenshotsFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_TAGS_FILE, out path))
                        FileSystem.TagsFile = path;

                    if (GetPathAndCreateIfNotFound(line, REGEX_SCHEDULES_FILE, out path))
                        FileSystem.SchedulesFile = path;
                }

                CheckAndCreateFolders();

                Settings.Load(FileSystem);
                Log = new Log(Settings, FileSystem, MacroParser);
                ScreenCapture = new ScreenCapture(this, FileSystem, Log);

                Security security = new Security();
                CheckAndCreateFiles(security, ScreenCapture, Log);
            }
            catch (Exception ex)
            {
                Log.WriteExceptionMessage("Config::Load", ex);
            }
        }

        // Check the folders to make sure that each folder was included in the config file and the folder exists.
        private void CheckAndCreateFolders()
        {
            if (string.IsNullOrEmpty(FileSystem.ScreenshotsFolder))
            {
                FileSystem.ScreenshotsFolder = FileSystem.DefaultScreenshotsFolder;

                FileSystem.AppendToFile(FileSystem.ConfigFile, "\nScreenshotsFolder=" + FileSystem.DefaultScreenshotsFolder);

                if (!FileSystem.DirectoryExists(FileSystem.DefaultScreenshotsFolder))
                {
                    FileSystem.CreateDirectory(FileSystem.DefaultScreenshotsFolder);
                }
            }

            if (string.IsNullOrEmpty(FileSystem.DebugFolder))
            {
                FileSystem.DebugFolder = FileSystem.DefaultDebugFolder;

                FileSystem.AppendToFile(FileSystem.ConfigFile, "\nDebugFolder=" + FileSystem.DefaultDebugFolder);

                if (!FileSystem.DirectoryExists(FileSystem.DefaultDebugFolder))
                {
                    FileSystem.CreateDirectory(FileSystem.DefaultDebugFolder);
                }
            }

            if (string.IsNullOrEmpty(FileSystem.LogsFolder))
            {
                FileSystem.LogsFolder = FileSystem.DefaultLogsFolder;

                FileSystem.AppendToFile(FileSystem.ConfigFile, "\nLogsFolder=" + FileSystem.DefaultLogsFolder);

                if (!FileSystem.DirectoryExists(FileSystem.DefaultLogsFolder))
                {
                    FileSystem.CreateDirectory(FileSystem.DefaultLogsFolder);
                }
            }
        }

        private void CheckAndCreateFiles(Security security, ScreenCapture screenCapture, Log log)
        {
            if (string.IsNullOrEmpty(FileSystem.CommandFile))
            {
                FileSystem.CommandFile = FileSystem.DefaultCommandFile;

                FileSystem.AppendToFile(FileSystem.ConfigFile, "\nCommandFile=" + FileSystem.DefaultCommandFile);

                if (!FileSystem.FileExists(FileSystem.DefaultCommandFile))
                {
                    FileSystem.CreateFile(FileSystem.DefaultCommandFile);
                }
            }

            if (string.IsNullOrEmpty(FileSystem.ApplicationSettingsFile))
            {
                FileSystem.ApplicationSettingsFile = FileSystem.DefaultApplicationSettingsFile;

                FileSystem.AppendToFile(FileSystem.ConfigFile, "\nApplicationSettingsFile=" + FileSystem.DefaultApplicationSettingsFile);

                if (!FileSystem.DirectoryExists(FileSystem.DefaultSettingsFolder))
                {
                    FileSystem.CreateDirectory(FileSystem.DefaultSettingsFolder);
                }
            }

            if (string.IsNullOrEmpty(FileSystem.SmtpSettingsFile))
            {
                FileSystem.SmtpSettingsFile = FileSystem.DefaultSmtpSettingsFile;

                FileSystem.AppendToFile(FileSystem.ConfigFile, "\nSMTPSettingsFile=" + FileSystem.DefaultSmtpSettingsFile);

                if (!FileSystem.DirectoryExists(FileSystem.DefaultSettingsFolder))
                {
                    FileSystem.CreateDirectory(FileSystem.DefaultSettingsFolder);
                }
            }

            if (string.IsNullOrEmpty(FileSystem.SftpSettingsFile))
            {
                FileSystem.SftpSettingsFile = FileSystem.DefaultSftpSettingsFile;

                FileSystem.AppendToFile(FileSystem.ConfigFile, "\nSFTPSettingsFile=" + FileSystem.DefaultSftpSettingsFile);

                if (!FileSystem.DirectoryExists(FileSystem.DefaultSettingsFolder))
                {
                    FileSystem.CreateDirectory(FileSystem.DefaultSettingsFolder);
                }
            }

            if (string.IsNullOrEmpty(FileSystem.UserSettingsFile))
            {
                FileSystem.UserSettingsFile = FileSystem.DefaultUserSettingsFile;

                FileSystem.AppendToFile(FileSystem.ConfigFile, "\nUserSettingsFile=" + FileSystem.DefaultUserSettingsFile);

                if (!FileSystem.DirectoryExists(FileSystem.DefaultSettingsFolder))
                {
                    FileSystem.CreateDirectory(FileSystem.DefaultSettingsFolder);
                }
            }

            Settings.User.Load(Settings, FileSystem);

            Settings.SMTP.Load(Settings, FileSystem);

            Settings.SFTP.Load(Settings, FileSystem);

            Settings.VersionManager.OldApplicationSettings = Settings.Application.Clone();

            Settings.VersionManager.OldUserSettings = Settings.User.Clone();

            Settings.UpgradeApplicationSettings(Settings.Application, FileSystem);

            Settings.UpgradeUserSettings(Settings.User, screenCapture, security, FileSystem);

            Settings.UpgradeSmtpSettings(Settings.SMTP, FileSystem);

            Settings.UpgradeSftpSettings(Settings.SFTP, FileSystem);

            if (string.IsNullOrEmpty(FileSystem.ScreenshotsFile))
            {
                ImageFormatCollection imageFormatCollection = new ImageFormatCollection();
                ScreenCollection screenCollection = new ScreenCollection();

                ScreenshotCollection screenshotCollection = new ScreenshotCollection(imageFormatCollection, screenCollection, screenCapture, this, FileSystem, log);
                screenshotCollection.SaveToXmlFile(this);
            }

            if (string.IsNullOrEmpty(FileSystem.EditorsFile))
            {
                // Loading the editor collection will automatically create the default editors and add them to the collection.
                EditorCollection editorCollection = new EditorCollection();
                editorCollection.LoadXmlFileAndAddEditors(this, FileSystem, log);
            }

            if (string.IsNullOrEmpty(FileSystem.ExternalProgramsFile))
            {
                // Loading the external program collection will automatically create the default external programs and add them to the collection.
                ExternalProgramCollection externalProgramsCollection = new ExternalProgramCollection();
                externalProgramsCollection.LoadXmlFileAndAddExternalPrograms(this, FileSystem, log);
            }

            if (string.IsNullOrEmpty(FileSystem.RegionsFile))
            {
                RegionCollection regionCollection = new RegionCollection();
                regionCollection.SaveToXmlFile(Settings, FileSystem, log);
            }

            if (string.IsNullOrEmpty(FileSystem.ScreensFile))
            {
                // Loading the screen collection will automatically create the available screens and add them to the collection.
                ScreenCollection screenCollection = new ScreenCollection();
                screenCollection.LoadXmlFileAndAddScreens(new ImageFormatCollection(), this, MacroParser, screenCapture, FileSystem, log);
            }

            if (string.IsNullOrEmpty(FileSystem.TriggersFile))
            {
                // Loading triggers will automatically create the default triggers and add them to the collection.
                TriggerCollection triggerCollection = new TriggerCollection();
                triggerCollection.LoadXmlFileAndAddTriggers(this, FileSystem, log);
            }

            if (string.IsNullOrEmpty(FileSystem.TagsFile))
            {
                // Loading tags will automatically create the default tags and add them to the collection.
                MacroTagCollection tagCollection = new MacroTagCollection();
                tagCollection.LoadXmlFileAndAddTags(this, MacroParser, FileSystem, log);
            }

            if (string.IsNullOrEmpty(FileSystem.SchedulesFile))
            {
                // Loading schedules will automatically create the default schedules and add them to the collection.
                ScheduleCollection scheduleCollection = new ScheduleCollection();
                scheduleCollection.LoadXmlFileAndAddSchedules(this, FileSystem, log);
            }
        }

        /// <summary>
        /// Gets the path from the configuration file based on what line is being processed and a regex pattern.
        /// If the path cannot be found then the directory or file will be created.
        /// </summary>
        /// <param name="line">The line to read from the file.</param>
        /// <param name="regex">The regex pattern to use against the line.</param>
        /// <param name="path">The output of the path being returned.</param>
        /// <returns>A boolean to indicate if getting a path was successful or not.</returns>
        private bool GetPathAndCreateIfNotFound(string line, string regex, out string path)
        {
            if (!Regex.IsMatch(line, regex))
            {
                path = null;
                return false;
            }

            path = Regex.Match(line, regex).Groups["Path"].Value;

            MacroTagCollection tagCollection = new MacroTagCollection();
            tagCollection.Add(new MacroTag(MacroParser, "user", "The user using this computer (%user%)", MacroTagType.User, active: true));
            tagCollection.Add(new MacroTag(MacroParser, "machine", "The name of the computer (%machine%)", MacroTagType.Machine, active: true));

            path = MacroParser.ParseTags(config: true, path, tagCollection, Log);

            if (FileSystem.HasExtension(path))
            {
                string dir = FileSystem.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(dir) && !FileSystem.DirectoryExists(dir))
                {
                    FileSystem.CreateDirectory(dir);
                }
            }
            else
            {
                if (!path.EndsWith(FileSystem.DirectorySeparatorChar().ToString()))
                {
                    path += FileSystem.DirectorySeparatorChar();
                }

                if (!string.IsNullOrEmpty(path) && !FileSystem.DirectoryExists(path))
                {
                    FileSystem.CreateDirectory(path);
                }
            }

            return true;
        }
    }
}
