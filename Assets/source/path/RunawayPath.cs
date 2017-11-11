using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayPath : PathBase {

    private float m_updateTargetTick = 1.0f;
    private float m_changeTimeOffset = 0.2f;
    private float m_changeDirectionTime = 0;
    private PathDefs.AI_Direction m_lastDirection = PathDefs.AI_Direction.MOVE_NONE;
    private int m_randomSeed = 0;
    private float m_lastUpdateTick;
    private Vector3 m_targetPosition;
    private float m_safeDistance = 3.0f;
    private bool m_changeDirWhenInRange = false;

    public override void init(Transform _transform, float _speed, Transform _lookAt, float _changeTimeOffset, float _safeDistance)
    {
        base.init(_transform, _speed);
        m_lookAt = _lookAt;

        m_lastUpdateTick = 0;
        m_targetPosition = m_lookAt.position;
        m_changeTimeOffset = _changeTimeOffset;
        m_safeDistance = _safeDistance;

        m_pathType = PathDefs.AI_PATH_TYPE.PATH_RUNAWAY;
        m_randomSeed = generateSeed();
    }

    private int generateSeed()
    {
        return Random.Range(0, 1000);
    }

    public override void update()
    {
        if (m_transform == null)// TODO: Remove it or move it to base class
            return;
        // TODO: Need to implement AI in the game to calculate the shortest path
        base.update();

        if (Time.time > m_changeDirectionTime || isAtBoundry() || (!m_changeDirWhenInRange && getDistanceFromPlayer() < m_safeDistance))
        {
            m_changeDirWhenInRange = (getDistanceFromPlayer() < m_safeDistance);
            changeDirection();
            m_randomSeed = generateSeed();
            m_changeDirectionTime = Time.time + (m_randomSeed % 10) * m_changeTimeOffset;
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

        PathDefs.AI_Direction playerDirection = getPlayerDirection();

        while (count < PathDefs.CONST_MAX_MOVE_DIRECTION)
        {
            PathDefs.AI_Direction direction = (PathDefs.AI_Direction)Mathf.Pow(2, rand);
            if (!isHitStatusSet(direction) && direction != m_lastDirection && direction != playerDirection)
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

    private PathDefs.AI_Direction getPlayerDirection()
    {
        float angle = Vector3.Angle(m_transform.right, m_lookAt.position - m_transform.position);

        if (angle < 45.0f || angle > 315.0f)
            return PathDefs.AI_Direction.MOVE_RIGHT;

        if (angle > 45.0f && angle < 135.0f)
            return PathDefs.AI_Direction.MOVE_UP;

        if (angle > 135.0f && angle < 225.0f)
            return PathDefs.AI_Direction.MOVE_LEFT;

        if (angle > 225.0f && angle < 315.0f)
            return PathDefs.AI_Direction.MOVE_DOWN;

        return PathDefs.AI_Direction.MOVE_NONE;
    }

    private float getDistanceFromPlayer()
    {
        return Vector3.Distance(m_transform.position, m_lookAt.position);
    }
    public override void fixedUpdate()
    {
        base.fixedUpdate();
    }

    public override Vector3 getMovingDirection()
    {
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_DOWN))
            return Vector3.down;
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_UP))
            return Vector3.up;
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_LEFT))
            return Vector3.left;
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_RIGHT))
            return Vector3.right;

        return Vector3.zero;
    }
}
