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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AbaciLabs.LEDConfig.UI.Models;

namespace AbaciLabs.LEDConfig.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowModel Model { get; private set; } = new MainWindowModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Model;
            return;
        }
        private void btnCommitSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Model.CommitSettings();
            return;
        }
        private void btnDiscover_Click(object sender, RoutedEventArgs e)
        {
            this.Model.Discover();
            return;
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
                this.Model.Discover();
            return;
        }

    }
}
