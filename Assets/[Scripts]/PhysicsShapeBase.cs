using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionShape
{
    Sphere = 0,
    Plane = 1,
    AABB = 2
}


public abstract class PhysicsShapeBase : MonoBehaviour
{

    public void Start()
    {
        PhysicsManager physicManager = FindObjectOfType<PhysicsManager>(); // return the first found component in the scene which has the type
        physicManager.PhysicsShapes.Add(this);
    }
    public abstract CollisionShape GetCollisionShape();

    public abstract bool IsCollidingWithSphere(PhysicsShapeSphere other);

    //public abstract bool IsCollidingWithPlane(PhysicsShapePlane other);
}
