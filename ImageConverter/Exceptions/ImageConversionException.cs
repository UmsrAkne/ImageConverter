using System;

namespace ImageConverter.Exceptions
{
    public class ImageConversionException : Exception
    {
        public ImageConversionException(string message)
            : base(message)
        {
        }

        public string FailedFileName { get; set; }
    }
}