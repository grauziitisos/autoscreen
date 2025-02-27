﻿//-----------------------------------------------------------------------
// <copyright file="FormMain-Help.cs" company="Gavin Kendall">
//     Copyright (c) 2008-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>All the methods for handling help tips in the help bar at the top of the main interface window.</summary>
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
using System.Collections.Generic;
using System.Windows.Forms;

namespace AutoScreenCapture
{
    public partial class FormMain : Form
    {
        int _helpTipIndex = 0;
        private ToolTip _toolTip = new ToolTip();
        private readonly List<string> _helpTips = new List<string>();

        private void LoadHelpTips()
        {
            _helpTips.Add("Help tips are displayed here in the help bar. You can either click this help bar to show the next tip or wait until the next tip is shown");
            _helpTips.Add("The Start Screen Capture button begins a session of taking screenshots at the specified interval. The application will run in your system tray");
            _helpTips.Add("The calendar shows you which days screenshots were taken. Any date shown in bold indicates screenshots were taken on that day");
            _helpTips.Add("Below the calendar are modules such as Screenshots, Screens, and Regions");
            _helpTips.Add("The Screenshots module displays the list of screenshots taken on the day selected from the calendar");
            _helpTips.Add("The Regions module displays a list of regions. Add, remove, or change regions from the Regions module");
            _helpTips.Add("Use the Configure drop-down menu to add, remove, or change a Screen or Region. You can add multiple regions each with its own set of attributes");
            _helpTips.Add("You can add as many editors as you want but only one editor can be set as the default editor");
            _helpTips.Add("Use the Macro field of a Screen or Region to define the filename pattern for each file. A macro can be very useful for keeping files organized");
            _helpTips.Add("Macro tags can represent the current date and time. You can also acquire the name of the user or computer. Have a look in the Macro Tags module");
            _helpTips.Add("There are keyboard shortcuts you can use to manually take screenshots in your own time rather than wait for the next screen capture cycle");
            _helpTips.Add("You can change the behaviour of the application by changing the triggers. Each trigger performs a certain action based on a particular condition");

            // Tool tips for various controls.
            _toolTip.SetToolTip(comboBoxFilterType, "Choose a type of filter. This could be a label or the title of an active window from an application that was captured during a session");
            _toolTip.SetToolTip(comboBoxFilterValue, "Select a filter value based on the chosen filter type. This will show you what days in the calendar are associated with the filter value");
            _toolTip.SetToolTip(buttonRefreshFilterValues, "Click this button to refresh the list of filter values");
        }

        private void timerShowNextHelpTip_Tick(object sender, EventArgs e)
        {
            if (_helpTipIndex < _helpTips.Count)
            {
                HelpMessage(_helpTips[_helpTipIndex]);
                _helpTipIndex++;
            }

            if (_helpTipIndex == _helpTips.Count)
            {
                _helpTipIndex = 0;
            }

            if (_preview)
            {
                ShowScreenshotBySlideIndex();
            }
        }

        private void labelHelp_Click(object sender, EventArgs e)
        {
            timerShowNextHelpTip_Tick(sender, e);
            RestartHelpTipTimer();
        }

        private void RestartHelpTipTimer()
        {
            timerShowNextHelpTip.Stop();
            timerShowNextHelpTip.Start();
        }

        private void HelpMessage(string message)
        {
            labelHelp.Text = "       " + message;
        }

        /// <summary>
        /// Shows the "Auto Screen Capture - Help" form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripDropDownButtonHelp_Click(object sender, EventArgs e)
        {
            if (!_formHelp.Visible)
            {
                _formHelp.Show();
            }
            else
            {
                _formHelp.WindowState = FormWindowState.Normal;
                _formHelp.Activate();
            }
        }
    }
}
