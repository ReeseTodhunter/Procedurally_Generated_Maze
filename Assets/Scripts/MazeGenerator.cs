using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This maze generator was originally made using the tutorial found here: https://www.youtube.com/watch?v=ya1HyptE5uc
//I've since rewritten the code myself multiple times using the backtracking method to generate a maze
//I've also added a perlin noise generator following this video here: https://www.youtube.com/watch?v=BO7x58NwGaU
//To add obstacles to the generated maze to further expand on the maze generator

public class MazeGenerator : MonoBehaviour
{
    [Range(10, 100)]
    public int width = 10;
    [Range(10, 100)]
    public int height = 10;
    public bool useSeed = false;
    public int seed = 0;
    public bool usePerlinNoise = false;
    [Range(0.5f, 1.0f)]
    public float perlinTolerance = 0.7f;
    [Range(1, 10)]
    public float perlinScale = 5;
    public bool randomiseOffset = false;
    public Vector2 perlinOffset = new Vector2(0, 0);

    [SerializeField]
    private GameObject wall;
    [SerializeField]
    private GameObject obstacle;
    [SerializeField]
    private GameObject camera;

    private Cell[,] maze;

    [Range(10,100)]
    public float camHeight = 50f;

    //Store Vectors directions for easy use
    public static Vector4 right = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
    public static Vector4 left = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
    public static Vector4 up = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
    public static Vector4 down = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

    private void Start()
    {
        maze = UpdateMaze(width, height, wall, obstacle, useSeed, seed, usePerlinNoise, perlinTolerance, perlinScale, randomiseOffset, perlinOffset.x, perlinOffset.y);
        camera.transform.position = new Vector3((maze.GetLength(0) / 2), camHeight, (maze.GetLength(1) / 2));
    }

    private void Update()
    {
        camera.transform.position = new Vector3((maze.GetLength(0) / 2), camHeight, (maze.GetLength(1) / 2));
    }

    public void UpdateAction()
    {
        if (maze != null)
        {
            //Clear any existing maze objects before visualising a new maze
            ClearMaze(maze);
        }
        maze = UpdateMaze(width, height, wall, obstacle, useSeed, seed, usePerlinNoise, perlinTolerance, perlinScale, randomiseOffset, perlinOffset.x, perlinOffset.y);
    }

    public Cell[,] UpdateMaze(int i_width, int i_height, GameObject i_wall, GameObject i_obstacle, bool i_useSeed = false, int i_seed = 0, bool i_usePerlin = false, float i_tolerance = 0.7f, float i_scale = 5, bool i_randomOffset = false, float i_xOffset = 0, float i_yOffset = 0)
    {
        Cell[,] maze = GenerateGrid(i_width, i_height);
        maze = GenerateMaze(maze, i_useSeed, i_seed, i_usePerlin, i_tolerance, i_scale, i_randomOffset, i_xOffset, i_yOffset);
        maze = VisualiseMaze(maze, i_wall, i_obstacle);

        return maze;
    }

    public Cell[,] GenerateGrid(int i_width, int i_height)
    {
        Cell[,] grid = new Cell[i_width, i_height]; //Create a new grid of cells of the selected width and height
        int cellID = 0;

        for (int y = 0; y < i_height; y++)
        {
            for (int x = 0; x < i_width; x++)
            {
                //Set a cell ID equal to how many cells have been created
                cellID += 1;
                //Create a new Cell with a cellID and the current position in the graph
                grid[x, y] = new Cell(cellID, new Vector2(x, y));
            }
        }

        return grid;
    }

    public Cell[,] GenerateMaze(Cell[,] i_maze, bool i_useSeed, int i_seed, bool i_usePerlin, float i_tolerance, float i_scale, bool i_randomOffset, float i_xOffset, float i_yOffset)
    {
        //Setup a new random value generator
        System.Random rand = new System.Random();
        //If there is a useSeed is true use a seeded the random
        if (i_useSeed)
        {
            rand = new System.Random(i_seed);
        }

        //If usePerlin is true generate a Perlin Noise to shape the maze terrain
        if (i_usePerlin)
        {
            //If randomOffset is true generate a random offset position for the perlin noise
            if (i_randomOffset)
            {
                i_xOffset = rand.Next(0, 100);
                i_yOffset = rand.Next(0, 100);
            }

            //Generate a 2D array of perlin noise
            float[,] noise = GeneratePerlinNoise(i_maze, i_scale, i_xOffset, i_yOffset);

            //Working through the array of noise if a point in the array has a noise value higher than the tolerance set the cell to be already visited
            for (int y = 0; y < i_maze.GetLength(1); y++)
            {
                for (int x = 0; x < i_maze.GetLength(0); x++)
                {
                    if (noise[x, y] > i_tolerance)
                    {
                        //Set the cell to be already visited so that when backtracker is run the cell is not visited
                        i_maze[x, y].visited = true;
                        //Set the cell to be a nullCell so that the visualiser instantiates a cube instead of walls
                        i_maze[x, y].nullCell = true;
                    }
                }
            }
        }

        //Create the maze using a backtracker
        i_maze = Backtracker(i_maze, rand);

        //Return the maze once generated
        return i_maze;
    }

