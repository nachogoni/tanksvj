// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public class TankBasic : TankBehaviour
{

	private int flagCount;
	private int curFlag;
	
	/// <summary>
	/// Initializating the tank properties
	/// </summary>
	public override TankProperties GetProperties()
	{
		TankProperties tp;
		
		tp.VisionDistance = 1;
		tp.VisionAngle = 1;
		tp.FirePower = 1;
		tp.FireRate = 1;
		tp.MovSpeed = 4;
		tp.Armor = 1;
		tp.RadDistance = 1;
		tp.RadRefresh = 1;
		tp.EnergyTotal = 1;
		tp.ShieldDuration = 1;
		tp.ShieldRechargeSpd = 1;
		
		return tp;
		
	}
	
	/// <summary>
	/// First tank thinking
	/// </summary>
	public override void StartThink()
	{
		//ActivateShield();	
	}
	
	bool f;
	/// <summary>
	/// Called once per Update
	/// </summary>
	public override void Think()
	{
		//MoveToUsingPF(10, 10);
		if(!f)MoveBackward();
		f=true;
		
		if (visionInfo.Length > 0)
		{		
			Fire(visionInfo[0].position, OnFireFinish);	

		}
	}
	
	public void OnFireFinish()
	{
		print("Fired");
	}
	
	public void OnMoveFinish()
	{
	}
	
	/// <summary>
	/// The shield is no longer active
	/// </summary>
	public override void OnShieldFinish()
	{
		Debug.Log("OnShieldFinish");
	}
	
	/// <summary>
	/// The shield is available again
	/// </summary>
	public override void OnShieldAvailable()
	{
		Debug.Log("OnShieldAvailable");
	}

	/// <summary>
	/// A shoot was received. The direction of the shoot is notified.
	/// </summary>
	public override void OnShootReceived(Vector3 dir)
	{
		Debug.Log(String.Format("Shoot received from: {0}", dir));
	}
	
	/// <summary>
	/// A shoot on the shield was received. No damage was taken. The direction of the shoot is notified.
	/// </summary>
	public override void OnShootShieldReceived(Vector3 dir)
	{
		Debug.Log(String.Format("Shield Shoot received from: {0}", dir));
	}
	
}
