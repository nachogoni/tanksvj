// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


/// <summary>
/// Find all the gameObjects that are flags in the project
/// </summary>
[ExecuteInEditMode]
public class FlagManager : MonoBehaviour
{
	/// <summary>
	/// All the flags in the level (note that index 0 is not the flag number zero!)
	/// </summary>
	private static FlagNum[] flag;
	
	/// <summary>
	/// A reference to the map structure of the level
	/// </summary>
	private static TileMap map;
	
	private static FlagManager instance;

	// Use this for initialization
	void Awake()
	{
		
		instance = this;
		
		// Get the map
		if (map == null) getMap();
		
		
		
		// Get all the flags
		GameObject[] flags = GameObject.FindGameObjectsWithTag("Flag");
		
		FlagNum[] flagTmp = new FlagNum[flags.Length];
		
		for (int i=0; i<flagTmp.Length; i++)
		{
			flagTmp[i] = flags[i].GetComponent<FlagNum>();
		}
		
		// Get the flags in the correct order number
		flag = new FlagNum[flagTmp.Length];
		int num = 0;
		for (int i=0; i<flagTmp.Length; i++)
		{
			int j = GetFlagIndex(flagTmp, num);
			
			if (j != -1)
				flag[i] = flagTmp[j];
			else
				Debug.Log(String.Format("Warning: The number of at least one flag is not correct (looking for flag num: {0})", num));
			
			num++;
		}
		
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		
		if (flag != null && flag.Length > 1)
		{
			for (int i=0; i<flag.Length - 1; i++)
				Gizmos.DrawLine(flag[i].transform.position, flag[i+1].transform.position);
		}
	}
	
	private void getMap()
	{
		// Get the map
		GameObject mapGO = GameObject.FindGameObjectWithTag("Map");
		TileMapBhv tmb = mapGO.GetComponent<TileMapBhv>();
		map = tmb.tileMap;
	}
	
	/// <summary>
	/// Returns the flag index specifying a flag number
	/// </summary>
	private int GetFlagIndex(FlagNum[] flags, int flagNum)
	{
		for (int i=0; i<flags.Length; i++)
		{			
			if (flags[i].num == flagNum)
			{
				return i;
			}
		}
		
		return -1;
	}

	/// <summary>
	/// Returns the total number of flags in the level
	/// </summary>
	public static int GetFlagCount()
	{
		return flag.Length;
	}
	
	/// <summary>
	/// Returns the position of the specified flag
	/// </summary>
	public static Vector3 GetFlagPosition(int i)
	{
		return flag[i].transform.position;
	}
	
	/// <summary>
	/// Returns in the output parameters the row/col of the specified flag.
	/// </summary>
	public static void GetFlagRowCol(int flagNum, out int row, out int col)
	{
		// Get the map
		if (map == null) instance.getMap();
		
		// Get the row/col
		map.GetRowColAtWorldPos(flag[flagNum].transform.position, out row, out col);
	}
}
