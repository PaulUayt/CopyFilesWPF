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

        // Цей код вже порефакторений
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

        

        // Порефакторений код
        private void PauseClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if(sender is Button pauseB)
            {
                ButtonActionHandler.HandlePause(pauseB);
            }
        }

        private void CancelClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button cancelB)
            {
                ButtonActionHandler.HandleCancel(cancelB);
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

        // Порефакторений код
        private void ProgressChanged(double persentage, Grid panel)
        {
            _mainWindowView.MainWindowView.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    ButtonActionHandler.UpdateProgressBar(persentage, panel);
                });
        }
    }
}
