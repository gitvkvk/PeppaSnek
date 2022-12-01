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

namespace Snek
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //dictionary that maps grid values to image sources
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new() //omited the type
        {
            //if position empty
            {GridValue.Empty, Images.Empty },
            //if contains snake
            {GridValue.Snake, Images.Body },
            {GridValue.Food, Images.Food }
        };

        private readonly int rows = 15, cols=15;
        private readonly Image[,] gridImages; //2d image array for image controls
        private GameState gameState; //gamestate object to initialize in constructor
        private bool gameRunning; //false by default


 




        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetUpGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
        }
 

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //when user presses a key, window_previewkeydown is called and also window_keydown
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true; //prevents window_keydown from being called while overlay visible
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        //async game loop method
        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100); //adding a delay
                gameState.Move();
                Draw();
            }
        }
        

        //adding image controls to game grid, return as 2d array
        private Image[,] SetUpGrid()
        {
            Image[,] images = new Image[rows, cols];
            //set number of rows/columns on game grid
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;


            //loop over all grid positions
            for (int r=0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image() //instantiate new image each grid
                    {
                        Source = Images.Empty //initially source will be empty image asset
                    };

                    images[r, c] = image; // store image to 2d array
                    GameGrid.Children.Add(image); //add as a child of game grid
                }
            }
            return images;

        }
        private void Draw()
        {
            DrawGrid();
            ScoreText.Text = $"SCORE {gameState.Score}"; //updates score text
        }



        private void DrawGrid()
        {
            //look at grid array in gamestate and update images
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c]; //get grid value at current position
                    gridImages[r, c].Source = gridValToImage[gridVal]; //get source for corresponding image from dictionary
                }
            }
        }
    }
}