    public Cell[,] VisualiseMaze(Cell[,] i_maze, GameObject i_wall, GameObject i_obstacle)
    {
        //Go through every Cell
        for (int y = 0; y < i_maze.GetLength(1); y++)
        {
            for (int x = 0; x < i_maze.GetLength(0); x++)
            {
                if (!i_maze[x, y].nullCell)
                {
                    if (i_maze[x, y].activeWalls.x == 1)    //Right Wall
                    {
                        //Instantiate the maze wall and add to the list of walls sorted in the cell
                        i_maze[x, y].walls.Add(Instantiate(i_wall, new Vector3(x + 0.5f, 0, y), Quaternion.Euler(0, 90, 0)));
                    }
                    if (i_maze[x, y].activeWalls.y == 1)    //Left Wall
                    {
                        //Instantiate the maze wall and add to the list of walls sorted in the cell
                        i_maze[x, y].walls.Add(Instantiate(i_wall, new Vector3(x - 0.5f, 0, y), Quaternion.Euler(0, 90, 0)));
                    }
                    if (i_maze[x, y].activeWalls.z == 1)    //Top Wall
                    {
                        //Instantiate the maze wall and add to the list of walls sorted in the cell
                        i_maze[x, y].walls.Add(Instantiate(i_wall, new Vector3(x, 0, y + 0.5f), Quaternion.Euler(0, 0, 0)));
                    }
                    if (i_maze[x, y].activeWalls.w == 1)    //Bottom Wall
                    {
                        //Instantiate the maze wall and add to the list of walls sorted in the cell
                        i_maze[x, y].walls.Add(Instantiate(i_wall, new Vector3(x, 0, y - 0.5f), Quaternion.Euler(0, 0, 0)));
                    }
                }
                else
                {
                    i_maze[x, y].walls.Add(Instantiate(i_obstacle, new Vector3(x, 0, y), Quaternion.Euler(0,0,0)));
                }
            }
        }
        return i_maze;
    }

    public Cell[,] ClearMaze(Cell[,] i_maze)
    {
        for (int y = 0; y < i_maze.GetLength(1); y++)
        {
            for (int x = 0; x < i_maze.GetLength(0); x++)
            {
                foreach (var wall in i_maze[x, y].walls)
                {
                    Destroy(wall);
                }
            }
        }
        return i_maze;
    }

    public float[,] GeneratePerlinNoise(Cell[,] i_maze, float i_scale, float i_xOffset, float i_yOffset)
    {
        //Perlin Noise made using https://www.youtube.com/watch?v=BO7x58NwGaU

        //Setup a 2D array to store the Perlin noise in
        float[,] noise = new float[i_maze.GetLength(0), i_maze.GetLength(1)];

        for (int x = 0; x < i_maze.GetLength(0); x++)
        {
            for (int y = 0; y < i_maze.GetLength(1); y++)
            {
                //Normalise and scale the perlin noise coordinates
                float XPerlin = (((float)x / i_maze.GetLength(0)) * i_scale) + i_xOffset;
                float YPerlin = (((float)y / i_maze.GetLength(1)) * i_scale) + i_yOffset;
                //Use the perlin noise coordinates to generate a perlinNoise value storing the value in the noise array
                noise[x,y] = Mathf.PerlinNoise(XPerlin, YPerlin);
            } 
        }
        return noise;
    }

