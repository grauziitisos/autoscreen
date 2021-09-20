//-----------------------------------------------------------------------
// <copyright file="ExternalProgramCollection.cs" company="Gavin Kendall">
//     Copyright (c) 2008-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A collection of ExternalPrograms.</summary>
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
using System.Text;
using System.Xml;

namespace AutoScreenCapture
{
        /// <summary>
        /// A collection class to store and manage ExternalProgram objects.
        /// </summary>
        public class ExternalProgramCollection : CollectionTemplate<ExternalProgram>
        {
            private const string XML_FILE_INDENT_CHARS = "   ";
            private const string XML_FILE_EXTERNAL_PROGRAM_NODE = "externalprogram";
            private const string XML_FILE_EXTERNAL_PROGRAMS_NODE = "externalprograms";
            private const string XML_FILE_ROOT_NODE = "autoscreen";

            private const string EXTERNAL_PROGRAM_NAME = "name";
            private const string EXTERNAL_PROGRAM_ARGUMENTS = "arguments";
            private const string EXTERNAL_PROGRAM_APPLICATION = "application";
            private const string EXTERNAL_PROGRAM_NOTES = "notes";

            private readonly string EXTERNAL_PROGRAM_XPATH;

            private string AppCodename { get; set; }
            private string AppVersion { get; set; }

            /// <summary>
            /// The empty constructor for the externalprogram collection.
            /// </summary>
            public ExternalProgramCollection()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("/");
                sb.Append(XML_FILE_ROOT_NODE);
                sb.Append("/");
                sb.Append(XML_FILE_EXTERNAL_PROGRAMS_NODE);
                sb.Append("/");
                sb.Append(XML_FILE_EXTERNAL_PROGRAM_NODE);

                EXTERNAL_PROGRAM_XPATH = sb.ToString();
            }

