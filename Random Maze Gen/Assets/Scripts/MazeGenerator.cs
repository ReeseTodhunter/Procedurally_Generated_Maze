using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    //Store Vector walls for easy use
    public static Vector4 rightWall = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
    public static Vector4 leftWall = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
    public static Vector4 topWall = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
    public static Vector4 bottomWall = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);


    public static Cell[,] GenerateStartGrid(int mazeWidth, int mazeHeight)
    {
        Cell[,] maze = new Cell[mazeWidth, mazeHeight];   //Setup the maze 2D array;
        Vector4 startState = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);   //Setup each point to have all 4 walls by default at first


        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y].walls = startState;  //Sets the cell to have all 4 walls
                maze[x, y].visited = false;     //Sets each cell to unvisited
            }
        }

        return maze;
    }


    public static Cell[,] Backtracker(Cell[,] maze, int mazeWidth, int mazeHeight, int seed = 0)
    {
        //Setup Random Start
        System.Random rng;
        if (seed != 0) { rng = new System.Random(seed); } //Setup random using chosen Seed
        else { rng = new System.Random(); }   //Setup Random using no Seed
        Vector2 currentPos = new Vector2(rng.Next(0, mazeWidth), rng.Next(0, mazeHeight));   //Get a random position to start at


        //setup Stack
        Stack<Vector2> posStack = new Stack<Vector2>();    //Setup Stack for moving back through previously visited positions
        posStack.Push(currentPos);


        //Set position as visited
        maze[(int)currentPos.x, (int)currentPos.y].visited = true;


        while (posStack.Count > 0)//If there are positions in the stack keep looping
        {
            currentPos = posStack.Pop(); //Get the current pos
            List<Neighbour> unvistedNeighbours = GetUnvisitedNeighbours(currentPos, maze, mazeWidth, mazeHeight);

            if (unvistedNeighbours.Count > 0)   //If there are any unvisited neighbours
            {
                posStack.Push(currentPos);  //Push the currentPos to the stack

                Neighbour randNeighbour = unvistedNeighbours[rng.Next(0, unvistedNeighbours.Count)]; //Get a random unvisitedNeighbour

                maze[(int)currentPos.x, (int)currentPos.y].walls -= randNeighbour.sharedWall; //Remove the sharedWall
                maze[(int)randNeighbour.position.x, (int)randNeighbour.position.y].walls -= GetOppositeWall(randNeighbour.sharedWall);

                maze[(int)randNeighbour.position.x, (int)randNeighbour.position.y].visited = true;  //Set neighbour to visited
                posStack.Push(randNeighbour.position);  //Push the neighbours position to the position stack for next loop
            }
        }
        return maze;
    }


    private static Vector4 GetOppositeWall(Vector4 sharedWall)
    {
        if (sharedWall == rightWall)  //Right Wall
        {
            return leftWall; //To Left Wall
        }
        else if (sharedWall == leftWall)  //Left Wall
        {
            return rightWall; //To Right Wall
        }
        else if (sharedWall == topWall)  //Top Wall
        {
            return bottomWall; //To Bottom Wall
        }
        else if (sharedWall == bottomWall)  //Bottom Wall
        {
            return topWall; //To Top Wall
        }
        else
        {
            return leftWall; //Left Wall
        }
    }


    private static List<Neighbour> GetUnvisitedNeighbours(Vector2 pos, Cell[,] maze, int mazeWidth, int mazeHeight)
    {
        List<Neighbour> neighbourslist = new List<Neighbour>(); //Setup a place to store unvisited neighbours

        if (pos.x > 0) //If the position isn't on the left wall
        {
            if (!maze[(int)pos.x - 1, (int)pos.y].visited)  //If the cell to the left hasn't been visited
            {
                neighbourslist.Add(new Neighbour()      //Add a new neighbour to the unvisited list
                {
                    position = new Vector2(pos.x - 1, pos.y),   //Setting the neighbour's position and sharedWall
                    sharedWall = leftWall
                });
            }
        }


        if (pos.x < mazeWidth - 1) //If the position isn't on the right wall
        {
            if (!maze[(int)pos.x + 1, (int)pos.y].visited)  //If the cell to the right hasn't been visited
            {
                neighbourslist.Add(new Neighbour()      //Add a new neighbour to the unvisited list
                {
                    position = new Vector2(pos.x + 1, pos.y),   //Setting the neighbour's position and sharedWall
                    sharedWall = rightWall
                });
            }
        }


        if (pos.y > 0) //If the position isn't on the bottom wall
        {
            if (!maze[(int)pos.x, (int)pos.y - 1].visited)  //If the cell to the bottom hasn't been visited
            {
                neighbourslist.Add(new Neighbour()      //Add a new neighbour to the unvisited list
                {
                    position = new Vector2(pos.x, pos.y - 1),   //Setting the neighbour's position and sharedWall
                    sharedWall = bottomWall
                });
            }
        }


        if (pos.y < mazeHeight - 1) //If the position isn't on the top wall
        {
            if (!maze[(int)pos.x, (int)pos.y + 1].visited)  //If the cell to the top hasn't been visited
            {
                neighbourslist.Add(new Neighbour()      //Add a new neighbour to the unvisited list
                {
                    position = new Vector2(pos.x, pos.y + 1),   //Setting the neighbour's position and sharedWall
                    sharedWall = topWall
                });
            }
        }


        return neighbourslist;
    }
}


public struct Cell
{
    public Vector2 position;    //(x, z)
    public Vector4 walls;       //(Right, Left, Top, Bottom)
    public bool visited;        //if the Cell has been visited
}


public struct Neighbour
{
    public Vector2 position;    //(x, z)
    public Vector4 sharedWall;  //The wall shared between 2 cells eg. (0,0,1,0) Top wall shared
}