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
    /// Interaction logic for Edit_Movies.xaml
    /// </summary>
    public partial class Edit_Movies : Window
    {
        private MySqlConnection connection;
        int movie_id = 0;

        public Edit_Movies()
        {
            InitializeComponent();
        }

        public Edit_Movies(MySqlConnection connection): this()
        {
            this.connection = connection;
            string query_all_movies = "select title from movie";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query_all_movies, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                movie_combobox.Items.Add(reader.GetString("title"));
            }
            connection.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string query_all_movies = "select id from movie where title='" + movie_combobox.SelectedItem.ToString() + "'";
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_all_movies, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    movie_id = reader.GetInt32("id");
                }
                connection.Close();

                if (movie_combobox.SelectedIndex != -1)
                {
                    selected_movie.Content = "FILM: " + movie_combobox.SelectedItem.ToString();

                    title.Text = movie_combobox.SelectedItem.ToString();

                    connection.Open();
                    cmd = new MySqlCommand("select release_year from movie where id=" + movie_id.ToString(), connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        release_year.Text = reader.GetInt32("release_year").ToString();
                    }
                    connection.Close();

                    connection.Open();
                    cmd = new MySqlCommand("select director from movie where id=" + movie_id.ToString(), connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        director.Text = reader.GetString("director");
                    }
                    connection.Close();

                    connection.Open();
                    cmd = new MySqlCommand("select duration from movie where id=" + movie_id.ToString(), connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        duration.Text = reader.GetInt32("duration").ToString();
                    }
                    connection.Close();

                    connection.Open();
                    cmd = new MySqlCommand("select description from movie where id=" + movie_id.ToString(), connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        description.Text = reader.GetString("description");
                    }
                    connection.Close();
                }
                else
                {
                    title.Text = "";
                    selected_movie.Content = "";
                    description.Clear();
                    duration.Clear();
                    director.Clear();
                }
            }
        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            if (movie_combobox.SelectedIndex == -1 || title.Text == "" || release_year.Text == "" || director.Text == "" || duration.Text == "" || description.Text == "")
            {
                MessageBox.Show("Imate praznih polja. Ne smiju vrijednosti biti prazne.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string query_update = "update movie set title='" + title.Text + "', release_year =" + release_year.Text + ", director = '" + director.Text + "', description = '" + description.Text + "', duration = " + duration.Text + " where id =" + movie_id.ToString();
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query_update, connection);
                int rowCount = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowCount > 0)
                {
                    MessageBox.Show("Operacija uspješna.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    title.Text = "";
                    selected_movie.Content = "";
                    description.Clear();
                    duration.Clear();
                    director.Clear();

                    string query_all_movies = "select title from movie";
                    connection.Open();
                    cmd = new MySqlCommand(query_all_movies, connection);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        movie_combobox.Items.Add(reader.GetString("title"));
                    }
                    connection.Close();

                    movie_combobox.SelectedIndex = -1;
                }
                else
                    MessageBox.Show("Operacija neuspješna. Provjerite polja.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (movie_combobox.SelectedIndex == -1)
            {
                MessageBox.Show("Morate izabrati film koji želite brisati.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Da li ste sigurni?", "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string query_delete = "delete from movie where id =" + movie_id.ToString();
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query_delete, connection);
                    int rowCount = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowCount > 0)
                    {
                        MessageBox.Show("Operacija uspješna.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        title.Text = "";
                        selected_movie.Content = "";
                        description.Clear();
                        duration.Clear();
                        director.Clear();

                        string query_all_movies = "select title from movie";
                        connection.Open();
                        cmd = new MySqlCommand(query_all_movies, connection);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            movie_combobox.Items.Add(reader.GetString("title"));
                        }
                        connection.Close();

                        movie_combobox.SelectedIndex = -1;
                    }
                    else
                        MessageBox.Show("Operacija neuspješna.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
