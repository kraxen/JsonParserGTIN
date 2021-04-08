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

            path.Text = File.ReadAllText(@"path\path.txt");
            host = File.ReadAllText(@"hosts\prod.txt");

            RadioButton rb1 = new RadioButton { IsChecked = true, GroupName = "tokenhost", Content = "Prod" };
            RadioButton rb3 = new RadioButton { IsChecked = true, GroupName = "tokenhost", Content = "NewToken" };
            rb1.Checked += getTokenProd;
            rb3.Checked += getTokenNew;
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

        private void getTokenProd(object sender, RoutedEventArgs e)
        {
            this.token = Import.getTokenProd();
        }

        private void getTokenNew(object sender, RoutedEventArgs e)
        {
            this.token = Import.getTokenNew();
        }
    }
}