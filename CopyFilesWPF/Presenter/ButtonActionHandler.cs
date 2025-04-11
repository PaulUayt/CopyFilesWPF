using CopyFilesWPF.Model;
using System.Windows.Controls;

namespace CopyFilesWPF.Presenter
{
    public static class ButtonActionHandler
    {
        public static void HandlePause(Button pauseButton)
        {
            if (TryGetFileCopier(pauseButton, out var fileCopier))
            {
                pauseButton.IsEnabled = false;

                if (pauseButton.Content.ToString() == "Pause") fileCopier.PauseFlag.Reset();
                else fileCopier.PauseFlag.Set();
            }
        }

        public static void HandleCancel(Button cancelButton)
        {
            if (TryGetFileCopier(cancelButton, out var fileCopier))
            {
                fileCopier.cancelTokenSource.Cancel();
            }
        }

        public static void UpdateProgressBar(double persentage, Grid grid)
        {
            foreach(var el in grid.Children)
            {
                switch(el)
                {
                    case ProgressBar progressBar:
                        progressBar.Value = persentage;
                        break;
                    case Button button when button.IsEnabled == false:
                        if (button.Content.ToString() == "Resume")
                        {
                            button.Content = "Pause";
                        }
                        else if (button.Content.ToString() == "Pause")
                        {
                            button.Content = "Resume";
                        }
                        button.IsEnabled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private static bool TryGetFileCopier(Button button, out FileCopier fileCopier)
        {
            fileCopier = null!;
            if (button.Tag is Grid grid && grid.Tag is FileCopier copier)
            {
                fileCopier = copier;
                return true;
            }
            return false;
        }
    }
}
