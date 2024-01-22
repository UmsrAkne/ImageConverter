using System;
using System.IO;
using Prism.Mvvm;

namespace ImageConverter.Models
{
    public class ExFileInfo : BindableBase
    {
        public ExFileInfo(FileInfo file)
        {
            FileSystemInfo = file;
            LoadedDateTime = DateTime.Now;
            FullName = file.FullName;
            Name = file.Name;
            FileType = file.Extension;
        }

        public FileInfo FileSystemInfo { get; set; }

        public DateTime LoadedDateTime { get; set; }

        public string FileType { get; private set; }

        public string FullName { get; private set; }

        public string Name { get; private set; }
    }
}