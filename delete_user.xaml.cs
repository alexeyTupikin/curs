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
    /// Логика взаимодействия для delete_user.xaml
    /// </summary>
    public partial class delete_user : Page
    {
        MainWindow mainWindow;
        
        public class users
        {
            public int iduser { get; set; }
            public string login { get; set; }
            public string password { get; set; }
            public int lvlaccess { get; set; }
        }

        List<users> users_list = new List<users>();
        public delete_user(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            list_users();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            DataTable selected_row_id = mainWindow.Select("SELECT id_user FROM users");
            int in_sel_row = Convert.ToInt32(data_grid_users.SelectedIndex);
            int sel_id = Convert.ToInt32(selected_row_id.Rows[in_sel_row][0]);
            DataTable sel_lvl_user = mainWindow.Select($"SELECT lvl_access FROM users WHERE id_user = {sel_id}");
            if (Convert.ToInt32(sel_lvl_user.Rows[0][0]) == 1) 
            {
                string text_del_teacher = "EXECUTE delete_teacher @id_user";
                SqlCommand del_teacher = SqlServer.CreateSqlCommand(text_del_teacher);
                del_teacher.Parameters.Add("@id_user", SqlDbType.Int).Value = sel_id;
                DataTable delete_teacher_p = mainWindow.CommandDB(del_teacher);
            } else if(Convert.ToInt32(sel_lvl_user.Rows[0][0]) == 2)
            {
                string text_del_pck = "EXECUTE delete_pck @id_user";
                SqlCommand del_pck = SqlServer.CreateSqlCommand(text_del_pck);
                del_pck.Parameters.Add("@id_user", SqlDbType.Int).Value = sel_id;
                DataTable delete_teacher_p = mainWindow.CommandDB(del_pck);
            }
            string text_command = "EXECUTE delete_user @id_user";
            SqlCommand command = SqlServer.CreateSqlCommand(text_command);
            command.Parameters.Add("@id_user", SqlDbType.Int).Value = sel_id;
            DataTable delete_user = mainWindow.CommandDB(command);
            list_users();
            delete.IsEnabled = false;
            MessageBox.Show("Польователь удален.");
        }

        public void list_users()
        {
            DataTable data_gr_sql = mainWindow.Select("SELECT (id_user) AS id__user, login, password, (lvl_access) AS lvl__access FROM users");
            data_grid_users.ItemsSource = data_gr_sql.DefaultView;
            DataTable user_id = mainWindow.Select("SELECT id_user FROM users");
        }

        private void data_grid_users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(data_grid_users.SelectedItem != null)
            {
                delete.IsEnabled = true;
            }
        }
    }
}
