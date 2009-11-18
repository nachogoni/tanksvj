// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

public delegate void ShieldNotification();

public class TankShield
{

	/// <summary>
	/// The remaining time to perform a new shield activation
	/// </summary>
	private float timeForShield;
	
	/// <summary>
	/// Time accum. for shield avail. calculus 
	/// </summary>
	private float shieldAccumTime;
	
	/// <summary>
	/// Reference to the renderer of the shield gameObject
	/// </summary>
	private Renderer shieldRenderer;
	
	/// <summary>
	/// Duration in secs. of the shield
	/// </summary>
	private float shieldDuration;
	
	/// <summary>
	/// Shield recharge speed in seconds
	/// </summary>
	private float shieldRechargeSpd;
	
	
	private ShieldNotification shieldShutdown;
	private ShieldNotification shieldIsAvailable;
	
	/// <summary>
	/// Constructor
	/// </summary>
	public TankShield(Renderer shieldRnd, float duration, float rechargeSpd)
	{
		shieldRenderer = shieldRnd;
		shieldDuration = duration;
		shieldRechargeSpd = rechargeSpd;
			
		timeForShield = 0;
	}
	
	/// <summary>
	/// Activate the shield if possible.
	/// </summary>
	/// <returns>
	/// Returns true if the shield was activated.
	/// </returns>
	public bool Activate()
	{
		if (timeForShield == 0)
		{
			shieldAccumTime = 0;
			shieldRenderer.enabled = true;
			
			return true;
		}
		else
			return false;
	}
	
	/// <summary>
	/// Returns if the tank can enable the shield
	/// </summary>
	public bool CanEnableShield()
	{
		return (timeForShield == 0);
	}
	
	/// <summary>
	/// Returns true if the shield is being applied
	/// </summary>
	public bool IsApplyingShield()
	{
		return shieldRenderer.enabled;
	}
	
	/// <summary>
	/// Update shield info
	/// </summary>
	public void UpdateShieldInfo()
	{
		if (timeForShield > 0)
		{
			timeForShield -= Time.deltaTime;
			
			if (timeForShield < 0)
			{
				timeForShield = 0;
				
				if (shieldIsAvailable != null)
					shieldIsAvailable();
			}
		}
		
		if (shieldRenderer.enabled)
		{
			shieldAccumTime += Time.deltaTime;
			
			if (shieldAccumTime > shieldDuration)
			{
				shieldRenderer.enabled = false;
				timeForShield = shieldRechargeSpd;
				
				if (shieldShutdown != null)
					shieldShutdown();
			}
		}
		
	}
	
	/// <summary>
	/// Notify me when the shield is being turned off
	/// </summary>
	public void SetShieldNotificationOnShutdown(ShieldNotification onShutdown)
	{
		shieldShutdown = onShutdown;
	}
	
	/// <summary>
	/// Notify me when the shield is available again
	/// </summary>
	public void SetShieldNotificationAvailable(ShieldNotification available)
	{
		shieldIsAvailable = available;
	}
}
