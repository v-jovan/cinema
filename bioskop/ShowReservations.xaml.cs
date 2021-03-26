using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace bioskop
{
    /// <summary>
    /// Interaction logic for ShowReservations.xaml
    /// </summary>
    public partial class ShowReservations : Page
    {
        public ShowReservations()
        {
            InitializeComponent();
        }

        public ShowReservations(MySqlConnection connection) : this()
        {
            string query = "select id as '#', name as 'NA IME', movie_title as 'NAZIV FILMA', auditorium_name as 'SALA', screening_time as 'PROJEKCIJA', concat(seller_name, ' ', seller_surname) as 'PRODAVAC', paid as 'PLACENO', active as 'AKTIVNO' from reservation_full ORDER BY id ASC";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            connection.Close();

            dg.DataContext = dt;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
