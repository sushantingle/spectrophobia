using UnityEngine;
using System.Collections;

public class LinearBoss : EnemyBase {

    public ProgressBar      m_healthBar;

	// Use this for initialization
	void Start () {
	}

    protected override void OnEnable()
    {
        base.OnEnable();
        EStart();
    }

    protected override void EStart()
    {
        base.EStart();
        m_isBoss = true;
        m_fireStartTime = Time.time;
    }

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

    private void updateHealthBar()
    {
        m_healthBar.setValue(m_health / m_maxHealth);
    }

	void OnTriggerEnter2D(Collider2D col)
	{
        base.OnETriggerEnter(col);

        if (col.gameObject.layer == LayerMask.NameToLayer("playerbullet"))
        {
            updateHealthBar();
        }

    }
}
