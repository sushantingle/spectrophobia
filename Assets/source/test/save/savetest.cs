using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class savetest : MonoBehaviour {
    public static List<CardDataBase> cardlist = new List<CardDataBase>();

    // Use this for initialization
    private void Awake()
    {
        loadData();
    }

    private void Start()
    {
        CardDataBase card = new CardDataBase();
        card.m_enemyType = (EnemyManager.ENEMY_TYPE) Random.Range((int)EnemyManager.ENEMY_TYPE.ENEMY_LINEAR, (int)EnemyManager.ENEMY_TYPE.ENEMY_COUNT - 1);
        card.m_npcType = (CardDataBase.NPC_TYPE)Random.Range((int)CardDataBase.NPC_TYPE.NPC_NONE, (int)CardDataBase.NPC_TYPE.NPC_COUNT);
        CustomDebug.Log("Card Enemy Type : " + card.m_enemyType);
        CustomDebug.Log("Card NPC type : " + card.m_npcType);
        cardlist.Add(card);
    }

    private void OnDestroy()
    {
        saveData();
    }

    void saveData()
    {
        CustomDebug.Log("Writing Data : ");
        foreach(CardDataBase obj in cardlist)
        {
            CustomDebug.Log("EnemyType : " + obj.m_enemyType);
            CustomDebug.Log("NPC type : " + obj.m_npcType);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream f = File.Create(Application.persistentDataPath + "/carddata.dat");
        bf.Serialize(f, cardlist);
        f.Close();

        CustomDebug.Log("File Created");
    }

    void loadData()
    {
        if (File.Exists(Application.persistentDataPath + "/carddata.dat"))
        {
            CustomDebug.Log("Card data file exist");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = File.Open(Application.persistentDataPath + "/carddata.dat", FileMode.Open);
            savetest.cardlist = (List<CardDataBase>)bf.Deserialize(f);
            f.Close();

            CustomDebug.Log("Reading carddata file : ");
            foreach (CardDataBase obj in cardlist)
            {
                CustomDebug.Log("EnemyType : " + obj.m_enemyType);
                CustomDebug.Log("NPC type : " + obj.m_npcType);
            }
        }
        else
        {
            CustomDebug.Log("Carddata File does not exist");
        }
    }
}
