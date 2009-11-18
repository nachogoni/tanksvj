// Mono Framework
using System;

// Unity Framework
using UnityEngine;

/// <summary>
/// DecalManager class
/// This class is used to draw decals on walls and other kind of geometries.
///
/// How to use it:
///
/// This class is a singelton, so you don't need to instantiate it. Just invoke
/// one of the flavor of the method AddDecal.
///
/// bool AddDecal(Vector3 ori, Vector3 dir, float distance, Material decalMaterial);
/// bool AddDecal(Ray ray, float distance, Material decalMaterial);
/// bool AddDecal(Vector3 ori, Vector3 dst, Material decalMaterial);
///
/// Every method returns true if there was a hit. You can get the hit information
/// invoking the method GetHit().
///
/// </summary>
public class DecalManager
{
    // Constants
	private const string DECAL_GO_NAME = "Decal";           // Name of the decal game object

    // Private Properties
    private static Ray _ray;                                // Reused ray struct
    private static RaycastHit _hit;                         // Reused Raycasthit struct
    private static bool _applyRanRotation = true;           // Apply a random rotation to the decal
    
    private const int DECAL_MAX_COUNT = 100;
    private static GameObject[] _decalList = new GameObject[DECAL_MAX_COUNT];
    private static int _lastDecalIdxPlaced = -1;

    /// <summary>
    /// Add a decal in the trajectoy of the specified point, direction and distance.
    /// </summary>
    /// <param name="ori">Origin point</param>
    /// <param name="dir">Direction vector</param>
    /// <param name="distance">Distance. Use 0.0f or float.PositiveInfinity to specify Infinity.</param>
    /// <param name="decalMaterial">The material of the splat</param>
    /// <returns>Returns true if there was a collision and an splat was placed.</returns>
    public static bool AddDecal(Vector3 ori, Vector3 dir, float distance, Material decalMaterial)
    {
        _ray.origin = ori;
        _ray.direction = dir;
        
        if (distance == 0.0f) distance = float.PositiveInfinity;

        // Get the place where the decal should be placed
        if (Physics.Raycast(_ray, out _hit, distance))
        {
            placeDecalGeometry(ref _hit, decalMaterial);
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Add a decal in the trajectoy of the specified ray.
    /// </summary>
    /// <param name="ray">Ray</param>
    /// <param name="distance">Distance. Use 0.0f or float.PositiveInfinity to specify Infinity.</param>
    /// <param name="decalMaterial">The material of the splat</param>
    /// <returns>Returns true if there was a collision and an splat was placed.</returns>
    public static bool AddDecal(Ray ray, float distance, Material decalMaterial)
    {
        _ray = ray;

        if (distance == 0.0f) distance = float.PositiveInfinity;

        // Get the place where the decal should be placed
        if (Physics.Raycast(_ray, out _hit, distance))
        {
            placeDecalGeometry(ref _hit, decalMaterial);
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Add a decal in the trajectoy of the specified point origin and point destination.
    /// </summary>
    /// <param name="ori">Origin point in world coordinates</param>
    /// <param name="dst">Destination point in world coordinates</param>
    /// <param name="decalMaterial">The material of the splat</param>
    /// <returns>Returns true if there was a collision and an splat was placed.</returns>
    public static bool AddDecal(Vector3 ori, Vector3 dst, Material decalMaterial)
    {
        _ray.origin = ori;
        _ray.direction = Vector3Util.GetLookAt(ori, dst);
		float distance = Vector3.Distance(ori, dst);
		
        // Get the place where the decal should be placed
        if (Physics.Raycast(_ray, out _hit, distance))
        {
            placeDecalGeometry(ref _hit, decalMaterial);
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Returns the hit point information.
    /// </summary>
    /// <returns>The hit point</returns>
    public static RaycastHit GetHitInfo()
    {
        return _hit;
    }

	public static Vector3 GetHitPosition()
	{
		return new Vector3(_hit.point.x, _hit.point.y, _hit.point.z);
	}
	
	public static Vector3 GetHitNormal()
	{
		return new Vector3(_hit.normal.x, _hit.normal.y, _hit.normal.z);
	}

    /// <summary>
    /// Draw a debug line using the last ray used by a AddDecal method.
    /// </summary>
    public static void DrawDebugLastRay()
    {
       Debug.DrawLine(_ray.origin, _ray.GetPoint(100.0f));

       
    }

    /// <summary>
    /// Apply random rotations to decals.
    /// </summary>
    /// <param name="val">Apply random rotations to decals?</param>
    public static void ApplyRandomRotationToDecals(bool val)
    {
        _applyRanRotation = val;
    }

    /// <summary>
    /// Place a decal in the specified hit point
    /// </summary>
    /// <param name="hit">The hit point</param>
    private static void placeDecalGeometry(ref RaycastHit hit, Material decalMaterial)
    {
		// HACK: Por ahora si el objeto colisionado tiene un rb, no aplico
		// el decal (ya que el objeto puede moverse y el decal no se
		// attachea al mismo)
		if (hit.rigidbody != null) return;
        
        //Debug.Log(String.Format("object layer: {0}", hit.collider.gameObject.layer));
        
	
		PlaceDecal(hit.point, hit.normal, decalMaterial);
       
    }

	public static void PlaceDecal(Vector3 pos, Vector3 normal, Material decalMaterial)
    {
        // Create an empty game object
        GameObject go = new GameObject(DECAL_GO_NAME);

        // Add the mesh filter
        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;

        // Add the Mesh Renderer
        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        // Set the Material
        mr.material = decalMaterial;
        mr.castShadows = false;
        mr.receiveShadows = false;

        mf.mesh = Primitive.CreateQuad(0.15f, DECAL_GO_NAME);

        // The decal go position is at the hit point
        go.transform.position = new Vector3(pos.x, pos.y, pos.z);
        
        // Rotate the quad using the rotation taken form the normals
        if (!_applyRanRotation)
            go.transform.rotation = Quaternion.FromToRotation(mf.mesh.normals[0], normal);
        else
            go.transform.rotation = Quaternion.FromToRotation(mf.mesh.normals[0], normal) * Quaternion.AngleAxis(UnityEngine.Random.value * 360.0f, Vector3.forward);


        // Overwritte the normals using the normal of the hitted object
        Vector3[] normals = new Vector3[4];
        normals[0] = new Vector3(normal.x, normal.y, normal.z);
        normals[1] = new Vector3(normal.x, normal.y, normal.z);
        normals[2] = new Vector3(normal.x, normal.y, normal.z);
        normals[3] = new Vector3(normal.x, normal.y, normal.z);

        mf.mesh.normals = normals;
        
        _lastDecalIdxPlaced++;
        if (_lastDecalIdxPlaced == DECAL_MAX_COUNT)
            _lastDecalIdxPlaced = 0;
        
        // Destroy the previous decal
        if (_decalList[_lastDecalIdxPlaced] != null)
            GameObject.Destroy(_decalList[_lastDecalIdxPlaced]);
            
        _decalList[_lastDecalIdxPlaced] = go;

	}
    
    /// Clear all decals of the level
    public static void Clear()
    {
        for (int i=0; i<DECAL_MAX_COUNT; i++)
            if (_decalList[i] != null)
                GameObject.Destroy(_decalList[i]);
                
        _lastDecalIdxPlaced = -1;
    }
}
