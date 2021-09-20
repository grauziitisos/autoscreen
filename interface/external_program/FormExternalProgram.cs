//-----------------------------------------------------------------------
// <copyright file="FormExternalProgram.cs" company="Gavin Kendall">
//     Copyright (c) 2008-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A form for adding a new external program or changing an existing external program.</summary>
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
using System.Drawing;
using System.Windows.Forms;

namespace AutoScreenCapture
{
    /// <summary>
    /// The form for managing external programs.
    /// </summary>
    public partial class FormExternalProgram : Form
    {
        private Config _config;
        private FileSystem _fileSystem;

        private ToolTip _toolTip = new ToolTip();

        /// <summary>
        /// A collection of external programs.
        /// </summary>
        public ExternalProgramCollection ExternalProgramCollection { get; } = new ExternalProgramCollection();

        /// <summary>
        /// The external program object to handle.
        /// </summary>
        public ExternalProgram ExternalProgramObject { get; set; }

        private readonly string defaultArguments = "%filepath%";

        private ComponentResourceManager resources = new ComponentResourceManager(typeof(FormExternalProgram));

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public FormExternalProgram(Config config, FileSystem fileSystem)
        {
            InitializeComponent();

            _config = config;
            _fileSystem = fileSystem;
        }

        private void FormExternalProgram_Load(object sender, EventArgs e)
        {
            textBoxName.Focus();

            HelpMessage("This is where to configure an application or script for editing screenshots. The optional %filepath% argument is the filepath of the screenshot");

            _toolTip.SetToolTip(checkBoxMakeDefaultExternalProgram, "When checked it will make this external program the default external program");
            _toolTip.SetToolTip(buttonChooseExternalProgram, "Browse for an application or script");

            checkBoxMakeDefaultExternalProgram.Checked = false;

            if (ExternalProgramObject != null)
            {
                Text = "Change ExternalProgram";

                if (!string.IsNullOrEmpty(ExternalProgramObject.Application) &&
                    _fileSystem.FileExists(ExternalProgramObject.Application))
                {
                    Icon = Icon.ExtractAssociatedIcon(ExternalProgramObject.Application);
                }
                else
                {
                    Icon = (Icon)resources.GetObject("$this.Icon");
                }

                textBoxName.Text = ExternalProgramObject.Name;
                textBoxApplication.Text = ExternalProgramObject.Application;
                textBoxArguments.Text = ExternalProgramObject.Arguments;

                string defaultExternalProgram = _config.Settings.User.GetByKey("DefaultExternalProgram", _config.Settings.DefaultSettings.DefaultExternalProgram).Value.ToString();

                if (ExternalProgramObject.Name.Equals(defaultExternalProgram))
                {
                    checkBoxMakeDefaultExternalProgram.Checked = true;
                }

                textBoxNotes.Text = ExternalProgramObject.Notes;
            }
            else
            {
                Text = "Add ExternalProgram";
                Icon = (Icon)resources.GetObject("$this.Icon");

                textBoxName.Text = "ExternalProgram " + (ExternalProgramCollection.Count + 1);
                textBoxApplication.Text = string.Empty;
                textBoxArguments.Text = defaultArguments;
                textBoxNotes.Text = string.Empty;
            }
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
            if (checkBoxMakeDefaultExternalProgram.Checked && !string.IsNullOrEmpty(textBoxName.Text))
            {
                _config.Settings.User.GetByKey("DefaultExternalProgram", _config.Settings.DefaultSettings.DefaultExternalProgram).Value = textBoxName.Text;
                _config.Settings.User.Save(_config.Settings, _fileSystem);
            }

            if (ExternalProgramObject != null)
            {
                ChangeExternalProgram();
            }
            else
            {
                AddExternalProgram();
            }
        }

        private void AddExternalProgram()
        {
            if (InputValid())
            {
                TrimInput();

                if (ApplicationExists())
                {
                    if (ExternalProgramCollection.GetByName(textBoxName.Text) == null)
                    {
                        ExternalProgram program = new ExternalProgram()
                        {
                            Name = textBoxName.Text,
                            Application = textBoxApplication.Text,
                            Arguments = textBoxArguments.Text,
                            Notes = textBoxNotes.Text
                        };

                        ExternalProgramCollection.Add(program);

                        Okay();
                    }
                    else
                    {
                        MessageBox.Show("An external program with this name already exists.", "Duplicate Name Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show($"Could not find \"{textBoxApplication.Text}\".", "Application Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter valid input for each field.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeExternalProgram()
        {
            if (InputValid())
            {
                if (NameChanged() || InputChanged())
                {
                    TrimInput();

                    if (ApplicationExists())
                    {
                        if (ExternalProgramCollection.GetByName(textBoxName.Text) != null && NameChanged())
                        {
                            MessageBox.Show("An external program with this name already exists.", "Duplicate Name Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            ExternalProgramCollection.Get(ExternalProgramObject).Application = textBoxApplication.Text;
                            ExternalProgramCollection.Get(ExternalProgramObject).Arguments = textBoxArguments.Text;
                            ExternalProgramCollection.Get(ExternalProgramObject).Name = textBoxName.Text;
                            ExternalProgramCollection.Get(ExternalProgramObject).Notes = textBoxNotes.Text;

                            Okay();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Could not find \"{textBoxApplication.Text}\".", "Application Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            textBoxName.Text = textBoxName.Text.Trim();
            textBoxApplication.Text = textBoxApplication.Text.Trim();
            textBoxArguments.Text = textBoxArguments.Text.Trim();
            textBoxNotes.Text = textBoxNotes.Text.Trim();
        }

        private bool InputValid()
        {
            if (!string.IsNullOrEmpty(textBoxName.Text) &&
                !string.IsNullOrEmpty(textBoxApplication.Text))
            {
                return true;
            }

            return false;
        }

        private bool InputChanged()
        {
            if (ExternalProgramObject != null &&
                (!ExternalProgramObject.Application.Equals(textBoxApplication.Text) ||
                    !ExternalProgramObject.Arguments.Equals(textBoxArguments.Text)) ||
                    !ExternalProgramObject.Notes.Equals(textBoxNotes.Text))
            {
                return true;
            }

            return false;
        }

        private bool NameChanged()
        {
            if (ExternalProgramObject != null &&
                !ExternalProgramObject.Name.Equals(textBoxName.Text))
            {
                return true;
            }

            return false;
        }

        private bool ApplicationExists()
        {
            if (_fileSystem.FileExists(textBoxApplication.Text))
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

        private void buttonChooseExternalProgram_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                FilterIndex = 0,
                Multiselect = false,
                AddExtension = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Applications (*.exe)|*.exe|Batch Scripts (*.bat)|*.bat|PowerShell Scripts (*.ps1)|*.ps1|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(openFileDialog.FileName) &&
                    _fileSystem.FileExists(openFileDialog.FileName))
                {
                    Icon = Icon.ExtractAssociatedIcon(openFileDialog.FileName);
                }
                else
                {
                    Icon = (Icon)resources.GetObject("$this.Icon");
                }

                textBoxApplication.Text = openFileDialog.FileName;
            }
        }
    }
}