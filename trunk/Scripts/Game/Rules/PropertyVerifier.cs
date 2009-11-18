// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

/// <summary>
/// This class verifies that the properties specified by a tank is correct.
/// </summary>
public class PropertyVerifier
{
	/// <summary>
	/// Verify the tan properties
	/// 
	/// PATO: Implementar esta func.
	/// Si la func. verifica un error, puede especificar de un modo friendly
	/// en errorDesc.
	/// </summary>
	public static bool Verify(TankProperties tp, out string errorDesc)
	{
		errorDesc = String.Empty;
		const int MAX_PROP_VALUE = 4;
		
		
		// Step 1. Check each property if is the correct range.
		if (tp.VisionDistance > MAX_PROP_VALUE)
		{
			errorDesc = "VisionDistance is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.VisionAngle > MAX_PROP_VALUE)
		{
			errorDesc = "VisionAngle is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.FirePower > MAX_PROP_VALUE)
		{
			errorDesc = "FirePower is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.FireRate > MAX_PROP_VALUE)
		{
			errorDesc = "FireRate is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.MovSpeed > MAX_PROP_VALUE)
		{
			errorDesc = "MovSpeed is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.Armor > MAX_PROP_VALUE)
		{
			errorDesc = "Armor is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.RadDistance > MAX_PROP_VALUE)
		{
			errorDesc = "RadDistance is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.RadRefresh > MAX_PROP_VALUE)
		{
			errorDesc = "RadRefresh is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.EnergyTotal > MAX_PROP_VALUE)
		{
			errorDesc = "EnergyTotal is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.ShieldDuration > MAX_PROP_VALUE)
		{
			errorDesc = "ShieldDuration is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
		
		if (tp.ShieldRechargeSpd > MAX_PROP_VALUE)
		{
			errorDesc = "ShieldRechargeSpd is greater than " + MAX_PROP_VALUE.ToString();
			return false;
		}
			
			
		// Step 2. Check the sum of the properties to be under or equal to
		// TankProperties.TOTAL_POINTS
		if (tp.VisionDistance + tp.VisionAngle + tp.FirePower + tp.FireRate + 
		    tp.MovSpeed + tp.Armor + tp.RadDistance + tp.RadRefresh + tp.EnergyTotal +
		    tp.ShieldDuration + tp.ShieldRechargeSpd > TankProperties.TOTAL_POINTS)
		{
			errorDesc = "The total sum of properties is greater than " + TankProperties.TOTAL_POINTS.ToString();
			return false;
		}
		
		return true;
	}
}
