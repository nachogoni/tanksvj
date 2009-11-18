// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public class ProfilerGUI : MonoBehaviour
{
	private bool en = false;
	
	// Use this for initialization
	void Start()
	{
		Profiler.enabled = true;
		Profiler.logFile = "./Profiler.log";
	}
	
	void OnGUI()
	{
		if (!en)
		{
			if (GUI.Button(new Rect(10, 10, 120, 40), "Start"))
			{
				Profiler.BeginSample("Test1");
				en = true;
			}
		}
		else
		{
			if (GUI.Button(new Rect(10, 10, 120, 40), "Stop"))
			{
				Profiler.EndSample();
				en = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update()
	{	
	}
}
