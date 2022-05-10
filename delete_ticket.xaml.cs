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
    /// Логика взаимодействия для delete_ticket.xaml
    /// </summary>
    public partial class delete_ticket : Page
    {
        MainWindow mainWindow;
        List<int> list_id_ticket = new List<int>();
        public delete_ticket(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            DataTable disc = mainWindow.Select("SELECT title_discipline FROM disciplines");
            for(int i = 0; i < disc.Rows.Count; i++)
            {
                combo_discipline.Items.Add(Convert.ToString(disc.Rows[i][0]));
            }
        }
        public void textBlock_ticket() //обновляем TextBox с билетами
        {
            list_id_ticket.Clear();
            combo_id_ticket.Items.Clear();
            text_ticket.Text = "";
            DataTable disc_sel = mainWindow.Select($"SELECT id_discipline FROM disciplines " +
                $"WHERE title_discipline = '{Convert.ToString(combo_discipline.SelectedValue)}'");
            int id_disc = Convert.ToInt32(disc_sel.Rows[0][0]);
            DataTable id_ticket = mainWindow.Select($"SELECT DISTINCT id_ticket FROM ticket WHERE id_discipline = {id_disc}");
            for(int l = 0; l<id_ticket.Rows.Count; l++)
            {
                list_id_ticket.Add(Convert.ToInt32(id_ticket.Rows[l][0]));
            }
            for(int i = 0; i<id_ticket.Rows.Count;i++)
            {
                combo_id_ticket.Items.Add(Convert.ToInt32(id_ticket.Rows[i][0]));
            }
            for (int j = 0; j < list_id_ticket.Count; j++)
            {
                string text_command = "EXECUTE ticket_sel_p @id_ticket";
                SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                command.Parameters.Clear();
                command.Parameters.Add("@id_ticket", SqlDbType.NVarChar).Value = list_id_ticket[j];
                DataTable ticket_sel_p = mainWindow.CommandDB(command);
                text_ticket.AppendText($"Билет №{Convert.ToString(list_id_ticket[j])}{'\n'}");
                text_ticket.AppendText($"{Convert.ToString(ticket_sel_p.Rows[0][1])}. {Convert.ToString(ticket_sel_p.Rows[0][2])}{'\n'}");
                text_ticket.AppendText($"{Convert.ToString(ticket_sel_p.Rows[1][1])}. {Convert.ToString(ticket_sel_p.Rows[1][2])}{'\n'}");
                text_ticket.AppendText($"{Convert.ToString(ticket_sel_p.Rows[2][1])}. {Convert.ToString(ticket_sel_p.Rows[2][2])}{'\n'}");
                text_ticket.AppendText($"{'\n'}");
            }
        }
        private void delete_ticket_button(object sender, RoutedEventArgs e) //удаляет выбранный билет
        {
            if ((combo_discipline.SelectedValue != null) && (combo_id_ticket.SelectedValue != null))
            {
                string text_command = "EXECUTE delete_ticket @id_ticket";
                SqlCommand command = SqlServer.CreateSqlCommand(text_command);
                command.Parameters.Add("@id_ticket", SqlDbType.Int).Value = Convert.ToString(combo_id_ticket.SelectedValue);
                DataTable delete_ticket_p = mainWindow.CommandDB(command);
                textBlock_ticket();
                delete_ticket_click.IsEnabled = false;
                combo_id_ticket.IsEnabled = false;
                combo_discipline.SelectedValue = null;
                combo_id_ticket.SelectedValue = null;
                MessageBox.Show("Билет удален.");
            }
        }

        private void combo_discipline_Selected(object sender, RoutedEventArgs e)
        {
            textBlock_ticket();
            combo_id_ticket.IsEnabled = true;
        }

        private void combo_id_ticket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            delete_ticket_click.IsEnabled = true;
        }
    }
}
