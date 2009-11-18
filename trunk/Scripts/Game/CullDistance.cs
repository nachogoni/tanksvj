// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public class CullDistance : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		float[] distances = new float[32];
		distances[23] = 100;
		gameObject.camera.layerCullDistances = distances;
		
	}
	
	// Update is called once per frame
	void Update()
	{	
	}
}
