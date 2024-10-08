﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageConverter.Exceptions;
using ImageConverter.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace ImageConverter.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        private ObservableCollection<ExFileInfo> exFileInfos = new ();
        private ProcessType processType = ProcessType.WebpToPng;
        private bool deleteOriginalFile;
        private string log;
        private bool uiEnabled = true;

        public ProcessType ProcessType { get => processType; set => SetProperty(ref processType, value); }

        public bool DeleteOriginalFile { get => deleteOriginalFile; set => SetProperty(ref deleteOriginalFile, value); }

        public string Log { get => log; private set => SetProperty(ref log, value); }

        public bool UiEnabled { get => uiEnabled; set => SetProperty(ref uiEnabled, value); }

        public TextWrapper TextWrapper { get; init; } = new ();

        public ObservableCollection<ExFileInfo> ExFileInfos
        {
            get => exFileInfos;
            private set => SetProperty(ref exFileInfos, value);
        }

        public AsyncDelegateCommand StartConvertAsyncCommand => new (async () =>
        {
            UiEnabled = false;

            var sb = new StringBuilder();

            if (ProcessType == ProcessType.WebpToPng)
            {
                var webpFiles = ExFileInfos
                    .Where(f => f.FileType == ".webp")
                    .Where(f => !f.Deleted).ToList();

                try
                {
                    await Task.Run(() => Convert(webpFiles, sb));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (ProcessType == ProcessType.BmpToPng)
            {
                var bmpFiles = ExFileInfos
                    .Where(f => string.Equals(f.FileType, ".bmp", StringComparison.OrdinalIgnoreCase))
                    .Where(f => !f.Deleted).ToList();

                try
                {
                    await Task.Run(() => Convert(bmpFiles, sb));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            Log += sb.ToString();
            UiEnabled = true;

            return;

            void Convert(IEnumerable<ExFileInfo> files, StringBuilder strBuilder)
            {
                var imageFiles = files.ToList();

                if (imageFiles.Count > 0)
                {
                    strBuilder.AppendLine($"{imageFiles.Count} ファイルの変換を開始します");
                }

                var counter = 0;

                foreach (var file in imageFiles)
                {
                    var output =
                        $@"{Path.GetDirectoryName(file.FullName)}\{Path.GetFileNameWithoutExtension(file.FullName)}.png";

                    counter++;
                    var countStr = $"{counter:D4}/{imageFiles.Count:D4}";

                    try
                    {
                        file.Status = ConvertImage(file.FullName, output);
                        file.Converted = true;
                        strBuilder.AppendLine($"ファイルの変換に成功しました {countStr} {file.FullName} -> {output}");

                        if (DeleteOriginalFile)
                        {
                            file.DeleteFile();
                            strBuilder.AppendLine($"変換に成功したファイルを削除しました {file.FullName}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        strBuilder.AppendLine($"ファイルの変換処理に失敗しました {file.FullName}");
                        throw;
                    }
                }
            }
        });

        public DelegateCommand ClearFileListCommand => new (() =>
        {
            ExFileInfos = new ObservableCollection<ExFileInfo>();
        });

        public DelegateCommand ClearConvertedCommand => new (() =>
        {
            ExFileInfos = new ObservableCollection<ExFileInfo>(ExFileInfos.Where(f => !f.Converted));
        });

        public void AddFile(string path)
        {
            ExFileInfos.Add(new ExFileInfo(new FileInfo(path)));
        }

        private static string ConvertImage(string inputPath, string outputPath)
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

                if (process.ExitCode == 0)
                {
                    return "変換成功";
                }

                // エラーコードが0以外の場合、変換に失敗した可能性があります
                Console.WriteLine($"Conversion failed. Error: {errorOutput}");
                throw new ImageConversionException("画像ファイルの変換に失敗しました")
                {
                    FailedFileName = inputPath,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "変換失敗";
            }
        }
    }
}