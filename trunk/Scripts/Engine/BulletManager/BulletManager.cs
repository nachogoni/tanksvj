// Mono Framework
using System;
using System.Collections;
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

/// <summary>
/// 
/// 
/// Requires:
/// 
/// A GameObject script set the bullet property to the bullet prefab.
/// The GameObject shooter should contain an Avatar script component.
/// 
/// </summary>
public class BulletManager : MonoBehaviour
{
    // Public Properties
    public int initialBulletCount = 0;              // Initial bullet count
    public int maxBulletsOnAir = 20;                // Max. number of bullets (never is going to be more bullets in scene than this num.)
    public float maxBulletTimeOnAirSecs = 10;       // Max. time in air (seconds)
    public float bulletDefaultSpeed = 10;           // The bullet default speed
    public GameObject bulletPrefab;                 // Reference to a bullet to instantiate

    public float bulletTimeDivider = 5.0f;          // SloMo the bullet
    public float bulletInitialSpeed = 20.0f;        // The initial speed of the bullet (affect trajectory)

    public Material redBulletMaterial;              // Red Bullet Material
    public Material blueBulletMaterial;             // Blue Bullet Material

    // Private Properties
	private GameObject[] _bullets;                  // List of bullets
    private int _lastBulletShooted;                 // The index of the last bullet shooted (used to recycle bullets)

    public static BulletManager instance;           // Reference to the first instance

	void Awake()
	{
		instance = this;
	}
	
	void Start()
    {
        
        if (bulletPrefab == null)
        {
            Debug.Log("@BulletManager. bullet is null. Require that a GameObject script set the bullet property to the bullet prefab.");
            return;
        }

		// Create the bullet array
        _bullets = new GameObject[maxBulletsOnAir];

        for (int i = 0; i < initialBulletCount; i++)
        {
            createBullet(i);
        }

        _lastBulletShooted = -1;
	}

    /// <summary>
    /// Create a bullet in the specified index of the array.
    /// </summary>
    /// <param name="idx"></param>
    private void createBullet(int idx)
    {
        // Create a new object
        _bullets[idx] = GameObject.Instantiate(bulletPrefab) as GameObject;
		
		_bullets[idx].name = bulletPrefab.name;
      
        // Add a mesh filter
        //_bullets[idx].AddComponent(typeof(MeshFilter));

        // Add the Mesh Renderer
        //MeshRenderer mr = _bullets[idx].AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        // Assign the material related to the pellet type (red or blue)
        //ParticleRenderer pr = _bullets[idx].GetComponent(typeof(ParticleRenderer)) as ParticleRenderer;
        //pr.material = redBulletMaterial;
        //mr.material = redBulletMaterial;

        _bullets[idx].active = false;
		

    }

    /// <summary>
    /// Shrink the bullet array to the initial number
    /// </summary>
    public void ShrinkArray()
    {
        for (int i = initialBulletCount; i < maxBulletsOnAir; i++)
            _bullets[i] = null;
    }


	
	public void Fire(GameObject owner, Vector3 shootPos, Vector3 dir)
	{
        // Find a idle bullet or take the older
        int idx = getBullet();

        BulletMotor bb = _bullets[idx].GetComponent(typeof(BulletMotor)) as BulletMotor;
		bb.Fire(owner, shootPos, dir);
	}

	
    /// <summary>
    /// Return an index of a bullet to shoot
    /// </summary>
    /// <returns>The index of the bullet</returns>
    private int getBullet()
    {
        for (int i = 0; i < maxBulletsOnAir; i++)
        {
            // The bullet was not yet created?
            if (_bullets[i] == null)
            {
                createBullet(i);
                _lastBulletShooted = i;
                return i;
            }

            // The bullet is ready for reuse?
            else if (!_bullets[i].active)
            {
                _lastBulletShooted = i;
                return i;
            }
        }

        // Take the older shooted bullet
        _lastBulletShooted++;
        if (_lastBulletShooted == maxBulletsOnAir) _lastBulletShooted = 0;
        return _lastBulletShooted;
    }
	
	
}
