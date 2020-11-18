﻿//-----------------------------------------------------------------------
// <copyright file="FormRegionSelectWithMouse.cs" company="Gavin Kendall">
//     Copyright (c) 2020 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary>A form that covers all the available screens so we can do a mouse-driven region select.</summary>
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace AutoScreenCapture
{
    /// <summary>
    /// A form that covers all the available screens so we can do a mouse-driven region select.
    /// </summary>
    public partial class FormRegionSelectWithMouse : Form
    {
        private int _selectX;
        private int _selectY;
        private int _selectWidth;
        private int _selectHeight;
        private Pen _selectPen;

        /// <summary>
        /// X output
        /// </summary>
        public int outputX;

        /// <summary>
        /// Y output
        /// </summary>
        public int outputY;

        /// <summary>
        /// Width output
        /// </summary>
        public int outputWidth;

        /// <summary>
        /// Height output
        /// </summary>
        public int outputHeight;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public FormRegionSelectWithMouse()
        {
            InitializeComponent();

            outputX = 0;
            outputY = 0;
            outputWidth = 0;
            outputHeight = 0;
        }

        /// <summary>
        /// The type of output this form should return.
        /// </summary>
        private int _outputMode { get; set; }

        /// <summary>
        /// An event handler for handling when the mouse selection has completed for the mouse-driven region capture.
        /// </summary>
        public event EventHandler MouseSelectionCompleted;

        private void CompleteMouseSelection(object sender, EventArgs e)
        {
            MouseSelectionCompleted?.Invoke(sender, e);
        }

        /// <summary>
        /// Laods the canvas with the chosen output mode.
        /// </summary>
        public void LoadCanvas(int outputMode)
        {
            _outputMode = outputMode;

            Top = 0;
            Left = 0;

            int width = 0;
            int height = 0;

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                width += screen.Bounds.Width;
                height += screen.Bounds.Height;
            }

            WindowState = FormWindowState.Normal;
            Width = width;
            Height = height;

            Hide();

            Bitmap bitmap = new Bitmap(width, height);

            Graphics graphics = Graphics.FromImage(bitmap as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            using (MemoryStream s = new MemoryStream())
            {
                bitmap.Save(s, System.Drawing.Imaging.ImageFormat.Bmp);
                pictureBoxMouseCanvas.Size = new Size(Width, Height);
                pictureBoxMouseCanvas.Image = Image.FromStream(s);
            }

            Show();

            Cursor = Cursors.Cross;
        }

        private void pictureBoxMouseCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBoxMouseCanvas.Image == null || _selectPen == null) return;

            pictureBoxMouseCanvas.Refresh();

            _selectWidth = e.X - _selectX;
            _selectHeight = e.Y - _selectY;

            pictureBoxMouseCanvas.CreateGraphics().DrawRectangle(_selectPen, _selectX, _selectY, _selectWidth, _selectHeight);
        }

        private void pictureBoxMouseCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _selectX = e.X;
                _selectY = e.Y;

                _selectPen = new Pen(Color.Red, 2)
                {
                    DashStyle = DashStyle.Dash
                };
            }

            pictureBoxMouseCanvas.Refresh();
        }

        private void pictureBoxMouseCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBoxMouseCanvas.Image == null || _selectPen == null) return;

            if (e.Button == MouseButtons.Left)
            {
                pictureBoxMouseCanvas.Refresh();

                _selectWidth = e.X - _selectX;
                _selectHeight = e.Y - _selectY;

                pictureBoxMouseCanvas.CreateGraphics().DrawRectangle(_selectPen, _selectX, _selectY, _selectWidth, _selectHeight);
            }

            Bitmap bitmap = null;

            switch (_outputMode)
            {
                case 0:
                    bitmap = SelectBitmap();

                    if (bitmap != null)
                    {
                        outputX = _selectX;
                        outputY = _selectY;
                        outputWidth = _selectWidth;
                        outputHeight = _selectHeight;

                        CompleteMouseSelection(sender, e);
                    }
                    break;
                case 1:
                    bitmap = SelectBitmap();

                    if (bitmap != null)
                    {
                        SaveToClipboard(bitmap);
                    }
                    break;
            }

            Cursor = Cursors.Arrow;

            Close();
        }

        private Bitmap SelectBitmap()
        {
            if (_selectWidth > 0)
            {
                Rectangle rect = new Rectangle(_selectX, _selectY, _selectWidth, _selectHeight);
                Bitmap bitmap = new Bitmap(pictureBoxMouseCanvas.Image, pictureBoxMouseCanvas.Width, pictureBoxMouseCanvas.Height);

                Bitmap img = new Bitmap(_selectWidth, _selectHeight);

                Graphics g = Graphics.FromImage(img);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(bitmap, 0, 0, rect, GraphicsUnit.Pixel);

                return img;
            }

            return null;
        }

        private void SaveToClipboard(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                Clipboard.SetImage(bitmap);
            }
        }
    }
}
