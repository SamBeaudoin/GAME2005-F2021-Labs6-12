using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public float gravityScale = -9.81f;
    public Vector3 gravity = new Vector3(0, 0, 0);
    public List<BasicObjectPhysics> BasicObjectsList = new List<BasicObjectPhysics>();

    // Start is called before the first frame update
    void Start()
    {
        gravity.y = gravityScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < BasicObjectsList.Count; i++)
        {
            BasicObjectPhysics obj = BasicObjectsList[i];

            if (!obj.lockPosition)
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
                if (objectA.shape.GetCollisionShape() == CollisionShape.AABB && objectB.shape.GetCollisionShape() == CollisionShape.AABB)
                {
                    // do the collision
                    // AABB to AABB
                    AABBAABBCollision((PhysiczColliderBase)objectA.shape, (PhysiczColliderBase)objectB.shape);
                }

            }
        }
    }

    static void SphereSphereCollision(PhysiczSphere a, PhysiczSphere b)
    {
        Vector3 displacement = a.transform.position - b.transform.position;
        float distance = displacement.magnitude;
        float sumRadii = a.radius + b.radius;
        float penitrationDepth = sumRadii - distance;
        bool isOverlapping = penitrationDepth > 0.0f;


        if (isOverlapping)
        {
            Debug.Log(a.name + " collided with: " + b.name);

        }
        else
        {
            return;
        }

        // Normalized vector of length 1, representing the directio from A to B
        Vector3 collisionNormalAtoB = displacement / distance;

        //ComputeMovementScalars(a.kinematicsObject, b.kinematicsObject, out float moveScalarA, out float moveScalarB);

        //// calculate Translations
        Vector3 minimumTranslationVectorAtoB = penitrationDepth * collisionNormalAtoB;
        //Vector3 TranslationVectorA = minimumTranslationVectorAtoB * -moveScalarA;
        //Vector3 TranslationVectorB = -minimumTranslationVectorAtoB * moveScalarB;

        ////// Update Positions based on Translations
        ////a.transform.position += TranslationVectorA;
        ////b.transform.position += TranslationVectorB;

        //b.transform.Translate(TranslationVectorB);
        //a.transform.Translate(TranslationVectorA);

        //ApplyVelocityResponse(a.kinematicsObject, b.kinematicsObject, collisionNormalAtoB);


        Vector3 contactPoint = a.transform.position + collisionNormalAtoB * a.radius;

        ApplyMinimumTraslationVector(a.kinematicsObject, b.kinematicsObject, minimumTranslationVectorAtoB, collisionNormalAtoB, contactPoint);
    }

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

    static void AABBAABBCollision(PhysiczColliderBase objectA, PhysiczColliderBase objectB)
    {
        // GetHalf sizes along each axis (x, y, and z)
        // Get distance detween the boxes on each axis (x, y, and z)

        Vector3 halfSizeA = ((PhysiczAABB)objectA).GetHalfSize();
        Vector3 halfSizeB = ((PhysiczAABB)objectB).GetHalfSize();

        Vector3 displacementAtoB = objectB.transform.position - objectA.transform.position;

        float distX = Mathf.Abs(displacementAtoB.x);
        float distY = Mathf.Abs(displacementAtoB.y);
        float distZ = Mathf.Abs(displacementAtoB.z);

        // For each axis:
        // If the distance between the boxes (along the axis) is less than the sum of the half sizes
        // then they are overlapping

        float penetrationX = halfSizeA.x + halfSizeB.x - distX;
        float penetrationY = halfSizeA.y + halfSizeB.y - distY;
        float penetrationZ = halfSizeA.z + halfSizeB.z - distZ;

        // If there is an overlap along ALL axis then they are colliding, else they are not

        if (penetrationX < 0 || penetrationY < 0 || penetrationZ < 0)
        {
            return;
        }

        // Find minimumTraslationVector (i.e. what is the shortest path we can take)
        // Along which axis are they closest to being seperate
        // Move along that axis according to how much overlap there is

        Vector3 minimumTranslationVector;
        Vector3 collisionNormalAtoB;
        Vector3 contact;

        if (penetrationX < penetrationY && penetrationX < penetrationZ) // is penX the shortest?
        {
            collisionNormalAtoB = new Vector3(Mathf.Sign(displacementAtoB.x), 0, 0);    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationX;
        }
        else if (penetrationY < penetrationX && penetrationY < penetrationZ) // is penY the shortest?
        {
            collisionNormalAtoB = new Vector3(0, Mathf.Sign(displacementAtoB.y), 0);    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationY;
        }
        else //if (penetrationZ < penetrationY && penetrationZ < penetrationX) // is penZ the shortest?   // could just be else
        {
            collisionNormalAtoB = new Vector3(0, 0, Mathf.Sign(displacementAtoB.z));    // Sign returns -1 or 1 based on sign
            minimumTranslationVector = collisionNormalAtoB * penetrationZ;
        }

        contact = objectA.transform.position + minimumTranslationVector;

        ApplyMinimumTraslationVector(objectA.kinematicsObject, objectB.kinematicsObject, minimumTranslationVector, collisionNormalAtoB, contact);

    }

    static void ApplyMinimumTraslationVector(BasicObjectPhysics a, BasicObjectPhysics b, Vector3 minimumTranslationVectorAtoB, Vector3 collisionNormalAtoB, Vector3 contact)
    {
        ComputeMovementScalars(a, b, out float moveScalarA, out float moveScalarB);

        // calculate Translations
        Vector3 TranslationVectorA = -minimumTranslationVectorAtoB * moveScalarA;
        Vector3 TranslationVectorB = minimumTranslationVectorAtoB * moveScalarB;

        //// Update Positions based on Translations
        a.transform.position += TranslationVectorA;
        b.transform.position += TranslationVectorB;

        //b.transform.Translate(TranslationVectorB);
        //a.transform.Translate(TranslationVectorA);

        Vector3 contactPoint = contact;

        //ApplyMinimumTraslationVector(a, b, minimumTranslationVectorAtoB, collisionNormalAtoB, contact);

        ApplyVelocityResponse(a, b, collisionNormalAtoB);

    }

    static void ComputeMovementScalars(BasicObjectPhysics a, BasicObjectPhysics b, out float mtvScalarA, out float mtvScalarB)
    {
        // Check to see if either object is Locked
        if (a.lockPosition && !b.lockPosition)
        {
            mtvScalarA = 0.0f;
            mtvScalarB = 1.0f;
            return;
        }
        if (!a.lockPosition && b.lockPosition)
        {
            mtvScalarA = 1.0f;
            mtvScalarB = 0.0f;
            return;
        }
        if (!a.lockPosition && !b.lockPosition)
        {
            mtvScalarA = 0.5f;
            mtvScalarB = 0.5f;
            return;
        }
        mtvScalarA = 0.0f;
        mtvScalarB = 0.0f;
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

        float deltaV;

        float minimumRelativeVelocityForBounce = 3.0f;

        // If we only need the objects to slide and not bounce, then...
        if (relativeNormalVelocityAB < -minimumRelativeVelocityForBounce)
        {
            // Determine change in velocity 
            deltaV = (relativeNormalVelocityAB * (1.0f + restitution));
        }
        else
        {
            // no bounce
            deltaV = (relativeNormalVelocityAB);
        }

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

        // subtract the component of relative velocity that is along the normal of the collision to receive the tangential velocity
        Vector3 relativeSurfaceVelocity = relativeVelocityAB - (relativeNormalVelocityAB * normal);

        ApplyFriction(objA, objB, relativeSurfaceVelocity, normal);
    }

    static void ApplyFriction(BasicObjectPhysics a, BasicObjectPhysics b, Vector3 relativeSurfaceVelocityAtoB, Vector3 normalAtoB)
    {
        // Need both objects
        // Need relative surface velocity between objects to know which direction to apply the friction force


        float minFrictionSpeed = 0.0001f;
        float relativeSpeed = relativeSurfaceVelocityAtoB.magnitude;

        // Only apply friction if the relative velocity is significant
        if (relativeSurfaceVelocityAtoB.sqrMagnitude < minFrictionSpeed)
        {
            return;
        }

        float kFrictionCoefficient = (a.frictioniness + b.frictioniness) * 0.5f;

        Vector3 directionToApplyFriction = relativeSurfaceVelocityAtoB / relativeSpeed; // normalizing

        Vector3 gravity1 = new Vector3(0.0f, -9.81f, 0.0f); // Not Sure Why I can't Access Gravity??________________________________???

        float gravityAccelerationAlongNormal = Vector3.Dot(gravity1, normalAtoB);    // * by mass to find force

        Vector3 frictionAcceleration = directionToApplyFriction * gravityAccelerationAlongNormal * kFrictionCoefficient;

        if (!a.lockPosition)
        {
            a.velocity -= frictionAcceleration * Time.fixedDeltaTime;   // didn't divide by mass, but could have if we multiplied by mas earlier
        }
        if (!b.lockPosition)
        {
            b.velocity += frictionAcceleration * Time.fixedDeltaTime;
        }


        // Find Normal
        // Find force of gravity
        // Calculate Normal Force from gravity
        // Choose a coefficient of friction



        // Calculate force of friction (Fnormal * coefficientOfFriction)
        // Apply the force of friction opposite to the relative velocity to create an acceleration



        // Keep in mind we have a feature for "Locked" objects



    }

}

