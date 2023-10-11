using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OS_3_3;

namespace OS_3_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int TIMEOUT = 100;
        private readonly List<Process> processes = new(4);
        public MainWindow()
        {
            InitializeComponent();
            ProcessInfGrid.ItemsSource = processes;
            new Thread(new ThreadStart( () =>
            {
                while (true)
                {
                    if(!this.IsVisible) // if window is not visible, then stop updating
                        Dispatcher.Invoke(() =>
                        {
                            ProcessInfGrid.Items.Refresh();
                        });
                    Thread.Sleep(TIMEOUT);
                }
            })).Start();
        }

        private void CreateProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotepadRadioButton.IsChecked!.Value)
                processes.Add( Process.Start("notepad.exe")!);
            else if (PingRadioButton.IsChecked!.Value)
                processes.Add(Process.Start("ping")!);//TODO: Set command
            else if (SearchRadioButton.IsChecked!.Value)
                processes.Add(Process.Start("search.exe")!);//TODO: Create Search aloritm
            else if (TabulationRadioButton.IsChecked.HasValue)
                processes.Add(Process.Start("tabulation.exe")!);//TODO : put tabl.exe in executing directory
        }
    }
}
