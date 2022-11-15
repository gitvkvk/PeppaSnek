using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snek
{
    internal class GameState
    {
        public int Rows { get;  }
        public int Cols { get;  }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }


        //  linked list containing positions currently occupied by snake (allows to add/delete from both ends of list), first element head, last element tail
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();

        // used to figure out where food should spawn
        private readonly Random random = new Random();

        //constructor, taking in number of rows/col in grid as parameters. First store the params into the properties
        //initialize 2D array with correct size
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            // every position in array will contain gridvalue.empty because it is the first enum value. 
            Grid = new GridValue[rows, cols];
            //want snakes direction to be right when game starts
            Dir = Direction.Right;

            AddSnake();
            AddFood();

        }
        //method that adds snake to the grid
        //want it to appear in the middle row, in colums 1,2,3
        private void AddSnake()
        {
            //first create variable for middle row
            int r = Rows / 2;

            //loop over first three columns
            for (int c=1 ; c<=3 ; c++)
            {
                Grid[r, c] = GridValue.Snake;
                //add to snake positions list
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        //adding food
        //method that adds all empty grid positions

        private IEnumerable<Position> EmptyPositions()
        {
            //loop through all rows and columns
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    //check if grid at r,c is empty
                    if (Grid[r,c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            //make a list of empty positions
            List<Position> empty = new List<Position>(EmptyPositions());
            //if no empty positions (they beat the game)
            if (empty.Count == 0)
            {
                return;
            }
            //pick an empty position at random
            Position pos = empty[random.Next(empty.Count)];
            //set corresponding array entry to gridvalue.food
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        //return postion of snek head
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        //return position of snek tail
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        //return all snek positions as ienumerable
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        //methods for modifying snek
        private void AddHead(Position pos)
        {
            //adds given position to the front of snek, making it the new head
            snakePositions.AddFirst(pos);
            //set corresponding value of grid array
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }
        //removing tail
        private void RemoveTail()
        {
            //get current tail position
            Position tail = snakePositions.Last.Value;
            //make the position empty in grid
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            //remove from linked list
            snakePositions.RemoveLast();

        }

        //public methods for gamestate
        public void ChangeDirection(Direction dir)
        {
            //set direction property to the direction parameter
            Dir = dir;
        }

        //way to move the snake
        //normal: snake head moves 1 unit in current direction, subtract tail
        //food condition: tail is not subtracted
        //boundary collision condition
        //tail collision condition

        //method to check if given position is outside grid
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols; 
        }

        //method takes position as parameter, returns what snake would hit if it moved there
        private GridValue WillHit(Position newHeadPos)
        {
            // if new head pos outside grid
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }

            //check if new head position is same as current tail position (no collision)
            if (newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }

            //general case, return what is stored in the grid at position
            return Grid[newHeadPos.Row, newHeadPos.Col]; 
        }

        //public move method, to move snake 1 step in current direction
        public void Move()
        {
            //first get new head position
            Position newHeadPos = HeadPosition().Translate(Dir); //what does translate do

            //check what head will hit
            GridValue hit = WillHit(newHeadPos);
            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            //if snake moving into empty position
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            //if hits position with food
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }



        }
    }
}
