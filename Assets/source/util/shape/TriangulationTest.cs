using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangulationTest : MonoBehaviour {

    List<Triangle> m_triangles;
    List<float> m_triangleWeight = new List<float>();
    List<float> m_triangleArea = new List<float>();
    float m_commulativeArea = 0.0f;
    public Transform obj;
    public float m_spawnDuration = 1.0f;
    float m_startTime;
    // Use this for initialization
    void Start() {
        m_startTime = Time.time;
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();

        Polygon poly = new Polygon(col.points);
        //Debug.Log("Points are : ");
        //foreach (Vector2 point in poly.m_points)
        //{
        //    Debug.Log(point);
        //}
        m_triangles = poly.triangulate();
        //renderTriangles();
        calculateWeight();

    }

    // Update is called once per frame
    void Update() {
        if (Time.time - m_startTime > m_spawnDuration)
        {
            m_startTime = Time.time;
            spawnPointRandomly();
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 30, 30), "check"))
            spawnPointRandomly();
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

    void spawnPointRandomly()
    {
        int index = selectRandomTriangle();
        Vector2 point = getRandomPointInTriangle(m_triangles[index]);
        renderTriangle(m_triangles[index]);
        obj.transform.position = point;
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

    void renderTriangle(Triangle triangle)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 3;
        lr.useWorldSpace = true;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;

        lr.SetPosition(0, triangle.m_points[0]);
        lr.SetPosition(1, triangle.m_points[1]);
        lr.SetPosition(2, triangle.m_points[2]);
    }

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
