// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public class DetonatorManager : MonoBehaviour
{
	public GameObject basicExplosion;
	public GameObject mushroomExplosion;
	
	private static DetonatorManager _instance;
	
	void Awake()
	{
		_instance = this;
	}
	
	public static void ExplosionAt(Vector3 pos)
	{
		GameObject.Instantiate(_instance.basicExplosion, pos, Quaternion.Euler(Vector3.zero));
	}
	
	public static void FinalExplosionAt(Vector3 pos)
	{
		GameObject.Instantiate(_instance.mushroomExplosion, pos, Quaternion.Euler(Vector3.zero));
	}
}
