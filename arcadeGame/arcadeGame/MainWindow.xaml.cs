using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        /// <summary>
        /// Set the Health of the players (they share the same health) and have a variable for damage of player.
        /// </summary>
        public int health = 5;
        public MainWindow()
        {
            InitializeComponent();
            this.Focus();
        }

        /// <summary>
        /// this Method will make you lose one life. Once Health reaches 0 you go to game over.
        /// </summary>
        public void takeDamage(int playerDamage)
        {
            health -= playerDamage;
            healthShow.Content = "Health: " + health;
            if (health <= 0)
            {
                GameOver gameOver = new GameOver();
                gameOver.Show();
                this.Close();
            }
        }


        /// when you click the button, you will lose one life
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            takeDamage(1);
        }

        /// when pressing R. you will go to the Gameover screen without losing your life one by one. 
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.R)
            {
                takeDamage(5);
            }
        }
    }
}
