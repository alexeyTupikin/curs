using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace curs
{
    /// <summary>
    /// Логика взаимодействия для generate.xaml
    /// </summary>
    public partial class generate : Page
    {
        MainWindow mainWindow;
        public int qty_disc = 0;
        //список id вопросов что бы избежать их повторения в билетах
        List<int> mas_id_questions = new List<int>();
        bool gen_true = false;
        List<int> list_id_ticket = new List<int>();
        public generate(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            //заполнение двух combobox'ов с дисциплинами и еще 1 combobox'а для выбора
            //кол-ва билетов для генерации
            DataTable disciplines = mainWindow.Select("SELECT [title_discipline] FROM [dbo].[disciplines]");
            if (disciplines.Rows.Count > 0)
            {
                for (int i = 0; i < disciplines.Rows.Count; i++)
                {
                    ChouseDicipline.Items.Add(disciplines.Rows[i][0].ToString());
                    ChouseDopDisc.Items.Add(disciplines.Rows[i][0].ToString());
                }
            }
            for (int i = 1; i < 31; i++)
            {
                combo_qty_ticket.Items.Add($"{i}");
            }
        }
        private void gen(object sender, RoutedEventArgs e) //приступает к генерации 
        {
            list_id_ticket.Clear();
            if (ChouseDicipline.SelectedItem == null)
            {
                MessageBox.Show("Выберите дисциплину, по которой хотите сгенерировать билеты.");
            }
            else if (combo_qty_ticket.SelectedItem == null)
            {
                MessageBox.Show("Выберите кол-во билетов, которое хотели бы сгенерировать.");
            }
            else if (ChouseDopDisc.SelectedItem == null)
            {
                gen_1_discipline();
                MessageBox.Show("Билеты успешно сгенерированы.");
                gen_true = true;
                qty_disc = 1;
            }
            else if (ChouseDicipline.SelectedItem != ChouseDopDisc.SelectedItem)
            {
                gen_2_discipline();
                MessageBox.Show("Билеты успешно сгенерированы.");
                gen_true = true;
                qty_disc = 2;
            }
            else MessageBox.Show("Выбранная вами доп. дисциплина равна выбранной вами основной дисциплине, " +
                 "пожалуйста, измените выбор для корректной работы.");
            save_as.IsEnabled = true;
        }
        public void gen_1_discipline()
        {
            list_id_ticket.Clear();
            mas_id_questions.Clear();
            DataTable last_id_ticket = mainWindow.Select("SELECT MAX(dbo.ticket.id_ticket) FROM dbo.ticket");
            int i = 0;
            int start_id = 0;
            int id_t_p = 0;
            int id_q_t = 1;
            int id_q_disc = 0;
            int id_disc = 0;
            int id_q_disc_proc1 = 0;
            int id_q_disc_proc2 = 0;
            int id_q_disc_proc3 = 0;
            if (last_id_ticket.Rows[0][0] != DBNull.Value)
            {
                start_id = Convert.ToInt32(last_id_ticket.Rows[0][0]);
            }
            else start_id = 0;
            Random rand_q_disc = new Random();
            /*узнаем выбранную дисциплину и "рамки" id вопросов*/
            string chouseDisc = "";
            chouseDisc = Convert.ToString(ChouseDicipline.SelectedValue);
            DataTable id_disciplie = mainWindow.Select($"SELECT id_discipline FROM dbo.disciplines WHERE title_discipline = '{chouseDisc}'");
            id_disc = Convert.ToInt32(id_disciplie.Rows[0][0]);
            DataTable check_disc = mainWindow.Select($"SELECT remove_or_not FROM disciplines WHERE id_discipline = {id_disc}");
            if (Convert.ToInt32(check_disc.Rows[0][0]) == 0)
            {
                DataTable min_theory_s = mainWindow.Select($"SELECT MIN(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc} AND id_type = 1");
                DataTable max_theory_s = mainWindow.Select($"SELECT MAX(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc} AND id_type = 1");
                int min_t = Convert.ToInt32(min_theory_s.Rows[0][0]);
                int max_t = Convert.ToInt32(max_theory_s.Rows[0][0]);
                DataTable min_practis_s = mainWindow.Select($"SELECT MIN(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc} AND id_type = 2");
                DataTable max_practis_s = mainWindow.Select($"SELECT MAX(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc} AND id_type = 2");
                int min_p = Convert.ToInt32(min_practis_s.Rows[0][0]);
                int max_p = Convert.ToInt32(max_practis_s.Rows[0][0]);
                /*узнаем кол-во билетов, которые надо сгенирироать */
                int gen_qty_ticket = Convert.ToInt32(combo_qty_ticket.SelectedValue);
                /*узнаем все id билетов*/
                DataTable id_for_theory = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc} AND id_type = 1");
                DataTable id_for_practis = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc} AND id_type = 2");
                //
                for (i = start_id; i < (gen_qty_ticket + start_id); i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        id_t_p = i + 1;
                        id_q_t = j + 1;
                        if (id_q_t == 3)
                        {
                            //вставляем практику по дисциплине в билет
                            id_q_disc = rand_q_disc.Next(min_p, ((id_for_practis.Rows.Count) + (min_p - 1)));
                            if (mas_id_questions.Contains(id_q_disc) == true)
                            {
                                while (mas_id_questions.Contains(id_q_disc) == true)
                                {
                                    id_q_disc = rand_q_disc.Next(min_p, ((id_for_practis.Rows.Count) + (min_p - 1)));
                                }
                                mas_id_questions.Add(id_q_disc);
                                switch (id_q_t)
                                {
                                    case 1:
                                        {
                                            id_q_disc_proc1 = id_q_disc;
                                            break;
                                        }
                                    case 2:
                                        {
                                            id_q_disc_proc2 = id_q_disc;
                                            break;
                                        }
                                    case 3:
                                        {
                                            id_q_disc_proc3 = id_q_disc;
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                mas_id_questions.Add(id_q_disc);
                                switch (id_q_t)
                                {
                                    case 1:
                                        {
                                            id_q_disc_proc1 = id_q_disc;
                                            break;
                                        }
                                    case 2:
                                        {
                                            id_q_disc_proc2 = id_q_disc;
                                            break;
                                        }
                                    case 3:
                                        {
                                            id_q_disc_proc3 = id_q_disc;
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            //вставляем в билет теоритические вопросы по дисцилине
                            id_q_disc = rand_q_disc.Next(min_t, ((id_for_theory.Rows.Count) + (min_t - 1)));
                            if (mas_id_questions.Contains(id_q_disc) == true)
                            {
                                while (mas_id_questions.Contains(id_q_disc) == true)
                                {
                                    id_q_disc = rand_q_disc.Next(min_t, ((id_for_theory.Rows.Count) + (min_t - 1)));
                                }
                                mas_id_questions.Add(id_q_disc);
                                switch (id_q_t)
                                {
                                    case 1:
                                        {
                                            id_q_disc_proc1 = id_q_disc;
                                            break;
                                        }
                                    case 2:
                                        {
                                            id_q_disc_proc2 = id_q_disc;
                                            break;
                                        }
                                    case 3:
                                        {
                                            id_q_disc_proc3 = id_q_disc;
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                mas_id_questions.Add(id_q_disc);
                                switch (id_q_t)
                                {
                                    case 1:
                                        {
                                            id_q_disc_proc1 = id_q_disc;
                                            break;
                                        }
                                    case 2:
                                        {
                                            id_q_disc_proc2 = id_q_disc;
                                            break;
                                        }
                                    case 3:
                                        {
                                            id_q_disc_proc3 = id_q_disc;
                                            break;
                                        }
                                }
                            }                        
                        }
                        if (list_id_ticket.Contains(id_t_p) == false)
                        {
                            list_id_ticket.Add(id_t_p);
                        }
                    }
                    string text_command = "EXECUTE gen_ticket @id_ticket_p, @id_q_disc_1, @id_q_disc_2, @id_q_disc_3, @id_disc";
                    SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                    command.Parameters.Add("@id_ticket_p", SqlDbType.Int).Value = id_t_p;
                    command.Parameters.Add("@id_q_disc_1", SqlDbType.Int).Value = id_q_disc_proc1;
                    command.Parameters.Add("@id_q_disc_2", SqlDbType.Int).Value = id_q_disc_proc2;
                    command.Parameters.Add("@id_q_disc_3", SqlDbType.Int).Value = id_q_disc_proc3;
                    command.Parameters.Add("@id_disc", SqlDbType.Int).Value = id_disc;
                    DataTable add_question = mainWindow.CommandDB(command);
                    id_q_disc_proc1 = 0;
                    id_q_disc_proc2 = 0;
                    id_q_disc_proc3 = 0;
                }
                //очищаем список id вопросов
                mas_id_questions.Clear();
            }
            else MessageBox.Show("Выбранная вами дисциплина на данный момент не доступна для генерации билетов, т.к. подлежит удалению." +
                " Если вы хотите использовать эту дисциплину при генерации, обратитесь к админу для ее восстановления.");
        } //генерирует билеты по 1 дисциплине

        public void gen_2_discipline()
        {
            list_id_ticket.Clear();
            if (ChouseDopDisc.SelectedItem != null)
            {
                DataTable last_id_ticket = mainWindow.Select("SELECT MAX(dbo.ticket.id_ticket) FROM dbo.ticket");
                int i = 0;
                int start_id = 0;
                int id_t_p = 0;
                int id_q_t = 1;
                int id_q_disc = 0;
                int id_disc1 = 0;
                int id_disc2 = 0;
                int id_q_disc_proc1 = 0;
                int id_q_disc_proc2 = 0;
                int id_q_disc_proc3 = 0;
                int num_disc = 0;
                //
                string chouseDisc1 = "";
                chouseDisc1 = Convert.ToString(ChouseDicipline.SelectedValue);
                DataTable id_disciplie1 = mainWindow.Select($"SELECT id_discipline FROM dbo.disciplines WHERE title_discipline = '{chouseDisc1}'");
                id_disc1 = Convert.ToInt32(id_disciplie1.Rows[0][0]);
                string chouseDisc2 = "";
                chouseDisc2 = Convert.ToString(ChouseDopDisc.SelectedValue);
                DataTable id_disciplie2 = mainWindow.Select($"SELECT id_discipline FROM dbo.disciplines WHERE title_discipline = '{chouseDisc2}'");
                id_disc2 = Convert.ToInt32(id_disciplie2.Rows[0][0]);
                DataTable check_discipline = mainWindow.Select($"SELECT remove_or_not FROM disciplines WHERE id_discipline = {id_disc1}" +
                    $" OR id_discipline = {id_disc2}");
                if ((Convert.ToInt32(check_discipline.Rows[0][0]) == 0) && (Convert.ToInt32(check_discipline.Rows[1][0]) == 0))
                {
                    DataTable id_question1 = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc1}");
                    DataTable id_question2 = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc2}");

                    DataTable id_for_theory1 = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc1} AND id_type = 1");
                    DataTable id_for_practis1 = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc1} AND id_type = 2");
                    DataTable id_for_theory2 = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc2} AND id_type = 1");
                    DataTable id_for_practis2 = mainWindow.Select($"SELECT id_question FROM dbo.questions WHERE id_discipline = {id_disc2} AND id_type = 2");
                    //
                    if (last_id_ticket.Rows[0][0] != DBNull.Value)
                    {
                        start_id = Convert.ToInt32(last_id_ticket.Rows[0][0]);
                    }
                    else start_id = 0;
                    Random rand_q_disc = new Random();
                    /*узнаем выбранную дисциплину и "рамки" id вопросов*/
                    //для первой дисциплины
                    DataTable min_theory_s1 = mainWindow.Select($"SELECT MIN(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc1} AND id_type = 1");
                    DataTable max_theory_s1 = mainWindow.Select($"SELECT MAX(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc1} AND id_type = 1");
                    int min_t1 = Convert.ToInt32(min_theory_s1.Rows[0][0]);
                    int max_t1 = Convert.ToInt32(max_theory_s1.Rows[0][0]);
                    DataTable min_practis_s1 = mainWindow.Select($"SELECT MIN(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc1} AND id_type = 2");
                    DataTable max_practis_s1 = mainWindow.Select($"SELECT MAX(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc1} AND id_type = 2");
                    int min_p1 = Convert.ToInt32(min_practis_s1.Rows[0][0]);
                    int max_p1 = Convert.ToInt32(max_practis_s1.Rows[0][0]);
                    //для второй дисциплины
                    DataTable min_theory_s2 = mainWindow.Select($"SELECT MIN(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc2} AND id_type = 1");
                    DataTable max_theory_s2 = mainWindow.Select($"SELECT MAX(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc2} AND id_type = 1");
                    int min_t2 = Convert.ToInt32(min_theory_s2.Rows[0][0]);
                    int max_t2 = Convert.ToInt32(max_theory_s2.Rows[0][0]);
                    DataTable min_practis_s2 = mainWindow.Select($"SELECT MIN(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc2} AND id_type = 2");
                    DataTable max_practis_s2 = mainWindow.Select($"SELECT MAX(dbo.questions.id_question) FROM dbo.questions WHERE id_discipline = {id_disc2} AND id_type = 2");
                    int min_p2 = Convert.ToInt32(min_practis_s2.Rows[0][0]);
                    int max_p2 = Convert.ToInt32(max_practis_s2.Rows[0][0]);
                    /*узнаем кол-во билетов, которые надо сгенирироать */
                    int gen_qty_ticket = Convert.ToInt32(combo_qty_ticket.SelectedValue);
                    for (i = start_id; i < (gen_qty_ticket + start_id); i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            id_t_p = i + 1;
                            id_q_t = j + 1;
                            if (id_q_t == 3)
                            {
                                //добавление практики в билет 3 вопросом, поочередно из 1 или 2 дисциплины, что бы
                                //в равной мере распределить вопросы по дисциплинам
                                if ((i % 2) == 1)
                                {
                                    id_q_disc = rand_q_disc.Next(min_p1, ((id_for_practis1.Rows.Count) + (min_p1 - 1)));
                                    if (mas_id_questions.Contains(id_q_disc) == true)
                                    {
                                        while (mas_id_questions.Contains(id_q_disc) == true)
                                        {
                                            id_q_disc = rand_q_disc.Next(min_p1, ((id_for_practis1.Rows.Count) + (min_p1 - 1)));
                                        }
                                        mas_id_questions.Add(id_q_disc);
                                        switch (id_q_t)
                                        {
                                            case 1:
                                                {
                                                    id_q_disc_proc1 = id_q_disc;
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    id_q_disc_proc2 = id_q_disc;
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    id_q_disc_proc3 = id_q_disc;
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        mas_id_questions.Add(id_q_disc);
                                        switch (id_q_t)
                                        {
                                            case 1:
                                                {
                                                    id_q_disc_proc1 = id_q_disc;
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    id_q_disc_proc2 = id_q_disc;
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    id_q_disc_proc3 = id_q_disc;
                                                    break;
                                                }
                                        }
                                    }
                                    num_disc = 1;
                                }
                                else
                                {
                                    id_q_disc = rand_q_disc.Next(min_p2, ((id_for_practis2.Rows.Count) + (min_p2 - 1)));
                                    if (mas_id_questions.Contains(id_q_disc) == true)
                                    {
                                        while (mas_id_questions.Contains(id_q_disc) == true)
                                        {
                                            id_q_disc = rand_q_disc.Next(min_p2, ((id_for_practis2.Rows.Count) + (min_p2 - 1)));
                                        }
                                        mas_id_questions.Add(id_q_disc);
                                        switch (id_q_t)
                                        {
                                            case 1:
                                                {
                                                    id_q_disc_proc1 = id_q_disc;
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    id_q_disc_proc2 = id_q_disc;
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    id_q_disc_proc3 = id_q_disc;
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        mas_id_questions.Add(id_q_disc);
                                        switch (id_q_t)
                                        {
                                            case 1:
                                                {
                                                    id_q_disc_proc1 = id_q_disc;
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    id_q_disc_proc2 = id_q_disc;
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    id_q_disc_proc3 = id_q_disc;
                                                    break;
                                                }
                                        }
                                    }
                                    num_disc = 2;
                                }
                            }
                            //добавляем теорию по 1 дисцплине
                            else if (id_q_t == 1)
                            {
                                id_q_disc = rand_q_disc.Next(min_t1, ((id_for_theory1.Rows.Count) + (min_t1 - 1)));
                                if (mas_id_questions.Contains(id_q_disc) == true)
                                {
                                    while (mas_id_questions.Contains(id_q_disc) == true)
                                    {
                                        id_q_disc = rand_q_disc.Next(min_t1, ((id_for_theory1.Rows.Count) + (min_t1 - 1)));
                                    }
                                    mas_id_questions.Add(id_q_disc);
                                    switch (id_q_t)
                                    {
                                        case 1:
                                            {
                                                id_q_disc_proc1 = id_q_disc;
                                                break;
                                            }
                                        case 2:
                                            {
                                                id_q_disc_proc2 = id_q_disc;
                                                break;
                                            }
                                        case 3:
                                            {
                                                id_q_disc_proc3 = id_q_disc;
                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    mas_id_questions.Add(id_q_disc);
                                    switch (id_q_t)
                                    {
                                        case 1:
                                            {
                                                id_q_disc_proc1 = id_q_disc;
                                                break;
                                            }
                                        case 2:
                                            {
                                                id_q_disc_proc2 = id_q_disc;
                                                break;
                                            }
                                        case 3:
                                            {
                                                id_q_disc_proc3 = id_q_disc;
                                                break;
                                            }
                                    }
                                }
                            }
                            //добавляем теорию по 2 дисцплине
                            else if (id_q_t == 2)
                            {
                                id_q_disc = rand_q_disc.Next(min_t2, ((id_for_theory2.Rows.Count) + (min_t2 - 1)));
                                if (mas_id_questions.Contains(id_q_disc) == true)
                                {
                                    while (mas_id_questions.Contains(id_q_disc) == true)
                                    {
                                        id_q_disc = rand_q_disc.Next(min_t2, ((id_for_theory2.Rows.Count) + (min_t2 - 1)));
                                    }
                                    mas_id_questions.Add(id_q_disc);
                                    switch (id_q_t)
                                    {
                                        case 1:
                                            {
                                                id_q_disc_proc1 = id_q_disc;
                                                break;
                                            }
                                        case 2:
                                            {
                                                id_q_disc_proc2 = id_q_disc;
                                                break;
                                            }
                                        case 3:
                                            {
                                                id_q_disc_proc3 = id_q_disc;
                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    mas_id_questions.Add(id_q_disc);
                                    switch (id_q_t)
                                    {
                                        case 1:
                                            {
                                                id_q_disc_proc1 = id_q_disc;
                                                break;
                                            }
                                        case 2:
                                            {
                                                id_q_disc_proc2 = id_q_disc;
                                                break;
                                            }
                                        case 3:
                                            {
                                                id_q_disc_proc3 = id_q_disc;
                                                break;
                                            }
                                    }
                                }
                                if (list_id_ticket.Contains(id_t_p) == false)
                                {
                                    list_id_ticket.Add(id_t_p);
                                }
                            }
                        }
                        string text_command = "EXECUTE gen_ticket2 @id_ticket_p, @id_q_disc_1, @id_q_disc_2, @id_q_disc_3, @id_disc1, @id_disc2, @num_disc";
                        SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                        command.Parameters.Add("@id_ticket_p", SqlDbType.Int).Value = id_t_p;
                        command.Parameters.Add("@id_q_disc_1", SqlDbType.Int).Value = id_q_disc_proc1;
                        command.Parameters.Add("@id_q_disc_2", SqlDbType.Int).Value = id_q_disc_proc2;
                        command.Parameters.Add("@id_q_disc_3", SqlDbType.Int).Value = id_q_disc_proc3;
                        command.Parameters.Add("@id_disc1", SqlDbType.Int).Value = id_disc1;
                        command.Parameters.Add("@id_disc2", SqlDbType.Int).Value = id_disc2;
                        command.Parameters.Add("@num_disc", SqlDbType.Int).Value = num_disc;
                        DataTable add_ticket = mainWindow.CommandDB(command);
                        id_q_disc_proc1 = 0;
                        id_q_disc_proc2 = 0;
                        id_q_disc_proc3 = 0;
                        num_disc = 0;
                    }
                    //очищаем список id вопросов
                    mas_id_questions.Clear();
                }
                else if (Convert.ToInt32(check_discipline.Rows[0][0]) == 1)
                {
                    MessageBox.Show("Первая выбранная вами дисциплина на данный момент не доступна для генерации билетов, т.к.подлежит удалению." +
                " Если вы хотите использовать эту дисциплину при генерации, обратитесь к админу для ее восстановления.");
                }
                else if (Convert.ToInt32(check_discipline.Rows[1][0]) == 1)
                {
                    MessageBox.Show("Первая выбранная вами дисциплина на данный момент не доступна для генерации билетов, т.к.подлежит удалению." +
                " Если вы хотите использовать эту дисциплину при генерации, обратитесь к админу для ее восстановления.");
                }
            }
        } //генерирует билеты по 2 дисциплинам

        private async void save_as_Click(object sender, RoutedEventArgs e) //сохраняет сгенерированные билеты в файл
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string full_path = openFileDialog.FileName;
            File.WriteAllText(@full_path, "");
            int year = DateTime.Today.Year;
            int k = 1;
            DataTable sel_qty = mainWindow.Select("SELECT COUNT(*) FROM chairmenCK");
            Random rand_num = new Random();
            int num_ck = rand_num.Next(0, Convert.ToInt32(sel_qty.Rows[0][0]));
            DataTable fio_title = mainWindow.Select("SELECT last_name + ' ' + SUBSTRING(name, 1, 1) " +
                "+ '. ' + SUBSTRING(second_name, 1, 1) AS fio, title_ck FROM dbo.chairmenCK");
            //проверяем генерировал ли преподаватель билеты, что бы их сохранить
            if (gen_true)
            {
                MessageBox.Show("True, go save");
                using (StreamWriter writer = new StreamWriter(@full_path, true))
                {
                    for (int i = 0; i < list_id_ticket.Count; i++)
                    {
                        if (qty_disc == 1)
                        {
                            DataTable ticket_w = mainWindow.Select("SELECT dbo.ticket.id_ticket, " +
                            "dbo.disciplines.title_discipline, dbo.questions.question FROM dbo.questions " +
                            "INNER JOIN dbo.ticket ON dbo.questions.id_discipline = dbo.ticket.id_discipline " +
                            "AND dbo.questions.id_question = dbo.ticket.id_question_discipline INNER JOIN " +
                            "dbo.disciplines ON dbo.questions.id_discipline = dbo.disciplines.id_discipline " +
                            $"WHERE(dbo.ticket.id_ticket = {list_id_ticket[i]})");
                            DataTable fio_teacher = mainWindow.Select($"SELECT dbo.teacher.last_name + ' ' + SUBSTRING" +
                                $"(dbo.teacher.name, 1, 1) + '. ' + SUBSTRING(dbo.teacher.second_name, 1, 1) + '.' AS " +
                                $"fio_teacher, dbo.[load].id_discipline, dbo.disciplines.title_discipline FROM dbo.teacher " +
                                $"INNER JOIN dbo.[load] ON dbo.teacher.id_user = dbo.[load].id_user INNER JOIN dbo.disciplines " +
                                $"ON dbo.[load].id_discipline = dbo.disciplines.id_discipline WHERE (title_discipline = '{ticket_w.Rows[0][1]}')");
                            string titular_1 = $"\t\tФБГОУ ВПО МГУТУ им. К.Г. Разумовского Университетский колледж информационных технологий.\n" +
                                $"Утверждаю\t\tЭкзаменационный билет №{k}\t\t\t\t\tРассмотрено цикловой комиссией:\n" +
                                $"Зам. директора по УМР\tПо дисциплине: {ticket_w.Rows[0][1]}\t\t\t\t{fio_title.Rows[num_ck][1]}\n" +
                                $"В.В. Лындина\t\tСпециальность: 09.02.03\t\t\t{year} г.\n" +
                                $"{year} г.\t\t\tКурс 3 Семестр 6\t\t\tПротокол №762348\n\n" +
                                $"1. {ticket_w.Rows[0][2]}\n" +
                                $"2. {ticket_w.Rows[1][2]}\n" +
                                $"3. {ticket_w.Rows[2][2]}\n\n" +
                                $"\t\t\tПреподаватель: {fio_teacher.Rows[0][0]}\n" +
                                $"\t\t\tПредседатель цикловой комиссии: {fio_title.Rows[num_ck][0]}\n\n\n";
                            k++;
                            await writer.WriteLineAsync(titular_1);
                        }
                        else if (qty_disc == 2)
                        {
                            DataTable ticket_w = mainWindow.Select("SELECT dbo.ticket.id_ticket, " +
                            "dbo.disciplines.title_discipline, dbo.questions.question FROM dbo.questions " +
                            "INNER JOIN dbo.ticket ON dbo.questions.id_discipline = dbo.ticket.id_discipline " +
                            "AND dbo.questions.id_question = dbo.ticket.id_question_discipline INNER JOIN " +
                            "dbo.disciplines ON dbo.questions.id_discipline = dbo.disciplines.id_discipline " +
                            $"WHERE(dbo.ticket.id_ticket = {list_id_ticket[i]})");
                            DataTable fio_teacher = mainWindow.Select($"SELECT dbo.teacher.last_name + ' ' + SUBSTRING" +
                                $"(dbo.teacher.name, 1, 1) + '. ' + SUBSTRING(dbo.teacher.second_name, 1, 1) + '.' AS " +
                                $"fio_teacher, dbo.[load].id_discipline, dbo.disciplines.title_discipline FROM dbo.teacher " +
                                $"INNER JOIN dbo.[load] ON dbo.teacher.id_user = dbo.[load].id_user INNER JOIN dbo.disciplines " +
                                $"ON dbo.[load].id_discipline = dbo.disciplines.id_discipline WHERE (title_discipline = '{ticket_w.Rows[0][1]}')");
                            string titular_2 = $"ФБГОУ ВПО МГУТУ им. К.Г. Разумовского Университетский колледж информационных технологий.\n" +
                                $"Утверждаю\t\tЭкзаменационный билет №{ticket_w.Rows[0][0]}\t\t\t\t\tРассмотрено цикловой комиссией:\n" +
                                $"Зам. директора по УМР\tПо дисциплинам: {ticket_w.Rows[0][1]},\n\t\t\t\t\t{ticket_w.Rows[1][1]}\t\t\t{fio_title.Rows[num_ck][1]}\n" +
                                $"В.В. Лындина\t\tСпециальность: 09.02.03\t\t\t{year} г.\n" +
                                $"{year} г.\t\t\tКурс 3 Семестр 6\t\t\tПротокол №762348\n\n" +
                                $"1. {ticket_w.Rows[0][2]}\n" +
                                $"2. {ticket_w.Rows[1][2]}\n" +
                                $"3. {ticket_w.Rows[2][2]}\n\n" +
                                $"\t\t\tПреподаватель: {fio_teacher.Rows[0][0]}\n" +
                                $"\t\t\tПредседатель цикловой комиссии: {fio_title.Rows[num_ck][0]}\n\n\n";
                            await writer.WriteLineAsync(titular_2);
                            k++;
                        }
                    }
                    writer.Close();
                }
                gen_true = false;
            }
            else MessageBox.Show("False, go back");
            ChouseDopDisc.IsEnabled = false;
            combo_qty_ticket.IsEnabled = false;
            generation.IsEnabled = false;
            save_as.IsEnabled = false;
            k = 1;
        }

        private void ChouseDicipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            combo_qty_ticket.IsEnabled = true;
            ChouseDopDisc.IsEnabled = true;
        }

        private void combo_qty_ticket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            generation.IsEnabled = true;
        }
    }
}
