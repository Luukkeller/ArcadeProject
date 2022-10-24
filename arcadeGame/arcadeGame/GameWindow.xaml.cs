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
    public partial class GameWindow : Window
    {
        ///Movement keys for player1.
        private bool moveLeft1, moveRight1, moveLeft2, moveRight2;

        private int enemyAttackRechargeCooldown = 10;
        private const int DefaultEnemyAttackRechargeCooldown = 10;
        private int enemyAttackTokens = 1;
        private const int maxEnemyAttackTokens = 5;
        private const int enemyChance = 10;
        /// Game Variables.
        private DispatcherTimer gameTimer = new DispatcherTimer();

        private bool isPressed1 = false;
        private bool isPressed2 = false;
        private bool gameOverOpen = false;
        private const int gameTick = 10;
        private const int playerSpeed = 10;
        private const int bulletSpeed = 10;
        private int enemyLeft = 0;

        private MediaPlayer mediaPlayer = new MediaPlayer();

        private ImageBrush player1Skin = new ImageBrush();
        private ImageBrush player2Skin = new ImageBrush();
        private string player1Name;
        private string player2Name;
        ///Martha: Added new brushes for the differend bullet sprites
        private ImageBrush bulletSkinPlayer = new ImageBrush();
        private ImageBrush bulletSkinBlue = new ImageBrush();
        private ImageBrush bulletSkinGreen = new ImageBrush();
        private ImageBrush bulletSkinYellow = new ImageBrush();
        ///Lists for both enemy bullets and player bullets. We need these to be able to loop over all the bullets in the scene.
        private List<Rectangle> enemyBullets = new List<Rectangle>();
        private List<Rectangle> playerBullets = new List<Rectangle>();
        private List<Rectangle> itemsToRemove = new List<Rectangle>();
        ///The List for enemies. This is required for enemy hit detection.
        private List<Rectangle> enemies = new List<Rectangle>();

        //strings for names game over screen
        public string name1 = "";
        public string name2 = "";

        //Dict of shield options and current variable of player shield per player.
        private Dictionary<int, string> shield = new Dictionary<int, string>();
        private int player1Shield = 1;
        private int player2Shield = 1;

        // Spawn enemy variables
        private Random rand = new Random();
        private int enemySpawnLimit = 200; //Defines the pace of making the enemys
        private int enemySpawnCounter = 0; //Timer for spawning of the enemies
        private const int enemyAmount = 9;
        private bool spawnRow1 = true;
        private bool spawnRow2 = true;
        private int enemyRow1 = 10;
        private int enemyRow2 = 60;
        private int enemyTop = 0;

        /// Martha: made three ints for player 1 score and player 2 score and player health.
        private int playerHealth = 3;
        private int player1Score = 0;
        private int player2Score = 0;




        public GameWindow()
        {
            //SoundPlayerAction soundPlayerAction = new SoundPlayerAction();
            //soundPlayerAction.Source = new Uri(@"../../assets/Enemyhit.mp3", UriKind.RelativeOrAbsolute);
            //EventTrigger eventTrigger = new EventTrigger(EnemyHitDetection); // this is the event you want to trigger the sound effect.
            //eventTrigger.Actions.Add(soundPlayerAction);

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
            shield.Add(1, "blue");
            shield.Add(2, "green");
            shield.Add(3, "yellow");

            ///Martha Adding the skin for the differend bullets
            bulletSkinPlayer.ImageSource = new BitmapImage(new Uri("pack://application:,,,/assets/glowstickWhite.png"));
            bulletSkinBlue.ImageSource = new BitmapImage(new Uri("pack://application:,,,/assets/glowstickBlue.png"));
            bulletSkinGreen.ImageSource = new BitmapImage(new Uri("pack://application:,,,/assets/glowstickGreen.png"));
            bulletSkinYellow.ImageSource = new BitmapImage(new Uri("pack://application:,,,/assets/glowstickYellow.png"));
            



        }

        private void GameEngine(object sender, EventArgs e)
        {
            Text5.Content = Player1.Tag;
            Text6.Content = Player2.Tag;


            switch (player1Shield) //Chooses sprite and fills sprite variable with color tag and fills temp colors
            {
                case 1:
                    Player1.Stroke = Brushes.Blue;
                    break;
                case 2:
                    Player1.Stroke = Brushes.Green;
                    break;
                case 3:
                    Player1.Stroke = Brushes.Yellow;
                    break;
            }
            switch (player2Shield) //Chooses sprite and fills sprite variable with color tag and fills temp colors
            {
                case 1:
                    Player2.Stroke = Brushes.Blue;
                    break;
                case 2:
                    Player2.Stroke = Brushes.Green;
                    break;
                case 3:
                    Player2.Stroke = Brushes.Yellow;
                    break;
            }



            PlayerMovement();
            PlayerHitDetection(Player1);
            PlayerHitDetection(Player2);
            EnemyHitDetection();
            BulletMovement();
            EnemyAttacks();

            ///Timer for enemy attack recharge
            if (enemyAttackRechargeCooldown > 0) //Makes sure it doesn't count negative
            {
                enemyAttackRechargeCooldown--; //Cuts 1 of the timer
            }
            else
            {
                ///adds one to the max attack tokens.
                enemyAttackRechargeCooldown = DefaultEnemyAttackRechargeCooldown;
                if (enemyAttackTokens != maxEnemyAttackTokens)
                    enemyAttackTokens++;
            }

            //player bullets logic
            //searches for all rectangles in Canvas
            foreach (Rectangle x in myCanvas.Children.OfType<Rectangle>())
            {
                //filters rectangles with bullet1, bullet2 tags
                if ((string)x.Tag == "bullet1" || (string)x.Tag == "bullet2")
                {

                    Canvas.SetTop(x, Canvas.GetTop(x) - bulletSpeed);
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

            if (enemySpawnCounter > 0) //Makes sure it doesn't count negative
            {
                enemySpawnCounter--; //Cuts 1 of the timer
            }

            if (enemySpawnCounter == 0) //This code is used when the timer is finished
            {

                foreach (Rectangle newEnemies in enemies) //Loops through the list enemies en does something with each rectangle
                {


                    if (Canvas.GetTop(newEnemies) == enemyRow1) //If there are no enemies left in row 1 then this stays true
                    {
                        spawnRow1 = false;
                    }
                    if (Canvas.GetTop(newEnemies) == enemyRow2) //If there are no enemies left in row 1 then this stays true
                    {
                        spawnRow2 = false;
                    }
                }
                enemyLeft = 0; //reset placement back to 0

                {
                    if (spawnRow1)
                    {
                        for (int i = 0; i < enemyAmount + 1; i++) //makes enemyamount enemys
                            MakeEnemies(1);
                    }
                }
                enemyLeft = 0; //reset placement back to 0

                {
                    if (spawnRow2)
                    {
                        for (int i = 0; i < enemyAmount; i++) //makes enemyamout enemys
                            MakeEnemies(2);
                    }
                }

                if (spawnRow1 || spawnRow2) //If row1 or row 2 is empty it resets spawnlimit
                {
                    enemySpawnCounter = enemySpawnLimit; //Resets the enemy counter back to the spawnlimit

                }

            }
        }


        private void BulletMovement()
        {
            for (int i = 0; i < enemyBullets.Count; i++)
            {
                ///Just loops over the bullets.
                Canvas.SetTop(enemyBullets[i], (Canvas.GetTop(enemyBullets[i]) + bulletSpeed / 2));
                if (Canvas.GetTop(enemyBullets[i]) > (Canvas.GetTop(Player1) + Player1.Height + enemyBullets[i].Height))
                {
                    myCanvas.Children.Remove(enemyBullets[i]);
                    enemyBullets.Remove(enemyBullets[i]);
                    return;
                }

            }
        }

        private void EnemyAttacks()
        {
            ///Pulls a random number to decide to attack or not.
            ///This basically decides the attack speed of the enemies.
            int temp = rand.Next(0, enemyChance);

            ///If we have an attack token, the list of enemies is not empty and we pull the right number we start the firing process.
            if (enemyAttackTokens > 0 && enemies.Count != 0 && temp == 1)
            {
                ///pulls a random enemy. 
                int selectedEnemy = rand.Next(0, (enemies.Count-1)); //Chooses random case
                Brush colour = Brushes.Red;

                ///Depending on the enemies colour we set the colour of the bullet to said colour.
                switch (enemies[selectedEnemy].Tag) //Chooses sprite and fills sprite variable with color tag and fills temp colors
                {
                    case "blue":
                        colour = bulletSkinBlue;
                        break;
                    case "green":
                        colour = bulletSkinGreen;
                        break;
                    case "yellow":
                        colour = bulletSkinYellow;
                        break;
                }

                ///Establishes the bullet.
                Rectangle bullet = new Rectangle
                {
                    ///Sets the tag to the enemy colour
                    Tag = enemies[selectedEnemy].Tag,
                    Height = 20,
                    Width = 10,
                    Fill = colour,
                };
                enemyBullets.Add(bullet);
                Canvas.SetTop(bullet, Canvas.GetTop(enemies[selectedEnemy]) + enemies[selectedEnemy].Height);
                Canvas.SetLeft(bullet, Canvas.GetLeft(enemies[selectedEnemy]) + enemies[selectedEnemy].Width / 2);
                myCanvas.Children.Add(bullet);
                enemyAttackTokens--;
            }


        }




        private void MakeEnemies(int row)
        {


            if (row == 1) //Defines the offset of enemies in row 1 for left and top
            {
                if (enemyLeft < 1)

                {
                    enemyLeft = 20;
                }
                enemyTop = enemyRow1;
            }
            if (row == 2) //Defines the offset of enemies in row 2 for left and top
            {
                if (enemyLeft < 1)
                {
                    enemyLeft = 60;

                }
                enemyTop = enemyRow2;
            }

            ImageBrush enemySprite = new ImageBrush(); //Show the enemy
            int enemySpriteCounter = rand.Next(0, 3); //Chooses random case
            string temp = ""; //Store's color tag
            {
                switch (enemySpriteCounter) //Chooses sprite and fills sprite variable with color tag and fills temp colors
                {
                    case 0:
                        enemySprite.ImageSource =
                            new BitmapImage(new Uri("pack://application:,,,/assets/enemy1.png"));
                        temp = "blue";
                        break;
                    case 1:
                        enemySprite.ImageSource =
                            new BitmapImage(new Uri("pack://application:,,,/assets/enemy2.png"));
                        temp = "green";
                        break;
                    case 2:
                        enemySprite.ImageSource =
                            new BitmapImage(new Uri("pack://application:,,,/assets/enemy3.png"));
                        temp = "yellow";
                        break;
                       

                }





                Rectangle newEnemy = new Rectangle //Create rectangle new enemy
                {
                    Tag = temp,
                    Height = 45,
                    Width = 45,
                    Fill = enemySprite
                };

                Canvas.SetTop(newEnemy, enemyTop); //Spawns enemy at this height
                Canvas.SetLeft(newEnemy, enemyLeft); //Spawns enemy at this left location
                myCanvas.Children.Add(newEnemy); //Create's the enemy on the canvas
                enemies.Add(newEnemy); //Adds enemy to the list
                enemyLeft += 80; //Adds 80 to offset left

                GC.Collect(); //Garbage collection
            }

        }



        ///Movement for player. Temporarily added up and down to hit the "Bullet" easier.
        ///It stays constrianed to the game window.
        private void PlayerMovement()
        {
            if (playerHealth > 0)
            {//Player 1 logic so it can't move outside the game window
                if (moveLeft1 == true && Canvas.GetLeft(Player1) > 0)
                {
                    Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) - playerSpeed);
                }
                if (moveRight1 == true && Canvas.GetLeft(Player1) + 75 < Application.Current.MainWindow.Width)
                {
                    Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) + playerSpeed);
                }
                // Player 2 logic so it can't move outside the game window
                if (moveLeft2 == true && Canvas.GetLeft(Player2) > 0)
                {
                    Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) - playerSpeed);
                }
                if (moveRight2 == true && Canvas.GetLeft(Player2) + 75 < Application.Current.MainWindow.Width)
                {
                    Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) + playerSpeed);
                }

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
                    PlayerTakeDamage(enemyBullets[i], 1, Player);
                }
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
                    if (Canvas.GetTop(playerBullets[ii]) < (Canvas.GetTop(enemies[i]) + enemies[i].Height))
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
        private void PlayerTakeDamage(Rectangle bullet, int damage, Rectangle player)
        {
            ///Martha: when player takes damage Health goes down by 1.
            ///Once playerHealth reaches 0, it will show you Game over.
            ///

            ///Puts the temp value on the shield of player taking potential damage.
            int temp = 0;
            if (player.Name.ToString() == "Player1")
                temp = player1Shield;
            if (player.Name.ToString() == "Player2")
                temp = player2Shield;

            ///If the shield doesnt match the bullet take damage. IF it does skip.
            if (shield[temp] != bullet.Tag.ToString())
            {
                playerHealth -= damage;
                healthShow.Content = "Health: " + playerHealth;


            }
            if (playerHealth <= 0 && !gameOverOpen) //gameOverOpen is set to false and will change to true when this command is executed causing it to only run once
            {
                GameOverScreen ScoreData = new GameOverScreen();
                //call function to add score to database
                ScoreData.Show();
                ScoreData.player1name = Player1.Tag.ToString();
                ScoreData.player2name = Player2.Tag.ToString();

                AddHighscoreToDatabase(player1Score, Player1.Tag.ToString());
                AddHighscoreToDatabase(player2Score, Player2.Tag.ToString());
                ScoreData.TextScore1.Text = "Score " + Player1.Tag.ToString() + ": " + player1Score.ToString();
                ScoreData.TextScore2.Text = "Score " + Player2.Tag.ToString() + ": " + player2Score.ToString();
                // close game window when at 0 health and open game over window

                
                this.Close(); //close game window on game over
                mediaPlayer.Stop(); //stop game theme on game over
                gameOverOpen = true;
            }

            enemyBullets.Remove(bullet);
            myCanvas.Children.Remove(bullet);
            return;
        }

        ///This removes the inserted bullet and does something with the inserted enemy.
        private void EnemyTakeDamage(Rectangle bullet, Rectangle Enemy)
        {
            ///Martha: First it looks at the tag for each of the enemies to assign a temp value for the score
            ///Looking which bullet had which tag and runs the code depending on which bullet tag hits.
            ///Added else if for the enemy Temp yellow (temporary value to keep the enemy scores) and the player bullet2

            int temp = 0;
            if (Enemy.Tag.ToString() == "blue")
            {
                temp = 10;
            }
            else if (Enemy.Tag.ToString() == "green")
            {
                temp = 20;
            }
            else if (Enemy.Tag.ToString() == "yellow")
            {
                temp = 50;
            }

            if (bullet.Tag.ToString() == "bullet1")
            {
                player1Score += temp;
                scorePlayer1.Content = "Player 1: " + player1Score;
            }
            else if (bullet.Tag.ToString() == "bullet2")
            {

                player2Score += temp;
                scorePlayer2.Content = "Player 2: " + player2Score;
            }
            myCanvas.Children.Remove(bullet);
            myCanvas.Children.Remove(Enemy);
            enemies.Remove(Enemy);
            playerBullets.Remove(bullet);
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


            ///Shield keys
            ///Player 2
            if (e.Key == Key.Down)
            {
                if (player2Shield == 3)
                {
                    player2Shield -= 2;
                }
                else
                {
                    player2Shield++;
                }
            }
            ///Player 1
            if (e.Key == Key.S)
            {
                if (player1Shield == 3)
                {
                    player1Shield -= 2;
                }
                else
                {
                    player1Shield++;
                }
            }



            //bullet Player1
            if (e.Key == Key.W && !isPressed1)
            {
                isPressed1 = true; //checks if button is pressed so it doesn't spam bullets
                //create bullet rectangle
                Rectangle bulletPlayer1 = new Rectangle
                {
                    Tag = "bullet1",
                    Height = 20,
                    Width = 10,
                    Fill = bulletSkinPlayer,
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
                isPressed2 = true; //checks if button is pressed so it doesn't spam bullets
                Rectangle bulletPlayer2 = new Rectangle
                {
                    Tag = "bullet2",
                    Height = 20,
                    Width = 10,
                    Fill = bulletSkinPlayer,
                };
                playerBullets.Add(bulletPlayer2);
                Canvas.SetTop(bulletPlayer2, Canvas.GetTop(Player2) - bulletPlayer2.Height);
                Canvas.SetLeft(bulletPlayer2, Canvas.GetLeft(Player2) + Player2.Width / 2);
                myCanvas.Children.Add(bulletPlayer2);
            }


            ///Martha: when you press the Key R on the keyboard it will loop PlayerTakeDamage 3 times and make shows "Game Over"
            if (e.Key == Key.R)
            {
                PlayerTakeDamage(Player1, 3, Player1);
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

        //function for adding player score and name to database
        private void AddHighscoreToDatabase(int highscore, string playername)
        {
            //string that stores local DB location
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\luukk\\source\\repos\\ArcadeProject\\arcadeGame\\arcadeGame\\database\\GameDataBase.mdf\";Integrated Security=True";
            //string that stores t-sql query
            string query = "INSERT INTO [Highscores] ([Highscore],[Player],[Date]) VALUES ('" + highscore + "','" + playername + "','Vandaag')";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query);


            try
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
            catch (Exception)
            {
                connection.Close();
            }

        }

    }
}
