// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public struct TankProperties
{
	public const int TOTAL_POINTS = 20;
	public const float MIN_MOV_SPEED = 10;
	
	// Vision
	/// <summary>
	/// The distance view of the tank.
	/// 
	/// Possible values:
	/// 
	/// 0: 30m
	/// 1: 45m
	/// 2: 60m
	/// 3: 75m
	/// 4: 100m
	/// </summary>
	public int VisionDistance;
	
	/// <summary>
	/// Vision angle of the tank.
	/// 
	/// Possible values:
	/// 
	/// 0: 45 deg
	/// 1: 75 deg
	/// 2: 90 deg
	/// 3: 115 deg
	/// 4: 130 deg
	/// 
	/// </summary>
	public int VisionAngle;
	
	/// <summary>
	/// How much damage the tank does.
	/// 
	/// Possible values:
	/// 
	/// 0: 10
	/// 1: 12
	/// 2: 14
	/// 3: 16
	/// 4: 18
	/// 
	/// </summary>
	public int FirePower;
	
	/// <summary>
	/// The fire rate of the tank.
	/// 
	/// Possible values:
	/// 
	/// </summary>
	public int FireRate;
	
	/// <summary>
	/// The armor of the tank.
	/// 
	/// Possible values:
	/// 
	/// 0:
	/// 1:
	/// 2:
	/// 3:
	/// 4:
	/// 
	/// </summary>
	public int MovSpeed;	
	
	/// <summary>
	/// NO USAR
	/// </summary>	
	public int Armor;
	
	/// <summary>
	/// Turn speed of the top part of the tank.
	/// 
	/// Possible values:
	/// 
	/// 0:
	/// 1:
	/// 2:
	/// 3:
	/// 4:
	/// 
	/// </summary>
	public int RadDistance;			// From 0 to 4 (def: 0)
	
	/// <summary>
	/// Refresh of the radar information.
	///  
	/// Possible values:
	/// 
	/// 0:
	/// 1:
	/// 2:
	/// 3:
	/// 4:
	/// 
	/// </summary>
	public int RadRefresh;			// From 0 to 4 (def: 0)
	
	/// <summary>
	/// The total energy that the tank has.
	/// 
	/// Possible values:
	/// 
	/// 0: 100
	/// 1: 120
	/// 2: 140
	/// 3: 160
	/// 4: 180
	/// </summary>
	public int EnergyTotal;			// From 0 to 4 (def: 0)
	
	
	/// <summary>
	/// The tank shield length in seconds.
	/// 
	/// Possible values:
	/// 
	/// 0: 2 secs.
	/// 1: 2.5 secs.
	/// 2: 3 secs.
	/// 3: 3.5 secs.
	/// 4: 4 secs.
	/// </summary>
	public int ShieldDuration;		// From 0 to 4 (def: 0)
	
	/// <summary>
	/// Number of seconds to have the shield available again
	/// 
	/// Possible values:
	/// 
	/// 0: 20 secs.
	/// 1: 15 secs.
	/// 2: 12 secs.
	/// 3: 10 secs.
	/// 4: 8 secs.
	/// 
	/// </summary>
	public int ShieldRechargeSpd;		// From 0 to 4 (def: 0)
	
	/// <summary>
	/// Returns the real speed of the tank
	/// </summary>
	public float GetSpeed()
	{
		return (MovSpeed + 1) * MIN_MOV_SPEED;
	}
	
	/// <summary>
	/// Returns the initial energy of the tank depending on the EnergyTotal
	/// </summary>
	public float GetEnergy()
	{
		switch (EnergyTotal)
		{
		case 1:
			return 120;
		case 2:
			return 140;
		case 3:
			return 160;
		case 4:
			return 180;
		default:
			return 100;
		}
	}
	
	
	/// <summary>
	/// Returns the time for the next shoot. It is related to the FireRate
	/// </summary>
	public float GetTimeForNextShoot()
	{
		switch (FireRate)
		{
		case 1:
			return 4;
		case 2:
			return 3;
		case 3:
			return 2;
		case 4:
			return 1;
		default:
			return 5;		
		}
	}
	
	/// <summary>
	/// Returns the power of the shoot
	/// </summary>
	public float GetFirePower()
	{
		switch (FirePower)
		{
		case 1:
			return 40;
		case 2:
			return 60;
		case 3:
			return 80;
		case 4:
			return 100;
		default:
			return 25;
		}
	}
	
	/// <summary>
	/// Returns the distance of sight
	/// </summary>
	public float GetMaxDistance()
	{
		switch (VisionDistance)
		{
		case 1:
			return 45 * 10;
		case 2:
			return 60 * 10;
		case 3:
			return 75 * 10;
		case 4:
			return 100 * 10;
		default:
			return 30 * 10;
		}
	}
	
	public float GetDistanceAngle()
	{
		switch (VisionDistance)
		{
		case 1:
			return 75;
		case 2:
			return 90;
		case 3:
			return 115;
		case 4:
			return 130;
		default:
			return 45;
		}
	}
	
	public float GetRadRefresh()
	{
		switch (RadRefresh)
		{
		case 1:
			return 4;
		case 2:
			return 3;
		case 3:
			return 2;
		case 4:
			return 1;
		default:
			return 5;
		}
	}
	
	public float GetRadarDistance()
	{
		switch (RadDistance)
		{
		case 1:
			return 45 * 10;
		case 2:
			return 60 * 10;
		case 3:
			return 75 * 10;
		case 4:
			return 100 * 10;
		default:
			return 30 * 10;
		}
	}
	
	public float GetArmor()
	{
		switch (Armor)
		{
			case 1:
			return 10;
		case 2:
			return 20;
		case 3:
			return 30;
		case 4:
			return 50;
		default:
			return 0;
		}
	}
	
	public float GetShieldDuration()
	{
		switch (ShieldDuration)
		{
			case 1:
			return 4;
		case 2:
			return 5;
		case 3:
			return 6;
		case 4:
			return 7;
		default:
			return 3;
		}
	}
	
	public float GetShieldRechargeSpd()
	{
		switch (ShieldRechargeSpd)
		{
			case 1:
			return 18;
		case 2:
			return 16;
		case 3:
			return 14;
		case 4:
			return 10;
		default:
			return 20;
		}
	}


}
