using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public Transform m_floor;

    private static LevelManager m_instance;

    public Transform m_leftWall;
    public Transform m_rightWall;
    public Transform m_upWall;
    public Transform m_downwall;

    public float m_minX, m_maxX, m_minY, m_maxY;
    private void Awake()
    {
        m_instance = this;
    }

    public static LevelManager getInstance()
    {
        return m_instance;
    }
    // Use this for initialization
    void Start() {
        m_minX = m_leftWall.position.x;
        m_maxX = m_rightWall.position.x;
        m_minY = m_downwall.position.y;
        m_maxY = m_upWall.position.y;
    }

    // Update is called once per frame
    void Update() {

    }

    public float getMinX() {
        return m_minX;
    }

    public float getMinY()
    {
        return m_minY;
    }

    public float getMaxX() {
        return m_maxX;
    }

    public float getMaxY() {
        return m_maxY;
    }
}
