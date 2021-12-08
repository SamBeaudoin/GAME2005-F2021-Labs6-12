using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysiczAABB : PhysiczColliderBase
{
    public Vector3 dimensions = new Vector3(1, 1, 1);

    Vector3 GetMin()
    {
        return transform.position - GetHalfSize();
    }

    Vector3 GetMax()
    {
        return transform.position + GetHalfSize();
    }

    Vector3 GetSize()
    {
        return transform.lossyScale;
    }
    public Vector3 GetHalfSize()
    {
        return transform.lossyScale * 0.5f;
    }

    public override CollisionShape GetCollisionShape()
    {
        return CollisionShape.AABB;
    }
}
