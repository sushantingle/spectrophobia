using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseParticleEmitter : MonoBehaviour {
   

    public GameObject m_particlePrefab;
    public float m_rate = 500; // per second
    public Vector2 m_minVelocity = new Vector2(-0.05f, 0.1f);
    public Vector2 m_maxVelocity = new Vector2(0.05f, 0.2f);
    public float m_life = 1.0f;
    public ParticleObject.AlphaFalloff m_alphaFallOff = ParticleObject.AlphaFalloff.NONE;

    private float m_timeSinceLastSpawn = 0;
    // Use this for initialization
    protected virtual void Start () {
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {
        m_timeSinceLastSpawn += Time.deltaTime;

        float correctTimeBetweenSpawns = 1f / m_rate;

        while (m_timeSinceLastSpawn > correctTimeBetweenSpawns)
        {
            // Time to spawn a particle
            spawnParticle();
            m_timeSinceLastSpawn -= correctTimeBetweenSpawns;
        }
    }

    protected virtual void spawnParticle() {
        // no default implementation
    }

    protected virtual void spawnAtPosition(Vector2 position, Quaternion rotation) {
        GameObject obj = ObjectPool.Spawn(m_particlePrefab, position, rotation);
        obj.GetComponent<ParticleObject>().setup(m_minVelocity, m_maxVelocity, m_life, m_alphaFallOff);
    } 
}
