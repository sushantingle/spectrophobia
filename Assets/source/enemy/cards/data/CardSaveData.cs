using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CardSaveData : MonoBehaviour {
    public static Dictionary<CardDataBase.CARD_TYPE, CardDataBase> m_cardDataList = new Dictionary<CardDataBase.CARD_TYPE, CardDataBase>();

    // Use this for initialization
    private void Awake()
    {
        loadData();
    }

    private void OnDestroy()
    {
        saveData();
    }

    void saveData()
    {
        CustomDebug.Log("Writing Data : ");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream f = File.Create(Application.persistentDataPath + "/carddata.dat");
        bf.Serialize(f, m_cardDataList);
        f.Close();

        CustomDebug.Log("File Created : "+m_cardDataList.Count);
        foreach(CardDataBase card in m_cardDataList.Values)
        {
            CustomDebug.Log("Card EnemyType : " + card.m_enemyType);
            CustomDebug.Log("Card NPCType : " + card.m_npcType);
            CustomDebug.Log("Card Health : " + card.m_health);
        }
    }

    void loadData()
    {
        if (File.Exists(Application.persistentDataPath + "/carddata.dat"))
        {
            CustomDebug.Log("Card data file exist");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = File.Open(Application.persistentDataPath + "/carddata.dat", FileMode.Open);
            CardSaveData.m_cardDataList = (Dictionary<CardDataBase.CARD_TYPE, CardDataBase>)bf.Deserialize(f);
            f.Close();

            CustomDebug.Log("Reading carddata file : "+m_cardDataList.Count);
            foreach (CardDataBase card in m_cardDataList.Values)
            {
                CustomDebug.Log("Card EnemyType : " + card.m_enemyType);
                CustomDebug.Log("Card NPCType : " + card.m_npcType);
                CustomDebug.Log("Card Health : " + card.m_health);
            }
        }
        else
        {
            CustomDebug.Log("Carddata File does not exist");
        }
    }
}
