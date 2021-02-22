﻿//-----------------------------------------------------------------------
// <copyright file="FormScreen.cs" company="Gavin Kendall">
//     Copyright (c) 2020 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A form for adding a new screen or changing an existing screen.</summary>
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
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutoScreenCapture
{
    /// <summary>
    /// A class for handling screens.
    /// </summary>
    public partial class FormScreen : Form
    {
        private Log _log;
        private MacroParser _macroParser;
        private ScreenCapture _screenCapture;
        private FileSystem _fileSystem;

        private FormMacroTagsToolWindow _formMacroTags;

        /// <summary>
        /// A collection of screens.
        /// </summary>
        public ScreenCollection ScreenCollection { get; } = new ScreenCollection();

        /// <summary>
        /// The current screen object this form handles when creating a new screen or changing a screen.
        /// </summary>
        public Screen ScreenObject { get; set; }

        /// <summary>
        /// A collection of image formats.
        /// </summary>
        public ImageFormatCollection ImageFormatCollection { get; set; }

        /// <summary>
        /// A collection of tags to be used for macro parsing.
        /// </summary>
        public TagCollection TagCollection { get; set; }

        /// <summary>
        /// A dictionary of available screens by device resolution.
        /// </summary>
        public Dictionary<int, ScreenCapture.DeviceOptions> ScreenDictionary;

        /// <summary>
        /// Constructor for FormScreen.
        /// </summary>
        public FormScreen(ScreenCapture screenCapture, MacroParser macroParser, FileSystem fileSystem, Log log)
        {
            InitializeComponent();

            _log = log;
            _macroParser = macroParser;
            _screenCapture = screenCapture;
            _fileSystem = fileSystem;

            ScreenDictionary = new Dictionary<int, ScreenCapture.DeviceOptions>();

            RefreshScreenDictionary();
        }

        private void FormScreen_Load(object sender, EventArgs e)
        {
            textBoxScreenName.Focus();

            HelpMessage("This is where to configure a screen capture. Select an available screen from the Component drop-down menu and keep an eye on Preview");

            comboBoxFormat.Items.Clear();
            comboBoxScreenComponent.Items.Clear();

            pictureBoxPreview.Image = null;

            foreach (ImageFormat imageFormat in ImageFormatCollection)
            {
                comboBoxFormat.Items.Add(imageFormat.Name);
            }

            comboBoxScreenComponent.Items.Add("Active Window");

            for (int i = 1; i <= ScreenDictionary.Count; i++)
            {
                ScreenCapture.DeviceOptions deviceOptions = ScreenDictionary[i];
                comboBoxScreenComponent.Items.Add("Screen " + i + " (" + deviceOptions.width + " x " + deviceOptions.height+ ")");
            }

            if (ScreenObject != null)
            {
                Text = "Change Screen";

                textBoxScreenName.Text = ScreenObject.Name;
                textBoxFolder.Text = _fileSystem.CorrectScreenshotsFolderPath(ScreenObject.Folder);
                textBoxMacro.Text = ScreenObject.Macro;

                if (ScreenObject.Component < comboBoxScreenComponent.Items.Count)
                {
                    comboBoxScreenComponent.SelectedIndex = ScreenObject.Component;
                }
                else
                {
                    comboBoxScreenComponent.SelectedIndex = 0;
                    MessageBox.Show("The configured screen component has an invalid index since it is not available on this system. The component has therefore been set to Active Window.", "Invalid Screen Index", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                comboBoxFormat.SelectedItem = ScreenObject.Format.Name;
                numericUpDownJpegQuality.Value = ScreenObject.JpegQuality;
                checkBoxMouse.Checked = ScreenObject.Mouse;
                checkBoxActive.Checked = ScreenObject.Active;
            }
            else
            {
                Text = "Add New Screen";

                textBoxScreenName.Text = "Screen " + (ScreenCollection.Count + 1);
                textBoxFolder.Text = _fileSystem.ScreenshotsFolder;
                textBoxMacro.Text = _macroParser.DefaultMacro;
                comboBoxScreenComponent.SelectedIndex = 0;
                comboBoxFormat.SelectedItem = ScreenCapture.DefaultImageFormat;
                numericUpDownJpegQuality.Value = 100;
                checkBoxMouse.Checked = true;
                checkBoxActive.Checked = true;
            }

            UpdatePreviewMacro();
            UpdatePreviewImage(_screenCapture);
        }

        private void HelpMessage(string message)
        {
            labelHelp.Text = "       " + message;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (ScreenObject != null)
            {
                ChangeScreen();
            }
            else
            {
                AddNewScreen();
            }
        }

        /// <summary>
        /// Updates the screen dictionary with the available screens.
        /// </summary>
        public void RefreshScreenDictionary()
        {
            ScreenDictionary.Clear();

            int component = 1;

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                ScreenDictionary.Add(component, _screenCapture.GetDevice(screen));
                component++;
            }
        }

        private void AddNewScreen()
        {
            if (InputValid())
            {
                TrimInput();

                if (ScreenCollection.GetByName(textBoxScreenName.Text) == null)
                {
                    ScreenCollection.Add(new Screen()
                    {
                        ViewId = Guid.NewGuid(),
                        Name = textBoxScreenName.Text,
                        Folder = _fileSystem.CorrectScreenshotsFolderPath(textBoxFolder.Text),
                        Macro = textBoxMacro.Text,
                        Component = comboBoxScreenComponent.SelectedIndex,
                        Format = ImageFormatCollection.GetByName(comboBoxFormat.Text),
                        JpegQuality = (int)numericUpDownJpegQuality.Value,
                        Mouse = checkBoxMouse.Checked,
                        Active = checkBoxActive.Checked
                    });

                    Okay();
                }
                else
                {
                    MessageBox.Show("A screen with this name already exists.", "Duplicate Name Conflict",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter valid input for each field.", "Invalid Input", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ChangeScreen()
        {
            if (InputValid())
            {
                if (NameChanged() || InputChanged())
                {
                    TrimInput();

                    if (ScreenCollection.GetByName(textBoxScreenName.Text) != null && NameChanged())
                    {
                        MessageBox.Show("A screen with this name already exists.", "Duplicate Name Conflict",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        ScreenCollection.Get(ScreenObject).Name = textBoxScreenName.Text;
                        ScreenCollection.Get(ScreenObject).Folder = _fileSystem.CorrectScreenshotsFolderPath(textBoxFolder.Text);
                        ScreenCollection.Get(ScreenObject).Macro = textBoxMacro.Text;
                        ScreenCollection.Get(ScreenObject).Component = comboBoxScreenComponent.SelectedIndex;
                        ScreenCollection.Get(ScreenObject).Format = ImageFormatCollection.GetByName(comboBoxFormat.Text);
                        ScreenCollection.Get(ScreenObject).JpegQuality = (int) numericUpDownJpegQuality.Value;
                        ScreenCollection.Get(ScreenObject).Mouse = checkBoxMouse.Checked;
                        ScreenCollection.Get(ScreenObject).Active = checkBoxActive.Checked;

                        Okay();
                    }
                }
                else
                {
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Please enter valid input for each field.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TrimInput()
        {
            textBoxScreenName.Text = textBoxScreenName.Text.Trim();
            textBoxFolder.Text = textBoxFolder.Text.Trim();
            textBoxMacro.Text = textBoxMacro.Text.Trim();
        }

        private bool InputValid()
        {
            if (!string.IsNullOrEmpty(textBoxScreenName.Text) &&
                !string.IsNullOrEmpty(textBoxFolder.Text) &&
                !string.IsNullOrEmpty(textBoxMacro.Text))
            {
                return true;
            }

            return false;
        }

        private bool InputChanged()
        {
            if (ScreenObject != null &&
                (!ScreenObject.Folder.Equals(textBoxFolder.Text) ||
                 !ScreenObject.Macro.Equals(textBoxMacro.Text) ||
                 ScreenObject.Component != comboBoxScreenComponent.SelectedIndex ||
                 !ScreenObject.Format.Equals(comboBoxFormat.SelectedItem) ||
                 ScreenObject.JpegQuality != (int)numericUpDownJpegQuality.Value ||
                 !ScreenObject.Mouse.Equals(checkBoxMouse.Checked) ||
                 !ScreenObject.Active.Equals(checkBoxActive.Checked)))
            {
                return true;
            }

            return false;
        }

        private bool NameChanged()
        {
            if (ScreenObject != null &&
                !ScreenObject.Name.Equals(textBoxScreenName.Text))
            {
                return true;
            }

            return false;
        }

        private void Okay()
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();

            if (browser.ShowDialog() == DialogResult.OK)
            {
                textBoxFolder.Text = browser.SelectedPath;
            }
        }

        private void UpdatePreviewImage(ScreenCapture screenCapture)
        {
            try
            {
                if (checkBoxActive.Checked)
                {
                    if (comboBoxScreenComponent.SelectedIndex == 0)
                    {
                        pictureBoxPreview.Image = screenCapture.GetActiveWindowBitmap();
                    }
                    else
                    {
                        System.Windows.Forms.Screen screen = GetScreenByIndex(comboBoxScreenComponent.SelectedIndex);

                        pictureBoxPreview.Image = screen != null
                            ? screenCapture.GetScreenBitmap(
                                screen.Bounds.X,
                                screen.Bounds.Y,
                                screen.Bounds.Width,
                                screen.Bounds.Height,
                                checkBoxMouse.Checked
                            )
                            : null;
                    }

                    UpdatePreviewMacro();
                }
                else
                {
                    pictureBoxPreview.Image = null;

                    textBoxMacroPreview.ForeColor = System.Drawing.Color.White;
                    textBoxMacroPreview.BackColor = System.Drawing.Color.Black;
                    textBoxMacroPreview.Text = "[Active option is off. No screenshots of this screen will be taken during a running screen capture session]";
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionMessage("FormScreen::UpdatePreviewImage", ex);
            }
        }

        private void UpdatePreviewMacro()
        {
            textBoxMacroPreview.ForeColor = System.Drawing.Color.Black;
            textBoxMacroPreview.BackColor = System.Drawing.Color.LightYellow;

            textBoxMacroPreview.Text = _macroParser.ParseTags(config: false, textBoxFolder.Text, TagCollection, _log) +
                _macroParser.ParseTags(preview: true, config: false, textBoxScreenName.Text, textBoxMacro.Text, 1,
                ImageFormatCollection.GetByName(comboBoxFormat.Text), Text, TagCollection, _log);
        }

        private System.Windows.Forms.Screen GetScreenByIndex(int index)
        {
            try
            {
                ScreenCapture.DeviceOptions deviceResolution = ScreenDictionary[index];

                return deviceResolution.screen;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        private void comboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFormat.Text.Equals("JPEG"))
            {
                numericUpDownJpegQuality.Enabled = true;
            }
            else
            {
                numericUpDownJpegQuality.Enabled = false;
            }

            updatePreviewMacro(sender, e);
        }

        private void updatePreviewImage(object sender, EventArgs e)
        {
            UpdatePreviewImage(_screenCapture);
        }

        private void updatePreviewMacro(object sender, EventArgs e)
        {
            UpdatePreviewMacro();
        }

        private void buttonMacroTags_Click(object sender, EventArgs e)
        {
            if (_formMacroTags == null || _formMacroTags.IsDisposed)
            {
                _formMacroTags = new FormMacroTagsToolWindow(TagCollection, _macroParser, _log);
                _formMacroTags.Show();
            }
            else
            {
                _formMacroTags.BringToFront();
            }
        }

        private void checkBoxMouse_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("You can include the mouse pointer in your screenshots if the \"Include mouse pointer\" option is checked");
        }

        private void comboBoxFormat_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("Change the image format for the screenshots taken by this screen capture. JPEG is the recommended image format");
        }

        private void checkBoxActive_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("You can capture this screen if Active is checked (turned on)");
        }

        private void textBoxFolder_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("The folder where to store the files of the screenshots being taken");
        }

        private void buttonScreenBrowseFolder_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("Browse for a folder where screenshots of this screen capture will be saved to");
        }

        private void textBoxMacro_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("Macro tags are used for acquiring information associated with a particular tag (such as %date% and %time% for the current date and time)");
        }

        private void buttonMacroTags_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("Open a list of available macro tags. You can keep the Macro Tags window open while you modify your macro");
        }

        private void pictureBoxPreview_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("A preview of what will be captured during a running screen capture session. Click to update the preview image");
        }

        private void textBoxMacroPreview_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("A preview of how your files will be named. Use macro tags (such as %date% and %time%) in the Macro field to customize the filename pattern");
        }

        private void checkBoxActiveWindowTitle_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("If checked then the text you define will be compared with the active window title");
        }

        private void textBoxActiveWindowTitle_MouseHover(object sender, EventArgs e)
        {
            HelpMessage("The text to compare with the active window title. If it contains the defined text then this screen will be captured. An empty field will be ignored");
        }

        private void FormScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_formMacroTags != null)
            {
                _formMacroTags.Close();
            }
        }
    }
}