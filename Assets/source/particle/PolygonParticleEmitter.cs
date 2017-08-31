using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonParticleEmitter : BaseParticleEmitter {
    private List<Triangle> m_triangles;
    private List<float> m_triangleWeight = new List<float>();
    private List<float> m_triangleArea = new List<float>();
    private float m_commulativeArea = 0.0f;


    // Use this for initialization
    protected override void Start () {
        base.Start();

        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        Vector2[] localPoints = col.points;
        Vector2[] worldPoints = new Vector2[localPoints.Length];
        for (int i = 0; i < localPoints.Length; i++)
        {
            Vector3 lossyScale = transform.lossyScale;
            Vector2 p = new Vector2(localPoints[i].x * lossyScale.x, localPoints[i].y * lossyScale.y);
            Vector3 rotation = transform.rotation.eulerAngles;
            Vector3 localPos = p - new Vector2(transform.position.x, transform.position.y);
            //localPos.Normalize();
            float angle = Mathf.Deg2Rad * rotation.z;
            float newY = (angle != 0.0f) ? (localPos.y * Mathf.Cos(angle)) + (localPos.x * Mathf.Sin(angle)) : p.y;
            float newX = (angle != 0.0f) ? (- localPos.y * Mathf.Sin(angle)) + (localPos.x * Mathf.Cos(angle)) : p.x;
            //p = new Vector2()
            //p = new Vector2((p.x - transform.localPosition.x) * Mathf.Cos(Mathf.Deg2Rad * rotation.z), (p.y - transform.localPosition.z) * Mathf.Sin(Mathf.Deg2Rad *rotation.y));
            worldPoints[i] = new Vector2(newX, newY);
        }

        Polygon poly = new Polygon(worldPoints);
        m_triangles = poly.triangulate();
        //renderTriangles();
        calculateWeight();
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    protected override void spawnParticle()
    {
        base.spawnParticle();
        spawnParticleRandomly();
    }

    void calculateWeight()
    {
        // calculate area
        foreach (Triangle t in m_triangles)
        {
            float area = t.getArea();
            m_triangleArea.Add(area);
        }

        // calculate total area
        foreach (float a in m_triangleArea)
        {
            m_commulativeArea += a;
        }

        // calculate weight
        foreach (float a in m_triangleArea)
        {
            m_triangleWeight.Add(a / m_commulativeArea);
        }
    }

    void spawnParticleRandomly()
    {
        int index = selectRandomTriangle();
        Vector2 spawnPoint = getRandomPointInTriangle(m_triangles[index]);
        // spawn particle at point
        spawnAtPosition(spawnPoint + (Vector2)this.transform.position, Quaternion.identity);
    }

    Vector2 getRandomPointInTriangle(Triangle triangle)
    {
        Vector2 point;
        // get barycentric co-ordinates of triangle
        float a0 = Random.Range(0.0f, 1.0f);
        float a1 = Random.Range(0.0f, 1.0f);
        if (a0 + a1 > 1.0f)
        {
            a0 = 1 - a0;
            a1 = 1 - a1;
        }
        float a2 = 1 - a0 - a1;
        point = a0 * triangle.m_points[0] + a1 * triangle.m_points[1] + a2 * triangle.m_points[2];

        //Debug.Log("Point in polygon : " + triangle.pointInPolygon(point.x, point.y));
        return point;
    }

    int selectRandomTriangle()
    {
        return Random.Range(0, m_triangles.Count);
    }

    // For debug purpose
    void renderTriangles()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = m_triangles.Count * 3;
        lr.useWorldSpace = true;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        int posIndex = 0;

        foreach (Triangle T in m_triangles)
        {
            lr.SetPosition(posIndex++, T.m_points[0]);
            lr.SetPosition(posIndex++, T.m_points[1]);
            lr.SetPosition(posIndex++, T.m_points[2]);
        }

        lr.positionCount = posIndex;
    }
}
