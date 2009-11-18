// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public class DeviceCheck : MonoBehaviour
{
	void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.tag == "Tank")
		{
			TankBehaviour tb = c.gameObject.GetComponent<TankBehaviour>();
			
			Debug.Log(String.Format("The winner is the tank: {0}", tb.tankName));
		}
	}
}
