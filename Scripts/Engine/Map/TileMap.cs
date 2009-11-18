// Mono Framework
using System;

// Unity Framework
using UnityEngine;

/// <summary>
/// 
/// cols
/// n
/// 
/// 
///  
/// 0
///  0                    n
///     rows
/// 
/// 
/// 
/// </summary>
public class TileMap
{
	/// <summary>
	/// Max slope of two tiles to be considered an step
	/// </summary>
    public const float MAX_SLOPE = 10;
    public const float CRATE_HEIGHT = 40;
    public const float RAY_HEIGHT = 2000;

	/// <summary>
	/// The cell width (specified by the user)
	/// </summary>
	private float cellWidth = 1.0f;
	
	/// <summary>
	/// The cell height (specified by the user)
	/// </summary>
	private float cellHeight = 1.0f;
	
	/// <summary>
	/// The cells structure
	/// </summary>
	public Cell[,] cells;
	
	/// <summary>
	/// Rows of the map
	/// </summary>
	private int rows;
	
	/// <summary>
	/// Cols of the map
	/// </summary>
	private int cols;
	
	/// <summary>
	/// The left-bottom point of the map
	/// </summary>
	private Vector3 minPnt;
	
	/// <summary>
	/// The right-top point of the map
	/// </summary>
	private Vector3 maxPnt;
	
	private float _maxDistanceToGoal;
	
	/// <summary>
	/// Create a tileMap with the specified parameters
	/// </summary>
	public TileMap(float cellW, float cellH, Vector3 minP, Vector3 maxP)
	{
		cellWidth = cellW;
		cellHeight = cellH;
		minPnt = minP;
		maxPnt = maxP;
	}
	
	public float MaxDistanceToGoal
	{
		get { return _maxDistanceToGoal; }
	}
	
	/// <summary>
	/// Update the path grid map. Remake the calculus of the bounds and collisions inside
	/// the map. This process is CPU consuming.
	/// </summary>
	public void UpdatePathGrid()
	{
		// The point with max distance to a cell point
		_maxDistanceToGoal = Mathf.Sqrt(cellWidth * cellWidth + cellHeight * cellHeight) / 2.0f;
				
	
		// Create the cell structure
		createCells();

		// Throw the rays
		throwRays();
		
		// Check the slopes between cells
		checkSlopes();
		
	}
	
	/// <summary>
	/// Returns the cost of the cell
	/// </summary>
	/// <param name="pos">Position in world coords.</param>
	/// <returns>The block cost</returns>
	public float GetBlockCost(Vector3 pos)
	{
		int row = (int) Mathf.Floor((pos.x - minPnt.x) / cellWidth);
		int col = (int) Mathf.Floor((pos.z - minPnt.z) / cellHeight);
		
		if (row >= 0 && row < rows && col >= 0 && col < cols)
			return cells[row, col].cost;
		else
			return float.MaxValue;
	}
	
	/// <summary>
	/// Returns true if there is an obstacle in the cell
	/// </summary>
	public bool IsObstacle(Vector3 pos)
	{
		int row = (int) Mathf.Floor((pos.x - minPnt.x) / cellWidth);
		int col = (int) Mathf.Floor((pos.z - minPnt.z) / cellHeight);
		
		//Debug.Log(String.Format("Pos: {0} in row: {1} col: {2}", pos, row, col));
        return IsObstacle(row, col);
		
	}

	/// <summary>
	/// Returns the row and the col in the specified position (in world coords).
	/// </summary>
	/// <param name="pos">The specified position</param>
	/// <param name="row">The returned row</param>
	/// <param name="col">The returned col</param>
    public void GetRowColAtWorldPos(Vector3 pos, out int row, out int col)
    {
        row = (int)Mathf.Floor((pos.x - minPnt.x) / cellWidth);
        col = (int)Mathf.Floor((pos.z - minPnt.z) / cellHeight);
    }

	/// <summary>
	/// Returns the position of the tile specified in row/col. The returned position is at the center of the tile.
	/// </summary>
	/// <param name="row">The specified row</param>
	/// <param name="col">The specified col</param>
	/// <returns></returns>
    public Vector3 GetWorldPosAtRowCol(int row, int col)
    {
        float qx = minPnt.x + (cellWidth * row);
        float qz = minPnt.z + (cellHeight * col);

        float vx = qx % cellWidth;
        if (vx != 0)
        {
            if (vx < (cellWidth / 2.0f))
                vx = qx - vx;
            else
                vx = qx + (cellWidth - vx);
        }
        else
            vx = qx;

        float vz = qz % cellHeight;
        if (vz != 0)
        {
            if (vz < (cellHeight / 2.0f))
                vz = qz - vz;
            else
                vz = qz + (cellHeight - vz);
        }
        else
            vz = qz;


        return new Vector3(vx, 0, vz);
    }

