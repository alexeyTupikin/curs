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
using System.Text.RegularExpressions;

namespace curs
{
    /// <summary>
    /// Логика взаимодействия для add_question.xaml
    /// </summary>
    public partial class add_question : Page
    {
        MainWindow mainWindow;
        public int flag_disc = 1;
        public int flag_type = 1;
        public int flag_lvl = 1;
        public int flag_text = 1;
        public add_question(MainWindow _mainWindow)
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
            DataTable types_select = mainWindow.Select("SELECT dbo.[types].title_type FROM dbo.[types]");
            if (types_select.Rows.Count > 0)
            {
                for (int i = 0; i < types_select.Rows.Count; i++)
                {
                    chouseType.Items.Add(types_select.Rows[i][0].ToString());
                }
            }
            DataTable complexity_select = mainWindow.Select("SELECT dbo.complexity.lvl_difficulty FROM dbo.complexity");
            if (complexity_select.Rows.Count > 0)
            {
                for (int i = 0; i < complexity_select.Rows.Count; i++)
                {
                    chouseComplexity.Items.Add(complexity_select.Rows[i][0].ToString());
                }
            }
        }
        private void add_Click(object sender, RoutedEventArgs e) //добавляет вопрос в базу данных
        {
            string discipline = "";
            string type = "";
            string complexity = "";
            string text_question = "";
            discipline = Convert.ToString(chouseDiscipline.SelectedValue);
            if ((flag_lvl + flag_type + flag_disc + flag_text) == 0)
            {
                type = Convert.ToString(chouseType.SelectedValue);
                complexity = Convert.ToString(chouseComplexity.SelectedValue);
                text_question = Convert.ToString(textQuestion.Text);
                DataTable check_q = mainWindow.Select($"SELECT COUNT(id_question) FROM questions WHERE question = '{text_question}'");
                if (Convert.ToInt32(check_q.Rows[0][0]) == 0)
                {
                    DataTable id_discipline = mainWindow.Select($"" +
                    $"SELECT id_discipline FROM disciplines WHERE " +
                    $"title_discipline = '{discipline}'");
                    int id_disc = Convert.ToInt32(id_discipline.Rows[0][0]);
                    DataTable id_type = mainWindow.Select($"" +
                    $"SELECT id_type FROM[types] WHERE title_type = '{type}'");
                    DataTable remove_or_not = mainWindow.Select($"SELECT remove_or_not FROM disciplines WHERE id_discipline = {id_disc}");
                    if (Convert.ToInt32(remove_or_not.Rows[0][0]) == 0)
                    {
                        int id_type_p = Convert.ToInt32(id_type.Rows[0][0]);
                        DataTable id_complexity = mainWindow.Select($"" +
                        $"SELECT id_complexity FROM complexity WHERE " +
                        $"lvl_difficulty = '{complexity}'");
                        int id_complexity_p = Convert.ToInt32(id_complexity.Rows[0][0]);
                        string text_command = $@"EXECUTE add_question @id_disc, @id_type_p, @id_complexity_p, @question";
                        SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                        command.Parameters.Add("@id_disc", SqlDbType.Int).Value = id_disc;
                        command.Parameters.Add("@id_type_p", SqlDbType.Int).Value = id_type_p;
                        command.Parameters.Add("@id_complexity_p", SqlDbType.Int).Value = id_complexity_p;
                        command.Parameters.Add("@question", SqlDbType.NVarChar).Value = text_question;
                        DataTable add_question = mainWindow.CommandDB(command);
                        chouseComplexity.IsEnabled = false;
                        chouseType.IsEnabled = false;
                        chouseDiscipline.SelectedValue = null;
                        chouseComplexity.SelectedValue = null;
                        chouseType.SelectedValue = null;
                        textQuestion.Text = "";
                        add_button.IsEnabled = false;
                        flag_disc = 1;
                        flag_type = 1;
                        flag_lvl = 1;
                        flag_text = 1;
                        MessageBox.Show("Вопрос успешно добавлен.");
                    }
                    else
                    {
                        add_button.IsEnabled = false;
                        chouseComplexity.IsEnabled = false;
                        chouseType.IsEnabled = false;
                        chouseDiscipline.SelectedValue = null;
                        chouseComplexity.SelectedValue = null;
                        chouseType.SelectedValue = null;
                        textQuestion.Text = "";
                        flag_disc = 1;
                        flag_type = 1;
                        flag_lvl = 1;
                        flag_text = 1;
                        MessageBox.Show("На данный момент выбранная вами дисциплина предназначена к удалеию и действия с ней недоступны.");
                    }
                }
                else 
                {
                    add_button.IsEnabled = false;
                    flag_text = 1;
                    MessageBox.Show("Вопрос с таким содержанием же существует в базе данных."); 
                }
            }
        }

        private void chouseDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(chouseDiscipline.SelectedValue != null)
            {
                flag_disc = 0;
                chouseType.IsEnabled = true;
            }
            else flag_disc = 1;
        }

        private void chouseType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(chouseType.SelectedValue != null)
            {
                flag_type = 0;
                chouseComplexity.IsEnabled = true;
            }
            else flag_type = 1;
        }

        private void chouseComplexity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chouseComplexity.SelectedValue != null)
            {
                flag_lvl = 0;
                textQuestion.IsEnabled = true;
            }
            else flag_lvl = 1;
        }

        private void textQuestion_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((textQuestion.Text != "") && (textQuestion.Text.Length > 0))
            {
                flag_text = 0;
                add_button.IsEnabled = true;
            }
            else flag_text = 1;
        }
    }
}
