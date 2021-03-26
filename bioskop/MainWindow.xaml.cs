using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Windows.Media.Media3D;

namespace bioskop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string connectionString = System.IO.File.ReadAllText("connection.txt");
        MySqlConnection connection;
        string user_id;

        public MainWindow()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text != "" && password.Password != "")
            {
                string hash_password;

                using (SHA512 sha512Hash = SHA512.Create())
                {
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(password.Password);
                    byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                    hash_password = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                }

                connection.Open();
                string query = "select person_id, username, password from seller where username='" + username.Text + "' and password='" + hash_password.ToLower() + "'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader result = cmd.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    user_id = result["person_id"].ToString();
                    connection.Close();
                    Seller sellerWindow = new Seller(this, user_id, connection);
                    sellerWindow.Show();
                    this.Hide();

                }
                else
                {
                    connection.Close();
                    MessageBox.Show("Neispravni podaci!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    username.Clear();
                    password.Clear();
                }
            }
            else
            {
                MessageBox.Show("Neispravni podaci!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                username.Clear();
                password.Clear();
            }
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void View_Movies_Click(object sender, RoutedEventArgs e)
        {

            View_Movies view_movies = new View_Movies(connection);
            view_movies.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Edit_Movies edit_movies = new Edit_Movies(connection);
            edit_movies.Show();
        }
    }
}
