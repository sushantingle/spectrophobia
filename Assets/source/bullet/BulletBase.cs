using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour {

    protected float m_speed = 0.5f;
    protected Vector3 m_direction = Vector3.zero;
    public BulletManager.BulletType m_type = BulletManager.BulletType.BULLET_NONE;

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
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.x < 0 || screenPos.x > Screen.width)
            return true;
        if (screenPos.y < 0 || screenPos.y > Screen.height)
            return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BOnTriggerEnter2D(collision);    
    }

    protected virtual void BOnTriggerEnter2D(Collider2D collision)
    {

    }
}
