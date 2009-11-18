// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public class TankMovementPF
{
	enum MovType { ROTATING, MOVING }
	
	private MoveFinish movFinish;
	private Vector3 movDirection;
	
	private CharacterController tankCC;
	private TankProperties tankProps;
	private GameObject baseGO;
	
	private Vector3 startPos;
	private float moveDistance;
	
	private MoveFinish moveFinish;
	
	private TileMap map;
	
	private bool isMoving;
	
	// For PathFinding Move
	private int curNode; // The current solution node
	private Vector3[] pathToGoal;
	private float prevDistance;
	
	/// <summary>
	/// The path finder algorithm
	/// </summary>
	private static AStar astar = new AStar();
	
	private MovType movType;
	
	// Rotate Props
	private Quaternion rotateFrom;
	private Quaternion rotateTo;
	private float rotateTotalTime;
	private float accumTime;
	
	/// <summary>
	/// Draw some debug information
	/// </summary>
	public void DrawDebugGUI()
	{
		if (pathToGoal != null)
		{
			for (int i=0; i<pathToGoal.Length - 1; i++)
			{
				Debug.DrawLine(pathToGoal[i], pathToGoal[i+1], Color.red);
			}
		}
	}
	
	/// <summary>
	/// Constructor
	/// </summary>
	public TankMovementPF(CharacterController cc, TankProperties tp, GameObject bgo)
	{
		isMoving = false;
		tankProps = tp;
		
		tankCC = cc;
		baseGO = bgo;
		
		// Get the tile map
		GameObject mapGO = GameObject.FindGameObjectWithTag("Map");
		TileMapBhv tmb = mapGO.GetComponent<TileMapBhv>();
		map = tmb.tileMap;
	}

	/// <summary>
	/// Move the tank to the specified row/col avoiding obstacles
	/// </summary>
	public void MoveToUsingPF(int row, int col)
	{
		MoveToUsingPF(row, col, null);
	}
	
	/// <summary>
	/// Move the tank to the specified row/col avoiding obstacles.
	/// An event could be specified to be called when the tank arrives at goal.
	/// </summary>
	public void MoveToUsingPF(int row, int col, MoveFinish mf)
	{		
		moveFinish = mf;
	
		if (astar.Resolve(map, tankCC.transform.position, map.GetWorldPosAtRowCol(row, col)))
		{
			prevDistance = float.MaxValue;
			
			
			pathToGoal = astar.GetSolution();
			
			if (pathToGoal.Length > 1)
			{
				curNode = 0;
				
				setNewDirection(1);
			
			}

		}
		else
		{
			// Cannot find a path to the solution
			pathToGoal = null;
			
			Stop();
		}
		
		
	}
	
	
	
	/// <summary>
	/// Stop the movement
	/// </summary>
	public void Stop()
	{
		// [SOUND] Stop the engine sound
		baseGO.audio.Stop();
		
		isMoving = false;
	}
	
	/// <summary>
	/// Update the movement
	/// </summary>
	public void MoveUpdate()
	{
		if (isMoving)
		{
			switch(movType)
			{
			case MovType.ROTATING:
				accumTime += Time.deltaTime;
				
				baseGO.transform.localRotation = Quaternion.Slerp(rotateFrom, rotateTo, accumTime / rotateTotalTime);
				
				if (accumTime >= rotateTotalTime)
				{
					baseGO.transform.localRotation = rotateTo;
					
					movType = MovType.MOVING;
				}
				
				break;
				
			case MovType.MOVING:
				
				if (isCurrentNodeReached())
				{
									
					curNode++;
								
	                if (curNode < pathToGoal.Length)
	                {
						prevDistance = float.MaxValue;
						
	       				setNewDirection(curNode);
			
					}
					else
					{
						// Final destination
						Stop();
					
						if (moveFinish != null)
							moveFinish();
					}
				}
				else
				{
					tankCC.SimpleMove(new Vector3(movDirection.x, 0, movDirection.z).normalized * tankProps.GetSpeed());
				
				}
				break;
			}
			
		}
	}
	
	private void setNewDirection(int node)
	{
		// I am currently in the node 0, so target to node 1
		movDirection = pathToGoal[node] - tankCC.transform.position;
		
		// Prepare for the rotation stuff
		movType = MovType.ROTATING;
		rotateFrom = baseGO.transform.localRotation;
		rotateTo = Quaternion.LookRotation(baseGO.transform.localPosition + new Vector3(movDirection.x, baseGO.transform.localPosition.y, movDirection.z) * 100);
		accumTime = 0;

		// Get the total time for the rotation
		float angleDif = Mathf.Clamp(Mathf.Abs(rotateFrom.eulerAngles.y - rotateTo.eulerAngles.y), 0, 360);
		rotateTotalTime = angleDif / 360.0f;
		// ---
		
		// [SOUND] Play the engine sound
		baseGO.audio.Play();
	
		isMoving = true;
	}
	
	/// <summary>
	/// The target node is reached?
	/// </summary>
	private bool isCurrentNodeReached()
    {
        float distance = Vector3Util.DistanceXZ(tankCC.transform.position, pathToGoal[curNode]);

		// The distance between the current position and the target should never be
		// greater than previous, if so I pass over the goal.
        if (distance > prevDistance)
        {
			return true;
        }

        prevDistance = distance;

		// Check if I am close to the goal (the tank never will be exactly in the goal position)
        return (distance < 5.0f); //map.MaxDistanceToGoal);
    }
}
