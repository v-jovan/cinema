using MySqlConnector;
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
using System.Windows.Shapes;

namespace bioskop
{
    /// <summary>
    /// Interaction logic for Free_seats.xaml
    /// </summary>
    public partial class Free_seats : Window
    {
        private MySqlConnection connection;

        public Free_seats()
        {
            InitializeComponent();
        }

        public Free_seats(MySqlConnection connection, List<string> rooms, int movie_id, string time): this()
        {
            this.connection = connection;
            StringBuilder sb = new StringBuilder();

            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select distinct concat(seat_row,number) as sjediste from seat", connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                sb.Append(reader.GetString("sjediste") + " ");
            }
            connection.Close();

            string sb_backup = sb.ToString();

            foreach (string s in rooms)
            {
                string query = "select concat(seat_row, number) as sjediste from seat INNER JOIN seat_reserved on seat.id = seat_reserved.seat_id INNER JOIN screening on screening.id = screening_id where movie_id =" + movie_id + " and seat.auditorium_id =" + s[s.Length - 1] + " and screening_time = '" + time + "'";
                if (string.Equals(s, "SALA 1"))
                {
                    connection.Open();
                    MySqlCommand cmd1 = new MySqlCommand(query, connection);
                    var reader1 = cmd1.ExecuteReader();
                    while (reader1.Read())
                    {
                        sb.Replace(reader1.GetString("sjediste"), "xx");
                    }
                    connection.Close();
                    auditorium1.AppendText(sb.ToString());
                }
                else if (string.Equals(s, "SALA 2"))
                {
                    sb.Clear();
                    sb.Append(sb_backup);
                    connection.Open();
                    MySqlCommand cmd2 = new MySqlCommand(query, connection);
                    var reader2 = cmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        sb.Replace(reader2.GetString("sjediste"), "xx");
                    }
                    connection.Close();
                    auditorium2.AppendText(sb.ToString());
                }
                else if (string.Equals(s, "SALA 3"))
                {
                    sb.Clear();
                    sb.Append(sb_backup);
                    connection.Open();
                    MySqlCommand cmd3 = new MySqlCommand(query, connection);
                    var reader3 = cmd3.ExecuteReader();
                    while (reader3.Read())
                    {
                        sb.Replace(reader3.GetString("sjediste"), "xx");
                    }
                    connection.Close();
                    auditorium3.AppendText(sb.ToString());
                }
            }
        }

        private void auditorium1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void auditorium2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
