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
using System.Xml.Linq;

namespace arcadeGame
{
    /// <summary>
    /// Interaction logic for GameOverScreen.xaml
    /// </summary>
    public partial class GameOverScreen : Window
    {

        public string player1name;
        public string player2name;


        private MediaPlayer media2Player = new MediaPlayer();

        public GameOverScreen()
        {


            media2Player.Open(new Uri(@"../../assets/kateBush.mp3", UriKind.RelativeOrAbsolute));
            media2Player.Play();
            media2Player.Volume = 0.1f;


            InitializeComponent();
            
        }
        public void ReturnToMain(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            ///Sets the tags of player 1&2 to their respective names

            mw.Visibility = Visibility.Visible;
            this.Close();
        }

        private void TryAgainClick(object sender, RoutedEventArgs e)
        {
            GameWindow gw = new GameWindow();
            ///Sets the tags of player 1&2 to their respective names
            gw.Player1.Tag = player1name;
            gw.Player2.Tag = player2name;

            gw.Visibility = Visibility.Visible;
            this.Close();
        }

        private void GameOverHS(object sender, RoutedEventArgs e)
        {
            HighscoreWindow gameoverHS = new HighscoreWindow();
            gameoverHS.Visibility = Visibility.Visible;
            this.Close();
        }

        private void GameOverQuit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
