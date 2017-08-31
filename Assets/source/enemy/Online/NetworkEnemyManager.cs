using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkEnemyManager : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [Command]
    public void Cmd_SpawnEnemy(EnemyManager.ENEMY_TYPE type, Player.Player_Team team, Vector3 pos, NetworkInstanceId netId)
    {
        CustomDebug.Log("Command Spawn Enemy");
        EnemyManager.getInstance().CommandSpawnEnemy(type, team, pos, netId);
    }

    [Command]
    public void Cmd_spawnSpawnerChild(NetworkInstanceId parentNetId, EnemyManager.ENEMY_TYPE type, Vector3 pos, NetworkInstanceId netId)
    {
        GameObject parentObject = NetworkServer.FindLocalObject(parentNetId);
        if (parentObject != null)
        {
            parentObject.GetComponent<SpawnerEnemy>().CommandSpawnChild(type, pos, netId);
        }
        else
        {
            CustomDebug.Log("Spawner not found on host");
        }
    }
}
