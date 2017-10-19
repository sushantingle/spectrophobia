using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameStats : MonoBehaviour {

    private static string FILENAME = "gamedata.dat";

    [System.Serializable]
    struct Data {
        public int m_souls;
        public int m_highScore;
    }

    private static Data m_data;

    private void Start()
    {
        //load game stats
        loadStats();
    }

    private void OnDestroy()
    {
        // write game stats
        saveStats();
    }

    private void saveStats()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream f = File.Create(Application.persistentDataPath + FILENAME);
        bf.Serialize(f, m_data);
        f.Close();
    }

    private void loadStats()
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
            CustomDebug.Log("Game Stats File does not exist");
        }

    }

    public static int SOULS
    {
        get
        {
            return m_data.m_souls;
        }
        set
        {
            m_data.m_souls = value;
        }
    }

    public static int HIGHSCORE
    {
        get
        {
            return m_data.m_highScore;
        }
        set
        {
            m_data.m_highScore = value;
        }
    }
}
