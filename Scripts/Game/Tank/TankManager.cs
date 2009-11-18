// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public class TankManager : MonoBehaviour
{
	public GameObject[] tanks;
	public Texture energyBarTex;
	public GUIStyle fontFront;
	public GUIStyle fontBack;
	
	private TankBehaviour[] bots;
	private bool firstMove = true;
	
	private int frameSkipToStart = 3;
	private GameObject theDevice;
	
	private static TankManager _instance;
	
	void Awake()
	{
		_instance = this;
		
		
		theDevice = GameObject.FindGameObjectWithTag("TheDevice");
		if (theDevice == null)
		{
			Debug.Log("There in not a device in the level");
		}
		
				
		bots = new TankBehaviour[tanks.Length];
		
		for (int i=0; i<bots.Length; i++)
		{
			bots[i] = tanks[i].GetComponent<TankBehaviour>();

		}
	}
	
	void OnGUI()
	{
		for (int i=0; i<tanks.Length; i++)
		{
			string tankString;
			
			if (bots[i].IsDisqualified)
			{
				tankString = bots[i].tankName + " (disqualified)";
			}
			else if (bots[i].EnergyLeft == 0)
			{
				tankString = bots[i].tankName + " (death)";
			}
			else
			{
				tankString = bots[i].tankName;
			}
			
			GUI.Label(new Rect(10, 10 + (i*16), 200, 12), tankString, fontBack);
			GUI.Label(new Rect(10, 11 + (i*16), 200, 12), tankString, fontFront);
			
			GUI.DrawTexture(new Rect(100, 10 + (i*16), bots[i].EnergyLeft, 12), energyBarTex);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (frameSkipToStart != 0)
		{
			frameSkipToStart--;
			return;
		}
		
		if (firstMove)
		{
			// Do not put this code in the Start method. Not all the
			// information of the TankBehaviour could be setted (like
			// the map reference)
			
			for (int i=0; i<bots.Length; i++)
			{
				if (!bots[i].IsDisqualified)
					bots[i].StartThink();
			}
			
			firstMove = false;
		}
		else
		{
			
			for (int i=0; i<bots.Length; i++)
			{
				// Think only if the tank is alive
				if (bots[i].EnergyLeft > 0 && !bots[i].IsDisqualified)
				{
					bots[i].UpdateShootTime();
					bots[i].UpdateRadarInfo(theDevice);
					bots[i].UpdateShieldInfo();
					bots[i].UpdateSightInfo(bots);
					
					bots[i].Think();
				}
			}
		}
		
	}
	
	public static void NotifyHit(TankBehaviour tankShooter, TankBehaviour tankShooted, Vector3 bulletDir)
	{
		Debug.Log(String.Format("tank {0} hit tank {1}", tankShooter.tankName, tankShooted.tankName));
		
		// Notify the shooter				
		if (tankShooted.NotifyShooted(tankShooter.GetFirePower(), bulletDir))
		{
			if (tankShooted.EnergyLeft > 0)
				DetonatorManager.ExplosionAt(tankShooted.transform.position);
			else
				DetonatorManager.FinalExplosionAt(tankShooted.transform.position);
		}
		
	}
}