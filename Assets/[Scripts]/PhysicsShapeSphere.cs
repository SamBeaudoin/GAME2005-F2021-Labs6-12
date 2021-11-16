using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsShapeSphere : PhysicsShapeBase
{
    public float radius = 1.0f;
    

    public override CollisionShape GetCollisionShape()
    {
        return CollisionShape.Sphere;
    }

    //public override bool IsCollidingWithPlane(PhysicsShapePlane other)
    //{
    //    Vector3 pointOnThePlane = other.transform.position;
    //    Vector3 centerOfSphere = this.transform.position;
    //    Vector3 fromPlaneToSphere = centerOfSphere - pointOnThePlane;

    //    //This gives the shortest distance from the plane to the center of the sphere
    //    //The sign of this dot product indicaes which side of the normal this fromPlaneToSphere vector is on
    //    //If negative, they point in the opposite directions
    //    //If positive, they are at least somewhat in the smae direction
    //    //If 0, they are perpendicular
    //    float dot = Vector3.Dot(fromPlaneToSphere, other.GetNormal());
    //    float distance = Mathf.Abs(dot);


    //    /// Combining colors of colliding sphere + plane
    //    Color colorA = this.GetComponent<Renderer>().material.color;
    //    Color colorB = other.GetComponent<Renderer>().material.color;

    //    this.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
    //    other.GetComponent<Renderer>().material.color = Color.Lerp(colorB, colorA, 0.05f);


    //    return (this.radius >= distance);

    //    //Construct any vector from the plane to the sphere
    //    //Use dot product to find the lengt of the projection of the sphere onto the plane
    //    //This gives the shortest distance from the plane to the center of the sphere
    //    //if the distance is less than the radius of the sphere, they are overlapping
    //}

    public override bool IsCollidingWithSphere(PhysicsShapeSphere other)
    {
        //Debug.Log("Collision check with: " + other.gameObject.name);
        // Check collisions with two spheres

        float x1 = this.gameObject.transform.position.x;
        float y1 = this.gameObject.transform.position.y;
        float z1 = this.gameObject.transform.position.z;

        float x2 = other.gameObject.transform.position.x;
        float y2 = other.gameObject.transform.position.y;
        float z2 = other.gameObject.transform.position.z;

        float distance = Mathf.Sqrt(Mathf.Pow((x2 - x1), 2) + Mathf.Pow((y2 - y1), 2) + Mathf.Pow((z2 - z1), 2));

        return (distance < this.radius + other.radius);
        // seems like a lot of math for every single frame... maybe too much

        ////TODO: Collision Response
        ///// The Joss way
        //Vector3 displacement = this.transform.position - other.transform.position;
        //float distance1 = displacement.magnitude;
        //float sumRadii = this.radius + other.radius;


        //return distance < sumRadii;

        ///// Combining colors of colliding spheres
        //Color colorA = this.GetComponent<Renderer>().material.color;
        //Color colorB = other.GetComponent<Renderer>().material.color;

        //this.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
        //other.GetComponent<Renderer>().material.color = Color.Lerp(colorB, colorA, 0.05f);
        /////
    }
}
