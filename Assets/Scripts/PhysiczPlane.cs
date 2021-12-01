using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    X = 0,
    Y,
    Z
}

public enum PlaneID
{
    GROUND = 0,
    RIGHT,
    LEFT,
    FRONT,
    BACK
}


//We can define a plane with a normal vector and a point on the plane
public class PhysiczPlane : PhysiczColliderBase
{
    public Axis alignment = Axis.Y;
    public PlaneID planeID = PlaneID.GROUND;

    public override CollisionShape GetCollisionShape()
    {
        return CollisionShape.Plane;
    }

    public Vector3 GetNormal()
    {
        switch (alignment)
        {
            case (Axis.X):
                {
                    return transform.right;
                }
            case (Axis.Y):
                {
                    return transform.up;
                }
            case (Axis.Z):
                {
                    return transform.forward;
                }
            default:
                {
                    throw new Exception("Invalid plane alignment");
                }
        }
    }
}
