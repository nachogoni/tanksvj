// Mono Framework
using System;

// Mono Framework
using UnityEngine;

public enum SplatType
{
    DEF_RED,
    DEF_BLUE
}

public class SplatTypesGO : MonoBehaviour
{
    public Material defaultSplatRed;
	public Material optionalSplatRed1;
	public Material optionalSplatRed2;
	
    public Material defaultSplatBlue;
    public Material optionalSplatBlue1;
	public Material optionalSplatBlue2;
	
	
    internal static SplatTypesGO _instance;
    
    public void Start()
    {
        _instance = this;
    }
    
}

public class SplatTypes
{
    public static Material GetSplatType(SplatType st)
    {
        SplatTypesGO stgo = SplatTypesGO._instance;
        
        if (stgo == null) return null;
        switch (st)
        {
            case SplatType.DEF_RED:
                return ChooseMaterial(true);
                break;
            
            case SplatType.DEF_BLUE:
                return ChooseMaterial(false);
                break;
            
            default:
                return null;
        }
    }
    
	public static Material ChooseMaterial(bool teamRed)
	{
		int randomSplat = UnityEngine.Random.Range(0,3);
		
		SplatTypesGO stgo = SplatTypesGO._instance;
		
		switch(randomSplat){
			case 0:
				return teamRed ? stgo.defaultSplatRed : stgo.defaultSplatBlue;
				break;
			case 1:
				return teamRed ? stgo.optionalSplatRed1 : stgo.optionalSplatBlue1;
				break;
			case 2:
				return teamRed ? stgo.optionalSplatRed2 : stgo.optionalSplatBlue2;
				break;
			default:
				return null;
		}
	}
}
