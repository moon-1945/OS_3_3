using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
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
        bool isUpdateThreadRunning = true;
        public MainWindow()
        {
            InitializeComponent();
            ProcessInfGrid.ItemsSource = processes;
            Closing += OnClosingWindow;

            new Thread(new ThreadStart(() =>
            {
                while (isUpdateThreadRunning)
                {
                    if (this.IsVisible) // Перевірка видимості вікна
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ProcessInfGrid.Items.Refresh();
                        });
                    }
                    Thread.Sleep(TIMEOUT);
                }
            })).Start();
        }

    

        private void OnClosingWindow(object? sender, CancelEventArgs e)
        {
            isUpdateThreadRunning = false;
        }

        private void CreateProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotepadRadioButton.IsChecked!.Value)
                processes.Add(Process.Start("notepad.exe")!);
            else if (PingRadioButton.IsChecked!.Value)
                processes.Add(Process.Start("ping")!);//TODO: Set command
            else if (SearchRadioButton.IsChecked!.Value)
                processes.Add(Process.Start("search.exe")!);//TODO: Create Search aloritm
            else if (TabulationRadioButton.IsChecked.HasValue)
                processes.Add(Process.Start("tabulation.exe")!);//TODO : put tabl.exe in executing directory
        }
        private void SuspendMenuItem_Click(object sender, RoutedEventArgs e)
        {

            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Suspend();
            }
        }

        private void ResumeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Resume();
            }
        }

        private void TerminateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Kill();
            }
        }

        private void Realtime_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Priority = ProcessPriorityClass.REALTIME;
            }
        }

        private void High_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Priority = ProcessPriorityClass.HIGH;
            }
        }

        private void AboveNormal_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Priority = ProcessPriorityClass.ABOVE_NORMAL;
            }
        }

        private void Normal_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Priority = ProcessPriorityClass.NORMAL;
            }
        }

        private void BelowNormal_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Priority = ProcessPriorityClass.BELOW_NORMAL;
            }
        }

        private void Idle_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessInfGrid.SelectedItem is Process selectedProcess)
            {
                selectedProcess.Priority = ProcessPriorityClass.IDLE;
            }
        }


        private void SetPriorityMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }



        private void SetAffinityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProcessInfGrid.SelectedItem is Process selectedProcess)) throw new Exception();

            object affinityWindow = FormatterServices.GetUninitializedObject(typeof(Processor_affinity));
            affinityWindow.GetType().GetProperty("Process").SetValue(affinityWindow, selectedProcess);
            affinityWindow.GetType().GetConstructor(Type.EmptyTypes).Invoke(affinityWindow, null);
            (affinityWindow as Processor_affinity).ShowDialog();
        }

    }
}
