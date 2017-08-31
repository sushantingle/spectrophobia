﻿using UnityEngine;
using System.Collections;

public abstract class ItemBase : MonoBehaviour {

	protected ItemManager.ITEM_TYPE m_type;
	public float 	m_activeDuration;
	protected float	m_startTime;
    protected bool m_isCollected = false;

	// Use this for initialization
	protected virtual void Start () {
        m_startTime = Time.time;
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        // move item downwards (as in under gravity)
        transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.up, Time.deltaTime * 0.25f);

        if (Time.time - m_startTime > m_activeDuration)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            m_isCollected = true;
            ItemManager.getInstance().addItem(this);
            Destroy(gameObject);
        }
    }

    public ItemManager.ITEM_TYPE getItemType() {
		return m_type;
	}

}