using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using Microsoft.Win32;



namespace arcadeGame
{
    public partial class GameWindow : Window
    {
        ///Movement keys for player1.
        bool moveLeft1, moveRight1, moveLeft2, moveRight2;




        /// Game Variables.
        private DispatcherTimer gameTimer = new DispatcherTimer();

        bool isPressed1 = false;
        bool isPressed2 = false;
        private const int gameTick = 10;
        private const int playerSpeed = 10;
        int enemyLeft = 0;
        const int bulletSpeed = 10;

        // Spawn enemy variables
        private Random rand = new Random();
        private int enemySpawnLimit = 100; // Bepaald de snelheid van het aanmaken van enemies
        private int enemySpawnCounter = 0; //Timer voor het spawnen van de enemies
        private const int enemySpeed = 10; //Snelheid van de enemies
        private const int enemyAmount = 9;
        bool spawnRow1 = true;
        bool spawnRow2 = true;
        int enemyRow1 = 10;
        int enemyRow2 = 60;
        int enemyTop = 0;



        private MediaPlayer mediaPlayer = new MediaPlayer();

        ImageBrush player1Skin = new ImageBrush();
        ImageBrush player2Skin = new ImageBrush();
        ///Lists for both enemy bullets and player bullets. We need these to be able to loop over all the bullets in the scene.
        private List<Rectangle> enemyBullets = new List<Rectangle>();
        private List<Rectangle> playerBullets = new List<Rectangle>();
        private List<Rectangle> itemsToRemove = new List<Rectangle>();

        //List for spawnEnemies
        private List<Rectangle> spawnEnemies = new List<Rectangle>();

        ///The List for enemies. This is required for enemy hit detection.
        private List<Rectangle> enemies = new List<Rectangle>();





        public GameWindow()
        {
            InitializeComponent();
            mediaPlayer.Open(new Uri(@"../../assets/music.mp3", UriKind.RelativeOrAbsolute));
            mediaPlayer.Play();
            mediaPlayer.Volume = 0.1f;


            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Tick += GameEngine;
            gameTimer.Start();

            myCanvas.Focus();

            // player 1 skin
            player1Skin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/assets/strangerThings1.png"));
            Player1.Fill = player1Skin;

            // player 2 skin
            player2Skin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/assets/strangerThings2.png"));
            Player2.Fill = player2Skin;

            Rect playerHitBox = new Rect(Canvas.GetLeft(Player1), Canvas.GetTop(Player1), Player1.Width, Player1.Height);



            
            enemyLeft = 0;
            //for(int i = 0; i < 8; i++)

            //{
            //    Rectangle newEnemy = new Rectangle
            //    {
            //        Tag = "enemy",
            //        Height = 45,
            //        Width = 45,
            //        Fill = Brushes.Blue
            //    };

            //    enemies.Add(newEnemy);
            //    Canvas.SetTop(newEnemy, 10);
            //    Canvas.SetLeft(newEnemy, x);
            //    myCanvas.Children.Add(newEnemy);
            //    x += 80;

            //}



        }

        private void makeEnemies(int row)
        {
            

            if (row == 1)
            {
                if (enemyLeft < 1)

                {
                    enemyLeft = 20;
                }
                enemyTop = enemyRow1;
            }
            if (row == 2 )
            {
                if(enemyLeft < 1)
                {
                    enemyLeft = 60;

                }
                enemyTop = enemyRow2;
            }

            ImageBrush enemySprite = new ImageBrush();
            int enemySpriteCounter = rand.Next(1, 4);
            string temp = "";
            {
                switch (enemySpriteCounter)
                {
                    case 1:
                        enemySprite.ImageSource =
                            new BitmapImage(new Uri("pack://application:,,,/assets/invader1.gif"));
                        temp = "red";
                        break;
                    case 2:
                        enemySprite.ImageSource =
                            new BitmapImage(new Uri("pack://application:,,,/assets/invader2.gif"));
                        temp = "green";
                        break;
                    case 3:
                        enemySprite.ImageSource =
                            new BitmapImage(new Uri("pack://application:,,,/assets/invader3.gif"));
                        temp = "blue";
                        break;
                }





                Rectangle newEnemy = new Rectangle
                {
                    Tag = temp,
                    Height = 45,
                    Width = 45,
                    Fill = enemySprite
                };

                Canvas.SetTop(newEnemy, enemyTop);
                Canvas.SetLeft(newEnemy, enemyLeft);
                myCanvas.Children.Add(newEnemy);
                enemies.Add(newEnemy);
                enemyLeft += 80;

                GC.Collect();
            }
            
        }

