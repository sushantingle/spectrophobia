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

        GameObject deadObject = NetworkServer.FindLocalObject(netId);
        if (deadObject != null)
        {
            CustomDebug.LogError("Enemy object found : "+netId.Value);
            if (isBoss)
                EnemyManager.getInstance().onBossDead(deadObject);
            else
                EnemyManager.getInstance().OnEnemyDeath(deadObject);
        }
        else
        {
            CustomDebug.LogError("Enemy object not found : "+netId.Value);
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

    [Command]
    public void Cmd_AddScore(Player.Player_Team playerTeam, int score)
    {
        CustomDebug.Log("Command Add Score : " + playerTeam);
        Rpc_AddScore(playerTeam, score);
    }

    [ClientRpc]
    public void Rpc_AddScore(Player.Player_Team team, int score)
    {
        CustomDebug.Log("RPC Add score : Local Team : " + ClanManager.getInstance().SelectedTeam + "    Param Teamm : " + team);
        if(ClanManager.getInstance().SelectedTeam == team)
            GameManager.getInstance().addScore(score);
    }
}
