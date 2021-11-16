using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<BasicPhysics> BasicObjectsList = new List<BasicPhysics>();
    public List<PhysicsShapeBase> PhysicsShapes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(BasicPhysics obj in BasicObjectsList)
        {
            // bounce back off walls
            if(obj.transform.position.z < -5 ||
                obj.transform.position.z > 5 ||
                obj.transform.position.x > 5 ||
                obj.transform.position.x < -5)
            {
                obj.velocity *= -1;
            }
        }
        CollisionDetectionUpdate();
    }

    void CollisionDetectionUpdate()
    {
        for(int i = 0; i < PhysicsShapes.Count; i++)
        {
            for (int j = i + 1; j < PhysicsShapes.Count; j++)
            {
                CollisionShape shape1 = PhysicsShapes[i].GetCollisionShape();
                CollisionShape shape2 = PhysicsShapes[j].GetCollisionShape();

                if(shape1 == CollisionShape.Sphere &&
                    shape2 == CollisionShape.Sphere)
                {
                    // casting from base to derived class
                    if(PhysicsShapes[i].IsCollidingWithSphere((PhysicsShapeSphere)PhysicsShapes[j]))
                    {
                        Debug.Log(PhysicsShapes[i].gameObject.name + " and " + PhysicsShapes[j].gameObject.name + " are Colliding!");
                    }
                }

                //if (shape1 == CollisionShape.Sphere &&
                //    shape2 == CollisionShape.Plane)
                //{
                //    // casting from base to derived class
                //    if (PhysicsShapes[i].IsCollidingWithPlane((PhysicsShapePlane)PhysicsShapes[j]))
                //    {
                //        Debug.Log(PhysicsShapes[i].gameObject.name + " and " + PhysicsShapes[j].gameObject.name + " are Colliding!");
                //    }
                //}

            }
        }
    }
}