        private void GameEngine(object sender, EventArgs e)
        {
            //Coordinate Display for player1.
            //Can be removed

            Text1.Content = "TopLeft " + Canvas.GetTop(Player1) + "," + Canvas.GetLeft(Player1);
            Text2.Content = "TopRight " + Canvas.GetTop(Player1) + "," + (Canvas.GetLeft(Player1)+Player1.Width);
            Text3.Content = "BotLeft " + (Canvas.GetTop(Player1) + Player1.Height) + "," + Canvas.GetLeft(Player1);
            Text4.Content = "BotRight " + (Canvas.GetTop(Player1) + Player1.Height) + "," + (Canvas.GetLeft(Player1) + Player1.Width);

            Text5.Content = Player1.Tag;
            Text6.Content = Player2.Tag;



            PlayerMovement();
            PlayerHitDetection(Player1);
            PlayerHitDetection(Player2);
            EnemyHitDetection();

            //player bullets logic
            //searches for all rectangles in Canvas
            foreach (Rectangle x in myCanvas.Children.OfType<Rectangle>())
            {
                //filters rectangles with bullet1, bullet2 tags
                if ((string)x.Tag == "bullet1" || (string)x.Tag == "bullet2")
                {

                    Canvas.SetTop(x, Canvas.GetTop(x) - bulletSpeed);
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (Canvas.GetTop(x) < 10)
                    {
                        playerBullets.Remove(x);
                        myCanvas.Children.Remove(x);
                        return;
                    }

                }
            }

            spawnRow1 = true;
            spawnRow2 = true;
            
            if (enemySpawnCounter > 0) //Zorgt ervoor dat er niet negatief wordt doorgeteld.
            {
                enemySpawnCounter--;
            }

            if (enemySpawnCounter == 0) //Zorgt ervoor dat er een maximum enemies wordt gespawned.
            {

                foreach (Rectangle newEnemies in enemies)
                {


                    if (Canvas.GetTop(newEnemies) == enemyRow1)
                    {
                        spawnRow1 = false;
                    }
                    if (Canvas.GetTop(newEnemies) == enemyRow2)
                    {
                        spawnRow2 = false;
                    }
                }
                enemyLeft = 0;
                for (int i = 0; i < enemyAmount+1; i++)
                {
                    if (spawnRow1)
                    {
                        makeEnemies(1);
                    }
                }
                enemyLeft = 0;
                for (int i = 0; i < enemyAmount; i++)
                {
                    if (spawnRow2)
                    {
                        makeEnemies(2);
                    }
                }

                if (spawnRow1 || spawnRow2)
                {
                    enemySpawnCounter = enemySpawnLimit; //reset de enemy counter naar de limit int
                   
                }


            }
        }



        ///Movement for player. Temporarily added up and down to hit the "Bullet" easier.
        ///It stays constrianed to the game window.
        private void PlayerMovement()
        {
            //Player 1
            if (moveLeft1 == true && Canvas.GetLeft(Player1) > 0)
            {
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) - playerSpeed);
            }
            if (moveRight1 == true && Canvas.GetLeft(Player1) + 75 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) + playerSpeed);
            }
            // Player 2
            if (moveLeft2 == true && Canvas.GetLeft(Player2) > 0)
            {
                Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) - playerSpeed);
            }
            if (moveRight2 == true && Canvas.GetLeft(Player2) + 75 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) + playerSpeed);
            }
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
                    if (Canvas.GetTop(playerBullets[ii]) < (Canvas.GetTop(enemies[i])+ enemies[i].Height))
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
                        EnemyTakeDamage(playerBullets[ii], enemies[i]);

                         a = false;
                         b = false;
                        return;
                    }
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
            playerBullets.Remove(bullet);
            myCanvas.Children.Remove(bullet);
            enemies.Remove(Enemy);
            myCanvas.Children.Remove(Enemy);
            return;
        }

        ///Movement keys.
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //player 1 movement (left or right movement key is pressed)
            if (e.Key == Key.A)
            {
                moveLeft1 = true;
            }
            if (e.Key == Key.D)
            {
                moveRight1 = true;
            }
            //player 2 movement (A or D movement key is pressed)
            if (e.Key == Key.Left)
            {
                moveLeft2 = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight2 = true;
            }
            //bullet Player1
            if (e.Key == Key.W && !isPressed1)
            {
                isPressed1 = true;
                //create bullet rectangle
                Rectangle bulletPlayer1 = new Rectangle
                {
                    Tag = "bullet1",
                    Height = 20,
                    Width = 10,
                    Fill = Brushes.White,
                };
                //adds player bullet to list
                playerBullets.Add(bulletPlayer1);
                //sets location of bullet above the player vertically
                Canvas.SetTop(bulletPlayer1, Canvas.GetTop(Player1) - bulletPlayer1.Height);
                //sets location of bullet above the player horizontally
                Canvas.SetLeft(bulletPlayer1, Canvas.GetLeft(Player1) + Player1.Width / 2);
                //adds bullet to game
                myCanvas.Children.Add(bulletPlayer1);

                //bullet Player2
                

            }

            if (e.Key == Key.Up && !isPressed2)
            {
                isPressed2 = true;
                Rectangle bulletPlayer2 = new Rectangle
                {
                    Tag = "bullet2",
                    Height = 20,
                    Width = 10,
                    Fill = Brushes.White,
                };
                playerBullets.Add(bulletPlayer2);
                Canvas.SetTop(bulletPlayer2, Canvas.GetTop(Player2) - bulletPlayer2.Height);
                Canvas.SetLeft(bulletPlayer2, Canvas.GetLeft(Player2) + Player2.Width / 2);
                myCanvas.Children.Add(bulletPlayer2);
            }
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            //player 1 movement key is released
            if (e.Key == Key.A)
            {
                moveLeft1 = false;
            }
            if (e.Key == Key.D)
            {
                moveRight1 = false;
            }
            //player 2 movement key is released
            if (e.Key == Key.Left)
            {
                moveLeft2 = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight2 = false;
            }
            if (isPressed1)
                isPressed1 = false;

            if (isPressed2)
                isPressed2 = false;
        }
    }
}
