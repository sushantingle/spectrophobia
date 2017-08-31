using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitPath : PathBase {
    
    private float m_popOutInterval;
    private float m_stayOnDuration;
    private float m_lastPopOutTime = 0;
    private bool m_hasPoppedOut = false;

    public override void init(Transform _transform, float _popOutInterval, float _stayOnDuration)
    {
        CustomDebug.Log("Rabbit Path init");
        base.init(_transform, 0);
        m_popOutInterval = _popOutInterval;
        m_stayOnDuration = _stayOnDuration;
    }

    public override void update()
    {
        if (m_transform == null)// TODO: Remove it or move it to base class
            return;
        base.update();

        if(m_lastPopOutTime + m_popOutInterval < Time.time && !m_hasPoppedOut)
        {
            //CustomDebug.Log("Check position for popup out");
            bool check = true;
            int checkCount = 0;
            while (check)
            {
                if (checkCount > 10)
                    break;
                float x = Random.Range(LevelManager.getInstance().getMinX(), LevelManager.getInstance().getMaxX());
                float y = Random.Range(LevelManager.getInstance().getMinY(), LevelManager.getInstance().getMaxY());
                Vector3 worldPos = new Vector3(x, y, 1.0f);
                if (m_boxCollider.bounds.Contains(worldPos))
                {
                    checkCount++;
                    continue;
                }
                /*if (m_boxCollider != null)
                {
                    Vector2 topLeft = new Vector2(worldPos.x - m_boxCollider.size.x / 2, worldPos.y - m_boxCollider.size.y / 2);
                    Vector2 bottomRight = new Vector2(worldPos.x + m_boxCollider.size.x / 2, worldPos.y + m_boxCollider.size.y / 2);
                    if (Physics2D.OverlapArea(topLeft, bottomRight))
                    {
                        checkCount++;
                        continue;
                    }
                }
                else
                {
                    if (Physics2D.OverlapPoint(new Vector2(worldPos.x, worldPos.y)))
                    {
                        checkCount++;
                        continue;
                    }
                }*/

                //CustomDebug.Log("Pop out at : "+worldPos);
                m_hasPoppedOut = true;
                m_transform.position = new Vector3(worldPos.x, worldPos.y, m_transform.position.z);
                SpawnerEnemy spawnerEnemy = m_transform.GetComponent<SpawnerEnemy>();
                if (spawnerEnemy != null)
                {
                    spawnerEnemy.onPopoutOfGround();
                }
            }
        }
    }

    public void onHide()
    {
        //CustomDebug.Log("Rabbit Path On Hide");
        m_lastPopOutTime = Time.time;
        m_hasPoppedOut = false;
    }

    public override void fixedUpdate()
    {
        //base.fixedUpdate(); // steady object
    }
}
