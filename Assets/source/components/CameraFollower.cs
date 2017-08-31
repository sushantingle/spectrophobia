using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {
    private Transform m_target = null;

	// Use this for initialization
	void Start () {
		
	}

    public void setTarget(Transform target)
    {
        m_target = target;
    }

	// Update is called once per frame
	void Update () {
        if (m_target != null)
        {
            Vector3 pos = m_target.position;
            float orthoSize = Camera.main.orthographicSize;
//            CustomDebug.Log("ortho size : " + orthoSize);
//            CustomDebug.Log("targetPos : " + pos);
//            CustomDebug.Log("Aspect : " + (Camera.main.aspect));
            if (pos.x + orthoSize * Camera.main.aspect > LevelManager.getInstance().getMaxX())
                pos.x = transform.position.x;

            if (pos.x - orthoSize * Camera.main.aspect < LevelManager.getInstance().getMinX())
                pos.x = transform.position.x;

            if (pos.y + orthoSize > LevelManager.getInstance().getMaxY())
                pos.y = transform.position.y;

            if (pos.y - orthoSize < LevelManager.getInstance().getMinY())
                pos.y = transform.position.y;

            pos.z = transform.position.z;
            transform.position = pos;
        }
	}
}
