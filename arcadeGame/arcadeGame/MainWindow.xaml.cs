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

namespace arcadeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // Set the Health of the players (they share the same health)

        public int Health = 5;

        // when you click the button, you will lose one life
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Health -= 1;
            HealthShow.Content = "Health: " + Health;
            if (Health == 0)
            {
                GameOver gameOver = new GameOver();
                gameOver.Show();
                this.Close();
            }
        }

        // This button will bring you to the Gameover screen without haveing to lose your life. 
        private void InstaKill_Click(object sender, RoutedEventArgs e)
        {
            GameOver gameOver = new GameOver();
            gameOver.Show();
            this.Close();
        }
    }
}
