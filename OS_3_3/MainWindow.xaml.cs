using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly ProcessManager processes = new();

        public MainWindow()
        {
            InitializeComponent();
            ProcessInfGrid.ItemsSource = processes;
            Closing += OnClosingWindow;
        }

        private void OnClosingWindow(object? sender, CancelEventArgs e)
        {
            processes.Dispose();
        }

        private void CreateProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotepadRadioButton.IsChecked!.Value)
                processes.CreateNew("C:\\Program Files\\Notepad++\\notepad++.exe");
            else if (PingRadioButton.IsChecked!.Value)
                processes.CreateNew("ping");//TODO: Set command
            else if (SearchRadioButton.IsChecked!.Value)
                processes.CreateNew("search.exe");//TODO: Create Search aloritm
            else if (TabulationRadioButton.IsChecked!.Value)
                processes.CreateNew("tabulation.exe");//TODO : put tabl.exe in executing directory
            
        }
        #region ContentMenuClickHandlers
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

        #region PriorityClickHandlers
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
        #endregion

        private void SetAffinityMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProcessInfGrid.SelectedItem is Process selectedProcess)) throw new Exception();

            object affinityWindow = FormatterServices.GetUninitializedObject(typeof(Processor_affinity));
            affinityWindow.GetType().GetProperty("Process").SetValue(affinityWindow, selectedProcess);
            affinityWindow.GetType().GetConstructor(Type.EmptyTypes).Invoke(affinityWindow, null);
            (affinityWindow as Processor_affinity).ShowDialog();
        }
        #endregion

    }
}
