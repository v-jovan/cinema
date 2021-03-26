using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Seller.xaml
    /// </summary>
    public partial class Seller : Window
    {
        private MainWindow m_parent; // MainWindow as a Parent
        string user_id;
        string name, surname;
        MySqlConnection connection;


        public Seller()
        {
            InitializeComponent();

        }

        public Seller(MainWindow parent, string user_id, MySqlConnection connection) : this()
        {
            this.connection = connection;
            m_parent = parent;
            this.user_id = user_id;
            string query = "select name, surname from person where id='" + user_id + "'";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader result = cmd.ExecuteReader();
            if (result.HasRows)
            {
                result.Read();
                name = result["name"].ToString();
                surname = result["surname"].ToString();
            }
            connection.Close();

            logged_in.Content = name + " " + surname;

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Show_Reservation_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = new ShowReservations(connection);
        }

        private void Add_Reservation_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = new Add_Reservation(connection, user_id);
        }

        private void Edit_Reservation_Click(object sender, RoutedEventArgs e)
        {
            Frame.Content = new Edit_Reservation(connection, user_id);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            m_parent.Show();
            m_parent.username.Clear();
            m_parent.password.Clear();
            this.Hide();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
