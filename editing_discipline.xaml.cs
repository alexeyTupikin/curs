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
    /// Логика взаимодействия для editing_discipline.xaml
    /// </summary>
    public partial class editing_discipline : Page
    {
        MainWindow mainWindow;
        public editing_discipline(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            dg_update();
        }

        public void dg_update()
        {
            DataTable data_grid = mainWindow.Select("SELECT (title_discipline) AS title__discipline, (remove_or_not) AS remove__or__not FROM disciplines");
            data_grid_disc.ItemsSource = data_grid.DefaultView;
            proc_button.IsEnabled = false;
            save_button.IsEnabled = false;
            add_button.IsEnabled = true;
            delete_disc.IsEnabled = false;
            re_delete_disc.IsEnabled = false;
            new_title_disc.IsEnabled = false;
            edit_title_disc.IsEnabled = false;
        }

        private void delete_disc_Click(object sender, RoutedEventArgs e)
        {
            int sel_row = data_grid_disc.SelectedIndex;
            DataTable sel_id = mainWindow.Select("SELECT id_discipline FROM disciplines");
            int sel_id_disc = Convert.ToInt32(sel_id.Rows[Convert.ToInt32(sel_row)][0]);
            string text_commmand = "EXECUTE delete_disc @id_disc";
            SqlCommand command = SqlServer.CreateSqlCommand(text_commmand);
            command.Parameters.Add("@id_disc", SqlDbType.Int).Value = sel_id_disc;
            DataTable remove_disc = mainWindow.CommandDB(command);
            dg_update();
            MessageBox.Show("Дисциплина удалена.");
        }

        private void re_delete_disc_Click(object sender, RoutedEventArgs e)
        {
            int sel_row = data_grid_disc.SelectedIndex;
            DataTable sel_id = mainWindow.Select("SELECT id_discipline FROM disciplines");
            int sel_id_disc = Convert.ToInt32(sel_id.Rows[Convert.ToInt32(sel_row)][0]);
            string text_commmand = "EXECUTE re_del_disc @id_disc";
            SqlCommand command = SqlServer.CreateSqlCommand(text_commmand);
            command.Parameters.Add("@id_disc", SqlDbType.Int).Value = sel_id_disc;
            DataTable re_del_disc = mainWindow.CommandDB(command);
            dg_update();
            MessageBox.Show("Дисциплины восстановлена.");
        }

        private void proc_button_Click(object sender, RoutedEventArgs e)
        {
            new_title_disc.Text = "";
            int in_sel_row = data_grid_disc.SelectedIndex;
            DataTable sel_disc = mainWindow.Select($"SELECT id_discipline FROM disciplines");
            int id_disc = Convert.ToInt32(sel_disc.Rows[in_sel_row][0]);
            DataTable sel_title_disc = mainWindow.Select("SELECT title_discipline FROM disciplines " +
                $"WHERE id_discipline = {id_disc}");
            edit_title_disc.Text = Convert.ToString(sel_title_disc.Rows[0][0]);
            save_button.IsEnabled = true;
            edit_title_disc.IsEnabled = true;
            add_button.IsEnabled = false;
        }

        private void add_button_Click(object sender, RoutedEventArgs e)
        {
            new_title_disc.IsEnabled = true;
            edit_title_disc.Text = "";
            save_button.IsEnabled = true;            
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            if (edit_title_disc.Text != "")
            {
                int in_sel_row = data_grid_disc.SelectedIndex;
                DataTable sel_disc = mainWindow.Select($"SELECT id_discipline FROM disciplines");
                int id_disc = Convert.ToInt32(sel_disc.Rows[in_sel_row][0]);
                string text_command = "EXECUTE editing_discipline @id_disc, @title_discipline";
                SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                command.Parameters.Add("@id_disc", SqlDbType.Int).Value = id_disc;
                command.Parameters.Add("@title_discipline", SqlDbType.NVarChar).Value = edit_title_disc.Text;
                DataTable save_edit_disc = mainWindow.CommandDB(command);
                MessageBox.Show("Дисциплина успешно изменена.");
            } else if (new_title_disc.Text != "")
            {
                string text_command_add = "EXECUTE add_discipline @title_discipline";
                SqlCommand command_add = SqlServer.CreateSqlCommand(text_command_add);
                command_add.Parameters.Add("@title_discipline", SqlDbType.NVarChar).Value = new_title_disc.Text;
                DataTable add_disc = mainWindow.CommandDB(command_add);
                MessageBox.Show("Добавлена новая дисциплина.");
                //save_button.IsEnabled = true;
            }
            new_title_disc.Text = "";
            edit_title_disc.Text = "";
            dg_update();
        }

        private void data_grid_disc_Selected(object sender, RoutedEventArgs e)
        {
            proc_button.IsEnabled = true;
            add_button.IsEnabled = true;
            delete_disc.IsEnabled = true;
            re_delete_disc.IsEnabled = true;
        }
    }
}
