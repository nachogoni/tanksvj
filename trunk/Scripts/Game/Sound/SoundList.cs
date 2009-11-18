// Mono Framework
using System;
using System.Collections;
using System.Xml;
using System.IO;

// Unity Engine
using UnityEngine;

public enum SndType
{
    SND_FX,
    SND_MUSIC,
}

public enum SndId
{
	SND_ENGINE 						= 0,
	SND_FIRE							= 1,
	SND_TORRET_ENGINE				= 2,
}

/// <summary>
///
/// </summary>
public class SoundList : MonoBehaviour
{
	SoundProp[] sounds = {
		new SoundProp(SndId.SND_ENGINE,          "engine",         1, 100),
		new SoundProp(SndId.SND_FIRE,          	 "fire",         1, 100),
		new SoundProp(SndId.SND_TORRET_ENGINE,   "torretEngine",         1, 100),
		
	};

	public AudioClip[] soundClips;				// All the audioClips, filled in the Unity editor by a programmer or level designer

	void Start()
	{
		// Relate the array entries with the specified audioClip
		for (int i=0; i<soundClips.Length; i++)
		{
			SoundProp sp = getSoundPropByName(soundClips[i].name);
			
			if (sp != null)
			{
				sp.audioClip = soundClips[i];
			}
			else
			{
				Debug.LogWarning(String.Format("Cannot find the sound {0} on the array list of sounds.", soundClips[i].name));
			}
		}

        // TODO: Sacar esto en el release
        //if (Application.platform == RuntimePlatform.OSXPlayer)
        //    parseXMLSoundList();
	}


    bool _reloadingSounds = false;
    float _startTime = 0;
    void Update()
    {
        /*if (Input.GetKeyUp(KeyCode.R))
        {
            _reloadingSounds = true;
            _startTime = Time.realtimeSinceStartup;
            parseXMLSoundList();
        }*/
    }

    void OnGUI()
    {
        /*if (_reloadingSounds)
        {
            if (Time.realtimeSinceStartup - _startTime < 2.0f)
                GUI.Label(new Rect(Screen.width - 200, 20, 200, 22), "Sound clips volumes were updated");
            else
                _reloadingSounds = false;
        }*/
    }

    /*private void parseXMLSoundList()
    {
        XmlTextReader reader;

        if (Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.WindowsEditor)
            reader = new XmlTextReader(Application.dataPath + "/../Deploy/soundlist.xml");
        else
            reader = new XmlTextReader("soundlist.xml");

        Hashtable attributes = new Hashtable();

        // Comienzo a leer el archivo XML de entrada
        while (reader.Read())
        {
            string strName = reader.Name;

            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (strName)
                    {

                        // Tag de recurso
                        case "soundclip":

                            // Meto los atributos en un hashtable
                            if (reader.HasAttributes)
                            {
                                // Limpio el HashTable
                                attributes.Clear();

                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    attributes.Add(reader.Name, reader.Value);
                                }
                            }

                            // Proceso la entrada del recurso
                            setVolumeToClip(attributes["name"].ToString(), Convert.ToInt32(attributes["volume"]));

                            break;

                    }
                    break;
            }
        }

        reader.Close();
    }

    private void setVolumeToClip(string clipName, int volume)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (String.Compare(sounds[i].name, clipName, true) == 0)
            {
                sounds[i].volume = volume;
                return;
            }
        }

        Debug.Log("Sound Clip Not Found. clipName: " + clipName);
    }*/

	public SoundProp getSoundPropByName(string name)
	{
		for (int i=0; i<sounds.Length; i++)
		{
			if (String.Compare(name, sounds[i].name, true) == 0)
			{
				return sounds[i];
			}
		}

		return null;
	}

	public SoundProp GetSoundProp(SndId sndId)
	{
		return sounds[(int) sndId];
	}

    
}



