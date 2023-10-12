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
using static OS_3_3.WindowsApi;

namespace OS_3_3
{
    /// <summary>
    /// Interaction logic for Processor_affinity.xaml
    /// </summary>
    public partial class Processor_affinity : Window
    {
        public uint _coresNumber;
        public Process Process {get; init;}

        public Processor_affinity()
        {
            InitializeComponent();

            _coresNumber = GetCoresNumber();

            kernelsCheck.Items.Clear();

            CheckBox allCheck = new CheckBox()
            {
                Content = $"<All processors>",
            };

            allCheck.Checked += AllCheck_Checked;
            allCheck.Unchecked += AllCheck_Unchecked;
            
            kernelsCheck.Items.Add(allCheck);

            for (int i = 0; i < _coresNumber; i++)
            {
                CheckBox affinityCheck = new CheckBox()
                {
                    Content = $"CPU {i}",
                };

                kernelsCheck.Items.Add(affinityCheck);
            }
        }

        private uint GetCoresNumber()
        {
            GetSystemInfo(out SYSTEM_INFO systemInfo);

            return systemInfo.dwNumberOfProcessors;
        }

        private void ChangeAllKernelChecks(bool state)
        {
            for (int i = 1; i < kernelsCheck.Items.Count; i++)
            {
                if (!(kernelsCheck.Items[i] is CheckBox kernelCheck)) throw new Exception();

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

            Process.SetAffinityMask(affinityMask);

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
