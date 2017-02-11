using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Navigation;

namespace livechart2
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : MetroWindow
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //  credits:
            //  http://stackoverflow.com/questions/21881124/how-do-you-get-navigateuri-to-work-in-a-wpf-window
            //
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void KeyUp_Handler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
