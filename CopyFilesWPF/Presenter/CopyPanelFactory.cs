using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace CopyFilesWPF.Presenter
{
    class CopyPanelFactory
    {
        public Grid Create(string filePath, RoutedEventHandler pauseClick, RoutedEventHandler cancelClick )
        {
            var newPanel = CreatePanel();

            var nameFile = CreateTextBlock(filePath);
            var progressBar = CreateProgressBar();
            var pauseB = CreatePauseButton(newPanel, "Pause");
            var cancelB = CreatePauseButton(newPanel, "Cancel");

            pauseB.Click += pauseClick;
            cancelB.Click += cancelClick;

            AddElementOnGrid(nameFile, newPanel, 0, 0);
            AddElementOnGrid(progressBar, newPanel, 1, 0);
            AddElementOnGrid(pauseB, newPanel, 1, 1);
            AddElementOnGrid(cancelB, newPanel, 1, 2);

            DockPanel.SetDock(newPanel, Dock.Top);
            newPanel.Height = 60;

            return newPanel;
        }

        private void AddElementOnGrid(UIElement elem, Grid grid, int? row, int? column)
        {
            if (row.HasValue) Grid.SetRow(elem, (int)row);
            if (column.HasValue) Grid.SetColumn(elem, (int)column);
            grid.Children.Add(elem);
        }

        private Grid CreatePanel()
        {
            var newPanel = new Grid();
            newPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(320) });
            newPanel.ColumnDefinitions.Add(new ColumnDefinition());
            newPanel.ColumnDefinitions.Add(new ColumnDefinition());
            newPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            newPanel.RowDefinitions.Add(new RowDefinition());

            return newPanel;
        }

        private TextBlock CreateTextBlock(string filePathFrom)
        {
            var nameFile = new TextBlock
            {
                Text = Path.GetFileName(filePathFrom),
                Margin = new Thickness(5, 0, 5, 0)
            };
            return nameFile;
        }

        private ProgressBar CreateProgressBar()
        {
            var progressBar = new ProgressBar
            {
                Margin = new Thickness(10, 10, 10, 10)
            };
            return progressBar;
        }

        private Button CreatePauseButton(Grid newPanel, string content)
        {
            var button = new Button
            {
                Content = content,
                Margin = new Thickness(5),
                Tag = newPanel
            };
            return button;
        }
    }
}