            /// <summary>
            /// Loads the image externalprograms from the externalprograms.xml file.
            /// </summary>
            public bool LoadXmlFileAndAddExternalPrograms(Config config, FileSystem fileSystem, Log log)
            {
                try
                {
                    if (fileSystem.FileExists(fileSystem.ExternalProgramsFile))
                    {
                        log.WriteDebugMessage("ExternalPrograms file \"" + fileSystem.ExternalProgramsFile + "\" found. Attempting to load XML document");

                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(fileSystem.ExternalProgramsFile);

                        log.WriteDebugMessage("XML document loaded");

                        AppVersion = xDoc.SelectSingleNode("/autoscreen").Attributes["app:version"]?.Value;
                        AppCodename = xDoc.SelectSingleNode("/autoscreen").Attributes["app:codename"]?.Value;

                        XmlNodeList xExternalPrograms = xDoc.SelectNodes(EXTERNAL_PROGRAM_XPATH);

                        foreach (XmlNode xExternalProgram in xExternalPrograms)
                        {
                            ExternalProgram externalprogram = new ExternalProgram();
                            XmlNodeReader xReader = new XmlNodeReader(xExternalProgram);

                            while (xReader.Read())
                            {
                                if (xReader.IsStartElement() && !xReader.IsEmptyElement)
                                {
                                    switch (xReader.Name)
                                    {
                                        case EXTERNAL_PROGRAM_NAME:
                                            xReader.Read();
                                            externalprogram.Name = xReader.Value;
                                            break;

                                        case EXTERNAL_PROGRAM_APPLICATION:
                                            xReader.Read();
                                            externalprogram.Application = xReader.Value;
                                            break;

                                        case EXTERNAL_PROGRAM_ARGUMENTS:
                                            xReader.Read();

                                            string value = xReader.Value;

                                            // Change the data for each Tag that's being loaded if we've detected that
                                            // the XML document is from an older version of the application.
                                            if (config.Settings.VersionManager.IsOldAppVersion(config.Settings, AppCodename, AppVersion))
                                            {
                                                log.WriteDebugMessage("An old version of the externalprograms.xml file was detected. Attempting upgrade to new schema.");

                                                Version v2300 = config.Settings.VersionManager.Versions.Get("Boombayah", "2.3.0.0");
                                                Version configVersion = config.Settings.VersionManager.Versions.Get(AppCodename, AppVersion);

                                                if (v2300 != null && configVersion != null && configVersion.VersionNumber < v2300.VersionNumber)
                                                {
                                                    log.WriteDebugMessage("Dalek 2.2.4.6 or older detected");

                                                    // Starting with 2.3.0.0 the %screenshot% argument tag became the %filepath% argument tag.
                                                    value = value.Replace("%screenshot%", "%filepath%");

                                                    // Set this externalprogram as the default externalprogram. Version 2.3 requires at least one externalprogram to be the default externalprogram.
                                                    config.Settings.User.SetValueByKey("DefaultExternalProgram", externalprogram.Name);
                                                }
                                            }

                                            externalprogram.Arguments = value;
                                            break;

                                        case EXTERNAL_PROGRAM_NOTES:
                                            xReader.Read();
                                            externalprogram.Notes = xReader.Value;
                                            break;
                                    }
                                }
                            }

                            xReader.Close();

                            if (!string.IsNullOrEmpty(externalprogram.Name) &&
                                !string.IsNullOrEmpty(externalprogram.Application))
                            {
                                Add(externalprogram);
                            }
                        }

                        if (config.Settings.VersionManager.IsOldAppVersion(config.Settings, AppCodename, AppVersion))
                        {
                            log.WriteDebugMessage("ExternalPrograms file detected as an old version");
                            SaveToXmlFile(config.Settings, fileSystem, log);
                        }
                    }
                    else
                    {
                        log.WriteDebugMessage("WARNING: Unable to load externalprograms");

                        // Setup default externalprograms.
                        // This is going to get maintenance heavy. I just know it.

                        // Microsoft Outlook
                        if (fileSystem.FileExists(@"C:\Program Files\Microsoft Office\root\Office16\OUTLOOK.EXE"))
                        {
                            Add(new ExternalProgram("Microsoft Outlook", @"C:\Program Files\Microsoft Office\root\Office16\OUTLOOK.EXE", "/c ipm.note /a %filepath%"));
                        }

                        // Chrome
                        if (fileSystem.FileExists(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"))
                        {
                            Add(new ExternalProgram("Chrome", @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "%filepath%"));
                        }

                        // Firefox
                        if (fileSystem.FileExists(@"C:\Program Files\Mozilla Firefox\firefox.exe"))
                        {
                            Add(new ExternalProgram("Firefox", @"C:\Program Files\Mozilla Firefox\firefox.exe", "%filepath%"));
                        }

                        SaveToXmlFile(config.Settings, fileSystem, log);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    log.WriteExceptionMessage("ExternalProgramCollection::LoadXmlFileAndAddExternalPrograms", ex);

                    return false;
                }
            }

            /// <summary>
            /// Saves the image externalprograms in the collection to the externalprograms.xml file.
            /// </summary>
            public bool SaveToXmlFile(Settings settings, FileSystem fileSystem, Log log)
            {
                try
                {
                    XmlWriterSettings xSettings = new XmlWriterSettings
                    {
                        Indent = true,
                        CloseOutput = true,
                        CheckCharacters = true,
                        Encoding = Encoding.UTF8,
                        NewLineChars = Environment.NewLine,
                        IndentChars = XML_FILE_INDENT_CHARS,
                        NewLineHandling = NewLineHandling.Entitize,
                        ConformanceLevel = ConformanceLevel.Document
                    };

                    if (string.IsNullOrEmpty(fileSystem.ExternalProgramsFile))
                    {
                        fileSystem.ExternalProgramsFile = fileSystem.DefaultExternalProgramsFile;

                        if (fileSystem.FileExists(fileSystem.ConfigFile))
                        {
                            fileSystem.AppendToFile(fileSystem.ConfigFile, "\nExternalProgramsFile=" + fileSystem.ExternalProgramsFile);
                        }
                    }

                    if (fileSystem.FileExists(fileSystem.ExternalProgramsFile))
                    {
                        fileSystem.DeleteFile(fileSystem.ExternalProgramsFile);
                    }

                    using (XmlWriter xWriter = XmlWriter.Create(fileSystem.ExternalProgramsFile, xSettings))
                    {
                        xWriter.WriteStartDocument();
                        xWriter.WriteStartElement(XML_FILE_ROOT_NODE);
                        xWriter.WriteAttributeString("app", "version", XML_FILE_ROOT_NODE, settings.ApplicationVersion);
                        xWriter.WriteAttributeString("app", "codename", XML_FILE_ROOT_NODE, settings.ApplicationCodename);
                        xWriter.WriteStartElement(XML_FILE_EXTERNAL_PROGRAMS_NODE);

                        foreach (ExternalProgram externalprogram in base.Collection)
                        {
                            xWriter.WriteStartElement(XML_FILE_EXTERNAL_PROGRAM_NODE);
                            xWriter.WriteElementString(EXTERNAL_PROGRAM_NAME, externalprogram.Name);
                            xWriter.WriteElementString(EXTERNAL_PROGRAM_APPLICATION, externalprogram.Application);
                            xWriter.WriteElementString(EXTERNAL_PROGRAM_ARGUMENTS, externalprogram.Arguments);
                            xWriter.WriteElementString(EXTERNAL_PROGRAM_NOTES, externalprogram.Notes);

                            xWriter.WriteEndElement();
                        }

                        xWriter.WriteEndElement();
                        xWriter.WriteEndElement();
                        xWriter.WriteEndDocument();

                        xWriter.Flush();
                        xWriter.Close();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    log.WriteExceptionMessage("ExternalProgramCollection::SaveToXmlFile", ex);

                    return false;
                }
            }
        }
}