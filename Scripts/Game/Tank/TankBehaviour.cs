// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


/// <summary>
/// A custom tank AI should inherit from this class and implement at least the following methods:
/// 
/// GetProperties
/// Think
/// 
/// There are other methods that could be inherit, like:
/// 
/// StartThink
/// OnCollide
/// ...
/// 
/// This class implements other helpful methods in order to move the tank,
/// fire and more.
///  
/// </summary>
public class TankBehaviour : MonoBehaviour
{
	// ---------------------------------------------------
	// Public Properties
	
	/// <summary>
	/// Referenced in the editor. The base GameObject of the tank
	/// </summary>
	public GameObject baseGO;
	
	/// <summary>
	/// Referenced in the editor. The torret GameObject of the tank
	/// </summary>
	public GameObject torretGO;
	
	
	/// <summary>
	/// The tank name
	/// </summary>
	public string tankName;
	
	// ---------------------------------------------------
	// Private Properties
	
	private Transform firePoint;
	private Renderer shieldRenderer;
	
	/// <summary>
	/// The tank straight movement
	/// </summary>
	private TankMovement tankMov;
	private TankMovementPF tankMovPF;
	
	/// <summary>
	/// Attack functions
	/// </summary>
	private TankTorret tankTorret;
	
	/// <summary>
	/// Tank Properties
	/// </summary>
	private TankProperties tp;
	
	/// <summary>
	/// The tank is disqualified
	/// </summary>
	private bool disqualified = false;
	
	
	private bool isApplyingShield = false;
	
	/// <summary>
	/// Energy left of this tank
	/// </summary>
	private float energyLeft;
		
	/// <summary>
	/// The remaining time to could perform another shoot
	/// </summary>
	private float timeForNextShoot;
	
	/// <summary>
	/// The remaining time to perform a new radar operation
	/// </summary>
	private float timeForNextRadarValue;

	/// <summary>
	/// Tank Shield
	/// </summary>
	private TankShield shield;
	
	// ---------------------------------------------------
	// Protected Properties
	// This information is visible by the tanks AI
	
	/// <summary>
	/// The radar information
	/// </summary>
	protected Radar radarInfo;
	
	/// <summary>
	/// The Vision information
	/// </summary>
	protected TankInfo[] visionInfo;
	
	/// <summary>
	/// Shield information
	/// </summary>
	protected Shield shieldInfo;
	
	public float EnergyLeft
	{
		get { return energyLeft; }	
	}
	
	public float TimeForNextShoot
	{
		get { return timeForNextShoot; }
	}
		
	public float Energy
	{
		get { return energyLeft; }
	}
	
	/// <summary>
	/// Returns true if the tank is disqualified
	/// </summary>
	public bool IsDisqualified
	{
		get { return disqualified; }
	}
	
