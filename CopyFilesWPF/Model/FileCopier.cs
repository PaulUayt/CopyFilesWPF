using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CopyFilesWPF.Model
{
    public class FileCopier
    {
        private readonly Grid _gridPanel;
        private readonly FilePath _filePath;

        public delegate void ProgressChangeDelegate(double progress, Grid gridPanel);
        public delegate void CompleteDelegate(Grid gridPanel);
        public event ProgressChangeDelegate OnProgressChanged;
        public event CompleteDelegate OnComplete;

        public CancellationTokenSource cancelTokenSource;
        public CancellationToken token;
        public ManualResetEvent PauseFlag = new(true);

        public FileCopier(
            FilePath filePath,
            ProgressChangeDelegate onProgressChange,
            CompleteDelegate onComplete,
            Grid gridPanel)
        {
            OnProgressChanged += onProgressChange;
            OnComplete += onComplete;
            _filePath = filePath;
            _gridPanel = gridPanel;
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
        }

        public void CopyFile()
        {
            byte[] buffer = new byte[1024 * 1024];


            while (!token.IsCancellationRequested)
            {
                try
                {
                    using(var source = new FileStream(_filePath.PathFrom, FileMode.Open, FileAccess.Read))
                    {
                        var fileLength = source.Length;
                        using var destination = new FileStream(_filePath.PathTo, FileMode.CreateNew, FileAccess.Write);
                        long totalBytes = 0;
                        int currentBlockSize = 0;
                        while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytes += currentBlockSize;
                            double persentage = totalBytes * 100.0 / fileLength;
                            destination.Write(buffer, 0, currentBlockSize);
                            OnProgressChanged(persentage, _gridPanel);

                            if(token.IsCancellationRequested)
                            {
                                File.Delete(_filePath.PathTo);
                                break;
                            }

                            PauseFlag.WaitOne(Timeout.Infinite); // переделать на thread suspend
                        }
                    }
                    cancelTokenSource.Cancel();
                    cancelTokenSource.Dispose();
                }
                catch (IOException error)
                {
                    // порефакторить код ниже
                    if (token.IsCancellationRequested)
                    {
                        var result = MessageBox.Show(error.Message + " Copying was canceled!", "Cancel", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        File.Delete(_filePath.PathTo);
                    }
                    else
                    {
                        var result = MessageBox.Show(error.Message + " Replace?", "Replace?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes) File.Delete(_filePath.PathTo);
                        else cancelTokenSource.Cancel();
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "Error occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            OnComplete(_gridPanel);
        }
    }
}
