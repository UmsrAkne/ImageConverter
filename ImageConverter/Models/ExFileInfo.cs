using System;
using System.IO;
using Prism.Mvvm;

namespace ImageConverter.Models
{
    public class ExFileInfo : BindableBase
    {
        private string status = "未変換";

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

        public string Status { get => status; set => SetProperty(ref status, value); }

        public string FileType { get; private set; }

        public string FullName { get; private set; }

        public string Name { get; private set; }

        public bool Deleted { get; private set; }

        /// <summary>
        ///     FileSystemInfo クラスが参照しているファイルを削除します。
        ///     削除した後、 Deleted プロパティが true に設定されます。
        ///     このメソッドの実行時、既にファイルが削除済みだった場合は、動作しません。
        /// </summary>
        public void DeleteFile()
        {
            if (Deleted)
            {
                return;
            }

            File.Delete(FullName);
            Deleted = true;
            Status = "変換 / 削除済み";
        }
    }
}