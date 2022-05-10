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
    /// Логика взаимодействия для options.xaml
    /// </summary>
    public partial class options : Page
    {
        MainWindow mainWindow;
        public options(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }

        private void saveSettings(object sender, RoutedEventArgs e) //создание нового подключения
        {
            if (computer.Text.Length > 0)
            {
                if (database.Text.Length > 0)
                {
                    mainWindow.computer = computer.Text;
                    mainWindow.database = database.Text;
                    try
                    {
                        SqlServer.CreateConnecion(mainWindow.computer, mainWindow.database);
                    }
                    catch
                    {
                        MessageBox.Show("Введенный вами сервер или название базы данных не " +
                            "существует. Попробуйте ввести данные снова.");
                        mainWindow.OpenPage(MainWindow.pages.options);
                    }
                    mainWindow.OpenPage(MainWindow.pages.login);
                }
                else MessageBox.Show("Вы не ввели имя базы данных");
            }
            else MessageBox.Show("Вы не ввели имя компьютера");
        }
        private void returnToLogin(object sender, RoutedEventArgs e) //возврат на форму авторизации
        {
            mainWindow.OpenPage(MainWindow.pages.login);
        }
    }
}
