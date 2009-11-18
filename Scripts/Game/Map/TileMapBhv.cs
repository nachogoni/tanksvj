// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

[ExecuteInEditMode]
public class TileMapBhv : MonoBehaviour
{
	/// <summary>
	/// The cell width units
	/// </summary>
	public float cellWidth = 1.0f;
	
	/// <summary>
	/// The cell height units
	/// </summary>
	public float cellHeight = 1.0f;
	
	private Transform _pntA;
	private Transform _pntB;

	/// <summary>
	/// Draw cell guide lines (for debuguing)
	/// </summary>
	public bool drawCells = true;
	
	/// <summary>
	/// Draw the path between cells (for debuguing)
	/// </summary>
	public bool drawPaths = true;
	
	public TileMap tileMap;
	
	void Awake()	
	{
		Component[] cs = gameObject.GetComponentsInChildren(typeof(Transform));
				
		for (int i=0; i<cs.Length; i++)
		{
			if (String.Compare(cs[i].gameObject.name, "PointA", false) == 0)
				_pntA = cs[i] as Transform;
			else if (String.Compare(cs[i].gameObject.name, "PointB", false) == 0)
				_pntB = cs[i] as Transform;
		}
		
		// Create the tile map
		tileMap = new TileMap(cellWidth, cellHeight, _pntA.transform.position, _pntB.transform.position);
		tileMap.UpdatePathGrid();
	}
	
	
	// Update is called once per frame
	void Update()
	{
		if (tileMap == null) return;
		
		// The Map structure is not called every update
		if (tileMap.cells == null)
			tileMap.UpdatePathGrid();
		
		float widthDiv2 = cellWidth / 2.0f;
		float heightDiv2 = cellHeight / 2.0f;
		
		if (!drawCells) return;
		
		Cell[,] cells = tileMap.cells;
		
		// About to draw debug information
		for (int i=0; i<tileMap.Cols; i++)
			for (int j=0; j<tileMap.Rows; j++)
		{
			if (cells[j, i].collide.IsObstacle())
				DebugEx.DrawRectCrossXZ(new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z), cellWidth, cellHeight, Color.red);
			else
			{
				DebugEx.DrawRectXZ(new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z), cellWidth, cellHeight, Color.green);
			
				if (drawPaths)
				{
					// N
					if (cells[j, i].collide.Contains(CellPossibleMoves.N))
						Debug.DrawLine(new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z),
							           new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z + heightDiv2),
							           Color.cyan);
				
					// S
					if (cells[j, i].collide.Contains(CellPossibleMoves.S))
						Debug.DrawLine(new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z),
							           new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z - heightDiv2),
							           Color.cyan);
				
					// E
					if (cells[j, i].collide.Contains(CellPossibleMoves.E))
						Debug.DrawLine(new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z),
							           new Vector3(cells[j, i].pos.x + widthDiv2, cells[j, i].pos.y, cells[j, i].pos.z),
							           Color.cyan);
				
					// W
					if (cells[j, i].collide.Contains(CellPossibleMoves.W))
						Debug.DrawLine(new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z),
							           new Vector3(cells[j, i].pos.x - widthDiv2, cells[j, i].pos.y, cells[j, i].pos.z),
							           Color.cyan);
				}
			
			}
		}
	}
}
