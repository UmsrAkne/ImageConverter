using System;
using System.Collections.ObjectModel;
using System.IO;
using ImageConverter.Models;
using Prism.Mvvm;

namespace ImageConverter.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Image Converter";
        private ObservableCollection<ExFileInfo> exFileInfos = new ();

        public string Title { get => title; set => SetProperty(ref title, value); }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public ObservableCollection<ExFileInfo> ExFileInfos
        {
            get => exFileInfos;
            set => SetProperty(ref exFileInfos, value);
        }

        public void AddFile(string path)
        {
            ExFileInfos.Add(new ExFileInfo(new FileInfo(path)));
        }
    }
}