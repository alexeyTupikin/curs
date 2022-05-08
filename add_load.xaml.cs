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
    /// Логика взаимодействия для add_load.xaml
    /// </summary>
    public partial class add_load : Page
    {
        MainWindow mainWindow;
        public int old_id_user;
        public int old_id_disc;
        public int flag_disc = 1;
        public int flag_teacher = 1;
        public add_load(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            dg_load();
        }

        public void dg_load()
        {
            id_disc_combo.Items.Clear();
            id_user_combo.Items.Clear();
            DataTable dg_update = mainWindow.Select("SELECT dbo.disciplines.title_discipline AS id__discipline, " +
                "dbo.teacher.last_name AS last__name FROM dbo.[load] INNER JOIN dbo.teacher ON " +
                "dbo.[load].id_user = dbo.teacher.id_user INNER JOIN dbo.disciplines " +
                "ON dbo.[load].id_discipline = dbo.disciplines.id_discipline");
            DataTable disc = mainWindow.Select("SELECT title_discipline FROM disciplines");
            for (int i =0; i< disc.Rows.Count; i++)
            {
                id_disc_combo.Items.Add(Convert.ToString(disc.Rows[i][0]));
            }
            DataTable teacher = mainWindow.Select("SELECT last_name FROM teacher");
            for(int j = 0; j<teacher.Rows.Count; j++)
            {
                id_user_combo.Items.Add(Convert.ToString(teacher.Rows[j][0]));
            }
            data_grid_load.ItemsSource = dg_update.DefaultView;
            chouse_teacher.Items.Clear();
            chouse_disc.Items.Clear();
            DataTable teachers = mainWindow.Select("SELECT(last_name + ' ' + SUBSTRING(name, 1, 1) + '. ' + SUBSTRING(second_name, 1, 1) + '.') AS fio_teacher FROM teacher");
            for (int i = 0; i < teachers.Rows.Count; i++)
            {
                chouse_teacher.Items.Add(Convert.ToString(teachers.Rows[i][0]));
            }
            DataTable disciplines = mainWindow.Select("SELECT title_discipline FROM disciplines");
            for (int d = 0; d < disciplines.Rows.Count; d++)
            {
                chouse_disc.Items.Add(Convert.ToString(disciplines.Rows[d][0]));
            }
            id_disc_combo.IsEnabled = false;
            id_user_combo.IsEnabled = false;
            delete_load.IsEnabled = false;
            add_new_load.IsEnabled = false;
        }

        private void add_new_load_Click(object sender, RoutedEventArgs e)
        {
            string text_command = "EXECUTE sel_id_teacher @last_name";
            SqlCommand command = SqlServer.CreateSqlCommand(text_command);
            command.Parameters.Add("@last_name", SqlDbType.NVarChar).Value = Convert.ToString(chouse_teacher.SelectedValue);
            DataTable id_teacher = mainWindow.CommandDB(command);
            int id_user = Convert.ToInt32(id_teacher.Rows[0][0]);
            DataTable id_disc = mainWindow.Select($"SELECT id_discipline FROM disciplines " +
                $"WHERE title_discipline = '{Convert.ToString(chouse_disc.SelectedValue)}'");
            int id_discipline = Convert.ToInt32(id_disc.Rows[0][0]);
            if (check_load(id_user, id_discipline) == 0)
            {
                string text_load = "insert into [load] (id_user, id_discipline)" +
                                   "values" +
                                   "(@id_user, @id_disc)";
                SqlCommand command_load = SqlServer.CreateSqlCommand(text_load);
                command_load.Parameters.Add("@id_user", SqlDbType.Int).Value = id_user;
                command_load.Parameters.Add("@id_disc", SqlDbType.Int).Value = id_discipline;
                DataTable add_load_p = mainWindow.CommandDB(command_load);
                dg_load();
                MessageBox.Show("Нагрузка успешно добавлена.");
            }
            else MessageBox.Show("Проверьте правильность введенных вами данных. Такая нагрузка уже существует.");
        }

        private void delete_load_Click(object sender, RoutedEventArgs e)
        {
            DataTable selected_row_id = mainWindow.Select("SELECT id_user FROM [load]");
            int in_sel_row = Convert.ToInt32(data_grid_load.SelectedIndex);
            int sel_id_user = Convert.ToInt32(selected_row_id.Rows[in_sel_row][0]);
            DataTable selected_row_id_disc = mainWindow.Select("SELECT id_discipline FROM [load]");
            int sel_id_disc = Convert.ToInt32(selected_row_id_disc.Rows[in_sel_row][0]);
            string text_command = "EXECUTE delete_load @id_user, @id_disc";
            SqlCommand command = SqlServer.CreateSqlCommand(text_command);
            command.Parameters.Add("@id_user", SqlDbType.Int).Value = sel_id_user;
            command.Parameters.Add("@id_disc", SqlDbType.Int).Value = sel_id_disc;
            DataTable delete_load_p = mainWindow.CommandDB(command);
            dg_load();
            MessageBox.Show("Нагузка успешно удалена.");
        }

        private void editing_load_Click(object sender, RoutedEventArgs e)
        {

            if ((id_user_combo.SelectedValue != null) && (id_disc_combo.SelectedValue != null))
            {
                //
                DataTable teacher_sel = mainWindow.Select($"SELECT id_user FROM teacher WHERE last_name = '{Convert.ToString(id_user_combo.SelectedValue)}'");
                int new_id_user_p = Convert.ToInt32(teacher_sel.Rows[0][0]);
                //
                DataTable disc_sel = mainWindow.Select($"SELECT id_discipline FROM disciplines WHERE title_discipline = '{Convert.ToString(id_disc_combo.SelectedValue)}'");
                int new_id_disc_p = Convert.ToInt32(disc_sel.Rows[0][0]);
                //
                if (check_load(new_id_user_p, new_id_disc_p) == 0)
                {
                    string text_command = "EXECUTE save_editing_load @new_id_user, @new_id_disc, @old_id_user, @old_id_disc";
                    SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                    command.Parameters.Add("@new_id_user", SqlDbType.Int).Value = new_id_user_p;
                    command.Parameters.Add("@new_id_disc", SqlDbType.Int).Value = new_id_disc_p;
                    command.Parameters.Add("@old_id_user", SqlDbType.Int).Value = old_id_user;
                    command.Parameters.Add("@old_id_disc", SqlDbType.Int).Value = old_id_disc;
                    DataTable delete_load = mainWindow.CommandDB(command);
                    editing_load.IsEnabled = false;
                    id_user_combo.SelectedValue = null;
                    id_disc_combo.SelectedValue = null;
                    dg_load();
                    MessageBox.Show("Нагрузка отредактирована");
                }
                else MessageBox.Show("Проверьте правильность введенных вами данных. Такая нагрузка уже существует.");
            }
            else if (id_user_combo.SelectedValue == null)
            {
                MessageBox.Show("Вы не ввели id пользователя");
            }
            else if (id_disc_combo.SelectedValue == null)
            {
                MessageBox.Show("Вы не выбрали дисциплину");
            }
        }

        private void proc_Click(object sender, RoutedEventArgs e)
        {
            DataTable sel_load = mainWindow.Select("SELECT        dbo.[load].id_user, dbo.[load].id_discipline, " +
                "dbo.disciplines.title_discipline, dbo.teacher.last_name " +
                "FROM dbo.[load] INNER JOIN dbo.disciplines ON dbo.[load].id_discipline = dbo.disciplines.id_discipline " +
                "INNER JOIN dbo.teacher ON dbo.[load].id_user = dbo.teacher.id_user");
            int ind_sel_row = Convert.ToInt32(data_grid_load.SelectedIndex);
            int id_user_p = Convert.ToInt32(sel_load.Rows[ind_sel_row][0]);
            int id_disc_p = Convert.ToInt32(sel_load.Rows[ind_sel_row][1]);
            id_user_combo.SelectedValue = null;
            id_disc_combo.SelectedValue = null;
            //
            id_user_combo.SelectedValue = Convert.ToString(sel_load.Rows[ind_sel_row][3]);
            //
            id_disc_combo.SelectedValue = Convert.ToString(sel_load.Rows[ind_sel_row][2]);
            old_id_user = Convert.ToInt32(sel_load.Rows[ind_sel_row][0]);
            old_id_disc = Convert.ToInt32(sel_load.Rows[ind_sel_row][1]);
            editing_load.IsEnabled = true;
            id_user_combo.IsEnabled = true;
            id_disc_combo.IsEnabled = true;
        }

        private void data_grid_load_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delete_load.IsEnabled = true;
            proc.IsEnabled = true;
        }

        private void chouse_disc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            flag_disc = 0;
            if ((flag_disc + flag_teacher) == 0)
            {
                add_new_load.IsEnabled = true;
            }
        }

        private void chouse_teacher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            flag_teacher = 0;
            if ((flag_disc + flag_teacher) == 0)
            {
                add_new_load.IsEnabled = true;
            }
        }

        public int check_load(int id_user, int id_disc)
        {
            int flag_check = 0;
            string text_command = "SELECT COUNT(*) FROM load WHERE id_user = @id_user AND id_discipline = @id_disc";
            SqlCommand command = SqlServer.CreateSqlCommand(text_command);
            command.Parameters.Add("@id_user", SqlDbType.Int).Value = id_user;
            command.Parameters.Add("@id_disc", SqlDbType.Int).Value = id_disc;
            DataTable check = mainWindow.CommandDB(command);
            if (Convert.ToInt32(check.Rows[0][0]) != 0)
            {
                flag_check = 1;
            }
            return flag_check;
        }

    }
}
