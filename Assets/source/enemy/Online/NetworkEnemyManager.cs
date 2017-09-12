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
    public void Cmd_SpawnEnemy(EnemyManager.ENEMY_TYPE type, Player.Player_Team team, Vector3 pos, NetworkInstanceId netId, NetworkInstanceId parentNetId)
    {
        CustomDebug.Log("Command Spawn Enemy");
        EnemyManager.getInstance().CommandSpawnEnemy(type, team, pos, netId, parentNetId);
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
            CustomDebug.LogError("Spawner not found on host");
        }
    }

    [Command]
    public void Cmd_SpawnBoss(EnemyManager.BOSS_TYPE type, Player.Player_Team team, Vector3 pos, NetworkInstanceId netId, NetworkInstanceId parentNetId)
    {
        CustomDebug.Log("On Host Spawn Boss");
        EnemyManager.getInstance().CommandSpawnBoss(type, team, pos, netId, parentNetId);
    }

    [Command]
    public void Cmd_onDeath(NetworkInstanceId netId, bool isBoss)
    {
        CustomDebug.Log("Command On Death : "+netId.Value);
        Rpc_OnDeath(netId, isBoss);
    }

    [ClientRpc]
    public void Rpc_OnDeath(NetworkInstanceId netId, bool isBoss)
    {
        CustomDebug.Log("RPC Enemy On Death : "+netId.Value);
        GameObject deadObject = null;
        if (isServer)
            deadObject = NetworkServer.FindLocalObject(netId);
        else
            deadObject = ClientScene.FindLocalObject(netId);

        if (deadObject != null)
        {
            NetworkInstanceId parentNetId = deadObject.GetComponent<EnemyBase>().getParentNetworkId();
            if (parentNetId == NetworkInstanceId.Invalid)
            {
                CustomDebug.Log("Invalid Parent");
                return;
            }
            GameObject parentObj = ClientScene.FindLocalObject(parentNetId);
            if (parentObj == null)
            {
                CustomDebug.Log("Parent object is null");
                return;
            }

            if (parentObj.GetComponent<Player>() != null && parentObj.GetComponent<Player>().isLocalPlayer == false)
            {
                CustomDebug.Log("Parent is not local player");
                return;
            }

            if (parentObj.GetComponent<EnemyBase>() != null)
            {
                CustomDebug.Log("Parent is enemy");
                // TODO : handle if parent is Enemy
            }
            CustomDebug.Log("Dead object found : " + isBoss);
            if(isBoss)
                EnemyManager.getInstance().onBossDead(deadObject);
            else
                EnemyManager.getInstance().OnEnemyDeath(deadObject);
        }
        else
        {
            CustomDebug.LogError("Boss object not found");
        }
    }

    [Command]
    public void Cmd_destroyObject(NetworkInstanceId netId)
    {
        CustomDebug.Log("Command Destroy Object : " + netId.Value);
        GameObject obj = NetworkServer.FindLocalObject(netId);
        if (obj != null)
        {
            //Destroy(obj);
            CustomDebug.Log("Command Destroy Object : Despawn Object");
            GameManager.getInstance().getNetworkPool().Despawn(obj);
        }
        else
        {
            CustomDebug.LogError("Command Destroy Object : object not found");
        }
    }
}
