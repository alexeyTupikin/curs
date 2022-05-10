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
using System.Data.SqlClient;
using System.Data;

namespace curs
{
    /// <summary>
    /// Логика взаимодействия для adminChoise.xaml
    /// </summary>
    public partial class adminChoise : Page
    {
        MainWindow mainWindow;
        public int flag_table = 1;
        public int flag_action = 1;
        public adminChoise(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;

            chouseTable.Items.Add("Дисциплины");
            chouseTable.Items.Add("Вопросы");
            chouseTable.Items.Add("Билеты");
            chouseTable.Items.Add("Пользователи");
            chouseTable.Items.Add("Нагрузки");

            chouseAction.Items.Add("Добавление записей");
            chouseAction.Items.Add("Удаление записей");
            chouseAction.Items.Add("Редактирование записей");
        }

        private void proceed_to_Click(object sender, RoutedEventArgs e) //получает выбранную таблицу и действие над ней и перенаправляет на необходимую форму
        {
            if(Convert.ToString(chouseTable.SelectedValue) == "Вопросы")
            {
                switch_questions(Convert.ToInt32(chouseAction.SelectedIndex));
                chouseAction.IsEnabled = false;
                proc_button.IsEnabled = false;
                chouseTable.SelectedValue = null;
                chouseAction.SelectedValue = null;
            }
            if (Convert.ToString(chouseTable.SelectedValue) == "Пользователи")
            {
                switch_users(Convert.ToInt32(chouseAction.SelectedIndex));
                chouseAction.IsEnabled = false;
                proc_button.IsEnabled = false;
                chouseTable.SelectedValue = null;
                chouseAction.SelectedValue = null;
            }
            if (Convert.ToString(chouseTable.SelectedValue) == "Билеты")
            {
                switch_ticket(Convert.ToInt32(chouseAction.SelectedIndex));
                chouseAction.IsEnabled = false;
                proc_button.IsEnabled = false;
                chouseTable.SelectedValue = null;
                chouseAction.SelectedValue = null;
            }
            if (Convert.ToString(chouseTable.SelectedValue) == "Нагрузки")
            {
                switch_load(Convert.ToInt32(chouseAction.SelectedIndex));
                chouseAction.IsEnabled = false;
                proc_button.IsEnabled = false;
                chouseTable.SelectedValue = null;
                chouseAction.SelectedValue = null;
            }
            if(Convert.ToString(chouseTable.SelectedValue) == "Дисциплины")
            {
                switch_discipline(Convert.ToInt32(chouseAction.SelectedIndex));
                chouseAction.IsEnabled = false;
                proc_button.IsEnabled = false;
                chouseTable.SelectedValue = null;
                chouseAction.SelectedValue = null;
            }
        }

        public int switch_questions(int action)
        {
            switch (action)
            {
                case 0:
                    {
                        mainWindow.OpenPage(MainWindow.pages.add_question);
                        break;
                    }
                case 1:
                    {
                        mainWindow.OpenPage(MainWindow.pages.deleteQuestion);
                        break;
                    }
                case 2:
                    {
                        mainWindow.OpenPage(MainWindow.pages.editing);
                        break;
                    }
            }
            return 0;
        }

        public int switch_users(int action)
        {
            switch (action)
            {
                case 0:
                    {
                        mainWindow.OpenPage(MainWindow.pages.add_users);
                        break;
                    }
                case 1:
                    {
                        mainWindow.OpenPage(MainWindow.pages.delete_user);
                        break;
                    }
                case 2:
                    {
                        mainWindow.OpenPage(MainWindow.pages.editing_user);
                        break;
                    }
            }
            return 0;
        }

        public int switch_ticket(int action)
        {
            switch (action)
            {
                case 0:
                    {
                        MessageBox.Show("Для таблицы ticket доступно лишь действие 'Удаление записей'");
                        break;
                    }
                case 1:
                    {
                        mainWindow.OpenPage(MainWindow.pages.delete_ticket);
                        break;
                    }
                case 2:
                    {
                        MessageBox.Show("Для таблицы ticket доступно лишь действие 'Удаление записей'");
                        break;
                    }
            }
            return 0;
        }

        public int switch_load(int action)
        {
            switch (action)
            {
                case 0:
                    {
                        mainWindow.OpenPage(MainWindow.pages.add_load);
                        break;
                    }
                case 1:
                    {
                        mainWindow.OpenPage(MainWindow.pages.add_load);
                        break;
                    }
                case 2:
                    {
                        mainWindow.OpenPage(MainWindow.pages.add_load);
                        break;
                    }
            }
            return 0;
        }

        public int switch_discipline(int action)
        {
            switch (action)
            {
                case 0:
                    {
                        mainWindow.OpenPage(MainWindow.pages.editing_discipline);
                        break;
                    }
                case 1:
                    {
                        mainWindow.OpenPage(MainWindow.pages.editing_discipline);
                        break;
                    }
                case 2:
                    {
                        mainWindow.OpenPage(MainWindow.pages.editing_discipline);
                        break;
                    }
            }
            return 0;
        }

        private void q_akk_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Вы действительно хотите выйти из аккаунта?","Нет", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                mainWindow.OpenPage(MainWindow.pages.login);
            }
        }

        private void q_p_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Вы действительно хотите выйти из приложения?", "Нет", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                mainWindow.Close();
            } 
        }

        private void chouseTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chouseTable.SelectedValue != null)
            {
                flag_table = 0;
                chouseAction.IsEnabled = true;
            }
            else flag_table = 1;
        }

        private void chouseAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(chouseAction.SelectedValue != null)
            {
                flag_action = 0;
                proc_button.IsEnabled = true;
            }
        }
    }
}
