﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameSettings : MonoBehaviour {

    private const string FILENAME = "gamesettings.dat";

    [System.Serializable]
    struct Data {
        public bool m_autoAim;
        public bool m_soundOn;
    }

    private static Data m_data;

    private void Start()
    {
        loadData();    
    }

    private void OnDestroy()
    {
        saveData();
    }

    private void saveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream f = File.Create(Application.persistentDataPath + FILENAME);
        bf.Serialize(f, m_data);
        f.Close();
    }

    private void loadData()
    {
        if (File.Exists(Application.persistentDataPath + FILENAME))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = File.Open(Application.persistentDataPath + FILENAME, FileMode.Open);
            m_data = (Data)bf.Deserialize(f);
            f.Close();
        }
        else
        {
            CustomDebug.Log("Game Settings File does not exist");
        }
    }

    public static bool AUTOAIM
    {
        get
        {
            return m_data.m_autoAim;
        }
        set
        {
            m_data.m_autoAim = value;
        }
    }

    public static bool SOUNDS_ON
    {
        get
        {
            return m_data.m_soundOn;
        }
        set
        {
            m_data.m_soundOn = value;
        }
    }
}
