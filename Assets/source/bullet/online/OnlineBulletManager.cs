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

    [Command]
    public void Cmd_LinearBullet(BulletManager.BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed)
    {
        CustomDebug.Log("Command Spawn Linear Bullet");
        BulletManager.getInstance().Local_LinearBullet(_type, layer, _position, _direction, _speed);
        Rpc_LinearBullet(_type, layer, _position, _direction, _speed);
    }

    [ClientRpc]
    public void Rpc_LinearBullet(BulletManager.BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed)
    {
        if (isServer)
            return;
        CustomDebug.Log("Rpc Spawn Linear Bullet");
        BulletManager.getInstance().Local_LinearBullet(_type, layer, _position, _direction, _speed);
    }

    [Command]
    public void Cmd_MissileBullet(BulletManager.BulletType _type, int layer, Vector3 _position, NetworkInstanceId _netId, float _speed)
    {
        GameObject _lookAt = ClientScene.FindLocalObject(_netId);
        if (_lookAt != null)
        {
            BulletManager.getInstance().Local_MissileBullet(_type, layer, _position, _lookAt.transform, _speed);
            Rpc_MissileBullet(_type, layer, _position, _netId, _speed);
        }
    }

    [ClientRpc]
    public void Rpc_MissileBullet(BulletManager.BulletType _type, int layer, Vector3 _position, NetworkInstanceId _netId, float _speed)
    {
        if (isServer)
            return;
        GameObject _lookAt = ClientScene.FindLocalObject(_netId);
        if (_lookAt != null)
            BulletManager.getInstance().Local_MissileBullet(_type, layer, _position, _lookAt.transform, _speed);
    }

    [Command]
    public void Cmd_ProjectileBullet(BulletManager.BulletType _type, int layer, Vector3 _position, ProjectileBullet.Direction _direction, float _distance, float _time, float _gravity, float _angle)
    {
        BulletManager.getInstance().Local_ProjectileBullet(_type, layer, _position, _direction, _distance, _time, _gravity, _angle);
        Rpc_ProjectileBullet(_type, layer, _position, _direction, _distance, _time, _gravity, _angle);
    }

    [ClientRpc]
    public void Rpc_ProjectileBullet(BulletManager.BulletType _type, int layer, Vector3 _position, ProjectileBullet.Direction _direction, float _distance, float _time, float _gravity, float _angle)
    {
        if (isServer)
            return;
        BulletManager.getInstance().Local_ProjectileBullet(_type, layer, _position, _direction, _distance, _time, _gravity, _angle);
    }

    [Command]
    public void Cmd_SineBullet(BulletManager.BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed, float _amplitude, float _startPoint)
    {
        BulletManager.getInstance().Local_SineBullet(_type, layer, _position, _direction, _speed, _amplitude, _startPoint);
        Rpc_SineBullet(_type, layer, _position, _direction, _speed, _amplitude, _startPoint);
    }

    [ClientRpc]
    public void Rpc_SineBullet(BulletManager.BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed, float _amplitude, float _startPoint)
    {
        if (isServer)
            return;
        BulletManager.getInstance().Local_SineBullet(_type, layer, _position, _direction, _speed, _amplitude, _startPoint);
    }

}
