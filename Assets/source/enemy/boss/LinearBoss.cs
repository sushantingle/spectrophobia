using UnityEngine;
using System.Collections;

public class LinearBoss : EnemyBase {

	private	bool 			m_hitLeft 		= false;
	private bool 			m_hitRight 		= false;
	private bool 			m_hitUp 		= false;
	private bool 			m_hitDown 		= false;
	private float 			m_YmovementOff 	= 0.0f;
	private bool			m_isMovingLeft	= false;
	private	bool 			m_isMovingRight = false;
	private bool			m_isMovingUp	= false;
	private bool			m_isMovingDown	= false;

    public ProgressBar      m_healthBar;

	// Use this for initialization
	void Start () {
        EStart();		
	}

    protected override void EStart()
    {
        base.EStart();

        m_isBoss = true;
        CustomDebug.Log("EStart linear boss");
        m_YmovementOff = m_boxCollider.bounds.size.y;
        updateLinearPath();
        m_fireStartTime = Time.time;
        m_maxHealth = m_health;
    }

    protected override void EUpdate()
    {
        base.EUpdate();
        updateLinearPath();
        Vector3 pos = transform.position;

        if (m_isMovingLeft)
        {
            pos.x -= m_speed;
        }
        else if (m_isMovingRight)
        {
            pos.x += m_speed;
        }
        else if (m_isMovingUp)
        {
            pos.y += m_speed;
        }
        else if (m_isMovingDown)
        {
            pos.y -= m_speed;
        }

        transform.position = pos;
    }

    protected override void EFixedUpdate()
    {
        int layermask = 1 << LayerMask.NameToLayer("wall");
        //layermask = ~layermask;

        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, m_boxCollider.bounds.size.y / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.up * (m_boxCollider.bounds.size.y / 2 + m_raycastOffset), Color.green);
        m_hitUp = (hitUp.collider != null);

        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, m_boxCollider.bounds.size.y / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, -Vector2.up * (m_boxCollider.bounds.size.y / 2 + m_raycastOffset), Color.blue);
        m_hitDown = (hitDown.collider != null);

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, m_boxCollider.bounds.size.x / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.right * (m_boxCollider.bounds.size.x / 2 + m_raycastOffset), Color.grey);
        m_hitRight = (hitRight.collider != null);

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, m_boxCollider.bounds.size.x / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.left * (m_boxCollider.bounds.size.x / 2 + m_raycastOffset), Color.magenta);
        m_hitLeft = (hitLeft.collider != null);
    }

	private void updateLinearPath()
	{
		if ((m_isMovingRight && !m_hitRight) || (m_isMovingLeft && !m_hitLeft) || (m_isMovingUp && !m_hitUp) || (m_isMovingDown && !m_hitDown))
			return;

		m_isMovingRight = m_isMovingLeft = m_isMovingUp = m_isMovingDown = false;
		int[] randomArr = { 1, 2, 3, 4 };

		for (int t = 0; t < 4; t++ )
		{
			int tmp = randomArr[t];
			int r = Random.Range(t, 4);
			randomArr[t] = randomArr[r];
			randomArr[r] = tmp;
		}

		for (int t = 0; t < 4; t++) {
			if (randomArr[t] == 1) {
				if (!m_hitRight) {
					m_isMovingRight = true;
					break;
				}
			}

			if (randomArr[t] == 2) {
				if (!m_hitLeft) {
					m_isMovingLeft = true;
					break;
				}
			}

			if (randomArr[t] == 3) {
				if (!m_hitUp) {
					m_isMovingUp = true;
					break;
				}
			}

			if (randomArr[t] == 4) {
				if (!m_hitDown) {
					m_isMovingDown = true;
					break;
				}
			}
		}
	}

    private void updateHealthBar()
    {
        m_healthBar.setValue(m_health / m_maxHealth);
        CustomDebug.Log("Health Bar : " + m_health);
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
