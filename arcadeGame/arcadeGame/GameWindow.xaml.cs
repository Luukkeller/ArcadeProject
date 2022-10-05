using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
using System.Windows.Threading;



namespace arcadeGame
{
    public partial class GameWindow : Window
    {
        ///Movement keys for player1.
        private bool P1moveLeft = false;
        private bool P1moveRight = false;


        //REMOVE THIS IF YOU DONT WANT TO USE IT AS DEMO
        private bool P1moveUp = false;
        private bool P1moveDown = false;
        //REMOVE THIS IF YOU DONT WANT TO USE IT AS DEMO



        /// Game Variables.
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private const int gameTick = 10;
        private const int playerSpeed = 10;



        ///Lists for both enemy bullets and player bullets. We need these to be able to loop over all the bullets in the scene.
        private List<Rectangle> enemyBullets = new List<Rectangle>();
        private List<Rectangle> playerBullets = new List<Rectangle>();

        ///The List for enemies. This is required for enemy hit detection.
        private List<Rectangle> enemies = new List<Rectangle>();





        public GameWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Tick += GameEngine;
            gameTimer.Start();

            myCanvas.Focus();


            //REMOVE THIS IF YOU DONT WANT TO USE IT AS DEMO

            Rectangle newBullet = new Rectangle
            {
                Name = "Bullet",
                Tag = "red",
                Height = 20,
                Width = 20,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };
            Canvas.SetTop(newBullet, 100 - newBullet.Height);
            Canvas.SetLeft(newBullet, 100 );

            enemyBullets.Add(newBullet);

            myCanvas.Children.Add(newBullet);

            //REMOVE THIS IF YOU DONT WANT TO USE IT AS DEMO
        }

        private void GameEngine(object sender, EventArgs e)
        {
            //Coordinate Display for player1.
            //Can be removed

            Text1.Content = "TopLeft " + Canvas.GetTop(Player1) + "," + Canvas.GetLeft(Player1);
            Text2.Content = "TopRight " + Canvas.GetTop(Player1) + "," + (Canvas.GetLeft(Player1)+Player1.Width);
            Text3.Content = "BotLeft " + (Canvas.GetTop(Player1) + Player1.Height) + "," + Canvas.GetLeft(Player1);
            Text4.Content = "BotRight " + (Canvas.GetTop(Player1) + Player1.Height) + "," + (Canvas.GetLeft(Player1) + Player1.Width);






            PlayerMovement();
            PlayerHitDetection(Player1);
            PlayerHitDetection(Player2);
            EnemyHitDetection();
        }



