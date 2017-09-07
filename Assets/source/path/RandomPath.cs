using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPath : PathBase {

    private int m_randomSeed = 0;
    public float m_changeTimeOffset = 0.2f;
    private float m_changeDirectionTime = 0;
    private PathDefs.AI_Direction m_lastDirection = PathDefs.AI_Direction.MOVE_NONE;

    public override void init(Transform _transform, float _speed, float _changeTimeOffset)
    {
        base.init(_transform, _speed);
        m_changeTimeOffset = _changeTimeOffset;
        m_randomSeed = generateSeed();
    }

    private int generateSeed()
    {
        return Random.Range(0, 10);
    }

    public override void update()
    {
        if (m_transform == null)// TODO: Remove it or move it to base class
            return;
        base.update();
        updateRandomPath();
    }

    private void updateRandomPath()
    {
        if (Time.time > m_changeDirectionTime || isAtBoundry())
        {
            changeDirection();
            m_randomSeed = generateSeed();
            m_changeDirectionTime = Time.time + m_randomSeed * m_changeTimeOffset;
        }

        // update pod
        Vector3 pos = m_transform.position;

        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_LEFT) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_LEFT))
        {
            pos.x -= m_speed;
        }
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_RIGHT) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_RIGHT))
        {
            pos.x += m_speed;
        }
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_UP) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_UP))
        {
            pos.y += m_speed;
        }
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_DOWN) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_DOWN))
        {
            pos.y -= m_speed;
        }

        m_transform.position = pos;
    }

    private void changeDirection()
    {
        resetMoveDirections();
        int rand = m_randomSeed % PathDefs.CONST_MAX_MOVE_DIRECTION;
        int count = 0;

        while (count < PathDefs.CONST_MAX_MOVE_DIRECTION)
        {
            PathDefs.AI_Direction direction = (PathDefs.AI_Direction)Mathf.Pow(2, rand);
            if (!isHitStatusSet(direction) && direction != m_lastDirection)
            {
                //CustomDebug.Log("Moving In : " + direction);
                setMoveStatus(direction);
                m_lastDirection = direction;
                break;
            }

            rand = (rand == PathDefs.CONST_MAX_MOVE_DIRECTION) ? 0 : rand + 1;
            count++;
        }
    }

    public override void fixedUpdate()
    {
        base.fixedUpdate();
    }
}
