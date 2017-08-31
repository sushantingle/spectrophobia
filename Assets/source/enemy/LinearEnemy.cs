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
	void Update () {
        if (GameManager.getInstance ().isGamePaused ())
			return;

        EUpdate();

	}

    protected override void EUpdate()
    {
        base.EUpdate();
        m_path.update();
    }

    void FixedUpdate() {
        if (GameManager.getInstance().isGamePaused())
            return;

        EFixedUpdate();
        m_path.fixedUpdate();
    }

    protected override void EFixedUpdate()
    {
        base.EFixedUpdate();
    }

    void OnTriggerEnter2D(Collider2D col)
	{
        base.OnETriggerEnter(col);
	}
}
