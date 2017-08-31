using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBase {

    protected PathDefs.AI_PATH_TYPE m_pathType;
    public float m_raycastOffset = 0.5f;
    protected PolygonCollider2D m_boxCollider;
    protected Transform m_transform;
    protected float m_speed;
    protected Transform m_lookAt;

    protected int m_hitStatus = 0;
    protected int m_moveStatus = 0;

    public void reset()
    {
        m_transform = null;
        m_lookAt = null;
        //m_boxCollider = null;
    }

    public virtual void init(Transform _transform, float _speed)
    {
        m_transform = _transform;
        m_speed = _speed;
        m_boxCollider = m_transform.GetComponent<PolygonCollider2D>();
    }

    public virtual void init(Transform _transform, float _speed, Transform _lookAt)
    {
        init(_transform, _speed);
        m_lookAt = _lookAt;
    }

    public virtual void init(Transform _transform, float _speed, float _changeDirectionOffset)
    {
        init(_transform, _speed);
    }

    public virtual void init(Transform _transform, float _speed, Transform _lookAt, float _changeTimeOffset, float _safeDistance)
    {
        init(_transform, _speed);
    }
    public virtual void update()
    {

    }

    public virtual void fixedUpdate()
    {
        if (m_transform == null) // TODO: Do we really need it
            return;
        int layermask = 1 << LayerMask.NameToLayer("wall");
        //layermask = ~layermask;

        RaycastHit2D hitUp = Physics2D.Raycast(m_transform.position, Vector2.up, m_boxCollider.bounds.size.y / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.up * (m_boxCollider.bounds.size.y / 2 + m_raycastOffset), Color.green);
        if (hitUp.collider != null)
            setHitStatus(PathDefs.AI_Direction.MOVE_UP);
        else resetHitStatus(PathDefs.AI_Direction.MOVE_UP);

        RaycastHit2D hitDown = Physics2D.Raycast(m_transform.position, Vector2.down, m_boxCollider.bounds.size.y / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, -Vector2.up * (m_boxCollider.bounds.size.y / 2 + m_raycastOffset), Color.blue);
        if (hitDown.collider != null)
            setHitStatus(PathDefs.AI_Direction.MOVE_DOWN);
        else resetHitStatus(PathDefs.AI_Direction.MOVE_DOWN);

        RaycastHit2D hitRight = Physics2D.Raycast(m_transform.position, Vector2.right, m_boxCollider.bounds.size.x / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.right * (m_boxCollider.bounds.size.x / 2 + m_raycastOffset), Color.grey);
        if (hitRight.collider != null)
            setHitStatus(PathDefs.AI_Direction.MOVE_RIGHT);
        else resetHitStatus(PathDefs.AI_Direction.MOVE_RIGHT);

        RaycastHit2D hitLeft = Physics2D.Raycast(m_transform.position, Vector2.left, m_boxCollider.bounds.size.x / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.left * (m_boxCollider.bounds.size.x / 2 + m_raycastOffset), Color.magenta);
        if (hitLeft.collider != null)
            setHitStatus(PathDefs.AI_Direction.MOVE_LEFT);
        else resetHitStatus(PathDefs.AI_Direction.MOVE_LEFT);
    }

    protected void setHitStatus(PathDefs.AI_Direction flag)
    {
        m_hitStatus = m_hitStatus | (int)flag;
    }

    protected void resetHitStatus(PathDefs.AI_Direction flag)
    {
        m_hitStatus = m_hitStatus & ~(int)flag;
    }

    protected bool isHitStatusSet(PathDefs.AI_Direction flag)
    {

        return ((m_hitStatus & (int)flag) != 0);
    }

    protected void resetAllHitStatus()
    {
        m_hitStatus = 0;
    }

    protected void setMoveStatus(PathDefs.AI_Direction flag)
    {
        m_moveStatus = m_moveStatus | (int)flag;
    }

    protected void resetMoveStatus(PathDefs.AI_Direction flag)
    {
        m_moveStatus = m_moveStatus & ~(int)flag;
    }

    protected bool isMovingInDirection(PathDefs.AI_Direction flag)
    {

        return ((m_moveStatus & (int)flag) != 0);
    }

    protected void resetMoveDirections()
    {
        m_moveStatus = 0;
    }

    protected bool isAtBoundry()
    {
        if ((isMovingInDirection(PathDefs.AI_Direction.MOVE_RIGHT) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_RIGHT))
            || (isMovingInDirection(PathDefs.AI_Direction.MOVE_LEFT) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_LEFT))
            || (isMovingInDirection(PathDefs.AI_Direction.MOVE_UP) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_UP))
            || (isMovingInDirection(PathDefs.AI_Direction.MOVE_DOWN) && !isHitStatusSet(PathDefs.AI_Direction.MOVE_DOWN)))
            return false;

        return true;
    }

    protected bool isMoving()
    {
        return (m_moveStatus != 0);
    }
}
