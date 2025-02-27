﻿//-----------------------------------------------------------------------
// <copyright file="FormSchedule.cs" company="Gavin Kendall">
//     Copyright (c) 2008-2021 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A form for adding a new schedule or changing an existing schedule.</summary>
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

namespace AutoScreenCapture
{
    /// <summary>
    /// A form for managing a schedule.
    /// </summary>
    public partial class FormSchedule : Form
    {
        private DataConvert _dataConvert;

        private ToolTip _toolTip = new ToolTip();

        /// <summary>
        /// A collection of schedules.
        /// </summary>
        public ScheduleCollection ScheduleCollection { get; private set; }

        /// <summary>
        /// The screen capture interval for the schedule to use for overriding the main screen capture interval.
        /// </summary>
        public int ScreenCaptureInterval { get; set; }

        /// <summary>
        /// The schedule object to handle.
        /// </summary>
        public Schedule ScheduleObject { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public FormSchedule()
        {
            InitializeComponent();

            _dataConvert = new DataConvert();
            ScheduleCollection = new ScheduleCollection();
        }

        private void FormSchedule_Load(object sender, EventArgs e)
        {
            textBoxName.Focus();

            HelpMessage("This is where to configure a schedule to determine when screenshots should be taken");

            _toolTip.SetToolTip(checkBoxEnable, "The times and days in this schedule will be considered if Active is checked (turned on)");
            _toolTip.SetToolTip(dateTimePickerCaptureAt, "The time at which screenshots will be taken for a single capture cycle");
            _toolTip.SetToolTip(dateTimePickerStartAt, "The time at which a screen capture session will start running");
            _toolTip.SetToolTip(dateTimePickerStopAt, "The time at which a running screen capture session will stop");

            if (ScheduleObject != null)
            {
                Text = "Change Schedule";

                textBoxName.Text = ScheduleObject.Name;
                checkBoxEnable.Checked = ScheduleObject.Enable;

                radioButtonOneTime.Checked = ScheduleObject.ModeOneTime;
                radioButtonPeriod.Checked = ScheduleObject.ModePeriod;

                dateTimePickerCaptureAt.Value = ScheduleObject.CaptureAt;
                dateTimePickerStartAt.Value = ScheduleObject.StartAt;
                dateTimePickerStopAt.Value = ScheduleObject.StopAt;

                decimal screenCaptureIntervalHours = Convert.ToDecimal(TimeSpan.FromMilliseconds(Convert.ToDouble(ScheduleObject.ScreenCaptureInterval)).Hours);
                decimal screenCaptureIntervalMinutes = Convert.ToDecimal(TimeSpan.FromMilliseconds(Convert.ToDouble(ScheduleObject.ScreenCaptureInterval)).Minutes);
                decimal screenCaptureIntervalSeconds = Convert.ToDecimal(TimeSpan.FromMilliseconds(Convert.ToDouble(ScheduleObject.ScreenCaptureInterval)).Seconds);
                decimal screenCaptureIntervalMilliseconds = Convert.ToDecimal(TimeSpan.FromMilliseconds(Convert.ToDouble(ScheduleObject.ScreenCaptureInterval)).Milliseconds);

                numericUpDownHoursInterval.Value = screenCaptureIntervalHours;
                numericUpDownMinutesInterval.Value = screenCaptureIntervalMinutes;
                numericUpDownSecondsInterval.Value = screenCaptureIntervalSeconds;
                numericUpDownMillisecondsInterval.Value = screenCaptureIntervalMilliseconds;

                checkBoxMonday.Checked = ScheduleObject.Monday;
                checkBoxTuesday.Checked = ScheduleObject.Tuesday;
                checkBoxWednesday.Checked = ScheduleObject.Wednesday;
                checkBoxThursday.Checked = ScheduleObject.Thursday;
                checkBoxFriday.Checked = ScheduleObject.Friday;
                checkBoxSaturday.Checked = ScheduleObject.Saturday;
                checkBoxSunday.Checked = ScheduleObject.Sunday;

                if (checkBoxMonday.Checked &&
                    checkBoxTuesday.Checked &&
                    checkBoxWednesday.Checked &&
                    checkBoxThursday.Checked &&
                    checkBoxFriday.Checked)
                {
                    checkBoxWorkWeek.Checked = true;
                }

                if (checkBoxSaturday.Checked &&
                    checkBoxSunday.Checked)
                {
                    checkBoxWeekend.Checked = true;
                }

                textBoxNotes.Text = ScheduleObject.Notes;
            }
            else
            {
                Text = "Add Schedule";

                textBoxName.Text = "Schedule " + (ScheduleCollection.Count + 1);
                checkBoxEnable.Checked = true;

                dateTimePickerCaptureAt.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
                dateTimePickerStartAt.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
                dateTimePickerStopAt.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);

                decimal screenCaptureIntervalHours = Convert.ToDecimal(TimeSpan.FromMilliseconds(Convert.ToDouble(ScreenCaptureInterval)).Hours);
                decimal screenCaptureIntervalMinutes = Convert.ToDecimal(TimeSpan.FromMilliseconds(Convert.ToDouble(ScreenCaptureInterval)).Minutes);
                decimal screenCaptureIntervalSeconds = Convert.ToDecimal(TimeSpan.FromMilliseconds(Convert.ToDouble(ScreenCaptureInterval)).Seconds);
                decimal screenCaptureIntervalMilliseconds = Convert.ToDecimal(TimeSpan.FromMilliseconds(Convert.ToDouble(ScreenCaptureInterval)).Milliseconds);

                numericUpDownHoursInterval.Value = screenCaptureIntervalHours;
                numericUpDownMinutesInterval.Value = screenCaptureIntervalMinutes;
                numericUpDownSecondsInterval.Value = screenCaptureIntervalSeconds;
                numericUpDownMillisecondsInterval.Value = screenCaptureIntervalMilliseconds;

                radioButtonOneTime.Checked = true;
                radioButtonPeriod.Checked = false;

                labelTakeScreenshotsOnce.Enabled = true;
                dateTimePickerCaptureAt.Enabled = true;

                labelTakeScreenshotsPeriod.Enabled = false;
                dateTimePickerStartAt.Enabled = false;
                labelAnd.Enabled = false;
                dateTimePickerStopAt.Enabled = false;
                labelInterval.Enabled = false;
                numericUpDownHoursInterval.Enabled = false;
                numericUpDownMinutesInterval.Enabled = false;
                numericUpDownSecondsInterval.Enabled = false;
                numericUpDownMillisecondsInterval.Enabled = false;

                checkBoxWorkWeek.Checked = true;
                checkBoxWeekend.Checked = false;

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
            if (ScheduleObject != null)
            {
                ChangeSchedule();
            }
            else
            {
                AddSchedule();
            }
        }

        private void AddSchedule()
        {
            if (InputValid())
            {
                TrimInput();

                if (ScheduleCollection.GetByName(textBoxName.Text) == null)
                {
                    int screenCaptureInterval = _dataConvert.ConvertIntoMilliseconds((int)numericUpDownHoursInterval.Value,
                        (int)numericUpDownMinutesInterval.Value, (int)numericUpDownSecondsInterval.Value,
                        (int)numericUpDownMillisecondsInterval.Value);

                    Schedule schedule = new Schedule()
                    {
                        Name = textBoxName.Text,
                        Enable = checkBoxEnable.Checked,
                        ModeOneTime = radioButtonOneTime.Checked,
                        ModePeriod = radioButtonPeriod.Checked,
                        CaptureAt = dateTimePickerCaptureAt.Value,
                        StartAt = dateTimePickerStartAt.Value,
                        StopAt = dateTimePickerStopAt.Value,
                        ScreenCaptureInterval = screenCaptureInterval,
                        Monday = checkBoxMonday.Checked,
                        Tuesday = checkBoxTuesday.Checked,
                        Wednesday = checkBoxWednesday.Checked,
                        Thursday = checkBoxThursday.Checked,
                        Friday = checkBoxFriday.Checked,
                        Saturday = checkBoxSaturday.Checked,
                        Sunday = checkBoxSunday.Checked,
                        Notes = textBoxNotes.Text
                    };

                    ScheduleCollection.Add(schedule);

                    Okay();
                }
                else
                {
                    MessageBox.Show("A schedule with this name already exists.", "Duplicate Name Conflict",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter valid input for each field.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeSchedule()
        {
            if (InputValid())
            {
                if (NameChanged() || InputChanged())
                {
                    TrimInput();

                    if (ScheduleCollection.GetByName(textBoxName.Text) != null && NameChanged())
                    {
                        MessageBox.Show("A schedule with this name already exists.", "Duplicate Name Conflict",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        ScheduleCollection.Get(ScheduleObject).Name = textBoxName.Text;
                        ScheduleCollection.Get(ScheduleObject).Enable = checkBoxEnable.Checked;
                        ScheduleCollection.Get(ScheduleObject).ModeOneTime = radioButtonOneTime.Checked;
                        ScheduleCollection.Get(ScheduleObject).ModePeriod = radioButtonPeriod.Checked;
                        ScheduleCollection.Get(ScheduleObject).CaptureAt = dateTimePickerCaptureAt.Value;
                        ScheduleCollection.Get(ScheduleObject).StartAt = dateTimePickerStartAt.Value;
                        ScheduleCollection.Get(ScheduleObject).StopAt = dateTimePickerStopAt.Value;

                        int screenCaptureInterval = _dataConvert.ConvertIntoMilliseconds((int)numericUpDownHoursInterval.Value,
                            (int)numericUpDownMinutesInterval.Value, (int)numericUpDownSecondsInterval.Value,
                            (int)numericUpDownMillisecondsInterval.Value);

                        ScheduleCollection.Get(ScheduleObject).ScreenCaptureInterval = screenCaptureInterval;

                        ScheduleCollection.Get(ScheduleObject).Monday = checkBoxMonday.Checked;
                        ScheduleCollection.Get(ScheduleObject).Tuesday = checkBoxTuesday.Checked;
                        ScheduleCollection.Get(ScheduleObject).Wednesday = checkBoxWednesday.Checked;
                        ScheduleCollection.Get(ScheduleObject).Thursday = checkBoxThursday.Checked;
                        ScheduleCollection.Get(ScheduleObject).Friday = checkBoxFriday.Checked;
                        ScheduleCollection.Get(ScheduleObject).Saturday = checkBoxSaturday.Checked;
                        ScheduleCollection.Get(ScheduleObject).Sunday = checkBoxSunday.Checked;
                        ScheduleCollection.Get(ScheduleObject).Notes = textBoxNotes.Text;

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
            textBoxName.Text = textBoxName.Text.Trim();
            textBoxNotes.Text = textBoxNotes.Text.Trim();
        }

        private bool InputValid()
        {
            if (!string.IsNullOrEmpty(textBoxName.Text) &&
                (checkBoxMonday.Checked ||
                checkBoxTuesday.Checked ||
                checkBoxWednesday.Checked ||
                checkBoxThursday.Checked ||
                checkBoxFriday.Checked ||
                checkBoxSaturday.Checked ||
                checkBoxSunday.Checked))
            {
                return true;
            }

            return false;
        }

        private bool InputChanged()
        {
            int screenCaptureInterval = _dataConvert.ConvertIntoMilliseconds((int)numericUpDownHoursInterval.Value,
                            (int)numericUpDownMinutesInterval.Value, (int)numericUpDownSecondsInterval.Value,
                            (int)numericUpDownMillisecondsInterval.Value);

            if (ScheduleObject != null &&
                (!ScheduleObject.Enable.Equals(checkBoxEnable.Checked) ||
                !ScheduleObject.CaptureAt.Equals(dateTimePickerCaptureAt.Value) ||
                !ScheduleObject.StartAt.Equals(dateTimePickerStartAt.Value) ||
                !ScheduleObject.StopAt.Equals(dateTimePickerStopAt.Value) ||
                !ScheduleObject.ScreenCaptureInterval.Equals(screenCaptureInterval) ||
                !ScheduleObject.Monday.Equals(checkBoxMonday.Checked) ||
                !ScheduleObject.Tuesday.Equals(checkBoxTuesday.Checked) ||
                !ScheduleObject.Wednesday.Equals(checkBoxWednesday.Checked) ||
                !ScheduleObject.Thursday.Equals(checkBoxThursday.Checked) ||
                !ScheduleObject.Friday.Equals(checkBoxFriday.Checked) ||
                !ScheduleObject.Saturday.Equals(checkBoxSaturday.Checked) ||
                !ScheduleObject.Sunday.Equals(checkBoxSunday.Checked) ||
                !ScheduleObject.Notes.Equals(textBoxNotes.Text)))
            {
                return true;
            }

            return false;
        }

        private bool NameChanged()
        {
            if (ScheduleObject != null &&
                !ScheduleObject.Name.Equals(textBoxName.Text))
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

        private void radioButtonOneTime_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOneTime.Checked)
            {
                labelTakeScreenshotsOnce.Enabled = true;
                dateTimePickerCaptureAt.Enabled = true;

                labelTakeScreenshotsPeriod.Enabled = false;
                dateTimePickerStartAt.Enabled = false;
                labelAnd.Enabled = false;
                dateTimePickerStopAt.Enabled = false;
                labelInterval.Enabled = false;
                numericUpDownHoursInterval.Enabled = false;
                numericUpDownMinutesInterval.Enabled = false;
                numericUpDownSecondsInterval.Enabled = false;
                numericUpDownMillisecondsInterval.Enabled = false;
            }
        }

        private void radioButtonPeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPeriod.Checked)
            {
                labelTakeScreenshotsOnce.Enabled = false;
                dateTimePickerCaptureAt.Enabled = false;

                labelTakeScreenshotsPeriod.Enabled = true;
                dateTimePickerStartAt.Enabled = true;
                labelAnd.Enabled = true;
                dateTimePickerStopAt.Enabled = true;
                labelInterval.Enabled = true;
                numericUpDownHoursInterval.Enabled = true;
                numericUpDownMinutesInterval.Enabled = true;
                numericUpDownSecondsInterval.Enabled = true;
                numericUpDownMillisecondsInterval.Enabled = true;
            }
        }

        private void checkBoxWorkWeek_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWorkWeek.Checked)
            {
                checkBoxMonday.Checked = true;
                checkBoxTuesday.Checked = true;
                checkBoxWednesday.Checked = true;
                checkBoxThursday.Checked = true;
                checkBoxFriday.Checked = true;
            }
            else
            {
                checkBoxMonday.Checked = false;
                checkBoxTuesday.Checked = false;
                checkBoxWednesday.Checked = false;
                checkBoxThursday.Checked = false;
                checkBoxFriday.Checked = false;
            }
        }

        private void checkBoxWeekend_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWeekend.Checked)
            {
                checkBoxSaturday.Checked = true;
                checkBoxSunday.Checked = true;
            }
            else
            {
                checkBoxSaturday.Checked = false;
                checkBoxSunday.Checked = false;
            }
        }
    }
}