    public Cell[,] Backtracker(Cell[,] i_maze, System.Random i_rand, Cell i_startCell = null)
    {
        Vector2 currentPosition;

        //If there is no startCell
        if (i_startCell == null)
        {
            do
            {
                //Get a random starting position within the range of the maze
                currentPosition = new Vector2(i_rand.Next(0, i_maze.GetLength(0)), i_rand.Next(0, i_maze.GetLength(1)));
                //Make sure the maze's starting point is not a nullCell
            } while (i_maze[(int)currentPosition.x, (int)currentPosition.y].nullCell);
        }
        else
        {
            //If there is a startCell set the current position to the startCell's position
            currentPosition = i_startCell.position;
        }

        //Set the starting Cell as Visited
        i_maze[(int)currentPosition.x, (int)currentPosition.y].visited = true;

        //Setup Backtracking Stack
        Stack<Vector2> positionStack = new Stack<Vector2>();

        //Setup a list of deadends
        List<Deadend> deadends = new List<Deadend>();

        //Add the starting cell to the stack
        positionStack.Push(currentPosition);

        //Contains a bool value for if the backtracker is going back through previous nodes
        bool reversing = false;

        while (positionStack.Count > 0)
        {
            //Set the current position equal to the top of the stack
            currentPosition = positionStack.Pop();

            //Get the neighbouring cells to visit
            List<Neighbour> unvisitedNeighbouringCells = CheckForUnvisited(i_maze, currentPosition);

            if (unvisitedNeighbouringCells.Count > 0)
            {
                //If there is an available neighbour push the current position back into the stack
                positionStack.Push(currentPosition);

                //Get a random neighbour in the unvisited cells
                Neighbour nextCell = unvisitedNeighbouringCells[i_rand.Next(0, unvisitedNeighbouringCells.Count)];

                //Remove the shared wall from the active walls of both cells
                i_maze[(int)currentPosition.x, (int)currentPosition.y].activeWalls -= nextCell.sharedWall;
                i_maze[(int)nextCell.cell.position.x, (int)nextCell.cell.position.y].activeWalls -= OppositeWall(nextCell.sharedWall);

                //Set the next Cell as visited
                nextCell.cell.visited = true;

                //Add the next Cell to the stack
                positionStack.Push(nextCell.cell.position);

                //As an unvisited cell has been found set reversing to false
                reversing = false;
            }
            else if ((unvisitedNeighbouringCells.Count <= 0) && (!reversing))
            {
                //If there are no unvisited 
                deadends.Add(new Deadend()
                {
                    cell = i_maze[(int)currentPosition.x, (int)currentPosition.y]
                });

                //As there are no unvisited cells nearby set reversing to true
                reversing = true;
            }
        }

        //Setup a list for any Cells that were missed by the backtracker
        List<Cell> missedCells = new List<Cell>();

        //If a cell in the maze array was missed
        for (int x = 0; x < i_maze.GetLength(0); x++)
        {
            for (int y = 0; y < i_maze.GetLength(1); y++)
            {
                if (i_maze[x, y].visited == false)
                {
                    //Add cell to missed Cells list
                    missedCells.Add(i_maze[x, y]);
                }
            } 
        }

        //If there is more than 0 missed cells
        if(missedCells.Count > 0)
        {
            //Connect missed cells back to the maze
            i_maze = ConnectUnvisitedCells(i_maze, missedCells, i_rand);
        }

        return i_maze;
    }

    List<Neighbour> CheckForUnvisited(Cell[,] i_maze, Vector2 i_pos)
    {
        List<Neighbour> neighbours = new List<Neighbour>();

        //Make sure the cell isn't on the left edge
        if (i_pos.x > 0)
        {
            //If the cell left of the current cell has not been visited add it to the neighbour list
            if (i_maze[(int)i_pos.x - 1, (int)i_pos.y].visited == false)
            {
                //Store the new cell in the Neighbours list
                neighbours.Add(new Neighbour()
                {
                    //set the cell to the neighbouring cell
                    cell = i_maze[(int)i_pos.x - 1, (int)i_pos.y],
                    //set shared wall to the left wall
                    sharedWall = left
                });
            }
        }

        //Ensure the cell isn't on the right edge
        if (i_pos.x < i_maze.GetLength(0) - 1)
        {
            //If the cell left of the current cell has not been visited add it to the neighbour list
            if (i_maze[(int)i_pos.x + 1, (int)i_pos.y].visited == false)
            {
                //Store the new cell in the Neighbours list
                neighbours.Add(new Neighbour()
                {
                    //set the cell to the neighbouring cell
                    cell = i_maze[(int)i_pos.x + 1, (int)i_pos.y],
                    //set shared wall to the right wall
                    sharedWall = right
                });
            }
        }

        //Make sure the cell isn't on the bottom edge
        if (i_pos.y > 0)
        {
            //If the cell left of the current cell has not been visited add it to the neighbour list
            if (i_maze[(int)i_pos.x, (int)i_pos.y - 1].visited == false)
            {
                //Store the new cell in the Neighbours list
                neighbours.Add(new Neighbour()
                {
                    //set the cell to the neighbouring cell
                    cell = i_maze[(int)i_pos.x, (int)i_pos.y - 1],
                    //set shared wall to the right wall
                    sharedWall = down
                });
            }
        }

        //Ensure the cell isn't on the top edge
        if (i_pos.y < i_maze.GetLength(1) - 1)
        {
            //If the cell left of the current cell has not been visited add it to the neighbour list
            if (i_maze[(int)i_pos.x, (int)i_pos.y + 1].visited == false)
            {
                //Store the new cell in the Neighbours list
                neighbours.Add(new Neighbour()
                {
                    //set the cell to the neighbouring cell
                    cell = i_maze[(int)i_pos.x, (int)i_pos.y + 1],
                    //set shared wall to the right wall
                    sharedWall = up
                });
            }
        }

        return neighbours;
    }

