// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

/// <summary>
/// Represents a A* base. The unit that manages the algorithm.
/// </summary>
public class AStarBase
{
	public Vector3 pos;               // Position in the map
    public float cost;                // Cost from the start point to this point
    public AStarBase prevBase;        // Previous base
    public AStarBase nextBase;        // Next base
	
    public AStarBase()
    {
    }

    public AStarBase(Vector3 p, float c)
    {
        pos = p;
        cost = c;
        prevBase = null;
        nextBase = null;
    }
}
