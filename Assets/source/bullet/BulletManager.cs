using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;

[System.Serializable]
public class BulletDictionary : DictionaryTemplate<BulletManager.BulletType, GameObject> { }

public class BulletManager : NetworkBehaviour {

    public enum BulletType {
        BULLET_NONE,
        BULLET_LINEAR,
        BULLET_LINEAR_N,
        BULLET_LINEAR_N_PI,
        BULLET_PROJECTILE,
        BULLET_PROJECTILE_4,
        BULLET_SINE,
        BULLET_SPIRAL,
        BULLET_MISSILE,
    }
    private BulletType m_type;

    private static BulletManager m_instance;
    public List<BulletDictionary> m_bulletPrefabs;
    public int m_preloadCount = 1000;
    private OnlineBulletManager m_onlineBulletManager = null;

    public static BulletManager getInstance()
    {
        return m_instance;
    }

    private void Awake()
    {
        m_instance = this;

        foreach (BulletDictionary obj in m_bulletPrefabs)
        {
            ObjectPool.Preload(obj._value, m_preloadCount);
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void setOnlineManager(GameObject obj)
    {
        m_onlineBulletManager = obj.GetComponent<OnlineBulletManager>();
    }

    public GameObject getBulletPrefab(BulletType id)
    {
        var obj = m_bulletPrefabs.Find(item => item._key == id);
        if (obj != null)
            return obj._value;
        return null;
    }

    // for linear bullet
    public void initBullet(BulletType _type, int layer, Transform _spawner, Vector3 _direction, float _speed, NetworkInstanceId _parentNetId)
    {
        if (_type == BulletType.BULLET_LINEAR)
        {
            if(GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
                Local_LinearBullet(_type, layer, _spawner.position, _direction, _speed, _parentNetId);
            else if(GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.MULTIPLAYER)
                m_onlineBulletManager.Cmd_LinearBullet(_type, layer, _spawner.position, _direction, _speed, _parentNetId);
        }
    }

    public void initBullet(BulletType _type, int layer, Transform _spawner, Transform _lookAt, float _speed, NetworkInstanceId _parentNetId)
    {
        if (_type == BulletType.BULLET_LINEAR)
        {
            Vector3 _direction = (_lookAt.position - _spawner.position).normalized;

            if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
                Local_LinearBullet(_type, layer, _spawner.position, _direction, _speed, _parentNetId);
            else if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.MULTIPLAYER)
                m_onlineBulletManager.Cmd_LinearBullet(_type, layer, _spawner.position, _direction, _speed, _parentNetId);
        }
        else if (_type == BulletType.BULLET_MISSILE)
        {
            if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
                Local_MissileBullet(_type, layer, _spawner.position, _lookAt, _speed, _parentNetId);
            else if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.MULTIPLAYER)
            {
                NetworkIdentity netIdentity =_lookAt.GetComponent<NetworkIdentity>();
                if(netIdentity != null) // This is network object
                    m_onlineBulletManager.Cmd_MissileBullet(_type, layer, _spawner.position, netIdentity.netId, _speed, _parentNetId);
            }
        }
    }

    // for linear bullet N
    public void initBullet(BulletType _type, int layer, Transform _spawner, Vector3 _direction, float _speed, int _count, NetworkInstanceId _parentNetId)
    {
        float sector;
        float angleFactor = 0;
        float angle = 0;

        if (_type == BulletType.BULLET_LINEAR_N)
        {
            sector = Mathf.PI * 2.0f;
            angleFactor = sector / _count;
            angle = -angleFactor * _count * 0.5f;
        }
        else if (_type == BulletType.BULLET_LINEAR_N_PI)
        {
            sector = Mathf.PI;
            angleFactor = sector / (_count + 1);
            angle = -angleFactor * (_count - 1) * 0.5f;
        }
        else // default
            sector = Mathf.PI * 2.0f;

        Vector3 direction = _direction.normalized;

        for (int i = 0; i < _count; i++)
        {
            float x1 = direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle);
            float y1 = direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle);
            Vector3 newDirection = new Vector3(x1, y1, direction.z);

            if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
                Local_LinearBullet(_type, layer, _spawner.position, newDirection.normalized, _speed, _parentNetId);
            else if(GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.MULTIPLAYER)
                m_onlineBulletManager.Cmd_LinearBullet(_type, layer, _spawner.position, newDirection.normalized, _speed, _parentNetId);
            angle += angleFactor;
        }
    }
    // for projectile bullet
    public void initBullet(BulletType _type, int layer, Transform _spawner, ProjectileBullet.Direction _direction, float _distance, float _time, float _gravity, float _angle, NetworkInstanceId _parentNetId)
    {
        if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
        {
            Local_ProjectileBullet(_type, layer, _spawner.position, _direction, _distance, _time, _gravity, _angle, _parentNetId);
        }
        else if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.MULTIPLAYER)
        {
            m_onlineBulletManager.Cmd_ProjectileBullet(_type, layer, _spawner.position, _direction, _distance, _time, _gravity, _angle, _parentNetId);
        }
    }

    // for projectile bullet 4
    public void initBullet(BulletType _type, int layer, Transform _spawner, float _distance, float _time, float _gravity, float _angle, NetworkInstanceId _parentNetId)
    {
        if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
        {
            Local_ProjectileBullet(_type, layer, _spawner.position, ProjectileBullet.Direction.DIRECTION_RIGHT, _distance, _time, _gravity, _angle, _parentNetId);
            Local_ProjectileBullet(_type, layer, _spawner.position, ProjectileBullet.Direction.DIRECTION_RIGHT, _distance + 1.0f, _time, _gravity - 1.0f, _angle + 10.0f, _parentNetId);

            Local_ProjectileBullet(_type, layer, _spawner.position, ProjectileBullet.Direction.DIRECTION_LEFT, _distance, _time, _gravity, _angle, _parentNetId);
            Local_ProjectileBullet(_type, layer, _spawner.position, ProjectileBullet.Direction.DIRECTION_LEFT, _distance + 1.0f, _time, _gravity - 1.0f, _angle + 10.0f, _parentNetId);
        }
        else if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.MULTIPLAYER)
        {
            m_onlineBulletManager.Cmd_ProjectileBullet(_type, layer, _spawner.position, ProjectileBullet.Direction.DIRECTION_RIGHT, _distance, _time, _gravity, _angle, _parentNetId);
            m_onlineBulletManager.Cmd_ProjectileBullet(_type, layer, _spawner.position, ProjectileBullet.Direction.DIRECTION_RIGHT, _distance + 1.0f, _time, _gravity - 1.0f, _angle + 10.0f, _parentNetId);

            m_onlineBulletManager.Cmd_ProjectileBullet(_type, layer, _spawner.position, ProjectileBullet.Direction.DIRECTION_LEFT, _distance, _time, _gravity, _angle, _parentNetId);
            m_onlineBulletManager.Cmd_ProjectileBullet(_type, layer, _spawner.position, ProjectileBullet.Direction.DIRECTION_LEFT, _distance + 1.0f, _time, _gravity - 1.0f, _angle + 10.0f, _parentNetId);
        }
    }

    // for Sine / Spiral
    public void initBullet(BulletType _type, int layer, Transform _spawner, Vector3 _direction, float _speed, float _amplitude, NetworkInstanceId _parentNetId, float _startPoint = Mathf.PI)
    {
        if (_type == BulletType.BULLET_SINE)
        {
            if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
                Local_SineBullet(_type, layer, _spawner.position, _direction, _speed, _amplitude, _parentNetId, _startPoint);
            else if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.MULTIPLAYER)
                m_onlineBulletManager.Cmd_SineBullet(_type, layer, _spawner.position, _direction, _speed, _amplitude, _parentNetId, _startPoint);
        }
        else if (_type == BulletType.BULLET_SPIRAL)
        {
            initBullet(BulletManager.BulletType.BULLET_SINE, layer, _spawner, _direction, _speed, _amplitude, _parentNetId, Mathf.PI);
            initBullet(BulletManager.BulletType.BULLET_SINE, layer, _spawner, _direction, _speed, _amplitude, _parentNetId, Mathf.PI * 2.0f);
        }
    }

    // TODO: Remove public access of these functions

    // Linear Bullets
    public void Local_LinearBullet(BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed, NetworkInstanceId _parentNetId)
    {
        GameObject bullet = (GameObject)ObjectPool.Spawn(getBulletPrefab(_type), _position, Quaternion.identity);
        bullet.layer = layer;// LayerMask.NameToLayer("enemybullet");
        bullet.GetComponent<LinearBullet>().setup(_direction.normalized, _speed, _parentNetId);
    }

    // Missile Bullets
    public void Local_MissileBullet(BulletType _type, int layer, Vector3 _position, Transform _lookAt, float _speed, NetworkInstanceId _parentNetId)
    {
        GameObject bullet = (GameObject)ObjectPool.Spawn(getBulletPrefab(_type), _position, Quaternion.identity);
        bullet.layer = layer; // LayerMask.NameToLayer("enemybullet");
        bullet.GetComponent<MissileBullet>().setup(_lookAt, _speed, _parentNetId);
    }

    // projectile bullets
    public void Local_ProjectileBullet(BulletType _type, int layer, Vector3 _position, ProjectileBullet.Direction _direction, float _distance, float _time, float _gravity, float _angle, NetworkInstanceId _parentNetId)
    {
        GameObject bullet = (GameObject)ObjectPool.Spawn(getBulletPrefab(_type), _position, Quaternion.identity);
        bullet.layer = layer;// LayerMask.NameToLayer("enemybullet");
        bullet.GetComponent<ProjectileBullet>().setup(_direction, _distance, _time, _gravity, _angle, _parentNetId);
    }

    // Sine Bullets
    public void Local_SineBullet(BulletType _type, int layer, Vector3 _position, Vector3 _direction, float _speed, float _amplitude, NetworkInstanceId _parentNetId, float _startPoint = Mathf.PI)
    {
        GameObject bullet = (GameObject)ObjectPool.Spawn(getBulletPrefab(_type), _position, Quaternion.identity);
        bullet.layer = layer;// LayerMask.NameToLayer("enemybullet");
        bullet.GetComponent<SineBullet>().setup(new Vector3(_direction.x, 0, 0).normalized, _speed, _amplitude, _parentNetId, _startPoint);
    }

    public void onDestroyBullet(GameObject obj)
    {
        ObjectPool.Despawn(obj);
    }
}
