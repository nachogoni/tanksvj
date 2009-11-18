// Mono Framework
using System;

// Unity Framework
using UnityEngine;
using UnityEditor;

public class PathGridMenuItem
{
	
	
	[MenuItem ("Tools/Update Path Grid Map")]
	public static void UpdatePathGridMap()
	{
		GameObject go = GameObject.Find("PathMap");
		
		if (go != null)
		{
			TileMapBhv tm = go.GetComponent(typeof(TileMapBhv)) as TileMapBhv;
			
			tm.tileMap.UpdatePathGrid();
		}
	}
}
