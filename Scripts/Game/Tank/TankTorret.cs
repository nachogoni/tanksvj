// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

public delegate void FireFinish();
public delegate void RotateFinish();

public class TankTorret
{
	enum MovType { ROTATING, FIRE, RELOADING }
	
	/// <summary>
	/// The transformation of the tank
	/// </summary>
	private Transform tankTrnsf;
	
	private TankProperties tankProps;
	private GameObject torretGO;
	private Transform firePoint;
	
	private FireFinish fireFinish;
	private RotateFinish rotFinish;
	
	private bool isShooting;
	private bool isRotating;
	
	private MovType movType;
	
	// Rotate Props
	private Quaternion rotateFrom;
	private Quaternion rotateTo;
	private float rotateTotalTime;
	private float accumTime;
	
	private Vector3 shootDir;
	
	/// <summary>
	/// Constructor
	/// </summary>
	public TankTorret(TankProperties tp, GameObject tgo, Transform fp)
	{
		isShooting = isRotating = false;
		tankProps = tp;
		torretGO = tgo;
		firePoint = fp;
		
		tankTrnsf = torretGO.transform.parent.transform;
	}
	
	public void Rotate(float deg)
	{
		Rotate(deg, null);
	}
	
	public void Rotate(float deg, RotateFinish rf)
	{
		// Rotate the tank
		movType = MovType.ROTATING;
		rotateFrom = torretGO.transform.localRotation;
		rotateTo = Quaternion.Euler(torretGO.transform.localRotation.x,
		                            deg,
		                            torretGO.transform.localRotation.z);

		accumTime = 0;
		// Get the total time for the rotation
		float angleDif = Mathf.Clamp(Mathf.Abs(rotateFrom.eulerAngles.y - rotateTo.eulerAngles.y), 0, 360);
		rotateTotalTime = angleDif / 360.0f;
		
		rotFinish = rf;
		
		isRotating = true;
	}
	
	/// <summary>
	/// Fire to the specified position
	/// </summary>
	public void Fire(Vector3 pos, FireFinish ff)
	{
		shootDir = pos - tankTrnsf.position;
				
		// Rotate the tank
		movType = MovType.ROTATING;
		rotateFrom = torretGO.transform.localRotation;
		rotateTo = Quaternion.LookRotation(tankTrnsf.position + new Vector3(shootDir.x, tankTrnsf.position.y, shootDir.z) * 100);

		accumTime = 0;
		// Get the total time for the rotation
		float angleDif = Mathf.Clamp(Mathf.Abs(rotateFrom.eulerAngles.y - rotateTo.eulerAngles.y), 0, 360);
		rotateTotalTime = angleDif / 360.0f;
		
		fireFinish = ff;
		
		isShooting = true;
	}
	
	/// <summary>
	/// Update the fire function
	/// </summary>
	public void FireUpdate()
	{
		if (isShooting || isRotating)
		{
			switch (movType)
			{
			case MovType.ROTATING:
				accumTime += Time.deltaTime;
				
				torretGO.transform.localRotation = Quaternion.Slerp(rotateFrom, rotateTo, accumTime / rotateTotalTime);
				
				if (accumTime >= rotateTotalTime)
				{
					if (isRotating)
					{
						if (rotFinish != null)
							rotFinish();
						
						isRotating = false;
					}
					else
						movType = MovType.FIRE;
				}
				
				
				break;
			
			case MovType.FIRE:
				
				// [SOUND]
				SoundManager.PlaySound(torretGO.transform.position, SndId.SND_FIRE);
				
				BulletManager.instance.Fire(torretGO.transform.parent.gameObject,
				                            firePoint.position,
				                            shootDir);
				
				movType = MovType.RELOADING;
				
				if (fireFinish != null)
					fireFinish();
				
				break;
				
			case MovType.RELOADING:
				
				
				break;
			}
		}
	}
}
