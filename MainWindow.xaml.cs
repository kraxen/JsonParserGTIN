using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.ComponentModel;

namespace JsonParserGTIN
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Kiz> kizs = new List<Kiz>();
        List<ResultObject> results = new List<ResultObject>();
        List<string> gtins = new List<string>();
        string host;
        string token;
        public MainWindow()
        {
            InitializeComponent();

            RadioButton rb1 = new RadioButton { IsChecked = true, GroupName = "host", Content = "prod" };
            RadioButton rb2 = new RadioButton { IsChecked = true, GroupName = "host", Content = "preprod" };
            RadioButton rb3 = new RadioButton { IsChecked = true, GroupName = "host", Content = "demo" };
            rb1.Checked += radioButton_Checked;
            rb2.Checked += radioButton1_Checked;
            rb3.Checked += radioButton_Checked;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            int gtinCountOneOperation = Int32.Parse(GtinCount.Text);
            textBlock1.Text = "Обработка";
            gtins = Import.getGtins(path.Text);
            kizs = Import.getResponce(gtins, token, host, gtinCountOneOperation);
            results = Handler.hendler(kizs, gtins);
            Export.export(@"result\result.txt", results);
            textBlock1.Text = "Готово";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                path.Text = openFileDialog.FileName;
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.host = File.ReadAllText(@"hosts\prod.txt");
            this.token = Import.getTokenProd();
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            this.host = File.ReadAllText(@"hosts\preprod.txt");
            this.token = Import.getTokenPreProd();
        }

        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            this.host = File.ReadAllText(@"hosts\demo.txt");
            this.token = Import.getTokenDemo();
        }
    }
}