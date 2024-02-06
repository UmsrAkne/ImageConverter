using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ImageConverter.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ImageConverter.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Image Converter";
        private ObservableCollection<ExFileInfo> exFileInfos = new ();
        private ProcessType processType = ProcessType.WebpToPng;

        public string Title { get => title; set => SetProperty(ref title, value); }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public ProcessType ProcessType { get => processType; set => SetProperty(ref processType, value); }

        public ObservableCollection<ExFileInfo> ExFileInfos
        {
            get => exFileInfos;
            set => SetProperty(ref exFileInfos, value);
        }

        public DelegateCommand StartConvertCommand => new (() =>
        {
            if (ProcessType == ProcessType.WebpToPng)
            {
                var webpFiles = ExFileInfos.Where(f => f.FileType == ".webp");
                foreach (var webp in webpFiles)
                {
                    var outputFilePath = $"{webp.FullName.Remove(webp.FullName.Length - 5)}.png";
                    ConvertImage(webp.FullName, outputFilePath);
                }
            }

            if (ProcessType == ProcessType.BmpToPng)
            {
                var bmpFiles = ExFileInfos.Where(f =>
                    string.Equals(f.FileType, ".bmp", StringComparison.OrdinalIgnoreCase));

                foreach (var bmp in bmpFiles)
                {
                    var outputFilePath = $"{bmp.FullName.Remove(bmp.FullName.Length - 4)}.png";
                    ConvertImage(bmp.FullName, outputFilePath);
                }
            }
        });

        public DelegateCommand ClearFileListCommand => new (() =>
        {
            ExFileInfos = new ObservableCollection<ExFileInfo>();
        });

        public void AddFile(string path)
        {
            ExFileInfos.Add(new ExFileInfo(new FileInfo(path)));
        }

        private static void ConvertImage(string inputPath, string outputPath)
        {
            try
            {
                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    FileName = "magick",
                    Arguments = $"{inputPath} {outputPath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                process.StartInfo = startInfo;
                process.Start();

                // エラー出力を読み込む
                var errorOutput = process.StandardError.ReadToEnd();

                // プロセスが終了するまで待機
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    // エラーコードが0以外の場合、変換に失敗した可能性があります
                    Console.WriteLine($"Conversion failed. Error: {errorOutput}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}