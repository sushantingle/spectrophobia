using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MissileBullet : BulletBase {
    private Transform m_target;
    private int m_curvePointCount = 100;
    private List<Vector3> m_curvePoints = new List<Vector3>();
    private Coroutine MoveIE;
    private float m_updateTargetPosTimeOffset = 0.5f;
    private int m_currentIndex = 0;
    // Use this for initialization
    void Start () {
        base.BStart();

        StartCoroutine(moveObject());
        InvokeRepeating("updateCurvePoints", m_updateTargetPosTimeOffset, m_updateTargetPosTimeOffset);
    }

    void updateCurvePoints()
    {
        if (m_currentIndex < m_curvePointCount / 1.5f)
        {
            //CustomDebug.Log("Added point : " + m_target.position);
            m_curvePoints[m_curvePoints.Count - 1] = m_target.position;
        }
    }

	// Update is called once per frame
	void Update () {
        BUpdate();
	}

    protected override void BUpdate()
    {
        //generateCurve();
    }

    public void setup(Transform _target, float _speed, NetworkInstanceId _parentNetId)
    {
        base.setup(_parentNetId, Vector3.zero, _speed);
        m_target = _target;
        generateCurve();
    }

    private void generateCurve()
    {
        Vector3 direction = (m_target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        float dir = Random.Range(1, 1000) % 2 == 0 ? -1 : 1;
        float cpAngle_1 = Mathf.Deg2Rad * (angle + 90.0f * dir);
        Vector3 controlPoint_1 = Random.Range(0.0f, 2.0f) * new Vector3(Mathf.Cos(cpAngle_1) * direction.x, Mathf.Sin(cpAngle_1) * direction.y, transform.position.z);
        float cpAngle_2 = Mathf.Deg2Rad * (angle + 45.0f * dir);
        Vector3 controlPoint_2 = Random.Range(3.0f, 6.0f) * new Vector3(Mathf.Cos(cpAngle_2) * direction.x, Mathf.Sin(cpAngle_2) * direction.y, transform.position.z);

        BezierCurve.getQuadraticCurve(transform.position, controlPoint_1, controlPoint_2, m_target.position, m_curvePointCount, ref m_curvePoints);
    }

    IEnumerator moveObject()
    {
        for (int i = 0; i < m_curvePoints.Count; i++)
        {
            //CustomDebug.Log("Curve Point Size : " + m_curvePoints.Count);
            MoveIE = StartCoroutine(Moving(i));
            yield return MoveIE;
        }
        BulletManager.getInstance().onDestroyBullet(gameObject);
    }

    IEnumerator Moving(int currentPosition)
    {
        m_currentIndex = currentPosition;
        while (transform.position != m_curvePoints[currentPosition])
        {
            transform.position = Vector3.MoveTowards(transform.position, m_curvePoints[currentPosition], m_speed * Time.deltaTime);
            Vector3 direction = m_curvePoints[currentPosition] - transform.position;
            float angle = Vector3.Angle(transform.right, direction);
            angle = Vector3.Cross(transform.right, direction).z < 0 ? (360.0f - angle) % 360.0f : angle;
            transform.Rotate(new Vector3(0.0f, 0.0f, angle));
            yield return null;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.BOnTriggerEnter2D(collision);
    }

    public Transform getTarget()
    {
        return m_target;
    }
}
