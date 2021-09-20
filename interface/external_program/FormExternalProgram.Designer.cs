namespace AutoScreenCapture
{
    partial class FormExternalProgram
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExternalProgram));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelExternalProgramName = new System.Windows.Forms.Label();
            this.labelExternalProgramApplication = new System.Windows.Forms.Label();
            this.textBoxApplication = new System.Windows.Forms.TextBox();
            this.buttonChooseExternalProgram = new System.Windows.Forms.Button();
            this.labelExternalProgramArguments = new System.Windows.Forms.Label();
            this.textBoxArguments = new System.Windows.Forms.TextBox();
            this.checkBoxMakeDefaultExternalProgram = new System.Windows.Forms.CheckBox();
            this.labelHelp = new System.Windows.Forms.Label();
            this.labelNotes = new System.Windows.Forms.Label();
            this.textBoxNotes = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(117, 420);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(99, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.Location = new System.Drawing.Point(12, 420);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(99, 23);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(56, 32);
            this.textBoxName.MaxLength = 50;
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(546, 20);
            this.textBoxName.TabIndex = 2;
            // 
            // labelExternalProgramName
            // 
            this.labelExternalProgramName.AutoSize = true;
            this.labelExternalProgramName.Location = new System.Drawing.Point(9, 35);
            this.labelExternalProgramName.Name = "labelExternalProgramName";
            this.labelExternalProgramName.Size = new System.Drawing.Size(38, 13);
            this.labelExternalProgramName.TabIndex = 1;
            this.labelExternalProgramName.Text = "Name:";
            // 
            // labelExternalProgramApplication
            // 
            this.labelExternalProgramApplication.AutoSize = true;
            this.labelExternalProgramApplication.Location = new System.Drawing.Point(9, 67);
            this.labelExternalProgramApplication.Name = "labelExternalProgramApplication";
            this.labelExternalProgramApplication.Size = new System.Drawing.Size(62, 13);
            this.labelExternalProgramApplication.TabIndex = 4;
            this.labelExternalProgramApplication.Text = "Application:";
            // 
            // textBoxApplication
            // 
            this.textBoxApplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxApplication.Location = new System.Drawing.Point(77, 64);
            this.textBoxApplication.Name = "textBoxApplication";
            this.textBoxApplication.Size = new System.Drawing.Size(634, 20);
            this.textBoxApplication.TabIndex = 5;
            // 
            // buttonChooseExternalProgram
            // 
            this.buttonChooseExternalProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChooseExternalProgram.Image = global::AutoScreenCapture.Properties.Resources.application_add;
            this.buttonChooseExternalProgram.Location = new System.Drawing.Point(717, 61);
            this.buttonChooseExternalProgram.Name = "buttonChooseExternalProgram";
            this.buttonChooseExternalProgram.Size = new System.Drawing.Size(24, 24);
            this.buttonChooseExternalProgram.TabIndex = 6;
            this.buttonChooseExternalProgram.UseVisualStyleBackColor = true;
            this.buttonChooseExternalProgram.Click += new System.EventHandler(this.buttonChooseExternalProgram_Click);
            // 
            // labelExternalProgramArguments
            // 
            this.labelExternalProgramArguments.AutoSize = true;
            this.labelExternalProgramArguments.Location = new System.Drawing.Point(9, 96);
            this.labelExternalProgramArguments.Name = "labelExternalProgramArguments";
            this.labelExternalProgramArguments.Size = new System.Drawing.Size(60, 13);
            this.labelExternalProgramArguments.TabIndex = 7;
            this.labelExternalProgramArguments.Text = "Arguments:";
            // 
            // textBoxArguments
            // 
            this.textBoxArguments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxArguments.Location = new System.Drawing.Point(77, 93);
            this.textBoxArguments.Name = "textBoxArguments";
            this.textBoxArguments.Size = new System.Drawing.Size(664, 20);
            this.textBoxArguments.TabIndex = 8;
            // 
            // checkBoxMakeDefaultExternalProgram
            // 
            this.checkBoxMakeDefaultExternalProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMakeDefaultExternalProgram.AutoSize = true;
            this.checkBoxMakeDefaultExternalProgram.Location = new System.Drawing.Point(691, 34);
            this.checkBoxMakeDefaultExternalProgram.Name = "checkBoxMakeDefaultExternalProgram";
            this.checkBoxMakeDefaultExternalProgram.Size = new System.Drawing.Size(60, 17);
            this.checkBoxMakeDefaultExternalProgram.TabIndex = 3;
            this.checkBoxMakeDefaultExternalProgram.Text = "Default";
            this.checkBoxMakeDefaultExternalProgram.UseVisualStyleBackColor = true;
            // 
            // labelHelp
            // 
            this.labelHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHelp.AutoEllipsis = true;
            this.labelHelp.BackColor = System.Drawing.Color.LightYellow;
            this.labelHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelHelp.Image = global::AutoScreenCapture.Properties.Resources.about;
            this.labelHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelHelp.Location = new System.Drawing.Point(2, 4);
            this.labelHelp.Name = "labelHelp";
            this.labelHelp.Size = new System.Drawing.Size(752, 17);
            this.labelHelp.TabIndex = 0;
            this.labelHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelNotes
            // 
            this.labelNotes.AutoSize = true;
            this.labelNotes.Location = new System.Drawing.Point(9, 126);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(38, 13);
            this.labelNotes.TabIndex = 9;
            this.labelNotes.Text = "Notes:";
            // 
            // textBoxNotes
            // 
            this.textBoxNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNotes.Location = new System.Drawing.Point(9, 142);
            this.textBoxNotes.MaxLength = 500;
            this.textBoxNotes.Multiline = true;
            this.textBoxNotes.Name = "textBoxNotes";
            this.textBoxNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxNotes.Size = new System.Drawing.Size(735, 265);
            this.textBoxNotes.TabIndex = 10;
            // 
            // FormExternalProgram
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(756, 454);
            this.Controls.Add(this.labelNotes);
            this.Controls.Add(this.textBoxNotes);
            this.Controls.Add(this.labelHelp);
            this.Controls.Add(this.checkBoxMakeDefaultExternalProgram);
            this.Controls.Add(this.textBoxArguments);
            this.Controls.Add(this.labelExternalProgramArguments);
            this.Controls.Add(this.buttonChooseExternalProgram);
            this.Controls.Add(this.textBoxApplication);
            this.Controls.Add(this.labelExternalProgramApplication);
            this.Controls.Add(this.labelExternalProgramName);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(772, 493);
            this.Name = "FormExternalProgram";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.FormExternalProgram_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelExternalProgramName;
        private System.Windows.Forms.Label labelExternalProgramApplication;
        private System.Windows.Forms.TextBox textBoxApplication;
        private System.Windows.Forms.Button buttonChooseExternalProgram;
        private System.Windows.Forms.Label labelExternalProgramArguments;
        private System.Windows.Forms.TextBox textBoxArguments;
        private System.Windows.Forms.CheckBox checkBoxMakeDefaultExternalProgram;
        private System.Windows.Forms.Label labelHelp;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.TextBox textBoxNotes;
    }
}