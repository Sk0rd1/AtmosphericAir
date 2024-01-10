using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
using System.Data.OleDb;
using System.Data;
using Microsoft.Reporting.WinForms;
using Npgsql;
using wpfAA;
using System.Windows.Forms;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Password;
            if (login != string.Empty && password != string.Empty)
            {
                DB.CreateConnection("localhost", 1029, login, password, "AtmosphericAir");
                bool isConnected = DB.OpenConnection();
                if (isConnected)
                {

                    MainInfo mainInfo = new MainInfo(login);
                    Content = mainInfo.Content;
                }
                else
                {
                    System.Windows.MessageBox.Show("   Не підключено!   ", "Помилка");
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}