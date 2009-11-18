// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

public class BulletMotor : MonoBehaviour
{
    public float MAX_TIME_ON_AIR_SECS = 2;  // Max. time in air (seconds)

    private float _lifeTime;                // The current bullet life time in seconds

    private float _v0;                      // Initial velocity (in the future related to the marker)
    private float _t;                       // Accum. time from the shooting in ms. (used for the bullet)
    private float _shootAngle;              // Shoot angle (local pitch)
    private Vector3 _prevPosition;          // The position of the bullet in the prev. update
    private GameObject _shooter;            // Who is the shooter?
    private Transform _originalTransform;   // Store the current transform at the shooting moment (keep pos, scale and orientation)
    private SplatType _decalMaterialId;		// The decal id
	private Material _decalMaterial;        // The decal (could be obtained shoot per shoot, stored for performance purposes)

    private float _bulletTimeDivider;       // Bullet Time Divider (taken from BulletManager)

	private Vector3 _shootPos;
	
    int _idxPointToDraw;                    // DEBUG
	Vector3[] _lines;                       // DEBUG
   
	void Start()
    {
        _bulletTimeDivider = BulletManager.instance.bulletTimeDivider;
	}
	
	public void Fire(GameObject owner, Vector3 shootPos, Vector3 dir)
	{
		// Clear the particles (perhaps the bullet is being reused)
        //particleEmitter.ClearParticles();

        // The bullet is active
        gameObject.active = true;

		_shooter = owner;
		
		// Set the transformation (the bullet always starts from the gun)
		transform.position = shootPos;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0.0f, dir.z));
        
        // Store the original transform
        _originalTransform = transform;

        // Calculate the shoot angle (local pitch)
        _shootAngle = 90 - Vector3.Angle(dir, Vector3.up);

		_shootPos = shootPos;
		
        // Store the initial values
        _lifeTime = 0.0f;
		//_shooter = shooter;
        _t = 0;
        _v0 = BulletManager.instance.bulletInitialSpeed;
        _prevPosition = shootPos;
		
	}
	

	private void fire(Vector3 shootPos, Vector3 dir, SplatType splatId, float spd)
	{
		
		// Clear the particles (perhaps the bullet is being reused)
        particleEmitter.ClearParticles();

        // The bullet is active
        gameObject.active = true;

		// Set the transformation (the bullet always starts from the gun)
		transform.position = shootPos;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0.0f, dir.z));
        
        // Store the original transform
        _originalTransform = transform;

        // Calculate the shoot angle (local pitch)
        _shootAngle = 90 - Vector3.Angle(dir, Vector3.up);

        // Store the initial values
        _lifeTime = 0.0f;

		_t = 0;
        _v0 = spd;
        _prevPosition = shootPos;
		
		_decalMaterialId = splatId;
        _decalMaterial = SplatTypes.GetSplatType(splatId);
		
	}
	
	
	
	/// <summary>
    /// Update is called once per frame
	/// </summary>
	public void Update()
    {
	
        drawDebug();

        _lifeTime += Time.deltaTime;

        float bulletDeltaT = (Time.deltaTime / _bulletTimeDivider);

        // Kill the bullet?
        if (_lifeTime >= MAX_TIME_ON_AIR_SECS)
        {
            gameObject.active = false;
            return;
        }

        // ---
        // Locate the particles from the bullet pos to 10 prev positions
        /*particleEmitter.ClearParticles();
        float incT = bulletDeltaT / 20.0f;
        for (float t = _t + bulletDeltaT ; t > _t; t -= incT)
        {
            float xp = _v0 * Mathf.Cos(_shootAngle * Mathf.Deg2Rad) * t;
            float yp = _v0 * Mathf.Sin(_shootAngle * Mathf.Deg2Rad) * t - (0.5f * (Physics.gravity.y * -1) * t * t);
            transform.position = _originalTransform.TransformPoint(new Vector3(0.0f, yp, xp));
            particleEmitter.Emit(1);
        }*/

        // Calculate the local position of the bullet (from the shooter)
        float x = _v0 * Mathf.Cos(_shootAngle * Mathf.Deg2Rad) * (_t + bulletDeltaT);
        float y = _v0 * Mathf.Sin(_shootAngle * Mathf.Deg2Rad) * (_t + bulletDeltaT) - (0.5f * (Physics.gravity.y * -1) * (_t + bulletDeltaT) * (_t + bulletDeltaT));

        // Set the transform of the game object
        transform.position = _originalTransform.TransformPoint(new Vector3(0.0f, y, x));

		// The bullet hits something?
    		if (DecalManager.AddDecal(_prevPosition, transform.position, _decalMaterial))
    		{
		
			// Get information about the decal hit
			RaycastHit rh = DecalManager.GetHitInfo();

			if (rh.transform.tag == "Tank")
			{
				// Ok, I hit a tank
				// Notify the tanks
				
				//Debug.Log(String.Format("The shooter is: {0}", _shooter.name));
				
				TankBehaviour tbShooter = _shooter.GetComponent<TankBehaviour>();
				TankBehaviour tbShooted = rh.transform.gameObject.GetComponent<TankBehaviour>();
				
			
				// Calculate the bullet direction
				Vector3 dir = transform.position - _shootPos;
				dir.Normalize();
								
				TankManager.NotifyHit(tbShooter, tbShooted, dir);
				
			}
			
            	// The object is no longer active
            	gameObject.active = false;
		   	    	

			// Play a sound in the collide object
			playSoundInCollideObject(rh);

			return;
		}


        // Accum. delta t
        _t += bulletDeltaT;

        // Store the actual the position
        _prevPosition = transform.position;
	}
	
	private void playSoundInCollideObject(RaycastHit rh)
	{
		
		if (rh.rigidbody != null)
		{
			AudioSource asrc = rh.rigidbody.gameObject.GetComponent(typeof(AudioSource)) as AudioSource;
			
			if (asrc != null)
				asrc.Play();
		}
		else if (rh.collider != null)
		{
			AudioSource asrc = rh.collider.gameObject.GetComponent(typeof(AudioSource)) as AudioSource;
			
			if (asrc != null)
				asrc.Play();
		}
	}
	
    /// <summary>
    /// Draw debug lines
    /// </summary>
	private void drawDebug()
	{
		for (int i=1; i<_idxPointToDraw; i++)
			Debug.DrawLine(_lines[i-1], _lines[i]);
	}
}


