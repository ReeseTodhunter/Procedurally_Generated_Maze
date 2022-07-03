using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject wall = null; //Get the wall object to use in maze Generation

    [SerializeField]
    [Range(5, 50)]
    private int mazeWidth = 10;

    [SerializeField]
    [Range(5, 50)]
    private int mazeHeight = 10;

    [SerializeField]
    private int seed = 0;

    private Cell[,] maze;
    private List<GameObject> walls = new List<GameObject>();



    void Start()
    {
        maze = MazeGenerator.GenerateStartGrid(mazeWidth, mazeHeight);    //Generate a basic grid
        Visualise(maze);        //Visualise the maze
    }



    public void CreateMaze()
    {
        maze = MazeGenerator.GenerateStartGrid(mazeWidth, mazeHeight);    //Generate a basic grid
        maze = MazeGenerator.Backtracker(maze, mazeWidth, mazeHeight, seed);    //Use stack to create maze
        Visualise(maze);     //Visualise the maze
    }



    private void Visualise(Cell[,] maze)
    {
        if (walls.Count > 0)    //If there are walls in the walls list
        {
            foreach (var obj in walls)
            {
                Destroy(obj);  //Destroy all walls
            }
            walls.Clear();  //Refresh and clear walls list
        }


        for (int x = 0; x < mazeWidth; x++)
        {

            for (int y = 0; y < mazeHeight; y++)
            {

                Cell cell = maze[x, y];
                Vector3 position = new Vector3(-mazeWidth / 2 + x, 0, -mazeHeight / 2 + y);

                if (cell.walls.z == 1.0f)   //if the current cell has a top wall
                {
                    GameObject topWall = Instantiate(wall, transform);
                    topWall.transform.position = position + new Vector3(0.0f, 0.0f, 0.5f);
                    walls.Add(topWall);     //Add wall to the list of walls
                }


                if (cell.walls.y == 1.0f)   //if the current cell has a left wall
                {
                    GameObject leftWall = Instantiate(wall, transform);
                    leftWall.transform.position = position + new Vector3(-0.5f, 0.0f, 0.0f);
                    leftWall.transform.eulerAngles = new Vector3(0, 90, 0);     //Rotate wall to face correct way
                    walls.Add(leftWall);     //Add wall to the list of walls
                }


                if (x == mazeWidth - 1)
                {
                    if (cell.walls.x == 1.0f)   //if the current cell has a right wall
                    {
                        GameObject rightWall = Instantiate(wall, transform);
                        rightWall.transform.position = position + new Vector3(0.5f, 0.0f, 0.0f);
                        rightWall.transform.eulerAngles = new Vector3(0, 90, 0);    //Rotate wall to face correct way
                        walls.Add(rightWall);     //Add wall to the list of walls
                    }
                }


                if (y == 0)
                {
                    if (cell.walls.w == 1.0f)   //if the current cell has a bottom wall
                    {
                        GameObject bottomWall = Instantiate(wall, transform);
                        bottomWall.transform.position = position + new Vector3(0.0f, 0.0f, -0.5f);
                        walls.Add(bottomWall);     //Add wall to the list of walls
                    }
                }
            }
        }
    }
}