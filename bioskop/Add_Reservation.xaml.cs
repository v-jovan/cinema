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
using MySqlConnector;

namespace bioskop
{
    /// <summary>
    /// Interaction logic for Add_Reservation.xaml
    /// </summary>
    public partial class Add_Reservation : Page
    {
        private MySqlConnection connection;
        public List<string> rooms;
        public string user_id;
        Dictionary<int, int> screening_repertory = new Dictionary<int, int>();

        public Add_Reservation()
        {
            InitializeComponent();

        }

        public Add_Reservation(MySqlConnection connection, string user_id): this()
        {

            this.user_id = user_id;

            string query = "SELECT id, repertory_id FROM screening";
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            var reader_ = command.ExecuteReader();
            while (reader_.Read())
            {
                screening_repertory.Add(reader_.GetInt32("id"), reader_.GetInt32("repertory_id"));
            }
            connection.Close();


            screening.Items.Clear();
            auditorium.Items.Clear();
            movie.Items.Clear();
            this.connection = connection;
            string query_movies = "select id, title from movie";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query_movies, connection) ;
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                movie.Items.Add(reader.GetString("title"));
            }
            connection.Close();
        }

        private void reservation_name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void movie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                screening.Items.Clear();
                auditorium.Items.Clear();
                string query_screening = "select screening_time from movie m INNER JOIN screening scr on m.id = scr.movie_id AND title='" + movie.SelectedItem.ToString() + "' order by screening_time ASC";
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_screening, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    screening.Items.Add(Convert.ToDateTime(reader["screening_time"]).ToString("dd.MM.yyyy HH:mm"));
                }
                connection.Close();
            }
        }

        private void screening_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {

                auditorium.Items.Clear();
                rooms = new List<string>();
                string query_auditorium = "select distinct a.name from movie m INNER JOIN screening scr on m.id = scr.movie_id INNER JOIN auditorium a on a.id = scr.auditorium_id where title='" + movie.SelectedItem.ToString() + "' and scr.screening_time='" + Convert.ToDateTime(screening.SelectedItem.ToString()).ToString("yyyy-MM-dd HH:mm") + "' ORDER BY screening_time ASC";
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_auditorium, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    auditorium.Items.Add(reader.GetString("name"));
                    rooms.Add(reader.GetString("name"));
                }
                connection.Close();
            }
        }

        private void Free_seats_Button_Click(object sender, RoutedEventArgs e)
        {
            int movie_id = 0;
            if (string.IsNullOrEmpty(auditorium.Text))
            {
                MessageBox.Show("Izaberite prvo salu.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            connection.Open();

            string query = "select id from movie where title = '" + movie.SelectedItem.ToString() + "'";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                movie_id = reader.GetInt32("id");
            }
            connection.Close();


            Free_seats free_Seats = new Free_seats(connection, rooms, movie_id, Convert.ToDateTime(screening.SelectedItem.ToString()).ToString("yyyy-MM-dd HH:mm"));
            free_Seats.Show();
        }

        private void add_reservation_btn_Click(object sender, RoutedEventArgs e)
        {
            int auditorium_id = 0;
            bool seats_ok = true;

            if (string.IsNullOrWhiteSpace(reservation_name.Text) || string.IsNullOrEmpty(reservation_name.Text) || movie.SelectedIndex == -1 || screening.SelectedIndex == -1 || auditorium.SelectedIndex == -1)
            {
                MessageBox.Show("Nisu sva polja popunjena! Provjerite polja i pokušajte ponovo.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string[] seats_parsed = seats.Text.Split(',');
            string query_auditorium = "select id from auditorium where name = '" + auditorium.Text + " '";
            connection.Open();
            MySqlCommand cmd_auditorium = new MySqlCommand(query_auditorium, connection);
            var reader_auditorium = cmd_auditorium.ExecuteReader();
            while (reader_auditorium.Read())
            {
                auditorium_id = reader_auditorium.GetInt32("id");
            }
            connection.Close();



            string query = "select concat(seat_row, number) as sjediste from seat INNER JOIN seat_reserved on seat.id = seat_reserved.seat_id INNER JOIN screening on screening_id = screening.id where screening.auditorium_id ='" + auditorium_id.ToString() + "' and screening_time = '" + Convert.ToDateTime(screening.SelectedItem.ToString()).ToString("yyyy-MM-dd HH:mm") + "'";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                foreach (string s in seats_parsed)
                {
                    if (s.Trim().Contains(reader.GetString("sjediste")))
                    {
                        MessageBox.Show("Mjesto " + s.Trim() + " je već zauzeto. Izaberite drugo mjesto.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        seats_ok = false;
                        seats.Clear();
                        return;
                    }
                }
            }
            connection.Close();

            int movie_id = 0;
            connection.Open();
            query = "select id from movie where title = '" + movie.SelectedItem.ToString() + "'";
            cmd = new MySqlCommand(query, connection);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                movie_id = reader.GetInt32("id");
            }
            connection.Close();

            int screening_id = 0;
            connection.Open();
            cmd = new MySqlCommand("select * from screening_full where movie_id = " + movie_id.ToString() + " and auditorium_id = " + auditorium_id.ToString() + " and screening_time = '" + Convert.ToDateTime(screening.SelectedItem.ToString()).ToString("yyyy-MM-dd HH:mm") + "'", connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                screening_id = reader.GetInt32("screening_id");
            }
            connection.Close();

            int active_status = 0;
            if ((bool)active.IsChecked)
                active_status = 1;

            if (seats_ok)
            {
                string query_insert = "insert into reservation (screening_id, seller_id, paid, active, name) values (" + screening_id.ToString() + ", " + user_id + ", 0, " + active_status.ToString() + ", '" + reservation_name.Text +"')";
                connection.Open();
                cmd = new MySqlCommand(query_insert, connection);
                int rowCount = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowCount == 1)
                {
                    Add_Seat_Reserved(connection, seats_parsed);
                    MessageBox.Show("Operacija uspješna.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    reservation_name.Clear();
                    screening.SelectedIndex = -1;
                    auditorium.SelectedIndex = -1;
                    movie.SelectedIndex = -1;
                    seats.Clear();
                }
                else
                {
                    MessageBox.Show("Operacija nauspješna.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }

        public void Add_Seat_Reserved(MySqlConnection connection, string[] seats_parsed)
        {
            int reservation_id = 0;
            int screening_id = 0;
            string query_seat_reserved = "select id from reservation where reservation.id not in (select reservation_id from seat_reserved)";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query_seat_reserved, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                reservation_id = reader.GetInt32("id");
            }
            connection.Close();

            string query_screening = "select screening_id from reservation where id=" + reservation_id.ToString(); ;

            connection.Open();
            cmd = new MySqlCommand(query_screening, connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                screening_id = reader.GetInt32("screening_id");
            }
            connection.Close();

            List<int> seat_ids = new List<int>();
            foreach (string s in seats_parsed)
            {
                string query_seat_id = "select id from seat where seat_row = " + s.Trim()[0] + " and number =" + s.Trim()[1] + " and auditorium_id = " + auditorium.SelectedItem.ToString()[auditorium.SelectedItem.ToString().Length - 1];
                connection.Open();
                cmd = new MySqlCommand(query_seat_id, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    seat_ids.Add(reader.GetInt32("id"));
                }
                connection.Close();
            }


            foreach (int id in seat_ids)
            {
                connection.Open();
                string query_insert_seat_reserved = "insert into seat_reserved (seat_id, reservation_id, screening_id) values (" + id + ", " + reservation_id.ToString() + ", " + screening_id.ToString() + ")";
                cmd = new MySqlCommand(query_insert_seat_reserved, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
