using UnityEngine;
using System.Collections;

public class SoundProp
{
    public SndId id;
    public SndType type;
    public string name;
    public int priority;
    public AudioClip audioClip;
    public bool loop;

    public int volume;      // 0 to 100 -> Converted to 0.0 to 1.0f when it's used


    public SoundProp(SndId pId, string pName, int pPriority)
    {
        id = pId;
        name = pName;
        priority = pPriority;
        type = SndType.SND_FX;
        loop = false;
        volume = 100;
    }

    public SoundProp(SndId pId, string pName, int pPriority, int vol)
    {
        id = pId;
        name = pName;
        priority = pPriority;
        type = SndType.SND_FX;
        loop = false;
        volume = vol;
    }

    public SoundProp(SndId pId, string pName, int pPriority, bool playInLoop, SndType pType)
    {
        id = pId;
        name = pName;
        priority = pPriority;
        type = pType;
        loop = playInLoop;
        volume = 100;
    }

    public SoundProp(SndId pId, string pName, int pPriority, bool playInLoop, SndType pType, int vol)
    {
        id = pId;
        name = pName;
        priority = pPriority;
        type = pType;
        loop = playInLoop;
        volume = vol;
    }

}
