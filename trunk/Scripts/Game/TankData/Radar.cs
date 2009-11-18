// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public struct Radar
{
	/// <summary>
	/// Distance to the object
	/// </summary>
	public float distanceToObject;
	
	/// <summary>
	/// Number of the refresh (in order to know when the distance value is a new one)
	/// </summary>
	public int refreshNumber;
}
