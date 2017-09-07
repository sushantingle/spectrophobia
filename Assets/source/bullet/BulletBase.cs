using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletBase : MonoBehaviour {

    protected float m_speed = 0.5f;
    protected Vector3 m_direction = Vector3.zero;
    public BulletManager.BulletType m_type = BulletManager.BulletType.BULLET_NONE;
    protected NetworkInstanceId m_parentNetId = NetworkInstanceId.Invalid;

	// Use this for initialization
	void Start () {
        BStart();
	}

    protected virtual void BStart()
    {

    }

	// Update is called once per frame
	void Update () {
        BUpdate();
    }

    protected virtual void BUpdate()
    {

    }

    private void FixedUpdate()
    {
        BFixedUpdate();
    }

    protected virtual void BFixedUpdate()
    {

    }

    protected bool isOutsideOfScreen()
    {
        if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (screenPos.x < 0 || screenPos.x > Screen.width)
                return true;
            if (screenPos.y < 0 || screenPos.y > Screen.height)
                return true;
        }
            
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BOnTriggerEnter2D(collision);
    }

    protected virtual void BOnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.getInstance().getGameplayMode() == GameManager.GameplayMode.SINGLE_PLAYER)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("wall"))
            {
                Destroy(gameObject);
            }
        }
    }

    public NetworkInstanceId getParentNetId()
    {
        return m_parentNetId;
    }
        
}
