﻿//-----------------------------------------------------------------------
// <copyright file="FormMain-Tags.cs" company="Gavin Kendall">
//     Copyright (c) 2020 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>Methods for adding, removing, and changing macro tags.</summary>
//-----------------------------------------------------------------------
using System;
using System.Windows.Forms;

namespace AutoScreenCapture
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// Shows the "Add Tag" window to enable the user to add a chosen Tag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTag_Click(object sender, EventArgs e)
        {
            _formTag.TagObject = null;

            _formTag.ShowDialog(this);

            if (_formTag.DialogResult == DialogResult.OK)
            {
                BuildTagsModule();

                if (!_formTag.TagCollection.SaveToXmlFile())
                {
                    _screenCapture.ApplicationError = true;
                }
            }
        }

        /// <summary>
        /// Removes the selected Tags from the Tags tab page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedTags_Click(object sender, EventArgs e)
        {
            int countBeforeRemoval = _formTag.TagCollection.Count;

            foreach (Control control in tabPageTags.Controls)
            {
                if (control.GetType().Equals(typeof(CheckBox)))
                {
                    CheckBox checkBox = (CheckBox)control;

                    if (checkBox.Checked)
                    {
                        Tag trigger = _formTag.TagCollection.Get((Tag)checkBox.Tag);
                        _formTag.TagCollection.Remove(trigger);
                    }
                }
            }

            if (countBeforeRemoval > _formTag.TagCollection.Count)
            {
                BuildTagsModule();

                if (!_formTag.TagCollection.SaveToXmlFile())
                {
                    _screenCapture.ApplicationError = true;
                }
            }
        }

        /// <summary>
        /// Shows the "Change Tag" window to enable the user to edit a chosen Tag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeTag_Click(object sender, EventArgs e)
        {
            Button buttonSelected = (Button)sender;

            if (buttonSelected.Tag != null)
            {
                _formTag.TagObject = (Tag)buttonSelected.Tag;

                _formTag.ShowDialog(this);

                if (_formTag.DialogResult == DialogResult.OK)
                {
                    BuildTagsModule();

                    if (!_formTag.TagCollection.SaveToXmlFile())
                    {
                        _screenCapture.ApplicationError = true;
                    }
                }
            }
        }
    }
}
