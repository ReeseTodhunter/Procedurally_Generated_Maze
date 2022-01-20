using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour
{
    [SerializeField]
    [Range(1, 50)]
    int width = 10;

    [SerializeField]
    [Range(1, 50)]
    int height = 10;

    [SerializeField]
    GameObject Wall;

    [SerializeField]
    GameObject Destroyer;

    // Start is called before the first frame update
    void Start()
    {
        bool[,] maze = new bool[width, height];
        maze = GenerateInitialMaze(width, height, maze);
        GameObject[] walls = new GameObject[width * height * 4];
        GenerateWalls(width, height, maze, walls, Wall);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static bool[,] GenerateInitialMaze(int a_width, int a_height, bool[,] a_maze)
    {
        uint counter = 0;
        for (int i = 0; i < a_width; i++)
        {
            for(int j = 0; j < a_height; j++)
            {
                counter++;
                a_maze[i, j] = false; //Each 1 in the node represents a wall = (1 Right Wall, 1 Left Wall, 1 Top Wall, 1 Bottom Wall)
                Debug.Log("node " + counter + " is " + a_maze[i, j]);
            }
        }
        return a_maze;
    }
    public static bool[,] GenerateMaze(int a_width, int a_height, bool[,] a_maze)
    {
        return a_maze;
    }
    public static GameObject[] GenerateWalls(int a_width, int a_height, bool[,] a_maze, GameObject[] a_wall, GameObject Wall)
    {
        uint counter = 0;
        for (int i = 0; i < a_width; i++)
        {
            for (int j = 0; j < a_height; j++)
            {
                if(a_maze[i, j].x == 1)     //Right Wall
                {
                    a_wall[counter] = Instantiate(Wall, new Vector3((i - 0.5f) - 5, 0, j - 4.5f), Quaternion.Euler(0, 90, 0));
                    counter++;
                }
                if (a_maze[i, j].y == 1)    //Left Wall
                {
                    a_wall[counter] = Instantiate(Wall, new Vector3((i + 0.5f) - 5, 0, j - 4.5f), Quaternion.Euler(0, 90, 0));
                    counter++;
                }
                if (a_maze[i, j].z == 1)    //Top Wall
                {
                    a_wall[counter] = Instantiate(Wall, new Vector3(i - 5, 0, (j + 0.5f) - 4.5f), Quaternion.Euler(0, 0, 0));
                    counter++;
                }
                if (a_maze[i, j].w == 1)    //Bottom Wall
                {
                    a_wall[counter] = Instantiate(Wall, new Vector3(i - 5, 0, (j - 0.5f) - 4.5f), Quaternion.Euler(0, 0, 0));
                    counter++;
                }
            }
        }
        return a_wall;
    }
}
