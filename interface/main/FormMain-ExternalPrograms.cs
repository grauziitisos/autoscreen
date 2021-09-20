//-----------------------------------------------------------------------
// <copyright file="FormMain-ExternalPrograms.cs" company="Gavin Kendall">
//     Copyright (c) 2008-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>All the methods for handling ExternalPrograms.</summary>
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
using System.Diagnostics;
using System.Windows.Forms;

namespace AutoScreenCapture
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// Shows the "Add ExternalProgram" window to enable the user to add a chosen ExternalProgram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addExternalProgram_Click(object sender, EventArgs e)
        {
            ShowInterface();

            _formExternalProgram.ExternalProgramObject = null;

            if (!_formExternalProgram.Visible)
            {
                _formExternalProgram.ShowDialog(this);
            }
            else
            {
                _formExternalProgram.Focus();
                _formExternalProgram.BringToFront();
            }

            if (_formExternalProgram.DialogResult == DialogResult.OK)
            {
                BuildExternalProgramsModule();
                BuildViewTabPages();
                BuildScreenshotPreviewContextualMenu();

                if (!_formExternalProgram.ExternalProgramCollection.SaveToXmlFile(_config.Settings, _fileSystem, _log))
                {
                    _screenCapture.ApplicationError = true;
                }
            }
        }

        /// <summary>
        /// Removes the selected ExternalPrograms from the ExternalPrograms tab page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedExternalPrograms_Click(object sender, EventArgs e)
        {
            int countBeforeRemoval = _formExternalProgram.ExternalProgramCollection.Count;

            foreach (Control control in tabPageExternalPrograms.Controls)
            {
                if (control.GetType().Equals(typeof(CheckBox)))
                {
                    CheckBox checkBox = (CheckBox)control;

                    if (checkBox.Checked)
                    {
                        ExternalProgram ExternalProgram = _formExternalProgram.ExternalProgramCollection.Get((ExternalProgram)checkBox.Tag);
                        _formExternalProgram.ExternalProgramCollection.Remove(ExternalProgram);
                    }
                }
            }

            if (countBeforeRemoval > _formExternalProgram.ExternalProgramCollection.Count)
            {
                BuildExternalProgramsModule();
                BuildViewTabPages();
                BuildScreenshotPreviewContextualMenu();

                if (!_formExternalProgram.ExternalProgramCollection.SaveToXmlFile(_config.Settings, _fileSystem, _log))
                {
                    _screenCapture.ApplicationError = true;
                }
            }
        }

        /// <summary>
        /// Runs the chosen image ExternalProgram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runExternalProgram_Click(object sender, EventArgs e)
        {
            string ExternalProgramName = sender.ToString();

            if (ExternalProgramName.Equals("Edit"))
            {
                ExternalProgramName = _config.Settings.User.GetByKey("DefaultExternalProgram", _config.Settings.DefaultSettings.DefaultExternalProgram).Value.ToString();
            }

            ExternalProgram ExternalProgram = _formExternalProgram.ExternalProgramCollection.GetByName(ExternalProgramName);

            if (!RunExternalProgram(ExternalProgram, _slideShow.SelectedSlide))
            {
                MessageBox.Show("No image is available to edit.", "No Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Shows the "Change ExternalProgram" window to enable the user to edit a chosen ExternalProgram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeExternalProgram_Click(object sender, EventArgs e)
        {
            ShowInterface();

            ExternalProgram ExternalProgram = new ExternalProgram();

            if (sender is Button)
            {
                Button buttonSelected = (Button)sender;
                ExternalProgram = (ExternalProgram)buttonSelected.Tag;
            }

            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem toolStripMenuItemSelected = (ToolStripMenuItem)sender;
                ExternalProgram = (ExternalProgram)toolStripMenuItemSelected.Tag;
            }

            _formExternalProgram.ExternalProgramObject = ExternalProgram;

            _formExternalProgram.ShowDialog(this);

            if (_formExternalProgram.DialogResult == DialogResult.OK)
            {
                BuildExternalProgramsModule();
                BuildViewTabPages();
                BuildScreenshotPreviewContextualMenu();

                if (!_formExternalProgram.ExternalProgramCollection.SaveToXmlFile(_config.Settings, _fileSystem, _log))
                {
                    _screenCapture.ApplicationError = true;
                }
            }
        }

        /// <summary>
        /// Executes a chosen image ExternalProgram from the interface.
        /// </summary>
        /// <param name="ExternalProgram">The image ExternalProgram to execute.</param>
        /// <param name="slide">The slide to use when running the ExternalProgram.</param>
        private bool RunExternalProgram(ExternalProgram ExternalProgram, Slide slide)
        {
            if (ExternalProgram != null && slide != null)
            {
                Screenshot selectedScreenshot = null;

                if (tabControlViews.SelectedTab.Tag.GetType() == typeof(Screen))
                {
                    Screen screen = (Screen)tabControlViews.SelectedTab.Tag;
                    selectedScreenshot = _screenshotCollection.GetScreenshot(slide.Name, screen.ViewId);
                }
                
                if (tabControlViews.SelectedTab.Tag.GetType() == typeof(Region))
                {
                    Region region = (Region)tabControlViews.SelectedTab.Tag;
                    selectedScreenshot = _screenshotCollection.GetScreenshot(slide.Name, region.ViewId);
                }

                if (selectedScreenshot.ViewId.Equals(Guid.Empty))
                {
                    // *** Auto Screen Capture - Region Select / Auto Save ***
                    selectedScreenshot = _screenshotCollection.GetScreenshot(slide.Name, Guid.Empty);
                }

                if (selectedScreenshot != null)
                {
                    return RunExternalProgram(ExternalProgram, selectedScreenshot);
                }
            }

            return false;
        }

        /// <summary>
        /// Executes a chosen image ExternalProgram from a Trigger to open the last set of screenshots taken with the image ExternalProgram.
        /// </summary>
        /// <param name="ExternalProgram">The image ExternalProgram to execute.</param>
        /// <param name="triggerActionType">The trigger's action type.</param>
        private void RunExternalProgram(ExternalProgram ExternalProgram, TriggerActionType triggerActionType)
        {
            if (ExternalProgram != null && triggerActionType == TriggerActionType.RunExternalProgram)
            {
                // Assume we're going to be passing in the path of the screenshot image to the program.
                if (ExternalProgram.Arguments != null && ExternalProgram.Arguments.Contains("%filepath%"))
                {
                    DateTime dt = _screenCapture.DateTimeScreenshotsTaken;
                    //"one liner": (but performance...)
                    //Screenshot screenshot = _screenshotCollection.GetScreenshots(dt.ToString(_macroParser.DateFormat), dt.ToString(_macroParser.TimeFormat)).FindLast(a => true);
                    System.Collections.Generic.List<Screenshot> list = _screenshotCollection.GetScreenshots(dt.ToString(_macroParser.DateFormat), dt.ToString(_macroParser.TimeFormat));
                    Screenshot screenshot = (list == null ? null : list.Count==0 ? null : list[list.Count-1]) ;
                        if (screenshot != null && screenshot.Slide != null && !string.IsNullOrEmpty(screenshot.Path))
                        {
                            _log.WriteDebugMessage("Running ExternalProgram (based on TriggerActionType.RunExternalProgram) \"" + ExternalProgram.Name + "\" using screenshot path \"" + screenshot.Path + "\"");

                            if (!RunExternalProgram(ExternalProgram, screenshot))
                            {
                                _log.WriteDebugMessage("Running ExternalProgram failed. Perhaps the filepath of the screenshot file is no longer available");
                            }
                        }
                }
                else
                {
                    // Just run the program without passing in the path of the screenshot.
                    if (!RunExternalProgram(ExternalProgram))
                    {
                        _log.WriteDebugMessage("Running ExternalProgram failed.");
                    }
                }
            }
        }

        /// <summary>
        /// Runs the ExternalProgram using the specified screenshot.
        /// </summary>
        /// <param name="ExternalProgram">The ExternalProgram to use.</param>
        /// <param name="screenshot">The screenshot to use.</param>
        /// <returns>Determines if the ExternalProgram was executed successfully or not.</returns>
        private bool RunExternalProgram(ExternalProgram ExternalProgram, Screenshot screenshot)
        {
            // Execute the chosen image ExternalProgram. If the %filepath% argument happens to be included
            // then we'll use that argument as the screenshot file path when executing the image ExternalProgram.
            if (ExternalProgram != null && (screenshot != null && !string.IsNullOrEmpty(screenshot.Path) &&
                _fileSystem.FileExists(ExternalProgram.Application) && _fileSystem.FileExists(screenshot.Path)))
            {
                _log.WriteDebugMessage("Starting process for ExternalProgram \"" + ExternalProgram.Name + "\" ...");
                _log.WriteDebugMessage("Application: " + ExternalProgram.Application);
                _log.WriteDebugMessage("Arguments before %filepath% tag replacement: " + ExternalProgram.Arguments);
                _log.WriteDebugMessage("Arguments after %filepath% tag replacement: " + ExternalProgram.Arguments.Replace("%filepath%", "\"" + screenshot.Path + "\""));

                _ = Process.Start(ExternalProgram.Application, ExternalProgram.Arguments.Replace("%filepath%", "\"" + screenshot.Path + "\""));

                // We successfully opened the ExternalProgram with the given screenshot path.
                return true;
            }

            // We failed to open the ExternalProgram with the given screenshot path.
            return false;
        }

        /// <summary>
        /// Runs an ExternalProgram.
        /// </summary>
        /// <param name="ExternalProgram">The ExternalProgram to run.</param>
        /// <returns>Determines if the ExternalProgram was executed successfully or not.</returns>
        private bool RunExternalProgram(ExternalProgram ExternalProgram)
        {
            try
            {
                if (ExternalProgram != null && _fileSystem.FileExists(ExternalProgram.Application))
                {
                    if (ExternalProgram.Arguments != null && !string.IsNullOrEmpty(ExternalProgram.Arguments))
                    {
                        _ = Process.Start(ExternalProgram.Application, ExternalProgram.Arguments);
                    }
                    else
                    {
                        _ = Process.Start(ExternalProgram.Application);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _log.WriteExceptionMessage("FormMain-ExternalPrograms::RunExternalProgram", ex);

                return false;
            }
        }
    }
}
