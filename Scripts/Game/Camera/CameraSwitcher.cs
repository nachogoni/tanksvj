// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;


public class CameraSwitcher : MonoBehaviour
{
	public Camera[] cameras;
	private int current;
	
	// Use this for initialization
	void Start()
	{
		
				
		current = 0;
		
		cameras[0].enabled = true;
		
		for (int i=1; i<cameras.Length; i++)
			cameras[i].enabled = false;
		
		
		/*UnityEngine.Object[] objs = GameObject.FindObjectsOfType(typeof(Camera));
		
		cameras = new Camera[objs.Length];
		
		for (int i=0; i>objs.Length; i++)
		{
			cameras[i] = objs[i] as Camera;
			
			cameras[i].enabled = (cameras[i].tag == "MainCamera");
			
			if (cameras[i].enabled)
				current = i;
		}*/
	}
	
	// Update is called once per frame
	void OnGUI()
	{	
		if (GUI.Button(new Rect(Screen.width - 120, 0, 120, 30), "Switch Camera"))
		{
			cameras[current].enabled = false;
			current++;
			current = current % cameras.Length;
			cameras[current].enabled = true;
		}
	}
}
