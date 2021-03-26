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
using System.Windows.Shapes;
using MySqlConnector;

namespace bioskop
{
    /// <summary>
    /// Interaction logic for View_Movies.xaml
    /// </summary>
    public partial class View_Movies : Window
    {

        public View_Movies()
        {
            InitializeComponent();

        }

        public View_Movies(MySqlConnection connection) : this()
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("select id as '#', title as 'NAZIV', release_year as 'GODINA', duration as 'TRAJANJE (min)', director as 'PRODUCENT', description as 'KRATAK OPIS' from movie ORDER BY id ASC", connection);
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            connection.Close();

            dg.DataContext = dt;
        }

    }
}
