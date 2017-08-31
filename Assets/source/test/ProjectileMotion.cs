using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{

    public float maxdistance = 5.0f;
    public float angle = 0.0f;
    public float maxtime = 2.0f;
    float time = 0.0f;
    Vector3 startPos;
    public float vo;
    public float gravity = 1.0f;
    public float direction = 1.0f;
    // Use this for initialization
    void Start()
    {
        maxdistance = Random.Range(2.0f, 5.0f);
        gravity = Random.Range(3.0f, 5.0f);
        vo = maxdistance / maxtime;
        direction = (Random.Range(1, 10) % 2 == 0) ? 1.0f : -1.0f;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            angle = 0;
            time = 0;
            maxdistance = Random.Range(2, 5);
            gravity = Random.Range(3.0f, 5.0f);
            vo = maxdistance / maxtime;
            direction = (Random.Range(1, 10) % 2 == 0) ? 1.0f : -1.0f;
        }
        if (time >= maxtime)
            return;
        Vector3 pos = transform.position;

        float vx = vo * Mathf.Cos(Mathf.Deg2Rad * 30) * time;
        float vy = vo * Mathf.Sin(Mathf.Deg2Rad * angle) * time - 0.5f * gravity * time * time;

        angle = Mathf.Lerp(0, 180, time / maxtime);
        time += (Time.deltaTime / maxtime);
        //angle += 1.0f;

        Debug.Log("vx : " + vx + "  vy: " + vy + "  angle : " + angle);
        pos.x = startPos.x + vx * direction;
        pos.y = startPos.y + vy;

        transform.position = pos;

    }
}
 