        ///Movement for player. Temporarily added up and down to hit the "Bullet" easier.
        ///It stays constrianed to the game window.
        private void PlayerMovement()
        {
            if (P1moveLeft && Canvas.GetLeft(Player1) > 0)
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) - playerSpeed);

            if (P1moveRight && Canvas.GetLeft(Player1) + Player1.Width < Application.Current.MainWindow.Width)
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) + playerSpeed);

            //REMOVE THIS IF YOU DONT WANT TO USE IT AS DEMO

            if (P1moveDown && Canvas.GetTop(Player1) + Player1.Height < Application.Current.MainWindow.Height)
                Canvas.SetTop(Player1, Canvas.GetTop(Player1) + playerSpeed);

            if (P1moveUp && Canvas.GetTop(Player1) > 0)
                Canvas.SetTop(Player1, Canvas.GetTop(Player1) - playerSpeed);

            //REMOVE THIS IF YOU DONT WANT TO USE IT AS DEMO
        }



        /// This accepts a player variable that will then only reference to said player. So if you put in player 1 it compares all the bullets to player 1.
        private void PlayerHitDetection(Rectangle Player)
        {
            ///We loop over each of the bullets.
            for (int i = 0; i < enemyBullets.Count; i++)
            {
                bool a = false;
                bool b = false;
                if ((Canvas.GetTop(enemyBullets[i]) + enemyBullets[i].Height) > Canvas.GetTop(Player))
                {
                    a = true;

                }
                ///if its not already below the top of the player we stop checking this bullet. This makes it so it does less calculations per bullet if its not close enough.
                else
                {
                    break;
                }
                /// We check if right corner or left corner is within the players width.
                if ((Canvas.GetLeft(enemyBullets[i]) + enemyBullets[i].Width) > Canvas.GetLeft(Player) && (Canvas.GetLeft(enemyBullets[i]) + enemyBullets[i].Width) < (Canvas.GetLeft(Player) + Player.Width))
                {
                    b = true;

                }
                if ((Canvas.GetLeft(enemyBullets[i])) < (Canvas.GetLeft(Player) + Player.Width) && Canvas.GetLeft(enemyBullets[i]) > Canvas.GetLeft(Player))
                {
                    b = true;

                }

                /// A = If the bullet is below the top of the player.
                /// B = if one of the bottom corners is between Topleft and Topright of the player.
                if (a && b)
                {
                    //Replace this with PlayerTakeDamage(enemyBullets[i]) when not in demo mode.
                    enemyBullets[i].Fill = Brushes.Red;
                }
                    //Remove this when not in demo mode
                else
                {
                    enemyBullets[i].Fill = Brushes.White;
                }

                    //Remove this when not in demo mode
            }
        }

       
        
        private void EnemyHitDetection()
        {
            ///We loop over the enemies and for each enemy we check if a bullet is hitting it.
            for (int i = 0; i < enemies.Count; i++)
            {
                ///We loop over each of the bullets
                for (int ii = 0; ii < playerBullets.Count; ii++)
                {
                    bool a = false;
                    bool b = false;
                    if (Canvas.GetTop(playerBullets[ii]) > (Canvas.GetTop(enemies[i])+ enemies[i].Height))
                    {
                        a = true;

                    }
                    ///if its not already above the bottom of the enemy we stop checking this bullet. This makes it so it does less calculations per bullet if its not close enough.
                    else
                    {
                        break;
                    }
                    /// We check if right corner or left corner is within the enemy's width.
                    if ((Canvas.GetLeft(playerBullets[ii]) + playerBullets[ii].Width) > Canvas.GetLeft(enemies[i]) && (Canvas.GetLeft(playerBullets[ii]) + playerBullets[ii].Width) < (Canvas.GetLeft(enemies[i]) + enemies[i].Width))
                    {
                        b = true;

                    }
                    if ((Canvas.GetLeft(playerBullets[ii])) < (Canvas.GetLeft(enemies[i]) + enemies[i].Width) && Canvas.GetLeft(playerBullets[ii]) > Canvas.GetLeft(enemies[i]))
                    {
                        b = true;

                    }

                    /// A = If the bullet is above the bottom of the enemy.
                    /// B = if one of the bottom corners is between Botleft and Botright of the enemy.
                    if (a && b)
                    {

                        //Replace this with EnemyTakeDamage(playerBullets[ii] ,enemies[i]) when not in demo mode.

                        playerBullets[ii].Fill = Brushes.Red;
                    }
                    //Remove this when not in demo mode
                    else
                    {
                        playerBullets[ii].Fill = Brushes.White;
                    }

                    //Remove this when not in demo mode
                }

            }
                
        }

        ///This removes the inserted bullet and executes any other code.
        private void PlayerTakeDamage(Rectangle bullet)
        {   
            ///Add damage &other code here
            //PlayerDamage()
            myCanvas.Children.Remove(bullet);
            return;
        }

        ///This removes the inserted bullet and does something with the inserted enemy.
        private void EnemyTakeDamage(Rectangle bullet, Rectangle Enemy)
        {
            ///Add damage &other code here
            //EnemyDamage(enemy)
            myCanvas.Children.Remove(bullet);
            return;
        }

        ///Movement keys.
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                P1moveLeft = true;
            if (e.Key == Key.Right)
                P1moveRight = true;

            //Remove when not in demo mode
            if (e.Key == Key.Down)
                P1moveDown = true;
            if (e.Key == Key.Up)
                P1moveUp = true;

            //Remove when not in demo mode

        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                P1moveLeft = false;
            if (e.Key == Key.Right)
                P1moveRight = false;


            //Remove when not in demo mode
            if (e.Key == Key.Down)
                P1moveDown = false;
            if (e.Key == Key.Up)
                P1moveUp = false;

            //Remove when not in demo mode
        }
    }
}
