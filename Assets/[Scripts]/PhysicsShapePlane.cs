//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public enum Axis
//{
//    X,
//    Y,
//    Z
//}


//public class PhysicsShapePlane : PhysicsShapeBase
//{
//    public Axis alignment = Axis.Y;

//    public override CollisionShape GetCollisionShape()
//    {
//        return CollisionShape.Plane;
//    }

//    public override bool IsCollidingWithPlane(PhysicsShapePlane other)
//    {
//        throw new System.NotImplementedException();
//    }

//    public override bool IsCollidingWithSphere(PhysicsShapeSphere other)
//    {
//        throw new System.NotImplementedException();
//    }

//    public Vector3 GetNormal()
//    {
//        switch(alignment)
//        {
//            case (Axis.X):
//                {
//                    return transform.right;
//                }
//            case (Axis.Y):
//                {
//                    return transform.up;
//                }
//            case (Axis.Z):
//                {
//                    return transform.forward;
//                }
//            default:
//                throw new Exception("Invalid plane alignment?");
//        }
//    }
//}
