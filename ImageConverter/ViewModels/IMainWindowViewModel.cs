using System.Collections.ObjectModel;
using ImageConverter.Models;
using Prism.Commands;

namespace ImageConverter.ViewModels
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<ExFileInfo> ExFileInfos { get; }

        ProcessType ProcessType { get; set; }

        bool DeleteOriginalFile { get; set; }

        string Log { get; }

        bool UiEnabled { get; set; }

        TextWrapper TextWrapper { get; init; }

        AsyncDelegateCommand StartConvertAsyncCommand { get; }

        DelegateCommand ClearFileListCommand { get; }

        DelegateCommand ClearConvertedCommand { get; }

        void AddFile(string path);
    }
}