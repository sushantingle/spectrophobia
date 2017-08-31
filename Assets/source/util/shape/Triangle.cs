using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : Polygon{

    public Triangle(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        m_points = new Vector2[] { p0, p1, p2 };
    }

    public void printPoints()
    {
        Debug.Log("Vertices : " + m_points[0] + m_points[1] + m_points[2]);
    }

    public float getArea()
    {
        float area = 0.0f;
        Vector2 AB = new Vector2(m_points[1].x - m_points[0].x, m_points[1].y - m_points[0].y);
        Vector2 BC = new Vector2(m_points[2].x - m_points[1].x, m_points[2].y - m_points[1].y);
        Vector2 CA = new Vector2(m_points[0].x - m_points[2].x, m_points[0].y - m_points[2].y);

        float ab = Vector3.Magnitude(new Vector3(AB.x, AB.y, 0));
        float bc = Vector3.Magnitude(new Vector3(BC.x, BC.y, 0));
        float ca = Vector3.Magnitude(new Vector3(CA.x, CA.y, 0));

        // Heron's formula
        float s = (ab + bc + ca) / 2;
        area = Mathf.Sqrt(s * (s - ab) * (s - bc) * (s - ca));
        //Debug.Log("Points : " + m_points[0] + m_points[1] + m_points[2]);
        //Debug.Log("Area : " + area);
        return area;
    }
}
