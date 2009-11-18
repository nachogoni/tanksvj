	// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

public delegate void MoveFinish();


public class TankMovement
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
	
	private MovType movType;
	
	// Rotate Props
	private Quaternion rotateFrom;
	private Quaternion rotateTo;
	private float rotateTotalTime;
	private float accumTime;
	
	/// <summary>
	/// Constructor
	/// </summary>
	public TankMovement(CharacterController cc, TankProperties tp, GameObject bgo)
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
	/// Specifies a tank movement using a Vector3
	/// </summary>
	public void Move(Vector3 dir)
	{
		Move(dir, float.MaxValue, null);
	}
	
	public void MoveToPos(Vector3 pos)
	{
		Vector3 dir = pos - tankCC.gameObject.transform.position;
		dir = new Vector3(dir.x, tankCC.gameObject.transform.position.y, dir.z);
		
		float distance = Vector3.Distance(pos, tankCC.gameObject.transform.position);
		
		Move(dir, distance);
	}
	
	public void MoveToPos(Vector3 pos, MoveFinish mf)
	{
		Vector3 dir = pos - tankCC.gameObject.transform.position;
		dir = new Vector3(dir.x, tankCC.gameObject.transform.position.y, dir.z);
		
		float distance = Vector3.Distance(pos, tankCC.gameObject.transform.position);
		
		Move(dir, distance, mf);
	}
	
	#region Move using Direction
	/// <summary>
	/// Specifies a tank movement using a Vector3
	/// </summary>
	public void Move(Vector3 dir, float distance)
	{
		Move(dir, distance, null);
	}
	
	public void MoveForward()
	{
		Move(baseGO.transform.forward);
	}
	
	public void MoveBackward()
	{
		Move(-baseGO.transform.forward);
	}
	
	/// <summary>
	/// Specifies a tank movement using a Vector3
	/// </summary>
	public void Move(Vector3 dir, float distance, MoveFinish mf)
	{
		startPos = tankCC.transform.position;
		moveDistance = distance;
		
		movDirection = dir.normalized;
		
		isMoving = true;
	
		moveFinish = mf;

		// Rotate the tank
		movType = MovType.ROTATING;
		rotateFrom = baseGO.transform.localRotation;
		rotateTo = Quaternion.LookRotation(baseGO.transform.localPosition + new Vector3(dir.x, baseGO.transform.localPosition.y, dir.z) * 100);

		accumTime = 0;
		// Get the total time for the rotation
		float angleDif = Mathf.Clamp(Mathf.Abs(rotateFrom.eulerAngles.y - rotateTo.eulerAngles.y), 0, 360);
		rotateTotalTime = angleDif / 360.0f;
		
		// [SOUND] Play the engine sound
		baseGO.audio.Play();
		
	}
	#endregion
	
	public void Rotate(float ang)
	{
		Rotate(ang, null);
	}
	
	public void Rotate(float ang, MoveFinish mf)
	{
		startPos = tankCC.transform.position;
		moveDistance = 0;
		movDirection = Vector3.zero;
		
		isMoving = true;
	
		moveFinish = mf;

		// Rotate the tank
		movType = MovType.ROTATING;
		rotateFrom = baseGO.transform.localRotation;
		rotateTo = Quaternion.Euler(rotateFrom.x, rotateFrom.y + ang, rotateFrom.z);
			
		accumTime = 0;
		// Get the total time for the rotation
		float angleDif = Mathf.Clamp(Mathf.Abs(rotateFrom.eulerAngles.y - rotateTo.eulerAngles.y), 0, 360);
		rotateTotalTime = angleDif / 360.0f;
		
		// [SOUND] Play the engine sound
		baseGO.audio.Play();
	}
	
	/// <summary>
	/// Specifies a tank movement using a position of the map to go
	/// </summary>
	public void MoveTo(int row, int col, MoveFinish mf)
	{
		
		Vector3 goalPoint = map.GetWorldPosAtRowCol(row, col);
		Vector3 dir = goalPoint - tankCC.transform.position;
		
		Move(dir,
		     Vector3.Distance(new Vector3(tankCC.transform.position.x, 0, tankCC.transform.position.z),
		                      new Vector3(goalPoint.x, 0, goalPoint.z)),
		     mf);
	}
		
	
	public void Stop()
	{
		baseGO.audio.Stop();
		
		isMoving = false;
	}
	
	public void MoveUpdate()
	{
		if (isMoving)
		{
			switch (movType)
			{
			case MovType.ROTATING:
				
				accumTime += Time.deltaTime;
				
				baseGO.transform.localRotation = Quaternion.Slerp(rotateFrom, rotateTo, accumTime / rotateTotalTime);
				
				if (accumTime >= rotateTotalTime)
				{
					if (	moveDistance == 0)
					{
						Stop();
					
						if (moveFinish != null)
							moveFinish();
					}
					else
						movType = MovType.MOVING;
				}
				
				break;
				
			case MovType.MOVING:
				
				tankCC.SimpleMove(new Vector3(movDirection.x, 0, movDirection.z).normalized * tankProps.GetSpeed());
				
				// If I move more than the move distance, stop!
				if (Vector3.Distance(startPos, tankCC.transform.position) >= moveDistance)
				{
		
					Stop();
					
					if (moveFinish != null)
						moveFinish();
				}
				break;
			}
		}
	}
}
