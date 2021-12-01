using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//struct CollisionInfo
//{
//    public PhysiczColliderBase colliderA;
//    public PhysiczColliderBase colliderB;
//    public Vector3 collisionNormalAtoB;
//    public Vector3 contactPoint;
//}
public class PhysicsManager : MonoBehaviour
{


    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<BasicObjectPhysics> BasicObjectsList = new List<BasicObjectPhysics>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < BasicObjectsList.Count; i++)
        {
            BasicObjectPhysics obj = BasicObjectsList[i];

            //if (!obj.lockPosition)
            {
                // Velocity Update
                obj.velocity += (gravity * obj.gravityScale) * Time.fixedDeltaTime;

                // Position Update
                obj.transform.position = obj.transform.position + obj.velocity * Time.fixedDeltaTime;
            }
        }

        CollisionUpdate();

        //foreach loop works for many types of containers in C#
        //
    }

    void CollisionUpdate()
    {
        for (int i = 0; i < BasicObjectsList.Count; i++)
        {
            for (int j = i + 1; j < BasicObjectsList.Count; j++)
            {
                BasicObjectPhysics objectA = BasicObjectsList[i];
                BasicObjectPhysics objectB = BasicObjectsList[j];

                //if one does not have collision
                if (objectA.shape == null || objectB.shape == null)
                {
                    continue;
                }

                //if both are spheres do a sphere sphere collision
                if (objectA.shape.GetCollisionShape() == CollisionShape.Sphere && objectB.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    //Do the collision
                    //PhysiczObject.shape is a base class refference to physiczcollisderbase
                    //to do specific things with it we need to do a cast to our derived class PhysiczSphere
                    SphereSphereCollision((PhysiczSphere)objectA.shape, (PhysiczSphere)objectB.shape);
                }
                if (objectA.shape.GetCollisionShape() == CollisionShape.Plane && objectB.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    //Do the collision
                    //PhysiczObject.shape is a base class refference to physiczcollisderbase
                    //to do specific things with it we need to do a cast to our derived class PhysiczSphere
                    PlaneSphereCollision(objectA, objectB);
                }
                if (objectB.shape.GetCollisionShape() == CollisionShape.Plane && objectA.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    PlaneSphereCollision(objectB, objectA);
                }

            }
        }
    }

    static void SphereSphereCollision(PhysiczSphere a, PhysiczSphere b)
    {
        Vector3 displacement = b.transform.position - a.transform.position;
        float distance = displacement.magnitude;
        float sumRadii = a.radius + b.radius;
        bool isOverlapping = distance < sumRadii;
        float penitrationDepth = sumRadii - distance;


        if (isOverlapping)
        {
            Debug.Log(a.name + " collided with: " + b.name);
            //Color colorA = a.GetComponent<Renderer>().material.color;
            //Color colorB = b.GetComponent<Renderer>().material.color;
            //a.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
            //b.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
        }
        else
        {
            return;
        }

        Vector3 collisionNormalAtoB;

        if (distance <= 0.0f)
        {
            return;
        }

        collisionNormalAtoB = displacement / distance;


        //// Velocity Calculations
        //Vector3 RelativeVelocity = a.kinematicsObject.velocity - b.kinematicsObject.velocity;
        //Vector3 VelocityNormal = Vector3.Dot(RelativeVelocity, collisionNormalAtoB) * collisionNormalAtoB;

        //// Velocity Adjustments
        //a.kinematicsObject.velocity = a.kinematicsObject.velocity - VelocityNormal;
        //b.kinematicsObject.velocity = b.kinematicsObject.velocity + VelocityNormal;

        //void GetLockedMovementScalars(BasicObjectPhysics a, BasicObjectPhysics b, out float moveScalarA, out float moveScalarB)
        float moveScalarA = 0.5f;
        float moveScalarB = 0.5f;

        if (a.kinematicsObject.lockPosition && !b.kinematicsObject.lockPosition)
        {
            moveScalarA = 0.0f;
            moveScalarB = 1.0f;
        }
        if (!a.kinematicsObject.lockPosition && b.kinematicsObject.lockPosition)
        {
            moveScalarA = 1.0f;
            moveScalarB = 0.0f;
        }
        if (!a.kinematicsObject.lockPosition && !b.kinematicsObject.lockPosition)
        {
            moveScalarA = 0.5f;
            moveScalarB = 0.5f;
        }

        //if(penitrationDepth < 0.1f)
        //{
        //    penitrationDepth = 0.1f;
        //}

        Vector3 minimumTranslationVectorAtoB = penitrationDepth * collisionNormalAtoB;
        Vector3 TranslationVectorA = -minimumTranslationVectorAtoB * moveScalarA;
        Vector3 TranslationVectorB = minimumTranslationVectorAtoB * moveScalarB;

        //a.transform.position += TranslationVectorA * 1.1f;
        //b.transform.position += TranslationVectorB * 1.1f;
        b.transform.Translate(TranslationVectorB);
        a.transform.Translate(TranslationVectorA);


        ApplyVelocityResponse(a.kinematicsObject, b.kinematicsObject, collisionNormalAtoB);

    }

    //// In C++ we can return more than one thing at a time using reference
    //// In C#, we can define "out" parameters, which allows us to return and initialize variables
    //void GetLockedMovementScalars(BasicObjectPhysics a, BasicObjectPhysics b, out float moveScalarA, out float moveScalarB)
    //{
    //    if (a.lockPosition && !b.lockPosition)
    //    {
    //        moveScalarA = 0.0f;
    //        moveScalarB = 1.0f;
    //        return;
    //    }
    //    if (!a.lockPosition && b.lockPosition)
    //    {
    //        moveScalarA = 1.0f;
    //        moveScalarB = 0.0f;
    //        return;
    //    }
    //    if (!a.lockPosition && !b.lockPosition)
    //    {
    //        moveScalarA = 0.5f;
    //        moveScalarB = 0.5f;
    //        return;
    //    }
    //}

    static void PlaneSphereCollision(BasicObjectPhysics plane, BasicObjectPhysics sphere)
    {
        // Construct a vector from any point on the plane to the sphere
        Vector3 somePointOnThePlane = plane.transform.position;

        Vector3 centerOfSphere = sphere.transform.position;

        Vector3 fromPlaneToSphere = centerOfSphere - somePointOnThePlane;

        float dot = Vector3.Dot(fromPlaneToSphere, ((PhysiczPlane)plane.shape).GetNormal());
        // Use dot product to find the length of the projection of the sphere onto the plane
        // This gives the shortest distance from the plane to the center of the sphere
        // The sign of this dot product indicates which side of the normal this fromPlaneToSphere vector is on
        // If the sign is negative they point in the oposite direction
        // If the sign is positive they are at least somewhat in the same direction

        float distance = Mathf.Abs(dot);
        float radius = ((PhysiczSphere)sphere.shape).radius;
        bool isOverlapping = distance <= radius;
        Vector3 penetrationDepth = ((PhysiczPlane)plane.shape).GetNormal() * (distance - radius);

        if (isOverlapping)
        {
            Debug.Log(sphere.name + " collided with: " + plane.name);


            // Adjust Reversal of Velocities based on rotation of plane
            if (((PhysiczPlane)plane.shape).planeID == PlaneID.LEFT     // Left Wall
                || (((PhysiczPlane)plane.shape).planeID == PlaneID.RIGHT))   // Right Wall
            {
                sphere.velocity.z *= -1.0f;
                penetrationDepth.x = 0.0f;
                penetrationDepth.y = 0.0f;
            }
            if (((PhysiczPlane)plane.shape).planeID == PlaneID.BACK   // Back Wall
                || ((PhysiczPlane)plane.shape).planeID == PlaneID.FRONT) // Front Wall
            {
                sphere.velocity.x *= -1.0f;
                penetrationDepth.z = 0.0f;
                penetrationDepth.y = 0.0f;
            }
            if (((PhysiczPlane)plane.shape).planeID == PlaneID.GROUND)        // Ground
            {
                sphere.velocity.y *= -1.0f;
                penetrationDepth.x = 0.0f;
                penetrationDepth.z = 0.0f;
            }

            //sphere.velocity *= 0.5f;       // Energy Loss on bounce
            sphere.transform.Translate(-penetrationDepth);  // Reset position if embedded
        }
    }

    static void ApplyVelocityResponse(BasicObjectPhysics objA, BasicObjectPhysics objB, Vector3 collisionNormal)
    {
        Vector3 normal = collisionNormal;

        // Velocity of B relative to A
        Vector3 relativeVelocityAB = objB.velocity - objA.velocity;
        // Find relative velocity
        float relativeNormalVelocityAB = Vector3.Dot(relativeVelocityAB, normal);

        // Early exit if they are not going towards each other (no bounce)
        if (relativeNormalVelocityAB >= 0.0f)
        {
            return;
        }
        // Choose a coefficient of restitution
        float restitution = (objA.bounciness + objB.bounciness) * 0.5f;

        // Determine change in velocity 
        float deltaV = (relativeNormalVelocityAB * (1.0f + restitution));
        float impulse;

        // respond differently based on locked states
        if (objA.lockPosition && !objB.lockPosition)
        {
            // Only B
            impulse = -deltaV * objB.mass;
            objB.velocity += normal * (impulse / (objB.mass));
        }
        else if (!objA.lockPosition && objB.lockPosition)
        {
            // impulse required to creat our desired change in velocity
            // impulse = Force * time = kg * m/s^2 * s = kg m/s
            // impulse / objA.mass == deltaV
            // Only A change velocity
            impulse = -deltaV * objA.mass; 
            objA.velocity -= normal * (impulse / (objA.mass));
        }
        else if (!objA.lockPosition && !objB.lockPosition)
        {
            // Both
            impulse = deltaV / ((1.0f / objA.mass) + (1.0f / objB.mass));
            objA.velocity += normal * (impulse / objA.mass);
            objB.velocity -= normal * (impulse / objB.mass);
        }
        else if (!objA.lockPosition && !objB.lockPosition)
        {
            // Nadda
        }
        else
        {
            return;
        }
        // Determine the impulse needed to apply this change



    }
}


