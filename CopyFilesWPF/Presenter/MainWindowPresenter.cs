using CopyFilesWPF.Model;
using CopyFilesWPF.View;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CopyFilesWPF.Presenter
{
    public class MainWindowPresenter : IMainWindowPresenter
    {
        private readonly IMainWindowView _mainWindowView;
        private readonly MainWindowModel _mainWindowModel;
        private readonly CopyPanelFactory _copyPanelFactory;

        public MainWindowPresenter(IMainWindowView mainWindowView) {
            _mainWindowView = mainWindowView;
            _mainWindowModel = new MainWindowModel();
            _copyPanelFactory = new CopyPanelFactory();
        }

        public void ChooseFileFromButtonClick(string path)
        {
            _mainWindowModel.FilePath.PathFrom = path;
        }

        public void ChooseFileToButtonClick(string path)
        {
            _mainWindowModel.FilePath.PathTo = path;
        }

        // порефакторить этот метод, убрать хардкод, разделить на более мелкие методы
        public void CopyButtonClick()
        {
            SetMainModelSettings();

            var filePathFrom = _mainWindowModel.FilePath.PathFrom;
            var newPanel = _copyPanelFactory.Create(filePathFrom, PauseClick, CancelClick);

            _mainWindowView.MainWindowView.MainPanel.Children.Add(newPanel);
            _mainWindowModel.CopyFile(ProgressChanged, ModelOnComplete, newPanel);
        }

        private void SetMainModelSettings()
        {
            _mainWindowModel.FilePath.PathFrom = _mainWindowView.MainWindowView.FromTextBox.Text;
            _mainWindowModel.FilePath.PathTo = _mainWindowView.MainWindowView.ToTextBox.Text;

            _mainWindowView.MainWindowView.FromTextBox.Text = "";
            _mainWindowView.MainWindowView.ToTextBox.Text = "";

            _mainWindowView.MainWindowView.Height += 60;
        }

        

        // порефакторить этот метод, убрать хардкод, и переделать его по SOLID (тут несколько ответсвенностей)
        private void PauseClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var pauseB = (Button)sender;
            pauseB.IsEnabled = false;

            if (pauseB!.Content.ToString()!.Equals("Pause"))
            {
                ((pauseB.Tag as Grid)!.Tag as FileCopier)!.PauseFlag.Reset();
            }
            else
            {
                ((pauseB.Tag as Grid)!.Tag as FileCopier)!.PauseFlag.Set();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var cancelB = (Button)sender;
            if (cancelB!.Content.ToString()!.Equals("Cancel"))
            {
                ((cancelB.Tag as Grid)!.Tag as FileCopier)!.cancelTokenSource.Cancel();
            }
        }

        private void ModelOnComplete(Grid panel)
        {
            _mainWindowView.MainWindowView.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    _mainWindowView.MainWindowView.Height = _mainWindowView.MainWindowView.Height - 60;
                    _mainWindowView.MainWindowView.MainPanel.Children.Remove(panel);
                    _mainWindowView.MainWindowView.CopyButton.IsEnabled = true;
                }
            );
        }

        // порефакторить этот метод, убрать хардкод, и переделать его по SOLID (тут несколько ответсвенностей)
        private void ProgressChanged(double persentage, Grid panel)
        {
            _mainWindowView.MainWindowView.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    foreach (var el in panel.Children)
                    {
                        if (el is ProgressBar bar)
                        {
                            bar.Value = persentage;
                        }
                        if (el is Button button1 && button1!.Content.ToString()!.Equals("Resume") && button1!.IsEnabled == false)
                        {
                            button1.Content = "Pause";
                            button1.IsEnabled = true;
                        }
                        else if (el is Button button && button!.Content.ToString()!.Equals("Pause") && button.IsEnabled == false)
                        {
                            button.Content = "Resume";
                            button.IsEnabled = true;
                        }
                    }
                }
            );
        }
    }
}
