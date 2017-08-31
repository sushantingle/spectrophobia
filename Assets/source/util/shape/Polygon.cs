using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon {

    public Polygon()
    { }

    public Polygon(Vector2[] _points)
    {
        m_points = new Vector2[_points.Length];
        Array.Copy(_points, m_points, _points.Length);
//        m_points = _points;
    }

    public Vector2[] m_points;

    private float signedPolygonArea()
    {
        // Add first point to end
        int num_points = m_points.Length;
        Vector2[] pts = new Vector2[num_points + 1];
        m_points.CopyTo(pts, 0);
        pts[num_points] = m_points[0];

        // get the areas
        float area = 0;
        for (int i = 0; i < num_points; i++)
        {
            area +=
                (pts[i + 1].x - pts[i].x) * 
                (pts[i + 1].y + pts[i].y) / 2;
        }

        return area;
    }

    private bool isPolygonOrientedInClockwise()
    {
        return (signedPolygonArea() > 0);
    }

    private void orientPolygonClockwise()
    {
        if (isPolygonOrientedInClockwise() == false)
            Array.Reverse(m_points);
    }

    // returns dot product : AB * BC
    private float dotProduct(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
    {
        // get vectors co-ordinates
        float vAx = Ax - Bx;
        float vAy = Ay - By;
        float vCx = Cx - Bx;
        float vCy = Cy - By;

        // calculate dot
        return (vAx * vCx + vAy * vCy);
    }

    // returns cross product length : basically z value of cross product
    private float crossProductLength(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
    {
        // get vectors co-ordinates
        float vAx = Ax - Bx;
        float vAy = Ay - By;
        float vCx = Cx - Bx;
        float vCy = Cy - By;

        // get cross product length
        return (vAx * vCy - vAy * vCx);
    }

    // Return angle ABC
    private float getAngle(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
    {
        float dot_product = dotProduct(Ax, Ay, Bx, By, Cx, Cy);
        float cross_product = crossProductLength(Ax, Ay, Bx, By, Cx, Cy);

        return (float) Mathf.Atan2(cross_product, dot_product);
    }

    public bool pointInPolygon(float X, float Y)
    {
        // Get the angle between the point and first and last vertex
        int max_points = m_points.Length - 1;
        float total_angle = getAngle(
            m_points[max_points].x, m_points[max_points].y,
            X, Y,
            m_points[0].x, m_points[0].y);

        // Add all angles
        for (int i = 0; i < max_points; i++)
        {
            total_angle += getAngle(
                m_points[i].x, m_points[i].y,
                X, Y,
                m_points[i + 1].x, m_points[i + 1].y);
        }

        return (Mathf.Abs(total_angle) > 0.00001);
    }

    // Return true if three points form an ear
    private bool formsEar(Vector2[] points, int A, int B, int C)
    {
        //Debug.Log("Index : " + A + B + C);
        //Debug.Log("Checking if forms an ear : " + points[A] + points[B] + points[C]);
        if (getAngle(
            points[A].x, points[A].y,
            points[B].x, points[B].y,
            points[C].x, points[C].y) <= 0)
        {
            // This is concave corner so the triangle can not be an ear
            return false;
        }

        Triangle triangle = new Triangle(points[A], points[B], points[C]);

        for (int i = 0; i < points.Length; i++)
        {
            if ((i != A) && (i != B) && (i != C))
            {
                if (triangle.pointInPolygon(points[i].x, points[i].y))
                {
                    return false;
                }
            }
        }
        //Debug.Log("Forms an ear : " + points[A] + points[B] + points[C]);
        return true;
    }

    // Find index of three points that form an ear
    private void findEar(ref int A, ref int B, ref int C)
    {
        int num_points = m_points.Length;
        //Debug.Log("Num Of Points : " + num_points);

        //foreach(Vector2 p in m_points)
        //    Debug.Log("Point : " + p);

        for (int i = 0; i < num_points; i++)
        {
            A = i;
            B = (A + 1) % num_points;
            C = (B + 1) % num_points;

            //Debug.Log("Before Form Ear : " + A + B + C);
            // check if forms ear
            if (formsEar(m_points, A, B, C)) return;
        }

        //Debug.Assert(true,"Should never get there");
    }

    // Remove Ear from the polygon and add it to triangles array
    private void removeEar(List<Triangle> triangles)
    {
        // Find an ear
        int A = 0, B = 0, C = 0;
        findEar(ref A, ref B, ref C);

        //Debug.Log("Index : " + A + B + C);
        //Debug.Log("Ear Found : " + m_points[A] + m_points[B] + m_points[C]);

        // create a triangle
        triangles.Add(new Triangle(m_points[A], m_points[B], m_points[C]));

        // Remove ear from polygon
        //Debug.Log("Remove Point : " + m_points[B]);
        removePoint(B);
    }

    private void removePoint(int target)
    {
        Vector2[] pts = new Vector2[m_points.Length - 1];
        Array.Copy(m_points, 0, pts, 0, target);
        Array.Copy(m_points, target + 1, pts, target, m_points.Length - target - 1);
        m_points = pts;
    }
    // Triangulate polygon
    public List<Triangle> triangulate()
    {
        Vector2[] pts = new Vector2[m_points.Length];
        Array.Copy(m_points, pts, pts.Length);

        Polygon polygon = new Polygon(pts);
        polygon.orientPolygonClockwise();

        List<Triangle> triangles = new List<Triangle>();

        while (polygon.m_points.Length > 3)
        {
            polygon.removeEar(triangles);
        }

        triangles.Add(new Triangle(polygon.m_points[0], polygon.m_points[1], polygon.m_points[2]));
        return triangles;
    }
}
