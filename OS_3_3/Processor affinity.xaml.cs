using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace OS_3_3
{
    /// <summary>
    /// Interaction logic for Processor_affinity.xaml
    /// </summary>
    public partial class ProcessorAffinityWindow : Window
    {
        private readonly uint _coresNumber;
        public Process Process {get; init;}

        public ProcessorAffinityWindow(Process process)
        {
            Process = process;

            InitializeComponent();

            upLabel.Text = $"Which processors are alowed to run \"{Process!.Name}\"";

            _coresNumber = WindowsApi.GetCoresNumber();

            ulong affinityMask = Process.GetAffinityMask();

            kernelsCheck.Items.Clear();

            CheckBox allCheck = new()
            {
                Content = $"<All processors>",
            };

            allCheck.Checked += AllCheck_Checked;
            allCheck.Unchecked += AllCheck_Unchecked;
            
            kernelsCheck.Items.Add(allCheck);

            for (int i = 0; i < _coresNumber; i++)
            {
                CheckBox affinityCheck = new()
                {
                    Content = $"CPU {i}",
                    IsChecked = (affinityMask >> i) % 2 != 0,
                };

                kernelsCheck.Items.Add(affinityCheck);
            }
            //if all processors are alowed to run set  allCheck.IsChecked = true
            if (affinityMask == (1ul << (int)_coresNumber) - 1)
            {
                allCheck.IsChecked = true;
            }
        }

        private void ChangeAllKernelChecks(bool state)
        {
            for (int i = 1; i < kernelsCheck.Items.Count; i++)
            {
                if (kernelsCheck.Items[i] is not CheckBox kernelCheck) continue;

                kernelCheck.IsChecked = state;
            }
        }

        private void AllCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeAllKernelChecks(false);
        }

        private void AllCheck_Checked(object sender, RoutedEventArgs e)
        {
            ChangeAllKernelChecks(true);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            ulong affinityMask = 0;

            for (int i = 0; i < _coresNumber;i++)
            {
                if (((CheckBox)(kernelsCheck.Items[i + 1])).IsChecked == false) continue;

                affinityMask |=  (1ul << i); 
            }

            Process!.SetAffinityMask(affinityMask);

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
