using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTest : MonoBehaviour {
    public Transform m_target;
    public int m_curvePointCount = 100;
    public float m_control_1_off = 4.0f;
    public float m_control_2_off = 2.5f;
    private List<Vector3> m_curvePoints = new List<Vector3>();
    public LineRenderer m_lineRenderer;
    Coroutine MoveIE;
    public float m_speed = 0.025f;
    private Vector3 m_originalPos;
    // Use this for initialization
    void Start () {
        m_originalPos = transform.position;
        Reset();
    }

    IEnumerator moveObject()
    {
        for (int i = 0; i < m_curvePoints.Count; i++)
        {
            MoveIE = StartCoroutine(Moving(i));
            yield return MoveIE;
        }
    }

    IEnumerator Moving(int currentPosition)
    {
        while (transform.position != m_curvePoints[currentPosition])
        {
            transform.position = Vector3.MoveTowards(transform.position, m_curvePoints[currentPosition], m_speed * (currentPosition + 1) * Time.deltaTime);
            yield return null;
        }

    }

    // Update is called once per frame
    float fps = 3.0f;
    float startTime = 0;
	void Update () {

/*        if (Time.time > startTime + fps)
        {
            startTime = Time.time;
            m_curvePoints.Clear();
            generateCurve();

            for (int i = 0; i < m_curvePoints.Count; i++)
                m_lineRenderer.SetPosition(i, m_curvePoints[i]);
        }*/
    }

    private void Reset()
    {
        transform.position = m_originalPos;
        m_curvePoints.Clear();
        generateCurve();
        CustomDebug.Log("Count : " + m_curvePoints.Count);
        m_lineRenderer.positionCount = m_curvePoints.Count;
        m_lineRenderer.useWorldSpace = true;
        for (int i = 0; i < m_curvePoints.Count; i++)
            m_lineRenderer.SetPosition(i, m_curvePoints[i]);

        StartCoroutine(moveObject());

    }
    private void OnGUI()
    {
        if(GUI.Button(new Rect(new Vector2(10,10), new Vector2(100, 50)), "Reset"))
        {
            Reset();
        }
            
    }
    private void generateCurve()
    {
        Vector3 direction = (m_target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        float dir = Random.Range(1, 1000) % 2 == 0 ? -1 : 1;
        float cpAngle_1 = Mathf.Deg2Rad * (angle + 90.0f * dir);
        Vector3 controlPoint_1 = Random.Range(0.0f, 5.0f) * new Vector3(Mathf.Cos(cpAngle_1) * direction.x, Mathf.Sin(cpAngle_1) * direction.y, transform.position.z);
        float cpAngle_2 = Mathf.Deg2Rad * (angle + 45.0f * dir);
        Vector3 controlPoint_2 = Random.Range(5.0f, 10.0f) * new Vector3(Mathf.Cos(cpAngle_2) * direction.x, Mathf.Sin(cpAngle_2) * direction.y, transform.position.z);

        CustomDebug.Log("Control Points : " + transform.position + "  " + controlPoint_1 + " " + controlPoint_2 + " " + m_target.position);
        BezierCurve.getQuadraticCurve(transform.position, controlPoint_1, controlPoint_2, m_target.position, m_curvePointCount, ref m_curvePoints);
        //foreach(var point in m_curvePoints)
        //    CustomDebug.Log("Curve Points: " + point);
    }
}
