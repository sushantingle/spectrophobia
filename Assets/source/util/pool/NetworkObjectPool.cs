using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkObjectPool : NetworkBehaviour{

    const int DEFAULT_POOL_SIZE = 3;

    class NetworkObject {
        public NetworkInstanceId   m_netId;
        public GameObject          m_object;

        public NetworkObject(NetworkInstanceId netId, GameObject obj)
        {
            m_netId = netId;
            m_object = obj;
        }
    }

    class NetworkPool {
        int nextid = 1;

        List<NetworkObject> m_inactive;
        List<NetworkObject> m_active;
        GameObject m_prefab;
        NetworkObjectPool m_networkObjectPool;

        // should be called on server only
        public NetworkPool(GameObject prefab, int initialQty, NetworkObjectPool pool) {
            m_prefab = prefab;
            m_inactive = new List<NetworkObject>(initialQty);
            m_active = new List<NetworkObject>(initialQty);
            m_networkObjectPool = pool;
        }

        [Command]
        public GameObject Cmd_Spawn(Vector3 pos, Quaternion rot)
        {
            GameObject obj = null;
            CustomDebug.LogError("Player object net id Spawn: " + m_networkObjectPool.gameObject.GetComponent<NetworkIdentity>().netId.Value);
            if (m_inactive.Count == 0)
            {
                Debug.LogError("Pool is empty. Instantiating new object : "+m_prefab.name);
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
                    return Cmd_Spawn(pos, rot);
                }
                Debug.Log("Pool has Object : " + m_prefab.name + "  setting it active "+nwObj.m_netId);
                obj = nwObj.m_object;
            }

            obj.transform.position = pos;
            obj.transform.rotation = rot;
            //obj.SetActive(true);
            activateObject(obj);
            NetworkInstanceId netId = obj.GetComponent<NetworkIdentity>().netId;
            m_active.Add(new NetworkObject(netId, obj));
            Debug.Log("activated obj : " + netId);
            // RPC client
            Rpc_Spawn(netId, pos, rot);
            return obj;
        }

        [ClientRpc]
        public void Rpc_Spawn(NetworkInstanceId netId, Vector3 pos, Quaternion rot)
        {
            CustomDebug.LogError("Player object net id Rpc Spawn: " + m_networkObjectPool.gameObject.GetComponent<NetworkIdentity>().netId.Value);
            // TODO : callback for object on setActive True ... i think OnEnable will do
            // need to check feasibility
            if (GameManager.getInstance().isServer())
                return;

            //NetworkObject networkObj = m_inactive.Find(networkObject => (networkObject.m_netId == netId));
            GameObject obj = ClientScene.FindLocalObject(netId);
            if (obj == null)
            {
                Debug.LogError("Pool does not contain object on client with net id " + netId);
            }
            else
            {
                Debug.Log("Spawning object on client from pool : "+obj.name+ "  and net id : "+netId);
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                //obj.SetActive(true);
                activateObject(obj);
                //m_inactive.Remove(networkObj);
                //m_active.Add(networkObj);
            }
        }

        [Command]
        public void Cmd_Despawn(NetworkInstanceId netId)
        {
            CustomDebug.LogError("Player object net id Command DeSpawn: " + m_networkObjectPool.gameObject.GetComponent<NetworkIdentity>().netId.Value);
            NetworkObject networkObj = m_active.Find(netObj => (netObj.m_netId == netId));
            if (networkObj == null)
            {
                Debug.LogError("Can not despawn. Object missing on Host : "+netId);
                Rpc_Despawn(netId);
                return;
            }
            Debug.LogError("Despawn Obj : " + netId.Value);
            //networkObj.m_object.SetActive(false);
            deactivateObject(networkObj.m_object);
            m_active.Remove(networkObj);
            m_inactive.Add(networkObj);
            Rpc_Despawn(networkObj.m_netId);
        }

        [ClientRpc]
        public void Rpc_Despawn(NetworkInstanceId netId)
        {
            CustomDebug.LogError("Player object net id Rpc DeSpawn: " + m_networkObjectPool.gameObject.GetComponent<NetworkIdentity>().netId.Value);
            CustomDebug.LogError("Rpc_Despawn : "+netId.Value);
            if (GameManager.getInstance().isServer())
                return;
            //NetworkObject networkObj = m_active.Find(netObj => (netObj.m_netId == netId));
            GameObject obj = ClientScene.FindLocalObject(netId);
            if (obj == null)
            {
                Debug.LogError("Can not despawn pool object. Object missing on Client");
                return;
            }
            Debug.Log("Despawn Object with Net Id : " + netId + "   and name : " + obj.name);
            //obj.SetActive(false);
            deactivateObject(obj);
            //m_active.Remove(networkObj);
            //m_inactive.Add(networkObj);
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
        }

        void deactivateObject(GameObject obj)
        {
            foreach (Behaviour comp in obj.GetComponents<Behaviour>())
                comp.enabled = false;

            foreach (Transform o in obj.transform)
            {
                o.gameObject.SetActive(false);
            }
        }
    }

    class NetworkPoolMemeber : MonoBehaviour{
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

        return m_networkPools[prefab].Cmd_Spawn(pos, rot);
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
            poolMember.m_networkPool.Cmd_Despawn(netId);
        }
    }
}
