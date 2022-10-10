﻿using System;
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

using System.Windows.Threading;

namespace arcadeGame
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        bool moveLeft, moveRight, moveLeft2, moveRight2;
        bool isPressed1 = false;
        bool isPressed2 = false;
        const int playerSpeed = 10;
        const int bulletSpeed = 10;
        private DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> itemsToRemove = new List<Rectangle>();
        List<Rectangle> playerBullets = new List<Rectangle>();
        ImageBrush playerSkin = new ImageBrush();
        ImageBrush player2Skin = new ImageBrush();

        public GameWindow()
        {
            InitializeComponent();

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Start();

            // player 1 skin
            playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/assets/strangerThings1.png"));
            player1.Fill = playerSkin;

            // player 2 skin
            player2Skin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/assets/strangerThings2.png"));
            player2.Fill = player2Skin;

            Canvas.Focus();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            //Player 1
            if (moveLeft == true && Canvas.GetLeft(player1) > 0)
            {
                Canvas.SetLeft(player1, Canvas.GetLeft(player1) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(player1) + 75 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player1, Canvas.GetLeft(player1) + playerSpeed);
            }
            // Player 2
            if (moveLeft2 == true && Canvas.GetLeft(player2) > 0)
            {
                Canvas.SetLeft(player2, Canvas.GetLeft(player2) - playerSpeed);
            }
            if (moveRight2 == true && Canvas.GetLeft(player2) + 75 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player2, Canvas.GetLeft(player2) + playerSpeed);
            }
            //player bullets logic
            //searches for all rectangles in Canvas
            foreach (Rectangle x in Canvas.Children.OfType<Rectangle>())
            {
                //filters rectangles with bullet1, bullet2 tags
                if ((string)x.Tag == "bullet1" || (string)x.Tag == "bullet2")
                {

                    Canvas.SetTop(x, Canvas.GetTop(x) - bulletSpeed);
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (Canvas.GetTop(x) < 10)
                        itemsToRemove.Add(x);
                }
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            //player 1 movement (left or right movement key is pressed)
            if (e.Key == Key.A)
            {
                moveLeft = true;
            }
            if (e.Key == Key.D)
            {
                moveRight = true;
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
            //bullet player1
            if (e.Key == Key.W && !isPressed1)
            {
                isPressed1 = true;
                //create bullet rectangle
                Rectangle bulletPlayer1 = new Rectangle
                {
                    Tag = "bullet1",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                };
                //adds player bullet to list
                playerBullets.Add(bulletPlayer1);
                //sets location of bullet above the player vertically
                Canvas.SetTop(bulletPlayer1, Canvas.GetTop(player1) - bulletPlayer1.Height);
                //sets location of bullet above the player horizontally
                Canvas.SetLeft(bulletPlayer1, Canvas.GetLeft(player1) + player1.Width / 2);
                //adds bullet to game
                Canvas.Children.Add(bulletPlayer1);
            }

            //bullet player2
            if (e.Key == Key.Up && !isPressed2)
            {
                isPressed2 = true;
                Rectangle bulletPlayer2 = new Rectangle
                {
                    Tag = "bullet2",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                };
                playerBullets.Add(bulletPlayer2);
                Canvas.SetTop(bulletPlayer2, Canvas.GetTop(player2) - bulletPlayer2.Height);
                Canvas.SetLeft(bulletPlayer2, Canvas.GetLeft(player2) + player2.Width / 2);
                Canvas.Children.Add(bulletPlayer2);
            }
        }



        private void KeyReleased(object sender, KeyEventArgs e)
        {
            //player 1 movement key is released
            if (e.Key == Key.A)
            {
                moveLeft = false;
            }
            if (e.Key == Key.D)
            {
                moveRight = false;
                //player 2 movement key is released
            }
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

