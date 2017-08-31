using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearPath : PathBase {

    public override void init(Transform _transform, float _speed)
    {
        base.init(_transform, _speed);
        m_pathType = PathDefs.AI_PATH_TYPE.PATH_LINEAR;
    }
    public override void update()
    {
        if (m_transform == null) // TODO: Remove it or move it to base class
            return;
        base.update();

        updateLinearPath();
        Vector3 pos = m_transform.position;

        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_LEFT))
        {
            pos.x -= m_speed;
        }
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_RIGHT))
        {
            pos.x += m_speed;
        }
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_UP))
        {
            pos.y += m_speed;
        }
        if (isMovingInDirection(PathDefs.AI_Direction.MOVE_DOWN))
        {
            pos.y -= m_speed;
        }

        m_transform.position = pos;
    }

    public override void fixedUpdate()
    {
        base.fixedUpdate();
    }

    private void updateLinearPath()
    {
        if ((isMovingInDirection(PathDefs.AI_Direction.MOVE_RIGHT) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_RIGHT))
            || (isMovingInDirection(PathDefs.AI_Direction.MOVE_LEFT) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_LEFT))
            || (isMovingInDirection(PathDefs.AI_Direction.MOVE_UP) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_UP))
            || (isMovingInDirection(PathDefs.AI_Direction.MOVE_DOWN) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_DOWN)))
            return;

        resetMoveDirections();

        int count = 0;
        int rand = Random.Range(0, PathDefs.CONST_MAX_MOVE_DIRECTION);
        while (count < PathDefs.CONST_MAX_MOVE_DIRECTION)
        {
            PathDefs.AI_Direction direction = (PathDefs.AI_Direction)Mathf.Pow(2, rand);
            if (!isHitStatusSet(direction))
            {
                //CustomDebug.Log("Moving In : " + direction);
                setMoveStatus(direction);
                break;
            }

            rand = (rand == PathDefs.CONST_MAX_MOVE_DIRECTION) ? 0 : rand + 1;
            count++;
        }
    }
}
