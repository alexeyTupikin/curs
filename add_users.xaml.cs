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
    /// Логика взаимодействия для add_users.xaml
    /// </summary>
    public partial class add_users : Page
    {
        MainWindow mainWindow;
        public int flag_lvl_user;
        public add_users(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            chouse_lvl.Items.Add("0 - Админ");
            chouse_lvl.Items.Add("1 - Преподаватель");
            chouse_lvl.Items.Add("2 - Председатель ЦК");
        }

        private void save_new_user_Click(object sender, RoutedEventArgs e) //сохраняет нового пользователя в базу данных
        {
            int flag_login = 0;
            int flag_password = 0;
            int flag_check_user = 0;
            DataTable qty_rows_user = mainWindow.Select("SELECT COUNT(id_user) FROM users");
            int qty_users = Convert.ToInt32(qty_rows_user.Rows[0][0]);
            string login = Convert.ToString(new_login.Text);
            //check new login
            DataTable check_login = mainWindow.Select("SELECT login FROM users");
            for (int i = 0; i < qty_users; i++)
            {
                if (login == (Convert.ToString(check_login.Rows[i][0])))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует, введите другой логин");
                    flag_login = 1;
                }
            }
            //
            //check new password
            string password = Convert.ToString(new_password.Text);
            DataTable check_password = mainWindow.Select("SELECT password FROM users");
            for (int i = 0; i < qty_users; i++)
            {
                if (password == (Convert.ToString(check_password.Rows[i][0])))
                {
                    MessageBox.Show("Пользователь с таким паролем уже существует, введите другой логин");
                    flag_password = 1;
                }
            }
            //
            if ((flag_login == 0) && (flag_password == 0))
            {
                if (flag_lvl_user == 1)
                {
                    //save_for_teacher
                    flag_check_user = check_for_teacher();
                    if (flag_check_user == 0)
                    {
                        save_for_teacher();
                    }
                }
                else if (flag_lvl_user == 2)
                {
                    //save_for_pck
                    flag_check_user = check_for_pck();
                    if (flag_check_user == 0)
                    {
                        save_for_pck();
                    }
                }
                else if (flag_lvl_user == 0)
                {
                    //save_for_admin
                    int lvl_access = Convert.ToInt32(chouse_lvl.SelectedIndex);
                    string insert_login = "";
                    string insert_password = "";
                    if ((flag_login == 0) && (flag_password == 0))
                    {
                        insert_login = login;
                        insert_password = password;
                        string text_command = "EXECUTE save_user @new_login, @new_password, @lvl_access";
                        SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                        command.Parameters.Add("@new_login", SqlDbType.NVarChar).Value = insert_login;
                        command.Parameters.Add("@new_password", SqlDbType.NVarChar).Value = insert_password;
                        command.Parameters.Add("@lvl_access", SqlDbType.Int).Value = lvl_access;
                        DataTable save_user = mainWindow.CommandDB(command);
                        MessageBox.Show("Новый пользователь успешно добавлен.");
                    }
                }
            }
        }

        private void check_new_user_Click(object sender, RoutedEventArgs e) //проверяет ввод
        {
            int flag_check = 0;
            MessageBoxResult messageBoxResult1 = System.Windows.MessageBox.Show("Вы уверены в введенных вами данных?", "Нет", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult1 == MessageBoxResult.Yes)
            {
                ///////////
                if (new_login.Text == "")
                {
                    MessageBox.Show("Вы не ввели логин для нового пользователя");
                }
                else if (new_password.Text == "")
                {
                    MessageBox.Show("Вы не ввели пароль для нового пользователя");
                }
                else if (chouse_lvl.SelectedIndex < 0)
                {
                    MessageBox.Show("Вы не выбрали уровень доступа для нового пользователя");
                }
                else if (new_login.Text.Length < 6)
                {
                    MessageBox.Show("Введенный вами логин содержит менее 6 символов.");
                }
                else if (new_password.Text.Length < 8)
                {
                    MessageBox.Show("Введенный вами пароль содержит менее 8 символов.");
                }
                else if (chouse_lvl.SelectedIndex == 1)
                {
                    flag_lvl_user = 1;
                    MessageBox.Show("Для выбранного вами типа пользователя (учитель) необходимо заполнить его ФИО. Поля для ввода " +
                        "этих данных будут справа, от введенных вами ранее.");
                    new_last_name.IsEnabled = true;
                    new_name.IsEnabled = true;
                    new_second_name.IsEnabled = true;
                }
                else if (chouse_lvl.SelectedIndex == 2)
                {
                    flag_lvl_user = 2;
                    MessageBox.Show("Для выбранного вами типа пользователя (председатель ЦК) необходимо заполнить его ФИО и данные о ЦК. Поля для ввода " +
                        "этих данных будут справа от тех, с котрыми вы работали ранее.");
                    new_last_name.IsEnabled = true;
                    new_name.IsEnabled = true;
                    new_second_name.IsEnabled = true;
                    new_num_ck.IsEnabled = true;
                    new_title_ck.IsEnabled = true;
                }
                else flag_lvl_user = 0;
                if(chouse_lvl.SelectedIndex == 1)
                {
                    flag_check = check_for_teacher();
                    if(flag_check == 0)
                    {
                        save_new_user.IsEnabled = true;
                    }
                } else if (chouse_lvl.SelectedIndex == 1)
                {
                    flag_check = check_for_pck();
                    if (flag_check == 0)
                    {
                        save_new_user.IsEnabled = true;
                    }
                } else if(chouse_lvl.SelectedIndex == 0)
                {
                    save_new_user.IsEnabled = true;
                }

            }
        }
        
        public int check_for_teacher() //проверяет ввод данных для учителя
        {
            int flag_check = 0;
            if (new_last_name.Text == "")
            {
                MessageBox.Show("Вы не ввели фамилию для нового пользователя.");
                flag_check = 1;
            }
            if (new_name.Text == "")
            {
                MessageBox.Show("Вы не ввели имя для нового пользователя.");
                flag_check = 1;
            }
            if (new_second_name.Text == "")
            {
                MessageBoxResult messageBoxResult1 = System.Windows.MessageBox.Show("Вы не ввели отчевство для нового пользователя. Нажмите Да в том случае, если" +
                    " у нового пользователя нет отчевства.", "Нет", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult1 == MessageBoxResult.No)
                {
                    MessageBox.Show("Пожалуйства введите отчевство для нового пользователя.");
                    flag_check = 1;
                }
            }
            return flag_check;
        }

        public int check_for_pck() //проверяет ввод данных для пцк
        {
            int flag_check = 0;
            if (new_last_name.Text == "")
            {
                MessageBox.Show("Вы не ввели фамилию для нового пользователя.");
                flag_check = 1;
            }
            if (new_name.Text == "")
            {
                MessageBox.Show("Вы не ввели имя для нового пользователя.");
                flag_check = 1;
            }
            if (new_second_name.Text == "")
            {
                MessageBoxResult messageBoxResult1 = System.Windows.MessageBox.Show("Вы не ввели отчевство для нового пользователя. Нажмите Да в том случае, если" +
                    " у нового пользователя нет отчевства.", "Нет", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult1 == MessageBoxResult.No)
                {
                    MessageBox.Show("Пожалуйства введите отчевство для нового пользователя.");
                    flag_check = 1;
                }
            }
            if(new_num_ck.Text == "")
            {
                MessageBox.Show("Вы не ввели номер ЦК");
                flag_check = 1;
            }
            if(new_title_ck.Text == "")
            {
                MessageBox.Show("Вы не ввели название ЦК");
                flag_check = 1;
            }
            return flag_check;
        }

        public void save_for_teacher() //сохраняет информацию о новом учителе
        {
            string text_command = "EXECUTE save_user @new_login, @new_password, @lvl_access";
            SqlCommand command = SqlServer.CreateSqlCommand(text_command);
            command.Parameters.Add("@new_login", SqlDbType.NVarChar).Value = new_login.Text;
            command.Parameters.Add("@new_password", SqlDbType.NVarChar).Value = new_password.Text;
            command.Parameters.Add("@lvl_access", SqlDbType.Int).Value = chouse_lvl.SelectedIndex;
            DataTable save_user = mainWindow.CommandDB(command);
            string text_sel_id = "SELECT id_user FROM users WHERE login = @login AND password = @password";
            SqlCommand sel_id = SqlServer.CreateSqlCommand(text_sel_id);
            sel_id.Parameters.Add("@login", SqlDbType.NVarChar).Value = new_login.Text;
            sel_id.Parameters.Add("@password", SqlDbType.NVarChar).Value = new_password.Text;
            DataTable sel_id_p = mainWindow.CommandDB(sel_id);
            string text_save_teacher = "EXECUTE save_teacher @new_id_user, @new_last_name, @new_name, @new_second_name";
            SqlCommand save_teacher = SqlServer.CreateSqlCommand(text_save_teacher);
            save_teacher.Parameters.Add("@new_id_user", SqlDbType.NVarChar).Value = Convert.ToInt32(sel_id_p.Rows[0][0]);
            save_teacher.Parameters.Add("@new_last_name", SqlDbType.NVarChar).Value = new_last_name.Text;
            save_teacher.Parameters.Add("@new_name", SqlDbType.NVarChar).Value = new_name.Text;
            save_teacher.Parameters.Add("@new_second_name", SqlDbType.NVarChar).Value = new_second_name.Text;
            DataTable save_teacher_sql = mainWindow.CommandDB(save_teacher);
        }

        public void save_for_pck() //сохраняет информацию для нового пцк
        {
            string text_command = "EXECUTE save_user @new_login, @new_password, @lvl_access";
            SqlCommand command = SqlServer.CreateSqlCommand(text_command);
            command.Parameters.Add("@new_login", SqlDbType.NVarChar).Value = new_login.Text;
            command.Parameters.Add("@new_password", SqlDbType.NVarChar).Value = new_password.Text;
            command.Parameters.Add("@lvl_access", SqlDbType.Int).Value = chouse_lvl.SelectedIndex;
            DataTable save_user = mainWindow.CommandDB(command);
            string text_sel_id = "SELECT id_user FROM users WHERE login = @login AND password = @password";
            SqlCommand sel_id = SqlServer.CreateSqlCommand(text_sel_id);
            sel_id.Parameters.Add("@login", SqlDbType.NVarChar).Value = new_login.Text;
            sel_id.Parameters.Add("@password", SqlDbType.NVarChar).Value = new_password.Text;
            DataTable sel_id_p = mainWindow.CommandDB(sel_id);
            string text_save_pck = "EXECUTE save_pck @new_id_user, @new_last_name, @new_name, @new_second_name, @new_num_ck, @new_title_ck";
            SqlCommand save_pck = SqlServer.CreateSqlCommand(text_save_pck);
            save_pck.Parameters.Add("@new_id_user", SqlDbType.NVarChar).Value = Convert.ToInt32(sel_id_p.Rows[0][0]);
            save_pck.Parameters.Add("@new_last_name", SqlDbType.NVarChar).Value = new_last_name.Text;
            save_pck.Parameters.Add("@new_name", SqlDbType.NVarChar).Value = new_name.Text;
            save_pck.Parameters.Add("@new_second_name", SqlDbType.NVarChar).Value = new_second_name.Text;
            save_pck.Parameters.Add("@new_num_ck", SqlDbType.Int).Value = Convert.ToInt32(new_num_ck.Text);
            save_pck.Parameters.Add("@new_title_ck", SqlDbType.NVarChar).Value = new_title_ck.Text;
            DataTable save_teacher_sql = mainWindow.CommandDB(save_pck);
        }

    }
}
