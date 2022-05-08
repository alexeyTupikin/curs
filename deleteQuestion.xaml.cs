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
    /// Логика взаимодействия для deleteQuestion.xaml
    /// </summary>
    public partial class deleteQuestion : Page
    {
        MainWindow mainWindow;
        bool delete_true = false;
        List<int> mas_id_questions = new List<int>();
        public deleteQuestion(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            mas_id_questions.Clear();
            DataTable disciplines_select = mainWindow.Select("SELECT [title_discipline] FROM [dbo].[disciplines]");
            if (disciplines_select.Rows.Count > 0)
            {
                for (int i = 0; i < disciplines_select.Rows.Count; i++)
                {
                    chouseDiscipline.Items.Add(disciplines_select.Rows[i][0].ToString());
                }
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        { 
            int id_question = Convert.ToInt32(chouseQty.SelectedValue);
            string text_command = "EXECUTE delete_question @id_question_p";
            SqlCommand command = SqlServer.CreateSqlCommand(text_command);
            command.Parameters.Add("@id_question_p", SqlDbType.Int).Value = id_question;
            DataTable delete_question = mainWindow.CommandDB(command);
            search.IsEnabled = false;
            delete.IsEnabled = false;
            chouseQty.IsEnabled = false;
            tableQuestion.Text = "";
            MessageBox.Show("Вопрос удален.");
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            string chouseDisc = "";
            chouseDisc = Convert.ToString(chouseDiscipline.SelectedValue);
            DataTable id_discipline = mainWindow.Select($"SELECT id_discipline FROM dbo.disciplines WHERE title_discipline = '{chouseDisc}'");
            int id_disc = Convert.ToInt32(id_discipline.Rows[0][0]);
            DataTable id_question = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc}");
            if (id_question.Rows.Count > 0)
            {
                int i = 0;
                while (i<id_question.Rows.Count)
                {
                    mas_id_questions.Add(Convert.ToInt32(id_question.Rows[i][0]));
                    i++;
                }
            }
            DataTable min_id_s = mainWindow.Select($"SELECT MIN(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc}");
            DataTable max_id_s = mainWindow.Select($"SELECT MAX(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc}");
            int min_id_q = Convert.ToInt32(min_id_s.Rows[0][0]);
            int max_id_q = Convert.ToInt32(max_id_s.Rows[0][0]);
            DataTable id_question_select = mainWindow.Select("SELECT [title_discipline] FROM [dbo].[disciplines]");
            if (id_question_select.Rows.Count > 0)
            {
                chouseQty.Items.Clear();
                int count_id_chouse = 0;
                for (int i = min_id_q; i < (id_question.Rows.Count + min_id_q); i++)
                {
                    chouseQty.Items.Add($"{mas_id_questions[count_id_chouse]}");
                    count_id_chouse++;
                }
            }
            DataTable text_block_question = mainWindow.Select($"" +
            $"SELECT dbo.questions.question FROM dbo.questions WHERE id_discipline = {id_disc}");
            tableQuestion.Text = "";
            int countStr = 0;
            for (int i = min_id_q; i<(id_question.Rows.Count+min_id_q); i++ )
            {
                tableQuestion.AppendText($"{mas_id_questions[countStr]}. {Convert.ToString(text_block_question.Rows[countStr][0])}");
                tableQuestion.AppendText($"\n");
                countStr++;
            }
            countStr = 0;
            chouseQty.IsEnabled = true;

        }

        private void chouseDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            search.IsEnabled = true;
        }

        private void chouseQty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delete.IsEnabled = true;
        }
    }
}
