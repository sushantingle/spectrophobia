using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeComponent : MonoBehaviour {
    private enum ParentType {
        PARENT_NONE,
        PARENT_PLAYER,
        PARENT_ENEMY,
    }

    private Transform m_eyeBall;
    public Transform m_parent;
    private ParentType m_parentType = ParentType.PARENT_NONE;
    public float m_radius = 0.15f;

    // Use this for initialization
    void Start () {
        m_eyeBall = transform.GetChild(0);
        if (m_parent.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            m_parentType = ParentType.PARENT_PLAYER;
        }
        else if (m_parent.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            m_parentType = ParentType.PARENT_ENEMY;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (m_parentType == ParentType.PARENT_PLAYER)
        {
            m_eyeBall.position = Vector3.Lerp(m_eyeBall.position, (transform.position + m_parent.GetComponent<Player>().getMovementDirection() * m_radius), Time.deltaTime * 10.0f);
        }
        else if (m_parentType == ParentType.PARENT_ENEMY)
        {
            m_eyeBall.position = Vector3.Lerp(m_eyeBall.position, (transform.position + m_parent.GetComponent<EnemyBase>().getMovingDirection() * m_radius), Time.deltaTime * 10.0f);
        }
	}
}
