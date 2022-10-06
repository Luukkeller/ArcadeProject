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

using System.Windows.Threading;

namespace arcadeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool moveLeft, moveRight, moveLeft2, moveRight2;
        DispatcherTimer gameTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();
        ImageBrush player2Skin = new ImageBrush();

       
        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += GameLoop;
            gameTimer.IsEnabled = true;
            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Start();

            // player 1 image
            playerSkin.ImageSource = new BitmapImage(new Uri(@"C:\HBO ICT\Projecten\ArcadeProject-main\arcadeGame\arcadeGame\assets\strangerThings1.png"));
            player.Fill = playerSkin;
            
            // player 2 image
            player2Skin.ImageSource = new BitmapImage(new Uri(@"C:\HBO ICT\Projecten\ArcadeProject-main\arcadeGame\arcadeGame\assets\strangerThings2.png"));
            player2.Fill = player2Skin;

            Canvas.Focus();
        }
        

        private void GameLoop(object sender, EventArgs e)
        {
           // Player 1
           if (moveLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
           if (moveRight == true && Canvas.GetLeft(player) + 75 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }
           // Player 2
            if (moveLeft2 == true && Canvas.GetLeft(player2) > 0)
            {
                Canvas.SetLeft(player2, Canvas.GetLeft(player2) - 10);
            }
            if (moveRight2 == true && Canvas.GetLeft(player2) + 75 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player2, Canvas.GetLeft(player2) + 10);
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            //player 1 movement (left or right movement key is pressed)
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
            //player 2 movement (A or D movement key is pressed)
            if (e.Key == Key.A)
            {
                moveLeft2 = true;
            }
            if (e.Key == Key.D)
            {
                moveRight2 = true;
            }
        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            //player 1 movement key is released
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            //player 2 movement key is released
            }
            if (e.Key == Key.A)
            {
                moveLeft2 = false;
            }
            if (e.Key == Key.D)
            {
                moveRight2 = false;
            }
        }


    }
}