    /// <summary>
    /// Returns true if the in the tile is there an obstacle
    /// </summary>
    /// <param name="row">The specified row</param>
	/// <param name="col">The specified col</param>
    /// <returns>Returns true if there is an ostacle</returns>
    public bool IsObstacle(int row, int col)
    {
        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            Vector3 pos = GetWorldPosAtRowCol(row, col);
            return (Physics.Raycast(new Vector3(pos.x, 1000, pos.z), Vector3.down, 2000, Layers.Props));
        }
        else
            return true;
    }

	
	/// <summary>
    /// Returns true if the in the tile is there an obstacle
    /// </summary>
    /// <param name="row">The specified row</param>
	/// <param name="col">The specified col</param>
	/// <param name="height">The returned height of the obstacle (0 if the function returns false)</param>
    /// <returns>Returns true if there is an ostacle</returns>
    public bool IsObstacle(int row, int col, out float height)
    {
        height = 0;

        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            Vector3 pos = GetWorldPosAtRowCol(row, col);

            //Debug.Log(String.Format("r: {0} c: {1} pos: {2}", row, col, pos));
            RaycastHit hit;

            if (Physics.Raycast(new Vector3(pos.x, 1000, pos.z), Vector3.down, out hit, 2000, Layers.Props))
            {
                height = hit.transform.position.y + CRATE_HEIGHT;

                return true;
            }
            else
                return false;
        }
        else
            return true;
    }

	
	/// <summary>
	/// Returns the floor height at the specified position. Not considering obstacles.
	/// </summary>
    public float GetFloorHeight(int row, int col)
    {
        if (row >= 0 && row < rows && col >= 0 && col < cols && cells[row, col].tile != null)
        {
            return cells[row, col].tile.transform.position.y;
        }
        else
            // Return a very high height
            return 10000.0f;
    }

	/// <summary>
	/// Returns the floor height at the specified position. Considering obstacles.
	/// </summary>
    public float GetHeight(int row, int col)
    {
        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            Vector3 pos = GetWorldPosAtRowCol(row, col);

            RaycastHit hit;

            if (Physics.Raycast(new Vector3(pos.x, 1000, pos.z), Vector3.down, out hit, 2000, Layers.Floor | Layers.Props))
            {
                if (hit.transform.gameObject.layer == Layers.PropsNum)
                    return hit.transform.position.y + CRATE_HEIGHT;
                else
                    return hit.transform.position.y;
            }

        }
        
        // Return a very high height
        return 10000.0f;
    }
	
	/// <summary>
	/// Returns the number of rows
	/// </summary>
	public int Rows
	{
		get { return rows; }
	}
	
	
	/// <summary>
	/// Returns the number of cols
	/// </summary>
	public int Cols
	{
		get { return cols; }
	}




    public bool GetMove(uint moveTo, Vector3 pos, out Vector3 outPos)
    {
            int row = (int)Mathf.Floor((pos.x - minPnt.x) / cellWidth);
            int col = (int)Mathf.Floor((pos.z - minPnt.z) / cellHeight);

            //Debug.Log(String.Format("Pos: {0} in row: {1} col: {2}", pos, row, col));

			if (row >= 0 && row < rows && col >= 0 && col < cols)
            {
				
				switch (moveTo)
                {
                    case CellPossibleMoves.N:
                        if (col < cols - 1 && !cells[row, col + 1].collide.IsObstacle() && cells[row, col].collide.Contains(CellPossibleMoves.N))
                        {
                            outPos = cells[row, col + 1].pos;
                            return true;
                        }
                        break;
                    case CellPossibleMoves.S:
                        if (col > 0 && !cells[row, col - 1].collide.IsObstacle() && cells[row, col].collide.Contains(CellPossibleMoves.S))
                        {
                            outPos = cells[row, col - 1].pos;
                            return true;
                        }
                        break;
                    case CellPossibleMoves.E:
                        if (row < rows - 1 && !cells[row + 1, col].collide.IsObstacle() && cells[row, col].collide.Contains(CellPossibleMoves.E))
                        {
                            outPos = cells[row + 1, col].pos;
                            return true;
                        }
                        break;
                    case CellPossibleMoves.W:
                        if (row > 0 && !cells[row - 1, col].collide.IsObstacle() && cells[row, col].collide.Contains(CellPossibleMoves.W))
                        {
                            outPos = cells[row - 1, col].pos;
                            return true;
                        }
                        break;
                    case CellPossibleMoves.NE:
                        if (col < cols - 1 && row < rows - 1 && !cells[row + 1, col + 1].collide.IsObstacle() && cells[row, col].collide.Contains(CellPossibleMoves.NE))
                        {
                            outPos = cells[row + 1, col + 1].pos;
                            return true;
                        }
                        break;
                    case CellPossibleMoves.NW:
                        if (col < cols - 1 && row > 0 && !cells[row - 1, col + 1].collide.IsObstacle() && cells[row, col].collide.Contains(CellPossibleMoves.NW))
                        {
                            outPos = cells[row - 1, col + 1].pos;
                            return true;
                        }
                        break;
                    case CellPossibleMoves.SE:
                        if (col > 0 && row < rows - 1 && !cells[row + 1, col - 1].collide.IsObstacle() && cells[row, col].collide.Contains(CellPossibleMoves.SE))
                        {
                            outPos = cells[row + 1, col - 1].pos;
                            return true;
                        }
                        break;
                    case CellPossibleMoves.SW:
                        if (col > 0 && row > 0 && !cells[row - 1, col - 1].collide.IsObstacle() && cells[row, col].collide.Contains(CellPossibleMoves.SW))
                        {
                            outPos = cells[row - 1, col - 1].pos;
                            return true;
                        }
                        break;
                }

            }

        outPos = Vector3.zero;
        return false;
        

    }
	
	/// <summary>
	/// Create the cells structure
	/// </summary>
	private void createCells()
	{
		int cellCount = 0;

		float widthDiv2 = cellWidth / 2.0f;
		float heightDiv2 = cellHeight / 2.0f;
		
		rows = (int) Mathf.Ceil((maxPnt.x - minPnt.x) / cellWidth);
		cols = (int) Mathf.Ceil((maxPnt.z - minPnt.z) / cellHeight);
		
		cells = new Cell[rows, cols];
		
		
		for (int i=0; i<cols; i++)
			for (int j=0; j<rows; j++)
		{
			cells[j, i] = new Cell(minPnt.x + (cellWidth * j) + widthDiv2,
			                       minPnt.y,
			                       minPnt.z + (cellHeight * i) + heightDiv2);

            cells[j, i].row = j;
            cells[j, i].col = i;
			
			cellCount++;
		}
		
	}


	/// <summary>
	/// Throw the rays
	/// </summary>
	private void throwRays()
	{
	
		RaycastHit hit;

		for (int i=0; i<cols; i++)
			for (int j=0; j<rows; j++)
		{
			if (Physics.Raycast(new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z),
			                    Vector3.down,
			                    out hit,
			                    RAY_HEIGHT,
			                    (Layers.Floor | Layers.Props | Layers.Doors)))
			{
				
				

                if (hit.transform.gameObject.layer == Layers.PropsNum)
                {
					
                    cells[j, i].collide.SetAsObstacle();

                    // It is the floor. Update the Y position of the cell
                    cells[j, i].pos = new Vector3(cells[j, i].pos.x, hit.point.y, cells[j, i].pos.z);
					cells[j, i].go = hit.transform.gameObject;
					
                    // Now get the tile itself
                    if (Physics.Raycast(new Vector3(cells[j, i].pos.x, cells[j, i].pos.y, cells[j, i].pos.z),
                                Vector3.down,
                                out hit,
                                RAY_HEIGHT,
                                Layers.Floor))
                    {
                        cells[j, i].tile = hit.transform.gameObject.GetComponent(typeof(Tile)) as Tile;
                    }

                }
                else
                {
                    cells[j, i].collide.RemoveAll();

                    // It is the floor. Update the Y position of the cell
                    cells[j, i].pos = new Vector3(cells[j, i].pos.x, hit.point.y, cells[j, i].pos.z);
					cells[j, i].go = hit.transform.gameObject;
                    cells[j, i].tile = hit.transform.gameObject.GetComponent(typeof(Tile)) as Tile;
                }
			}
			else
				cells[j, i].collide.RemoveAll();
		}
	}
	
	
	/// <summary>
	/// Check slopes
	/// </summary>
	private void checkSlopes()
	{
	
		for (int i=0; i<cols; i++)
			for (int j=0; j<rows; j++)
		{
			Cell cur = cells[j, i];
			
			// If the cell is not an obstacle, check if
			// I can walk from this cell to its neightbors
#if !CAN_CLIMB_ON_PROPS
			if (!cur.collide.IsObstacle())
#endif
			{
				cur.collide.RemoveAll();
				
				// Check with the West cell
				if (j > 0 && canWalk(cur, cells[j - 1, i]))
					cur.collide.Add(CellPossibleMoves.W);
			
				// Check with the North cell
				if (i < cols - 1 && canWalk(cur, cells[j, i + 1]))
					cur.collide.Add(CellPossibleMoves.N);
			
				// Check with the East cell
				if (j < rows - 1 && canWalk(cur, cells[j + 1, i]))
					cur.collide.Add(CellPossibleMoves.E);
			
				// Check with the South cell
				if (i > 0 && canWalk(cur, cells[j, i - 1]))
					cur.collide.Add(CellPossibleMoves.S);
			
				// Check with the NE cell
				if (i < cols - 1 && j < rows - 1 && canWalk(cur, cells[j + 1, i + 1]))
					cur.collide.Add(CellPossibleMoves.NE);
			
				// Check with the SE cell
				if (i > 0 && j < rows - 1 && canWalk(cur, cells[j + 1, i - 1]))
					cur.collide.Add(CellPossibleMoves.SE);
			
				// Check with the SW cell
				if (i > 0 && j > 0 && canWalk(cur, cells[j - 1, i - 1]))
					cur.collide.Add(CellPossibleMoves.SW);
			
				// Check with the NW cell
				if (i < cols - 1 && j > 0 && canWalk(cur, cells[j - 1, i + 1]))
					cur.collide.Add(CellPossibleMoves.NE);
			}
					
		}
	}
					
	private bool canWalk(Cell c1, Cell c2)
	{
#if !CAN_CLIMB_ON_PROPS
        if (c2.collide.IsObstacle())
            return false;
        else
#endif
            return (Mathf.Abs(c1.pos.y - c2.pos.y) < MAX_SLOPE);
	}

    public void UpdateDoorNeightbors(int row, int col)
    {
        Cell cur = cells[row, col];

        cur.collide.RemoveAll();

        // Check with the West cell
        if (row > 0 && canWalk(cur, cells[row - 1, col]))
            cur.collide.Add(CellPossibleMoves.W);
        else
            cur.collide.Remove(CellPossibleMoves.W);

        // Check with the North cell
        if (col < cols - 1 && canWalk(cur, cells[row, col + 1]))
            cur.collide.Add(CellPossibleMoves.N);
        else
            cur.collide.Remove(CellPossibleMoves.N);

        // Check with the East cell
        if (row < rows - 1 && canWalk(cur, cells[row + 1, col]))
            cur.collide.Add(CellPossibleMoves.E);
        else
            cur.collide.Remove(CellPossibleMoves.E);

        // Check with the South cell
        if (col > 0 && canWalk(cur, cells[row, col - 1]))
            cur.collide.Add(CellPossibleMoves.S);
        else
            cur.collide.Remove(CellPossibleMoves.S);

        // ---------------------------------------
        // Check FROM neightbors to door tile

        // Check with the West cell
        if (row > 0 && canWalk(cells[row - 1, col], cur))
            cells[row - 1, col].collide.Add(CellPossibleMoves.E);
        else
            cells[row - 1, col].collide.Remove(CellPossibleMoves.E);

        // Check with the North cell
        if (col < cols - 1 && canWalk(cells[row, col + 1], cur))
            cells[row, col + 1].collide.Add(CellPossibleMoves.S);
        else
            cells[row, col + 1].collide.Remove(CellPossibleMoves.S);

        // Check with the East cell
        if (row < rows - 1 && canWalk(cells[row + 1, col], cur))
            cells[row + 1, col].collide.Add(CellPossibleMoves.W);
        else
            cells[row + 1, col].collide.Remove(CellPossibleMoves.W);

        // Check with the South cell
        if (col > 0 && canWalk(cells[row, col - 1], cur))
            cells[row, col - 1].collide.Add(CellPossibleMoves.N);
        else
            cells[row, col - 1].collide.Remove(CellPossibleMoves.N);
    }


	/// <summary>
	/// Returns the max distance to the goal
	/// </summary>
	public float GetMaxDistanceToGoal()
	{
		return _maxDistanceToGoal;
	}

	/// <summary>
	/// Is a valid row
	/// </summary>
    public bool IsValidRow(int row)
    {
        return (row >= 0 && row < rows);
    }

	/// <summary>
	/// Is a valid col
	/// </summary>
    public bool IsValidCol(int col)
    {
        return (col >= 0 && col < cols);
    }
}
