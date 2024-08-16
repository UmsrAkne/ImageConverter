using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ImageConverter.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ImageConverter.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignTimeMainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        private bool uiEnabled;

        public DesignTimeMainWindowViewModel()
        {
            var fileInfos = new List<ExFileInfo>();
            for (var i = 0; i < 30; i++)
            {
                fileInfos.Add(new ExFileInfo(new FileInfo($"image_{i:000}.png")));
            }

            fileInfos[0].Status = "変換済み";
            fileInfos[1].Status = "変換済み";
            fileInfos[2].Status = "変換済み";

            // 過剰な長さのファイル名を一つ追加して表示しておく。
            fileInfos[3] = new ExFileInfo(new FileInfo("image_image_image_image100000000000000.png"));

            ExFileInfos = new ObservableCollection<ExFileInfo>(fileInfos);

            Log = "ログのテキストをここに表示します。";
            UiEnabled = true;
        }

        public ObservableCollection<ExFileInfo> ExFileInfos { get; }

        public ProcessType ProcessType { get; set; }

        public bool DeleteOriginalFile { get; set; }

        public string Log { get; }

        public bool UiEnabled { get => uiEnabled; set => SetProperty(ref uiEnabled, value); }

        public TextWrapper TextWrapper { get; init; }

        public AsyncDelegateCommand StartConvertAsyncCommand => new AsyncDelegateCommand(async () => { });

        public DelegateCommand ClearFileListCommand => new (() => { });

        public DelegateCommand ClearConvertedCommand => new (() => { });

        public void AddFile(string path)
        {
            // このメソッドは使いません。
        }
    }
}