	/// <summary>
	/// The map of the level
	/// </summary>
	protected TileMap map;
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start()
	{
		// Get the props of the tank
		tp = GetProperties();
		
		// Find the fire point
		firePoint = transform.Find("tank_weapon/FirePoint");
		
		if (map == null) getMap(); 
	
		string reason;
		if (!PropertyVerifier.Verify(tp, out reason))
		{
			disqualified = true;
			
			Debug.Log("Disqualified by " + reason);
			
			// Hide the gameObject
			gameObject.SetActiveRecursively(false);
			
			// Ok, the tank was disqualified
			OnDisqualified(reason);
		}
		else
		{
			
			energyLeft = tp.GetEnergy();

			// The tank could perform a shoot (because the timeleft is zero)
			timeForNextShoot = 0;
			
			tankMov = new TankMovement(gameObject.GetComponent<CharacterController>(), tp, baseGO);
			tankMovPF = new TankMovementPF(gameObject.GetComponent<CharacterController>(), tp, baseGO);
			tankTorret = new TankTorret(tp, torretGO, firePoint);
			
			shield = new TankShield(transform.Find("Shield").renderer, tp.GetShieldDuration(), tp.GetShieldRechargeSpd());
			shield.SetShieldNotificationAvailable(notifyShieldAvailable);
			shield.SetShieldNotificationOnShutdown(notifyShieldFinish);
		}
		
		// Set the name of the object, if it was not setted
		if (tankName == "")
		{
			tankName = name;
		}
		
		
		
		timeForNextShoot = 0;
		
		timeForNextRadarValue = tp.GetRadRefresh();

		
		
	}
	
	
	/// <summary>
	/// GUI drawing. Only for debugging purposes
	/// </summary>
	void OnGUI()
	{
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update()
	{
		if (!disqualified)
		{
			// Update the movement of the tank
			tankMov.MoveUpdate();
			tankMovPF.MoveUpdate();
			
			// Update the fire function
			tankTorret.FireUpdate();
		}
	}
	
	// Methods to control the tank
	
	private void getMap()
	{
		// Get the map
		GameObject mapGO = GameObject.FindGameObjectWithTag("Map");
		TileMapBhv tmb = mapGO.GetComponent<TileMapBhv>();
		map = tmb.tileMap;
	}
	
	#region Tank Movement
	/// <summary>
	/// Specifies a tank movement using a direction (use only
	/// the X and Z components)
	/// </summary>
	public void Move(Vector3 dir)
	{
		tankMov.Move(dir);
	}
	
	/// <summary>
	/// Specifies a tank movement using a direction (use only
	/// the X and Z components) and a distance.
	/// </summary>
	public void Move(Vector3 dir, float distance)
	{
		tankMov.Move(dir, distance);
	}
	
	/// <summary>
	/// Specifies a tank movement using a direction (use only
	/// the X and Z components) and a distance.
	/// When the tank reach the specified distance, the specified event is called.
	/// </summary>
	public void Move(Vector3 dir, float distance, MoveFinish mf)
	{
		tankMov.Move(dir, distance, mf);
	}
	
	public void MoveToPos(Vector3 pos)
	{
		tankMov.MoveToPos(pos);
	}
	
	public void MoveToPos(Vector3 pos, MoveFinish mf)
	{
		tankMov.MoveToPos(pos, mf);
	}
	
	/// <summary>
	/// Specifies a tank movement using a position of the map to go.
	/// When the tank reach the specified distance, the specified event is called.
	/// </summary>
	public void MoveTo(int row, int col, MoveFinish mf)
	{
		tankMov.MoveTo(row, col, mf);
	}
	
	/// <summary>
	/// Start moving toward the tank is facing.
	/// </summary>
	public void MoveForward()
	{
		tankMov.MoveForward();
	}
	
	/// <summary>
	/// Start moving toward the opposite direction the tank is facing.
	/// </summary>
	public void MoveBackward()
	{
		tankMov.MoveBackward();
	}
	
	public void MoveToUsingPF(int row, int col)
	{
		tankMovPF.MoveToUsingPF(row, col);
	}
	
	public void MoveToUsingPF(int row, int col, MoveFinish mf)
	{
		tankMovPF.MoveToUsingPF(row, col, mf);
	}
	
	/// <summary>
	/// Stop the tank
	/// </summary>
	public void Stop()
	{
		tankMov.Stop();
		tankMovPF.Stop();
	}
	
	/// <summary>
	/// Rotate clockwise the specified angle
	/// </summary>
	public void Rotate(float ang)
	{
		tankMov.Rotate(ang);
	}
	
	/// <summary>
	/// Rotate clockwise the specified angle.
	/// When the tank reach the specified angle movement, the specified event is called.
	/// </summary>
	public void Rotate(float ang, MoveFinish mf)
	{
		tankMov.Rotate(ang, mf);
	}
	#endregion
	
	#region Tank Attack
	/// <summary>
	/// Fire to the specified position.
	/// TODO: Not yet fully implemented.
	/// </summary>
	public void Fire(Vector3 pos, FireFinish ff)
	{
		if (timeForNextShoot == 0)
		{
			tankTorret.Fire(pos, ff);
			
			timeForNextShoot = tp.GetTimeForNextShoot();
		}
	}
	#endregion
	
	#region Turret Rotation
	public void RotateTorret(float deg)
	{
		tankTorret.Rotate(deg);
	}
	
	public void RotateTorret(float deg, RotateFinish rf)
	{
		tankTorret.Rotate(deg, rf);
	}
	#endregion
	
	#region Shield functions
	/// <summary>
	/// Activate the shield if it's possible. Returns true if the shield was activated.
	/// </summary>
	public bool ActivateShield()
	{
		return shield.Activate();
	}
	
	/// <summary>
	/// Returns if the tank can enable the shield
	/// </summary>
	public bool CanEnableShield()
	{
		return shield.CanEnableShield();
	}
	

	#endregion
	
	
	#region Update Information Functions
	/// <summary>
	/// Update the values of the tank
	/// </summary>
	public void UpdateShootTime()
	{
		// Update timeForNextShoot
		if (timeForNextShoot > 0)
		{
			timeForNextShoot -= Time.deltaTime;
			
			if (timeForNextShoot < 0)
				timeForNextShoot = 0;
		}
	}
	
	/// <summary>
	/// Update the radar information
	/// </summary>
	public void UpdateRadarInfo(GameObject theDev)
	{
		if (timeForNextRadarValue > 0)
		{
			timeForNextRadarValue -= Time.deltaTime;
			
			if (timeForNextRadarValue < 0)
				timeForNextRadarValue = 0;
			
		}
		
		if (timeForNextRadarValue == 0)
		{
		
			float dis = Vector3.Distance(transform.position, theDev.transform.position);
			
			if (dis <= tp.GetRadarDistance())
			{
				// Calculate the distance to the object
				radarInfo.distanceToObject = Vector3.Distance(transform.position, theDev.transform.position);
			}
			else
				// Device out of radar
				radarInfo.distanceToObject = float.MaxValue;
			
			// Increase the refresh number
			radarInfo.refreshNumber++;
			
			timeForNextRadarValue = tp.GetRadRefresh();
		}
	}
	
	/// <summary>
	/// Update sight info
	/// </summary>
	public void UpdateSightInfo(TankBehaviour[] bots)
	{
		TankInfo[] vi = new TankInfo[bots.Length];
		int curTank = 0;
		
		// Update the sight info
		// 1. Check the distance to all the other tanks
		for (int i=0; i<bots.Length; i++)
		{
			if (bots[i] != this)
			{
				if (checkDistance(bots[i]))
				{
					// 2. Check the angle
					if (checkAngle(bots[i]))
					{
						// 3. Check occlusion
						if (checkRay(bots[i]))
						{
							// Ok, I can see the tank. Add the information to the sight structur
							vi[curTank].name = bots[i].tankName;
							vi[curTank].position = bots[i].transform.position;
							vi[curTank].energyLeft = bots[i].energyLeft;
							
							curTank++;
						}
					}
				}
			}
		}
		
		// Copy all the elements to the final array
		visionInfo = new TankInfo[curTank];
		for (int i=0; i<curTank; i++)
		{
			visionInfo[i] = vi[i];
		}
		
	}
	
	/// <summary>
	/// Checks if the bot1 can see bot2 by distance
	/// </summary>
	private bool checkDistance(TankBehaviour otherBot)
	{
		return (Vector3.Distance(transform.position, otherBot.transform.position) <
				tp.GetMaxDistance());
	}
	
	private bool checkAngle(TankBehaviour otherBot)
	{
		float angleOfSight = tp.GetDistanceAngle();
			
		Vector3 vecRay = otherBot.transform.position - torretGO.transform.position;
        Vector2 v1 = Vector2Util.Normalize(new Vector2(vecRay.x, vecRay.z));
        Vector2 v2 = new Vector2(torretGO.transform.forward.x, torretGO.transform.forward.z);

        float angle = 0.0f;

        if (v1 != v2)
            angle = Vector2Util.GetAngleBetweenVectors(v1, v2);


        if ((angleOfSight / -2.0f) <= angle && angle <= (angleOfSight / 2.0f))
		{
			//Debug.Log(String.Format("tank {0} can see {1} angle {2} angleOfSight {3}", tankName, otherBot.tankName, angle, angleOfSight));
			
			return true;
		}
		else
			return false;
	}
						
	private bool checkRay(TankBehaviour otherBot)
	{
		Vector3 dir = new Vector3(otherBot.transform.position.x, firePoint.transform.position.y, otherBot.transform.position.z) - firePoint.transform.position;
		
		RaycastHit hit;
		
		//Debug.DrawLine(firePoint.transform.position, new Vector3(otherBot.transform.position.x, firePoint.transform.position.y, otherBot.transform.position.z), Color.red); 
		
		// TODO: Layer filtering
		if (Physics.Raycast(firePoint.transform.position, dir, out hit))
		{
			return (hit.transform.gameObject == otherBot.gameObject);
		}
		
		return false;
	}
	
	/// <summary>
	/// Update shield info
	/// </summary>
	public void UpdateShieldInfo()
	{
		shield.UpdateShieldInfo();
	}
	#endregion
	
	
	public float GetFirePower()
	{
		return tp.GetFirePower();
	}
	
	public bool NotifyShooted(float dmg, Vector3 dir)
	{
		if (!shield.IsApplyingShield())
		{
			OnShootReceived(dir);
			
			energyLeft -= Mathf.Clamp(dmg - tp.GetArmor(), 0, float.MaxValue);
			
			if (energyLeft <= 0)
			{
				energyLeft = 0;
				
				// Notify your death
				OnDestroy();
				
				// The tank is death
				gameObject.SetActiveRecursively(false);
			}
			
			return true;
		}
		else
		{
			OnShootShieldReceived(dir);
			
			return false;
		}
	}
	
	
	/// <summary>
	/// Oops. The tank hits something.
	/// </summary>
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (hit.transform.name != "Terrain")
		{
			tankMov.Stop();
				
			OnCollide(hit);
		}
	}
	
