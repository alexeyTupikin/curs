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
    /// Логика взаимодействия для login.xaml
    /// </summary>
    public partial class login : Page
    {
        MainWindow mainWindow;
        public login(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            loginText.Text = "";
            passwordText.Password = "";
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (loginText.Text.Length > 0) // проверяем введён ли логин     
            {
                if (passwordText.Password.Length > 0) // проверяем введён ли пароль         
                {             // ищем в базе данных пользователя с такими данными         

                    string text_command = "EXECUTE login_p @login, @password";
                    SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                    command.Parameters.Add("@login", SqlDbType.NVarChar).Value = Convert.ToString(loginText.Text);
                    command.Parameters.Add("@password", SqlDbType.NVarChar).Value = Convert.ToString(passwordText.Password);
                    DataTable dt_user = mainWindow.CommandDB(command);
                    if (dt_user.Rows.Count > 0) // если такая запись существует       
                    {
                        MessageBox.Show("Пользователь авторизовался"); // говорим, что авторизовался

                        switch (dt_user.Rows[0][0].ToString())
                        {
                            case "0":
                                mainWindow.current_user = 0;
                                mainWindow.Title = $"Генератор экзаменационных билетов (Админ)";
                                mainWindow.OpenPage(MainWindow.pages.adminChoise);
                                break;
                            case "2":
                                mainWindow.current_user = 2;
                                mainWindow.Title = $"Генератор экзаменационных билетов (ПЦК)";
                                break;
                            case "1":
                                mainWindow.current_user = 1;
                                mainWindow.Title = $"Генератор экзаменационных билетов (Преподаватель)";
                                mainWindow.OpenPage(MainWindow.pages.teacherhChoise);
                                break;
                        }

                        switch (mainWindow.current_user.ToString())
                        {
                            case "0":
                                MessageBox.Show("Ваш тип пользователя: Админ");
                                break;
                            case "1":
                                MessageBox.Show("Ваш тип пользователя: Преподаватель");
                                break;
                            case "2":
                                MessageBox.Show("Ваш тип пользователя: ПЦК");
                                break;
                        }
                    }
                    else MessageBox.Show("Пользователь не найден"); // выводим ошибку  
                }
                else MessageBox.Show("Введите пароль"); // выводим ошибку    
            }
            else MessageBox.Show("Введите логин"); // выводим ошибку 
        }
        private void changeConnection(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPage(MainWindow.pages.options);
        }
    }
}
