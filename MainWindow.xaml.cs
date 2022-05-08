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
using System.Data;
using System.Data.SqlClient;

namespace curs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string computer;
        public string database;
        public int current_user;
        public MainWindow()
        {
            InitializeComponent();
            OpenPage(pages.login);
            //DESKTOP-VCUQV2I\SQLEXPRESS
            //DESKTOP-9SNLLHT\SQLEXPRESS01
            computer = @"DESKTOP-9SNLLHT\SQLEXPRESS01";
            database = @"kurs_Alexey";
            try
            {
                SqlServer.CreateConnecion(computer, database);
            } 
            catch
            {
                MessageBox.Show("Для продолжения работы перейдите в меню Настройки и измените подключение.");
                OpenPage(pages.options);
            }
        }
        public enum pages
        {
            login,
            options,
            teacherhChoise,
            adminChoise,
            generate,
            add_question,
            deleteQuestion,
            editing,
            add_users,
            delete_user,
            editing_user,
            delete_ticket,
            add_load,
            editing_discipline
        }
        public void OpenPage(pages page)
        {
            if (page == pages.login)
            {
                go_back.IsEnabled = false;
                frame.Navigate(new login(this));
            }
            if (page == pages.options)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new options(this));
            }
            if (page == pages.teacherhChoise)
            {
                go_back.IsEnabled = false;
                frame.Navigate(new teacherhChoise(this));
            }
            if (page == pages.adminChoise)
            {
                go_back.IsEnabled = false;
                frame.Navigate(new adminChoise(this));
            }
            if (page == pages.generate)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new generate(this));
            }
            if (page == pages.add_question)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new add_question(this));
            }
            if (page == pages.deleteQuestion)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new deleteQuestion(this));
            }
            if (page == pages.editing)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new editing(this));
            }
            if (page == pages.add_users)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new add_users(this));
            }
            if (page == pages.delete_user)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new delete_user(this));
            }
            if (page == pages.editing_user)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new editing_user(this));
            }
            if (page == pages.delete_ticket)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new delete_ticket(this));
            }
            if (page == pages.add_load)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new add_load(this));
            }
            if (page == pages.editing_discipline)
            {
                go_back.IsEnabled = true;
                frame.Navigate(new editing_discipline(this));
            }
        }
        public DataTable Select(string selectSQL)  
        {
            DataTable dataTable = new DataTable();
            SqlCommand command = SqlServer.CreateSqlCommand(selectSQL);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }
        public void close()
        {
            this.Close();
        }

        public DataTable CommandDB(SqlCommand selectSql)
        {
            DataTable dataTable = new DataTable();
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(selectSql);
                dataAdapter.Fill(dataTable);
            } 
            catch (Exception ex)
            {
                MessageBox.Show($"Error {ex}");
            }
            return dataTable;
        }

        private void go_back_Click(object sender, RoutedEventArgs e)
        {
            if (frame.CanGoBack == true)
            {
                frame.GoBack();
                go_back.IsEnabled = false;
            }
        }
    }
}

