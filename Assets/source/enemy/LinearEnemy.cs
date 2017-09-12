using UnityEngine;
using System.Collections;

public class LinearEnemy : EnemyBase {

    private void Awake()
    {
        m_pathType = PathDefs.AI_PATH_TYPE.PATH_LINEAR;
    }

    // Use this for initialization
    void Start () {
        //EStart();
	}

    protected override void OnEnable()
    {
        base.OnEnable();
        EStart();
    }

    protected override void EStart()
    {
        base.EStart();
        m_fireStartTime = Time.time;
    }

	// Update is called once per frame

    protected override void EUpdate()
    {
        base.EUpdate();
        m_path.update();
    }

    protected override void EFixedUpdate()
    {
        base.EFixedUpdate();
        m_path.fixedUpdate();
    }

    void OnTriggerEnter2D(Collider2D col)
	{
        base.OnETriggerEnter(col);
	}
}
