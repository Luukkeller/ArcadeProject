using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using Microsoft.Win32;

namespace arcadeGame
{
    /// <summary>
    /// Interaction logic for HighscoreWindow.xaml
    /// </summary>
    public partial class HighscoreWindow : Window
    {
        public HighscoreWindow()
        {
            InitializeComponent();
            FillDataGrid();
        }

        /// <summary>
        /// Luuk
        /// function that imports sql highscore data into <datagrid Name= SQLDB>
        /// </summary>
        private void FillDataGrid()
        {
            //string containing local database location
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\marth\\source\\Repos\\Luukkeller\\ArcadeProject\\arcadeGame\\arcadeGame\\database\\GameDataBase.mdf\";Integrated Security=True";        
            string Cmdstring = "SELECT Player, Highscore FROM Highscores ORDER BY Highscore DESC OFFSET 0 ROWS FETCH FIRST 10 ROWS ONLY"; //string containing query that will run on SQL
            SqlConnection connection = new SqlConnection(connectionString); //connecting to database
            {
                SqlCommand cmd = new SqlCommand(Cmdstring, connection);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Highscores");
                sda.Fill(dt);
                SQLDB.ItemsSource = dt.DefaultView;
            }
        }

        /// <summary>
        /// when you click on the button Main Menu, it will close this window and opens up Main Window.
        /// </summary>
        /// <param name="sender">the object that reference to the called event</param>
        /// <param name="e">More information of the object</param>
        private void MainMenuClick(object sender, RoutedEventArgs e)
        {
            MainWindow gw = new MainWindow();
            ///Sets the tags of player 1&2 to their respective names

            gw.Visibility = Visibility.Visible;
            this.Close();

        }
    }

}
