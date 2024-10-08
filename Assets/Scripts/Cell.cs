using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public int ID;
    public Vector2 position;
    public Vector4 activeWalls;
    public List<GameObject> walls;
    public bool visited;
    public bool nullCell;

    public Cell(int i_ID, Vector2 i_position)
    {
        ID = i_ID;                                  //Stores an int ID for each individual cell
        position = i_position;                      //Stores the position of the Cell
        activeWalls = new Vector4(1, 1, 1, 1);      //Stores which walls are active 1,1,1,1 being the default for all walls active
        walls = new List<GameObject>();             //Stores a reference to all of the cells walls for use destroying them or transforming them
        visited = false;                            //Stores a bool value of if the cell has already been visited or not
        nullCell = false;                           //Stores a bool value for if the cell should be an obstacle when generating the maze
    }
}