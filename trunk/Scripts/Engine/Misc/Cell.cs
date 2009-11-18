// Mono Framework
using System;

// Unity Framework
using UnityEngine;

public class Cell
{
	public Vector3 pos;
	public CellPossibleMoves collide;
	public float cost;

    // Additional Information
    public int row;
    public int col;
	public GameObject go;
    public Tile tile;
	
	public Cell(float x, float y, float z)
	{
		pos = new Vector3(x, y, z);
		collide = new CellPossibleMoves();
		cost = 1.0f;

        row = -1;
        col = -1;
        tile = null;
	}
	
	public Cell(Vector3 p)
	{
		pos = p;
		collide = new CellPossibleMoves();
		cost = 1.0f;

        row = -1;
        col = -1;
        tile = null;
	}
	
}

public class CellPossibleMoves
{
	public const uint N = 0;
	public const uint S = 1;
	public const uint E = 2;
	public const uint W = 3;
	public const uint NE = 4;
	public const uint NW = 5;
	public const uint SE = 6;
	public const uint SW = 7;
	
	public const int OBSTACLE = 255;
	
	public uint obstacle;
	
	public CellPossibleMoves()
	{
		obstacle = 0;
	}
	
	public void Add(uint val)
	{
		obstacle |= (uint) Mathf.Pow(2, val);
	}
	
	public void Remove(uint val)
	{
		obstacle &= ~ (uint) (Mathf.Pow(2, val));
	}
	
	public void RemoveAll()
	{
		obstacle = 0;
	}
	
	public void SetAsObstacle()
	{
		obstacle = OBSTACLE;
	}
	
	public bool IsObstacle()
	{
		return (obstacle == OBSTACLE);
	}
	
    public bool Contains(uint val)
    {
        return ((obstacle & ~ (uint) (Mathf.Pow(2, val))) != obstacle);
    }
}
