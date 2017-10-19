using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineParticleEmitter : BaseParticleEmitter {

    Vector2[] m_points;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        m_points = col.GetPath(0);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void spawnParticle()
    {
        base.spawnParticle();
        spawnAlongOutline();
    }

    void spawnAlongOutline()
    {

        //PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        //int pathIndex = Random.Range(0, col.pathCount);

        //Vector2[] points = col.GetPath(pathIndex);

        int pointIndex = Random.Range(0, m_points.Length);

        Vector2 pointA = m_points[pointIndex];
        Vector2 pointB = m_points[(pointIndex + 1) % m_points.Length];

        Vector2 spawnPoint = Vector2.Lerp(pointA, pointB, Random.Range(0f, 1f));

        // Fixed issue : spawn point should change according to local scale.
        spawnPoint.x *= transform.localScale.x;
        spawnPoint.y *= transform.localScale.y;

        spawnAtPosition(spawnPoint + (Vector2)this.transform.position, Quaternion.identity);
    }

}
