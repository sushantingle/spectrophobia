using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve {
    static Vector3 p0, p1, p2, p3 = Vector3.zero;
//    static List<Vector3> points = new List<Vector3>();

    public static List<Vector3> getQuadraticCurve(Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3, int numOfPoints, ref List<Vector3> points)
    {
        p0 = _p0;
        p1 = _p1;
        p2 = _p2;
        p3 = _p3;

        //points.Clear();
        CalculatePoints(ref points, numOfPoints);
        return points;
    }

    // 0.0 >= t <= 1.0 her be magic and dragons
    private static Vector3 GetPointAtTime(float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; //first term
        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term

        return p;

    }

    //where _num is the desired output of points and _precision is how good we want matching to be
    private static void CalculatePoints(ref List<Vector3> points, int _num, int _precision = 100)
    {
        if (_num > _precision) Debug.LogError("_num must be less than _precision");

        //calculate the length using _precision to give a rough estimate, save lengths in array
        float length = 0;
        //store the lengths between PointsAtTime in an array
        float[] arcLengths = new float[_precision];

        Vector3 oldPoint = GetPointAtTime(0);

        for (int p = 1; p < arcLengths.Length; p++)
        {
            Vector3 newPoint = GetPointAtTime((float)p / _precision); //get next point
            arcLengths[p] = Vector3.Distance(oldPoint, newPoint); //find distance to old point
            length += arcLengths[p]; //add it to the bezier's length
            oldPoint = newPoint; //new is old for next loop
        }

        //target length for spacing
        float segmentLength = length / _num;

        //arc index is where we got up to in the array to avoid the Shlemiel error http://www.joelonsoftware.com/articles/fog0000000319.html
        int arcIndex = 0;

        float walkLength = 0; //how far along the path we've walked
        oldPoint = GetPointAtTime(0);

        //iterate through points and set them
        for (int i = 0; i < _num; i++)
        {
            float iSegLength = i * segmentLength; //what the total length of the walkLength must equal to be valid
                                                  //run through the arcLengths until past it
            while (walkLength < iSegLength)
            {
                walkLength += arcLengths[arcIndex]; //add the next arcLength to the walk
                arcIndex++; //go to next arcLength
            }
            //walkLength has exceeded target, so lets find where between 0 and 1 it is
            Vector3 point = GetPointAtTime((float)arcIndex / arcLengths.Length);
            //CustomDebug.Log("Curve Point : " + point);
            points.Add(point);

        }
    }
}
