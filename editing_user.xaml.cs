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
    /// Логика взаимодействия для editing_user.xaml
    /// </summary>
    
    public partial class editing_user : Page
    {
        MainWindow mainWindow;
        public int sel_id;
        public int flag_edit_login = 0;
        public string flag_text_login;
        public int flag_yes_no = 0;
        public editing_user(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            data_grid_update();
        }

        public void data_grid_update() //обновляет информацию о пользователях
        {
            DataTable data_gr_sql = mainWindow.Select("SELECT (id_user) AS id__user, login, password, (lvl_access) AS lvl__access FROM users");
            data_grid.ItemsSource = data_gr_sql.DefaultView;
            DataTable user_id = mainWindow.Select("SELECT id_user FROM users");
            combo_lvl_user.Items.Clear();
            combo_lvl_user.Items.Add("0 - Админ");
            combo_lvl_user.Items.Add("1 - Преподаватель");
            combo_lvl_user.Items.Add("2 - ПЦК");
            new_last_name.Text = "";
            new_name.Text = "";
            new_second_name.Text = "";
            new_login_text.Text = "";
            new_password_text.Text = "";
            new_num_pck.Text = "";
            new_title_pck.Text = "";
        }

        private void save_editing_user_Click(object sender, RoutedEventArgs e) //сохраняет изменения
        {
            if(Convert.ToInt32(combo_lvl_user.SelectedIndex) == 1)
            {
                MessageBox.Show("Изменения сохранены.");
                def_save();
                if (flag_yes_no == 1)
                {
                    save_for_teacher();
                }
            } if (Convert.ToInt32(combo_lvl_user.SelectedIndex) == 2)
            {
                MessageBox.Show("Изменения сохранены.");
                def_save();
                if (flag_yes_no == 1)
                {
                    save_for_pck();
                }
            }
            else
            {
                MessageBox.Show("Изменения сохранены.");
                def_save();
            }
            data_grid_update();
        }

        private void proccess_Click(object sender, RoutedEventArgs e) //выводит информацию о выбранном пользователе для редактирования
        {
            DataTable selected_row_id = mainWindow.Select("SELECT id_user FROM users");
            int in_sel_row = Convert.ToInt32(data_grid.SelectedIndex);
            int sel_id = Convert.ToInt32(selected_row_id.Rows[in_sel_row][0]);
            int combo_sel = def_view();
            if (combo_sel == 1)
            {
                MessageBoxResult messageBoxResult1 = System.Windows.MessageBox.Show("Вы хотите изменить другие данные пользователя, помимо логина и пароля?", "Нет", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult1 == MessageBoxResult.Yes)
                {
                    flag_yes_no = 1;
                    DataTable sel_teacher = mainWindow.Select($"SELECT last_name, [name], second_name FROM teacher WHERE id_user = {sel_id}");
                    new_last_name.Text = "";
                    new_name.Text = "";
                    new_second_name.Text = "";
                    new_last_name.IsEnabled = true;
                    new_name.IsEnabled = true;
                    new_second_name.IsEnabled = true;
                    new_last_name.Text = Convert.ToString(sel_teacher.Rows[0][0]);
                    new_name.Text = Convert.ToString(sel_teacher.Rows[0][1]);
                    new_second_name.Text = Convert.ToString(sel_teacher.Rows[0][2]);
                }
                else flag_yes_no = 0;
            }
            else if (combo_sel == 2)
            {
                MessageBoxResult messageBoxResult1 = System.Windows.MessageBox.Show("Вы хотите изменить другие данные пользователя, помимо логина и пароля?", "Нет", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult1 == MessageBoxResult.Yes)
                {
                    flag_yes_no = 1;
                    DataTable sel_pck = mainWindow.Select($"SELECT last_name, name, second_name, num_ck, title_ck FROM chairmenCK WHERE id_user = {sel_id}");
                    new_last_name.Text = "";
                    new_name.Text = "";
                    new_second_name.Text = "";
                    new_last_name.IsEnabled = true;
                    new_name.IsEnabled = true;
                    new_second_name.IsEnabled = true;
                    new_num_pck.IsEnabled = true;
                    new_title_pck.IsEnabled = true;
                    new_last_name.Text = Convert.ToString(sel_pck.Rows[0][0]);
                    new_name.Text = Convert.ToString(sel_pck.Rows[0][1]);
                    new_second_name.Text = Convert.ToString(sel_pck.Rows[0][2]);
                    new_num_pck.Text = Convert.ToString(sel_pck.Rows[0][3]);
                    new_title_pck.Text = Convert.ToString(sel_pck.Rows[0][4]);
                }
                else flag_yes_no = 0;
            }
        }

        private void data_grid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (data_grid.SelectedItem != null)
            {
                proccess.IsEnabled = true;
            }
        }

        public int def_view()
        {
            DataTable selected_row_id = mainWindow.Select("SELECT id_user FROM users");
            int in_sel_row = Convert.ToInt32(data_grid.SelectedIndex);
            int sel_id = Convert.ToInt32(selected_row_id.Rows[in_sel_row][0]);
            DataTable info_user = mainWindow.Select($"SELECT login, password, lvl_access FROM users WHERE id_user = {sel_id}");
            new_login_text.Text = Convert.ToString(info_user.Rows[0][0]);
            flag_text_login = Convert.ToString(info_user.Rows[0][0]);
            new_password_text.Text = Convert.ToString(info_user.Rows[0][1]);
            combo_lvl_user.SelectedIndex = Convert.ToInt32(info_user.Rows[0][2]);
            int combo_sel = Convert.ToInt32(info_user.Rows[0][2]);
            save_editing_user.IsEnabled = true;
            proccess.IsEnabled = false;
            return combo_sel;
        }

        public void def_save() //сохраняет только логин, пароль и уровень пользователя
        {
            int flag_check = check_user();
            if (flag_check == 0)
            {
                DataTable selected_row_id = mainWindow.Select("SELECT id_user FROM users");
                int in_sel_row = Convert.ToInt32(data_grid.SelectedIndex);
                sel_id = Convert.ToInt32(selected_row_id.Rows[in_sel_row][0]);
                string text_command = "EXECUTE save_editing_user @new_login, @new_password, @new_lvl_user, @id_user";
                SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                command.Parameters.Add("@new_login", SqlDbType.NVarChar).Value = Convert.ToString(new_login_text.Text);
                command.Parameters.Add("@new_password", SqlDbType.NVarChar).Value = Convert.ToString(new_password_text.Text);
                command.Parameters.Add("@new_lvl_user", SqlDbType.Int).Value = Convert.ToInt32(combo_lvl_user.SelectedIndex);
                command.Parameters.Add("@id_user", SqlDbType.Int).Value = sel_id;
                DataTable save_editing = mainWindow.CommandDB(command);
                save_editing_user.IsEnabled = false;
                new_last_name.IsEnabled = false;
                new_name.IsEnabled = false;
                new_second_name.IsEnabled = false;
                new_num_pck.IsEnabled = false;
                new_title_pck.IsEnabled = false;
            }
        }

        public void save_for_teacher() //сохраняет информацию для учителя
        {
            int flag_check = check_user();
            if(flag_yes_no == 1)
            {
                flag_check += +check_for_teacher();
            }
            if (flag_check == 0)
            {
                string text_command = "EXECUTE save_editing_teacher @new_id_user, @new_last_name, @new_name, @new_second_name";
                SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                command.Parameters.Add("@new_id_user", SqlDbType.Int).Value = sel_id;
                command.Parameters.Add("@new_last_name", SqlDbType.NVarChar).Value = new_last_name.Text;
                command.Parameters.Add("@new_name", SqlDbType.NVarChar).Value = new_name.Text;
                command.Parameters.Add("@new_second_name", SqlDbType.NVarChar).Value = new_second_name.Text;
                DataTable save_teacher = mainWindow.CommandDB(command);
            }
        }

        public void save_for_pck() //сохраняет информацию для пцк
        {
            int flag_check = check_user();
            if (flag_yes_no == 1)
            {
                flag_check += +check_for_pck();
            }
            if (flag_check == 0)
            {
                string text_command = "EXECUTE save_editing_pck @new_id_user, @new_last_name, @new_name, @new_second_name, @new_num_pck, @new_title_pck";
                SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                command.Parameters.Add("@new_id_user", SqlDbType.Int).Value = sel_id;
                command.Parameters.Add("@new_last_name", SqlDbType.NVarChar).Value = new_last_name.Text;
                command.Parameters.Add("@new_name", SqlDbType.NVarChar).Value = new_name.Text;
                command.Parameters.Add("@new_second_name", SqlDbType.NVarChar).Value = new_second_name.Text;
                command.Parameters.Add("@new_num_pck", SqlDbType.Int).Value = Convert.ToInt32(new_num_pck.Text);
                command.Parameters.Add("@new_title_pck", SqlDbType.NVarChar).Value = new_title_pck.Text;
                DataTable save_pck = mainWindow.CommandDB(command);
            }
        }

        public int check_for_teacher() //проверяет ввод данных для учителя
        {
            int flag_check = 0;
            if (flag_yes_no == 1)
            {
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
            }
            return flag_check;
        }

        public int check_user() //проверяет ввод 
        {
            int flag_check = 0;
            if (new_login_text.Text == "")
            {
                MessageBox.Show("Вы не ввели логин");
                flag_check = 1;
            }
            else if (new_password_text.Text == "")
            {
                MessageBox.Show("Вы не ввесли пароль");
                flag_check = 1;
            }
            else if (flag_edit_login == 2)
            {
                flag_check = check_login();
            }
                return flag_check;
        }

        public int check_login() //проверяет данные логина
        {
            int flag_check = 0;
            if (flag_edit_login > 1)
            {
                string text_command = "SELECT * FROM users WHERE login = @login";
                SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                command.Parameters.Add("@login", SqlDbType.NVarChar).Value = new_login_text.Text;
                DataTable check_login_p = mainWindow.CommandDB(command);
                if (check_login_p.Rows.Count > 0)
                {
                    MessageBox.Show("Введенный вами логин уже существует");
                    flag_check = 1;
                }
            }
            return flag_check;
        }

        public int check_for_pck() //проверяет ввод данных для пцк
        {
            int flag_check = 0;
            if (flag_yes_no == 1)
            {
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
                if (new_num_pck.Text == "")
                {
                    MessageBox.Show("Вы не ввели номер ЦК");
                    flag_check = 1;
                }
                if (new_title_pck.Text == "")
                {
                    MessageBox.Show("Вы не ввесли название ЦК");
                    flag_check = 1;
                }
            }
            return flag_check;
        }

        private void new_login_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            flag_edit_login++;
            if(flag_edit_login > 1)
            {
                if(flag_text_login == new_login_text.Text)
                {
                    flag_edit_login = 1;
                }
            }
        }
    }
}
