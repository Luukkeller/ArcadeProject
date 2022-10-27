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

    public partial class MainWindow : Window
    {
        ///Strings for the names
        public string name1 = "";
        public string name2 = "";

        


        bool x =true;

        private DispatcherTimer gameTimer = new DispatcherTimer();
        private const int gameTick = 10;


        NameInput ni;
        public MainWindow()
        {


            ///<summary>
            ///We run a "game engine" so the names on the main window get updated constantly
            /// </summary>
            InitializeComponent();
            //We make a new NameInput window instance and give it the mainwindow so we can get the names back.
            ni = new NameInput(this);

            
            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Tick += GameEngine;
            gameTimer.Start();

            //Defines controls on main window
            Controls1.Text = "Controls: \n W = Shoot \n S = Shield \n A = Left \n D = Right";
            Controls2.Text = "Controls: \n Up = Shoot \n Down = Shield \n Left = Left \n Right = Right";


        }


        /// <summary>
        /// set all the details for the Main Menu
        /// </summary>
        /// <param name="sender">the object that reference to the called event</param>
        /// <param name="e">More information of the object</param>
        private void GameEngine(object sender, EventArgs e)
        {
            ///Displays the names of the players on the main window.
            Text1.Content = "Player 1: " + name1;
            Text2.Content = "Player 2: " + name2;

            ///This checks if the names have been entered and if not we will ask the players for their names.
            if (name1 == "")
            {
                ///hides the main window so the name input window is clearly visible.
                this.Visibility = Visibility.Collapsed;

                ///Changes the text on the input window
                ni.InputText.Content = "Input name of player 1";
                ni.Activate();
                ni.Visibility = Visibility.Visible;
                ///sets the input window on 1 so it changes the name of player 1
                ni.i = 1;
            }
            ///Checks if the name of player 2 hasnt been filled and if so will ask the players to fill in for player 2
            else if(name1 != "" && name2 == "" )
            {
                this.Visibility = Visibility.Collapsed;
                ni.i = 2;
                ni.InputText.Content = "Input name of player 2";
                ni.Activate();
                ni.Visibility = Visibility.Visible;
            }
            ///Makes the main window visible again
            else if (x)
            {
                this.Visibility = Visibility.Visible;
                x = false;
            }
            
        }

        ///<summary>
        /// Starts the game when clicked
        /// </summary>
        /// <param name="sender">the object that reference to the called event</param>
        /// <param name="e">More information of the object</param>
        private void StartClick(object sender, RoutedEventArgs e)
        {


            GameWindow gw = new GameWindow();
            ///Sets the tags of player 1&2 to their respective names
            gw.Player1.Tag = name1;
            gw.Player2.Tag = name2;

            gw.Visibility = Visibility.Visible;
            this.Close();
        }

        /// <summary>
        /// Stops the game by clicking quit
        /// </summary>
        /// <param name="sender">the object that reference to the called event</param>
        /// <param name="e">More information of the object</param>
        private void QuitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Displays the High Score window and closes this window
        /// </summary>
        /// <param name="sender">the object that reference to the called event</param>
        /// <param name="e">More information of the object</param>
        private void MainMenuHS(object sender, RoutedEventArgs e)
        {
            HighscoreWindow mainmenuHS = new HighscoreWindow();
            mainmenuHS.Visibility = Visibility.Visible;
            this.Close();
        }
    }
}