    Vector4 OppositeWall(Vector4 i_wall)
    {
        //Flip the input wall and return

        if (i_wall == left)
        {
            return right;
        }
        else if (i_wall == right)
        {
            return left;
        }
        else if (i_wall == up)
        {
            return down;
        }
        else if (i_wall == down)
        {
            return up;
        }
        else
        {
            return new Vector4();
        }
    }

    public Cell[,] ConnectUnvisitedCells(Cell[,] i_maze, List<Cell> i_unvisitedCells, System.Random i_rand)
    {
        //Get a random unvisited cell to start from
        Cell startCell = i_unvisitedCells[i_rand.Next(0, i_unvisitedCells.Count - 1)];
        //Setup a bool for if a visited cell has been found
        bool foundCell = false;
        //Setup a Cell to store the endCell
        Cell endCell = new Cell(0, new Vector2());

        //variable to store direction of endCell (right, left, up, down)
        Vector4 direction = new Vector4();
        //variable to store distance to the endCell
        int distance = 0;

        if (!foundCell)
        {
            for (int i = (int)startCell.position.x; i >= 0; i--)
            {
                if (i_maze[i, (int)startCell.position.y].visited && !(i_maze[i, (int)startCell.position.y].nullCell))
                {
                    direction = left;    //Left direction
                    distance = (int)startCell.position.x - i;
                    foundCell = true;
                    endCell = i_maze[i, (int)startCell.position.y];
                    continue;
                }
            }
        }

        if (!foundCell)
        {
            for (int i = (int)startCell.position.y; i >= 0; i--)
            {
                if (i_maze[(int)startCell.position.x, i].visited && !(i_maze[(int)startCell.position.x, i].nullCell))
                {
                    direction = down;    //Down direction
                    distance = (int)startCell.position.y - i;
                    foundCell = true;
                    endCell = i_maze[(int)startCell.position.x, i];
                    continue;
                }
            }
        }

        if (!foundCell)
        {
            for (int i = (int)startCell.position.x; i < i_maze.GetLength(0); i++)
            {
                if (i_maze[i, (int)startCell.position.y].visited && !(i_maze[i, (int)startCell.position.y].nullCell))
                {
                    direction = right;    //Right direction
                    distance = i - (int)startCell.position.x;
                    foundCell = true;
                    endCell = i_maze[i, (int)startCell.position.y];
                    continue;
                }
            }
        }

        if (!foundCell)
        {
            for (int i = (int)startCell.position.y; i < i_maze.GetLength(1); i++)
            {
                if (i_maze[(int)startCell.position.x, i].visited && !(i_maze[(int)startCell.position.x, i].nullCell))
                {
                    direction = up;    //Up direction
                    distance = i - (int)startCell.position.y;
                    foundCell = true;
                    endCell = i_maze[(int)startCell.position.x, i];
                    continue;
                }
            }
        }

        //Set the endCell to be unvisited
        endCell.visited = false;
        //Set each cell between the start cell and end cell as unvisited and not null cells
        for (int i = 0; i < distance; i++)
        {
            if (direction == right)
            {
                i_maze[(int)startCell.position.x + i, (int)startCell.position.y].visited = false;
                i_maze[(int)startCell.position.x + i, (int)startCell.position.y].nullCell = false;
            }
            else if (direction == left)
            {
                i_maze[(int)startCell.position.x - i, (int)startCell.position.y].visited = false;
                i_maze[(int)startCell.position.x - i, (int)startCell.position.y].nullCell = false;
            }
            else if (direction == up)
            {
                i_maze[(int)startCell.position.x, (int)startCell.position.y + i].visited = false;
                i_maze[(int)startCell.position.x, (int)startCell.position.y + i].nullCell = false;
            }
            else if (direction == down)
            {
                i_maze[(int)startCell.position.x, (int)startCell.position.y - i].visited = false;
                i_maze[(int)startCell.position.x, (int)startCell.position.y - i].nullCell = false;
            }
        }

        i_maze = Backtracker(i_maze, i_rand, startCell);

        return i_maze;
    }

    public struct Neighbour
    {
        public Cell cell;   //Stores the neighbouring cell
        public Vector4 sharedWall;  //Stores the neighbouring cell's shared wall with the current cell
    }
    public struct Deadend
    {
        public Cell cell; //Stores the deadend cell
    }
}
