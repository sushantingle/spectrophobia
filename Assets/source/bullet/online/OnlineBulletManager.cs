using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineBulletManager : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    // TODO: Remove public access of these functions
    [Command]
    public void Cmd_LinearBullet(BulletManager.BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed, NetworkInstanceId _parentNetId)
    {
        BulletManager.getInstance().Local_LinearBullet(_type, layer, _position, _direction, _speed, _parentNetId);
        Rpc_LinearBullet(_type, layer, _position, _direction, _speed, _parentNetId);
    }

    [ClientRpc]
    public void Rpc_LinearBullet(BulletManager.BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed, NetworkInstanceId _parentNetId)
    {
        if (isServer)
            return;
        BulletManager.getInstance().Local_LinearBullet(_type, layer, _position, _direction, _speed, _parentNetId);
    }

    [Command]
    public void Cmd_MissileBullet(BulletManager.BulletType _type, int layer, Vector3 _position, NetworkInstanceId _netId, float _speed, NetworkInstanceId _parentNetId)
    {
        GameObject _lookAt = ClientScene.FindLocalObject(_netId);
        if (_lookAt != null)
        {
            BulletManager.getInstance().Local_MissileBullet(_type, layer, _position, _lookAt.transform, _speed, _parentNetId);
            Rpc_MissileBullet(_type, layer, _position, _netId, _speed, _parentNetId);
        }
    }

    [ClientRpc]
    public void Rpc_MissileBullet(BulletManager.BulletType _type, int layer, Vector3 _position, NetworkInstanceId _netId, float _speed, NetworkInstanceId _parentNetId)
    {
        if (isServer)
            return;
        GameObject _lookAt = ClientScene.FindLocalObject(_netId);
        if (_lookAt != null)
            BulletManager.getInstance().Local_MissileBullet(_type, layer, _position, _lookAt.transform, _speed, _parentNetId);
    }

    [Command]
    public void Cmd_ProjectileBullet(BulletManager.BulletType _type, int layer, Vector3 _position, ProjectileBullet.Direction _direction, float _distance, float _time, float _gravity, float _angle, NetworkInstanceId _parentNetId)
    {
        BulletManager.getInstance().Local_ProjectileBullet(_type, layer, _position, _direction, _distance, _time, _gravity, _angle, _parentNetId);
        Rpc_ProjectileBullet(_type, layer, _position, _direction, _distance, _time, _gravity, _angle, _parentNetId);
    }

    [ClientRpc]
    public void Rpc_ProjectileBullet(BulletManager.BulletType _type, int layer, Vector3 _position, ProjectileBullet.Direction _direction, float _distance, float _time, float _gravity, float _angle, NetworkInstanceId _parentNetId)
    {
        if (isServer)
            return;
        BulletManager.getInstance().Local_ProjectileBullet(_type, layer, _position, _direction, _distance, _time, _gravity, _angle, _parentNetId);
    }

    [Command]
    public void Cmd_SineBullet(BulletManager.BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed, float _amplitude, NetworkInstanceId _parentNetId, float _startPoint)
    {
        BulletManager.getInstance().Local_SineBullet(_type, layer, _position, _direction, _speed, _amplitude, _parentNetId, _startPoint);
        Rpc_SineBullet(_type, layer, _position, _direction, _speed, _amplitude, _parentNetId, _startPoint);
    }

    [ClientRpc]
    public void Rpc_SineBullet(BulletManager.BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed, float _amplitude, NetworkInstanceId _parentNetId, float _startPoint)
    {
        if (isServer)
            return;
        BulletManager.getInstance().Local_SineBullet(_type, layer, _position, _direction, _speed, _amplitude, _parentNetId, _startPoint);
    }

}
