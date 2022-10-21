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

namespace arcadeGame
{
    /// <summary>
    /// Interaction logic for GameOverScreen.xaml
    /// </summary>
    public partial class GameOverScreen : Window
    {



        private MediaPlayer media2Player = new MediaPlayer();

        public GameOverScreen()
        {


            media2Player.Open(new Uri(@"../../assets/kateBush.mp3", UriKind.RelativeOrAbsolute));
            media2Player.Play();
            media2Player.Volume = 0.1f;


            InitializeComponent();
        }
    }
}
