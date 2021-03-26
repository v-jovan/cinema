using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace bioskop
{
    /// <summary>
    /// Interaction logic for Edit_Reservation.xaml
    /// </summary>
    public partial class Edit_Reservation : Page
    {
        private MySqlConnection connection;
        private string user_id;

        public Edit_Reservation()
        {
            InitializeComponent();
        }

        public Edit_Reservation(MySqlConnection connection, string user_id): this()
        {
            this.user_id = user_id;
            string query_id = "select id from reservation ORDER BY id ASC";
            reservation_id.ItemsSource = Get_Id(query_id, connection);
        }

        public List<int> Get_Id(string query, MySqlConnection connection)
        {
            this.connection = connection;

            List<int> result = new List<int>();
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetInt32("id"));
            }
            connection.Close();

            return result;
        }

        private void Reservation_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reservation_name.Clear();
            reservation_movie.Items.Clear();
            reservation_screening.Items.Clear();
            auditorium.Items.Clear();
            reservation_seats.Clear();
            if (e.AddedItems.Count > 0)
            {
                int active_status = 0;
                string query_name = "select name, active from reservation where id =" + reservation_id.SelectedItem.ToString();
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_name, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reservation_name.AppendText(reader.GetString("name"));
                    active_status = reader.GetInt32("active");
                }
                connection.Close();

                if (active_status == 1)
                    active.IsChecked = true;
                else
                    active.IsChecked = false;


                string query_movies = "select title from movie";
                connection.Open();
                cmd = new MySqlCommand(query_movies, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reservation_movie.Items.Add(reader.GetString("title"));
                }
                connection.Close();

                string reserved_movie = "";
                string query_reserved_movie = "select movie_title from reservation_full where id = " + reservation_id.SelectedItem.ToString();
                connection.Open();
                cmd = new MySqlCommand(query_reserved_movie, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reserved_movie = reader.GetString("movie_title");
                }
                connection.Close();

                reservation_movie.SelectedItem = reserved_movie;

            }
        }

        private void reservation_movie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reservation_screening.Items.Clear();
            auditorium.Items.Clear();
            reservation_seats.Clear();
            if (e.AddedItems.Count > 0)
            {
                reservation_screening.Items.Clear();
                string query_screening = "select screening_time from screening_full where movie_title ='" + reservation_movie.SelectedItem.ToString() + "' ORDER BY screening_time ASC";
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_screening, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reservation_screening.Items.Add(Convert.ToDateTime(reader["screening_time"]).ToString("dd.MM.yyyy HH:mm"));
                }
                connection.Close();

                int screening_id = 0;
                connection.Open();
                cmd = new MySqlCommand("select screening_id from reservation where id=" + reservation_id.SelectedItem.ToString(), connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    screening_id = reader.GetInt32("screening_id");
                }
                connection.Close();

                string temp = "";
                connection.Open();
                cmd = new MySqlCommand("select screening_time from screening_full where screening_id = " + screening_id.ToString(), connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                     temp = Convert.ToDateTime(reader["screening_time"]).ToString("dd.MM.yyyy HH:mm");
                }
                connection.Close();
                reservation_screening.SelectedItem = temp;
            }
        }

        private void Seats_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(auditorium.SelectedIndex == -1))
            {
                List<string> rooms = new List<string>();
                string query_auditorium = "select distinct a.name from movie m INNER JOIN screening scr on m.id = scr.movie_id INNER JOIN auditorium a on a.id = scr.auditorium_id where title='" + reservation_movie.SelectedItem.ToString() + "' and scr.screening_time='" + Convert.ToDateTime(reservation_screening.SelectedItem.ToString()).ToString("yyyy-MM-dd HH:mm") + "' ORDER BY screening_time ASC";
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_auditorium, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    auditorium.Items.Add(reader.GetString("name"));
                    rooms.Add(reader.GetString("name"));
                }
                connection.Close();

                int movie_id = 0;
                if (string.IsNullOrEmpty(auditorium.Text))
                {
                    MessageBox.Show("Izaberite prvo salu.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                connection.Open();
                string query = "select id from movie where title = '" + reservation_movie.SelectedItem.ToString() + "'";
                cmd = new MySqlCommand(query, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    movie_id = reader.GetInt32("id");
                }
                connection.Close();


                Free_seats free_Seats = new Free_seats(connection, rooms, movie_id, Convert.ToDateTime(reservation_screening.SelectedItem.ToString()).ToString("yyyy-MM-dd HH:mm"));
                free_Seats.Show();
            }
            else
            {
                MessageBox.Show("Niste izabrali salu.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void reservation_screening_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            auditorium.Items.Clear();
            reservation_seats.Clear();
            if (e.AddedItems.Count > 0)
            {
                string auditorium_name = "";
                string query_auditorium_name = "select auditorium_name from reservation_full where id=" + reservation_id.SelectedItem.ToString(); ;
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_auditorium_name, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    auditorium_name = reader.GetString("auditorium_name");
                }
                connection.Close();


                connection.Open();
                string query_all_auditoriums = "select distinct a.name from movie m INNER JOIN screening scr on m.id = scr.movie_id INNER JOIN auditorium a on a.id = scr.auditorium_id where title='" + reservation_movie.SelectedItem.ToString() + "' and scr.screening_time='" + Convert.ToDateTime(reservation_screening.SelectedItem.ToString()).ToString("yyyy-MM-dd HH:mm") + "' ORDER BY screening_time ASC";
                cmd = new MySqlCommand(query_all_auditoriums, connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    auditorium.Items.Add(reader.GetString("name"));
                }
                connection.Close();

                auditorium.SelectedItem = auditorium_name;
            }
        }

        private void auditorium_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reservation_seats.Clear();
            if (e.AddedItems.Count > 0)
            {
                List<int> seat_ids = new List<int>();
                string query_reserved_seats = "select seat_id from seat_reserved where reservation_id = " + reservation_id.SelectedItem.ToString();
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_reserved_seats, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    seat_ids.Add(reader.GetInt32("seat_id"));
                }
                connection.Close();

                foreach (int id in seat_ids)
                {
                    string tmp = "";
                    connection.Open();
                    string query_seats = "select concat(seat_row, number) as seats from seat where id=" + id.ToString();
                    cmd = new MySqlCommand(query_seats, connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        tmp = (reader.GetString("seats") + ", ");
                    }
                    connection.Close();
                    reservation_seats.AppendText(tmp);
                }

                string s = reservation_seats.Text;

                if (s.Length > 1)
                {
                    s = s.Substring(0, s.Length - 2);
                }

                reservation_seats.Text = s;
            }
        }

        private void Delete_Clicked(object sender, RoutedEventArgs e)
        {
            if (reservation_id.SelectedIndex == -1)
            {
                MessageBox.Show("Niste izabrali rezervaciju za obrisati. Pokušajte ponovo.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reservation_id_backup = reservation_id.SelectedItem.ToString();
            string query_delete = "delete from reservation where id = " + reservation_id.SelectedItem.ToString();
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query_delete, connection);
            int rowCount = cmd.ExecuteNonQuery();
            connection.Close();

            if (rowCount == 1)
            {
                MessageBox.Show("Operacija uspješna.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                reservation_id.SelectedIndex = -1;
                //reservation_id.Remove(reservation_id_backup);
                reservation_id.Items.Refresh();
                reservation_name.Clear();
                reservation_movie.Items.Clear();
                reservation_screening.Items.Clear();
                auditorium.Items.Clear();
                reservation_seats.Clear();
                active.IsChecked = false;
            }
            else
            {
                MessageBox.Show("Operacija nauspješna. Provjerite polja", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void Update_Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (!(reservation_id.SelectedIndex == -1 || reservation_movie.SelectedIndex == -1 || reservation_name.Text == "" || auditorium.SelectedIndex == -1 || reservation_screening.SelectedIndex == -1))
            {
                string new_reservation_name = reservation_name.Text,
                       new_reservation_movie = reservation_movie.SelectedItem.ToString(),
                       new_reservation_screening = Convert.ToDateTime(reservation_screening.SelectedItem.ToString()).ToString("yyyy-MM-dd HH:mm"),
                       new_auditorium = auditorium.SelectedItem.ToString(),
                       new_seats = reservation_seats.Text;

                int active_status = active.IsChecked == true ? 1 : 0;

                string query_screening = "select scr.id from movie m INNER JOIN screening scr on m.id = scr.movie_id INNER JOIN auditorium a on a.id = scr.auditorium_id  where m.title='" + new_reservation_movie + "' and screening_time = '" + new_reservation_screening + "' and auditorium_id = " + new_auditorium[new_auditorium.Length - 1];

                int screening_id = 0;
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_screening, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    screening_id = reader.GetInt32("id");
                }
                connection.Close();

                string query_update = "update reservation set name = '" + new_reservation_name + "', active = b'" + active_status.ToString() + "', seller_id = " + user_id + ", screening_id = " +screening_id.ToString() + " where id = " + reservation_id.SelectedItem.ToString();
                connection.Open();
                cmd = new MySqlCommand(query_update, connection);
                int rowCount = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowCount == 1)
                {
                    MessageBox.Show("Operacija uspješna.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Operacija nauspješna. Provjerite polja", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                string query_delete = "delete from seat_reserved where reservation_id = " + reservation_id.SelectedItem.ToString();
                connection.Open();
                cmd = new MySqlCommand(query_delete, connection);
                _ = cmd.ExecuteNonQuery();
                connection.Close();

                string[] seats_parsed = reservation_seats.Text.Split(',');
                List<int> seat_ids = new List<int>();

  
                foreach (string s in seats_parsed)
                {
                    connection.Open();
                    cmd = new MySqlCommand("select id from seat where seat_row=" + s.Trim()[0] + " and number=" + s.Trim()[1] + " and auditorium_id=" + new_auditorium[new_auditorium.Length - 1], connection);
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
                    string query_insert = "insert into seat_reserved (seat_id, reservation_id, screening_id) values (" + id.ToString() + ", " + reservation_id.SelectedItem.ToString() + ", " + screening_id.ToString() + ")";
                    cmd = new MySqlCommand(query_insert, connection);
                    int rows = cmd.ExecuteNonQuery();
                    connection.Close();
                }

                reservation_id.SelectedIndex = -1;
                reservation_id.Items.Refresh();
                reservation_name.Clear();
                reservation_movie.Items.Clear();
                reservation_screening.Items.Clear();
                auditorium.Items.Clear();
                reservation_seats.Clear();
                active.IsChecked = false;

            }
        }
    }
}
