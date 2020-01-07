using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BarcodeReader
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {       

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

           

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.e_Hidtxt.PreviewKeyDown -= E_Hidtxt_PreviewKeyDown;
            this.e_Hidtxt.PreviewKeyDown += E_Hidtxt_PreviewKeyDown;
        }

        private void E_Hidtxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var vm = this.DataContext as MainViewModel;
                if(vm != null)
                {
                    vm.KeyStroke.Execute(null);
                    this.e_Hidtxt.SelectAll();
                }
            }
        }
    }
}