	private void notifyShieldFinish()
	{
		OnShieldFinish();
	}
	
	public void notifyShieldAvailable()
	{
		OnShieldAvailable();
	}
	
		
	/// <summary>
	/// You should override this method if you want to personalize your tank!
	/// This method is called before any other for this tank.
	/// </summary>
	public virtual TankProperties GetProperties()
	{
		return new TankProperties();
	}
	
	/// <summary>
	/// First method called once after GetProperties. Here you could
	/// initialize some of your structures.
	/// </summary>
	public virtual void StartThink() {}
	
	/// <summary>
	/// This method sould be overridden in order to add a custom behaviour to the tank
	/// </summary>
	public virtual void Think() {}
	
	/// <summary>
	/// Your tank was destroyed
	/// </summary>
	public virtual void OnDestroy() {}
	
	/// <summary>
	/// The tank collides with other objects (could be other tank or an obstacle of the map)
	/// </summary>
	public virtual void OnCollide(ControllerColliderHit hit) {}
	
	/// <summary>
	/// Your tank was disqualified because it broke some rule (explained in reason)
	/// </summary>
	public virtual void OnDisqualified(string reason) {}
	
	/// <summary>
	/// The shield is no longer active
	/// </summary>
	public virtual void OnShieldFinish() {}
	
	/// <summary>
	/// The shield is available again
	/// </summary>
	public virtual void OnShieldAvailable() {}
	
	/// <summary>
	/// A shoot was received. The direction of the shoot is notified.
	/// </summary>
	public virtual void OnShootReceived(Vector3 dir) {}
	
	/// <summary>
	/// A shoot on the shield was received. No damage was taken. The direction of the shoot is notified.
	/// </summary>
	public virtual void OnShootShieldReceived(Vector3 dir) {}
	
}
