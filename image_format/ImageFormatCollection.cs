﻿//-----------------------------------------------------------------------
// <copyright file="ImageFormatCollection.cs" company="Gavin Kendall">
//     Copyright (c) 2020 Gavin Kendall
// </copyright>
// <author>Gavin Kendall</author>
// <summary></summary>
//-----------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace AutoScreenCapture
{
    /// <summary>
    /// A collection class to store and manage ImageFormat objects.
    /// </summary>
    public class ImageFormatCollection : IEnumerable<ImageFormat>
    {
        private readonly List<ImageFormat> _imageFormatList = new List<ImageFormat>();

        /// <summary>
        /// Adds the default image formats to the ImageFormat collection.
        /// </summary>
        public ImageFormatCollection()
        {
            Add(new ImageFormat(ImageFormatSpec.NAME_BMP, ImageFormatSpec.EXTENSION_BMP));
            Add(new ImageFormat(ImageFormatSpec.NAME_EMF, ImageFormatSpec.EXTENSION_EMF));
            Add(new ImageFormat(ImageFormatSpec.NAME_GIF, ImageFormatSpec.EXTENSION_GIF));
            Add(new ImageFormat(ImageFormatSpec.NAME_JPEG, ImageFormatSpec.EXTENSION_JPEG));
            Add(new ImageFormat(ImageFormatSpec.NAME_PNG, ImageFormatSpec.EXTENSION_PNG));
            Add(new ImageFormat(ImageFormatSpec.NAME_TIFF, ImageFormatSpec.EXTENSION_TIFF));
            Add(new ImageFormat(ImageFormatSpec.NAME_WMF, ImageFormatSpec.EXTENSION_WMF));
        }

        /// <summary>
        /// Returns the enumerator for the collection.
        /// </summary>
        /// <returns>A list of ImageFormat objects.</returns>
        public List<ImageFormat>.Enumerator GetEnumerator()
        {
            return _imageFormatList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ImageFormat>)_imageFormatList).GetEnumerator();
        }

        IEnumerator<ImageFormat> IEnumerable<ImageFormat>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an ImageFormat object to the collection.
        /// </summary>
        /// <param name="imageFormat">The ImageFormat object to add.</param>
        public void Add(ImageFormat imageFormat)
        {
            _imageFormatList.Add(imageFormat);

            Log.WriteDebugMessage("Image format added: " + imageFormat.Name + " (" + imageFormat.Extension + ")");
        }

        /// <summary>
        /// Gets an ImageFormat object based on its name.
        /// </summary>
        /// <param name="name">The name of an ImageFormat object.</param>
        /// <returns>An ImageFormat object.</returns>
        public ImageFormat GetByName(string name)
        {
            foreach (ImageFormat imageFormat in _imageFormatList)
            {
                if (imageFormat.Name.Equals(name))
                {
                    return imageFormat;
                }
            }

            return null;
        }
    }
}