using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkObjectPool : NetworkBehaviour {

    const int DEFAULT_POOL_SIZE = 3;

    class NetworkObject {
        public NetworkInstanceId m_netId;
        public GameObject m_object;

        public NetworkObject(NetworkInstanceId netId, GameObject obj)
        {
            m_netId = netId;
            m_object = obj;
        }
    }

    class NetworkPool
    {
        int nextid = 1;

        List<NetworkObject> m_inactive;
        List<NetworkObject> m_active;
        GameObject m_prefab;
        NetworkObjectPool m_networkObjectPool;

        // should be called on server only
        public NetworkPool(GameObject prefab, int initialQty, NetworkObjectPool pool)
        {
            m_prefab = prefab;
            m_inactive = new List<NetworkObject>(initialQty);
            m_active = new List<NetworkObject>(initialQty);
            m_networkObjectPool = pool;
        }

        public GameObject CommandSpawn(Vector3 pos, Quaternion rot)
        {
            GameObject obj = null;
            //CustomDebug.LogError("Player object net id Spawn: " + m_networkObjectPool.gameObject.GetComponent<NetworkIdentity>().netId.Value);
            if (m_inactive.Count == 0)
            {
                Debug.LogError("Pool is empty. Instantiating new object : " + m_prefab.name);
                obj = (GameObject)GameObject.Instantiate(m_prefab, pos, rot);
                obj.name = m_prefab.name + "(" + (nextid++) + ")";
                obj.AddComponent<NetworkPoolMemeber>().m_networkPool = this;
                NetworkServer.Spawn(obj);
            }
            else
            {
                NetworkObject nwObj = m_inactive[0];
                m_inactive.RemoveAt(0);
                if (nwObj == null)
                {
                    // someone deleted it.. find out who did it and sentense him to death
                    Debug.LogError("Deleted Object from pool. Checking for next one." + m_prefab.name);
                    return CommandSpawn(pos, rot);
                }
                Debug.Log("Pool has Object : " + m_prefab.name + "  setting it active " + nwObj.m_netId);
                obj = nwObj.m_object;
            }

            obj.transform.position = pos;
            obj.transform.rotation = rot;
            //obj.SetActive(true);
            activateObject(obj);
            return obj;
        }

        public void CommandDespawn(NetworkInstanceId netId)
        {
            CustomDebug.Log("Player object net id Command DeSpawn: " + m_networkObjectPool.gameObject.GetComponent<NetworkIdentity>().netId.Value);
            NetworkObject networkObj = m_active.Find(netObj => (netObj.m_netId == netId));
            if (networkObj == null)
            {
                Debug.LogError("Can not despawn. Object missing on Host : " + netId);
                return;
            }
            Debug.Log("Despawn Obj : " + netId.Value);
            deactivateObject(networkObj);
        }

        void activateObject(GameObject obj)
        {
            foreach (Behaviour comp in obj.GetComponents<Behaviour>())
            {
                if (comp.GetType() == typeof(SpriteRenderer))
                    continue;
                comp.enabled = true;
            }

            foreach (Transform o in obj.transform)
            {
                o.gameObject.SetActive(true);
            }

            NetworkInstanceId netId = obj.GetComponent<NetworkIdentity>().netId;
            m_active.Add(new NetworkObject(netId, obj));
        }

        void deactivateObject(NetworkObject ntwObj)
        {
            GameObject obj = ntwObj.m_object;
            foreach (Behaviour comp in obj.GetComponents<Behaviour>())
                comp.enabled = false;

            foreach (Transform o in obj.transform)
            {
                o.gameObject.SetActive(false);
            }

            m_active.Remove(ntwObj);
            m_inactive.Add(ntwObj);
        }

        public List<NetworkObject> getActiveObjectList()
        {
            return m_active;
        }

        public List<NetworkObject> getInactiveObjectList()
        {
            return m_inactive;
        }
    }

    class NetworkPoolMemeber : MonoBehaviour {
        public NetworkPool m_networkPool;
    }

    Dictionary<GameObject, NetworkPool> m_networkPools;

    void Init(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE)
    {
        if (m_networkPools == null) {
            m_networkPools = new Dictionary<GameObject, NetworkPool>();
        }

        if (prefab != null && m_networkPools.ContainsKey(prefab) == false) {
            m_networkPools[prefab] = new NetworkPool(prefab, qty, this);
        }
    }

    public void Preload(GameObject prefab, int qty = 1)
    {
        if (isServer == false)
        {
            Debug.Log("Not Server : do not have authentication to preload");
            return;
        }

        Init(prefab, qty);

        GameObject[] obs = new GameObject[qty];
        for (int i = 0; i < qty; i++)
        {
            // spawn
            obs[i] = Spawn(prefab, Vector3.zero, Quaternion.identity);
        }

        for (int i = 0; i < qty; i++)
        {
            // despawn
            Despawn(obs[i]);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (isServer == false)
        {
            Debug.Log("Not Server : do not have authentication to spawn");
            return null;
        }

        Init(prefab);
        GameObject obj = m_networkPools[prefab].CommandSpawn(pos, rot);
        Rpc_Spawn(obj.GetComponent<NetworkIdentity>().netId, pos, rot);
        return obj;
    }

    [ClientRpc]
    public void Rpc_Spawn(NetworkInstanceId netId, Vector3 pos, Quaternion rot)
    {
        if (isServer)
            return;
        GameObject obj = ClientScene.FindLocalObject(netId);

        if (obj == null)
        {
            CustomDebug.Log("RPC SPAWN OBJECT NOT FOUND :" + netId.Value);
        }
        else
        {
            // activate network object
            activateObject(obj, pos, rot);
        }
    }

    public void Despawn(GameObject obj)
    {
        if (!isServer)
        {
            Debug.LogError("Not Server : do not have authentication to despawn");
            return;
        }

        NetworkInstanceId netId = obj.GetComponent<NetworkIdentity>().netId;
        NetworkPoolMemeber poolMember = obj.GetComponent<NetworkPoolMemeber>();

        if (poolMember == null)
        {
            Debug.Log("Object " + obj.name + " not a network pool member");
        }
        else
        {
            poolMember.m_networkPool.CommandDespawn(netId);
            Rpc_Despawn(netId);
        }
    }

    [ClientRpc]
    private void Rpc_Despawn(NetworkInstanceId netId)
    {
        if (isServer)
            return;

        GameObject obj = ClientScene.FindLocalObject(netId);

        if (obj == null)
        {
            CustomDebug.Log("RPC DESPAWN OBJECT NOT FOUND :" + netId.Value);
        }
        else
        {
            // deactivate network object
            deactivateObject(obj);
        }
    }

    [TargetRpc]
    public void Target_spawnObjectOnClient(NetworkConnection conn, NetworkInstanceId netId, Vector3 pos, Quaternion rot)
    {
        CustomDebug.Log("Target Spawn Object on Client");

        if (isServer)
            return;

        GameObject obj = ClientScene.FindLocalObject(netId);
        if (obj == null)
        {
            CustomDebug.Log("Object not found in Client Scene : " + netId.Value);
        }
        else
        {
            activateObject(obj, pos, rot);
        }
    }

    [TargetRpc]
    public void Target_despawnObjectOnClient(NetworkConnection conn, NetworkInstanceId netId)
    {
        CustomDebug.Log("Target DeSpawn Object on Client");
        if (isServer)
            return;

        GameObject obj = ClientScene.FindLocalObject(netId);
        if (obj == null)
        {
            CustomDebug.Log("Object not found in Client Scene : " + netId.Value);
        }
        else
        {
            deactivateObject(obj);
        }
    }

    public void OnServerAddPlayer(NetworkConnection conn)
    {
        CustomDebug.Log("On Client Connect : " + conn.connectionId);
        if (!isServer || conn.connectionId == 0)
            return;

        foreach (NetworkPool objPool in m_networkPools.Values)
        {
            foreach (NetworkObject networkObj in objPool.getActiveObjectList())
            {
                //activateObject(networkObj.m_object);
                GameObject obj = networkObj.m_object;
                NetworkInstanceId netId = obj.GetComponent<NetworkIdentity>().netId;
                Target_spawnObjectOnClient(conn,netId, obj.transform.position, obj.transform.rotation);
            }

            foreach (NetworkObject networkObj in objPool.getInactiveObjectList())
            {
                GameObject obj = networkObj.m_object;
                NetworkInstanceId netId = obj.GetComponent<NetworkIdentity>().netId;
                Target_despawnObjectOnClient(conn, netId);
            }
        }
    }

    void activateObject(GameObject obj, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
    {
        // activate network object
        foreach (Behaviour comp in obj.GetComponents<Behaviour>())
        {
            if (comp.GetType() == typeof(SpriteRenderer))
                continue;
            comp.enabled = true;
        }

        foreach (Transform o in obj.transform)
        {
            o.gameObject.SetActive(true);
        }

        obj.transform.position = pos;
        obj.transform.rotation = rot;
    }

    void deactivateObject(GameObject obj)
    {
        if (obj == null)
        {
            CustomDebug.Log("RPC DESPAWN OBJECT NOT FOUND :" + netId.Value);
        }
        else
        {
            // deactivate network object
            foreach (Behaviour comp in obj.GetComponents<Behaviour>())
                comp.enabled = false;

            foreach (Transform o in obj.transform)
            {
                o.gameObject.SetActive(false);
            }
        }
    }
}
