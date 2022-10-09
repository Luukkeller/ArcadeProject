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
using System.Windows.Threading;

namespace arcadeGame
{
    /// <summary>
    /// Interaction logic for NameInput.xaml
    /// </summary>
    public partial class NameInput : Window
    {

        private DispatcherTimer gameTimer = new DispatcherTimer();
        private const int gameTick = 10;

        public string InputRequest;
        public int i = 1;

        bool x = false;


        MainWindow mainWindow;
        public NameInput(MainWindow mw )
        {
            InitializeComponent();
            Input1.Text = "name";
            Input1.Focus();

            mainWindow = mw;

            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Tick += GameEngine;
            gameTimer.Start();

            
        }



        private void GameEngine(object sender, EventArgs e)
        {
            //Once we click the names will be set for the specific player (i).
            if (x)
            {
                if (i == 1)
                {
                    mainWindow.name1 = Input1.Text.ToString();
                    Input1.Text = "name";
                    x = false;
                }

                if (i == 2)
                {
                    mainWindow.name2 = Input1.Text;
                    this.Close();

                }
            }

        }

       
        private void DoneClick(object sender, RoutedEventArgs e)
        {
            x = true;
        }
    }
}
