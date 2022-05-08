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
    /// Логика взаимодействия для editing.xaml
    /// </summary>
    public partial class editing : Page
    {
        MainWindow mainWindow;
        public editing(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            DataTable disciplines_select = mainWindow.Select("SELECT [title_discipline] FROM [dbo].[disciplines]");
            if (disciplines_select.Rows.Count > 0)
            {
                for (int i = 0; i < disciplines_select.Rows.Count; i++)
                {
                    chouseDiscipline.Items.Add(disciplines_select.Rows[i][0].ToString());
                }
            }
            DataTable type_select = mainWindow.Select("SELECT [title_type] FROM [dbo].[types]");
            if (type_select.Rows.Count > 0)
            {
                for (int i = 0; i < type_select.Rows.Count; i++)
                {
                    chouseType.Items.Add(type_select.Rows[i][0].ToString());
                }
            }

        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            chouse_id.Items.Clear();
            text_box.Text = "";
            string chouse_disc = Convert.ToString(chouseDiscipline.SelectedValue);
            DataTable discipline = mainWindow.Select($"SELECT id_discipline FROM disciplines WHERE title_discipline = '{chouse_disc}'");
            int id_disc = Convert.ToInt32(discipline.Rows[0][0]);
            string chouse_type = Convert.ToString(chouseType.SelectedValue);
            DataTable type = mainWindow.Select($"SELECT id_type FROM types WHERE title_type = '{chouse_type}'");
            int id_type = Convert.ToInt32(type.Rows[0][0]);
            //
            DataTable id_question = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc} AND id_type = {id_type}");
            if (id_question.Rows.Count > 0)
            {
                int i = 0;
                while (i < id_question.Rows.Count)
                {
                    chouse_id.Items.Add(Convert.ToInt32(id_question.Rows[i][0]));
                    i++;
                }
            }
            //
            DataTable text_question = mainWindow.Select($"SELECT question FROM questions WHERE id_discipline = {id_disc} AND id_type = {id_type}");
            if (text_question.Rows.Count > 0)
            {
                int i = 0;
                while (i < text_question.Rows.Count)
                {
                    text_box.AppendText($"{Convert.ToInt32(id_question.Rows[i][0])}. {Convert.ToString(text_question.Rows[i][0])}");
                    text_box.AppendText($"\n");
                    i++;
                }
            }
            chouse_id.IsEnabled = true;
        }

        private void go_editing_Click(object sender, RoutedEventArgs e)
        {
            text_box.Text = "";
            DataTable edit_question = mainWindow.Select($"SELECT question FROM questions WHERE id_question = {Convert.ToInt32(chouse_id.SelectedValue)}");
            text_box.Text = "";
            text_box.AppendText($"{Convert.ToString(edit_question.Rows[0][0])}");
            save_question.IsEnabled = true;
        }

        private void save_question_Click(object sender, RoutedEventArgs e)
        {
            string update_question = $"{text_box.Text}";
            int id_question_p = Convert.ToInt32(chouse_id.SelectedValue);
            string text_command = "EXECUTE save_editing_question @new_text_question, @id_question";
            SqlCommand command = SqlServer.CreateSqlCommand(text_command);
            command.Parameters.Add("@new_text_question", SqlDbType.NVarChar).Value = update_question;
            command.Parameters.Add("@id_question", SqlDbType.Int).Value = id_question_p;
            DataTable update_q = mainWindow.CommandDB(command);
            chouse_id.IsEnabled = false;
            chouseType.IsEnabled = false;
            search.IsEnabled = false;
            go_editing.IsEnabled = false;
            save_question.IsEnabled = false;
            chouseType.SelectedValue = null;
            chouse_id.SelectedValue = null;
            chouseDiscipline.SelectedValue = null;
            text_box.Text = "";
            MessageBox.Show("Вопрос успешно отредактирован.");
        }

        private void chouseDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(chouseDiscipline.SelectedValue != null)
            {
                chouseType.IsEnabled = true;
            }
        }

        private void chouseType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(chouseType.SelectedValue != null)
            {
                search.IsEnabled = true;
            }
        }

        private void chouse_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(chouse_id.SelectedValue != null)
            {
                go_editing.IsEnabled = true;
            }
        }
    }
}
