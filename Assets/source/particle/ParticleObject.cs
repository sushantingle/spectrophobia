using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour {
    public enum AlphaFalloff { NONE, LINEAR, SQRT };

    private Vector2 m_minVelocity = new Vector2(-0.05f, 0.1f);
    private Vector2 m_maxVelocity = new Vector2(0.05f, 0.2f);
    private float m_lifeSpan = 2f;
    private AlphaFalloff m_alphaFalloff;
    private float m_actualLifeSpan;
    private float m_timeAlive;
    private SpriteRenderer m_spriteRenderer;
    private Color m_originalColor;
    private Vector2 m_velocity;
    private bool m_isAlive = false;

    // This only runs ONCE -- not on every Spawn
    void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_originalColor = m_spriteRenderer.color;
    }

    // TODO: Change function name
    void init()
    {
        m_isAlive = true;
        m_velocity = new Vector2(Random.Range(m_minVelocity.x, m_maxVelocity.x), Random.Range(m_minVelocity.y, m_maxVelocity.y));
        m_actualLifeSpan = m_lifeSpan * Random.Range(0.9f, 1.1f);

        m_timeAlive = 0;
        m_spriteRenderer.color = m_originalColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isAlive)
            return;

        m_timeAlive += Time.deltaTime;
        if (m_timeAlive >= m_actualLifeSpan)
        {
            m_isAlive = false;
            ObjectPool.Despawn(gameObject);
            return;
        }

        if (m_alphaFalloff == AlphaFalloff.LINEAR)
        {
            // As the particle gets older, it fades out

            float alpha = Mathf.Clamp01(1.0f - (m_timeAlive / m_actualLifeSpan));

            Color newColor = m_originalColor;
            newColor.a *= alpha;
            m_spriteRenderer.color = newColor;
        }
        else if (m_alphaFalloff == AlphaFalloff.SQRT)
        {
            // As the particle gets older, it fades out

            float alpha = Mathf.Clamp01(1.0f - (m_timeAlive / m_actualLifeSpan));

            alpha = Mathf.Sqrt(alpha);

            Color newColor = m_originalColor;
            newColor.a *= alpha;
            m_spriteRenderer.color = newColor;
        }

        this.transform.Translate(m_velocity * Time.deltaTime);
    }

    public void setup(Vector2 minVelocity, Vector2 maxVelocity, float life, AlphaFalloff alphaFallOff = AlphaFalloff.NONE)
    {
        m_minVelocity = minVelocity;
        m_maxVelocity = maxVelocity;
        m_lifeSpan = life;
        m_alphaFalloff = alphaFallOff;

        init();
    }
}
