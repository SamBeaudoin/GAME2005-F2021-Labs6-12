using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObjectPhysics : MonoBehaviour
{

    public float mass = 1.0f;
    public Vector3 velocity = Vector3.zero;

    public PhysicsManager physicManager;
    public float gravityScale = 1.0f;
    public PhysiczColliderBase shape = null;

    // should this object be able to be controlled by collision response physics?
    public bool lockPosition = false;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World from " + gameObject.name + "!");
        physicManager = FindObjectOfType<PhysicsManager>(); // return the first found component in the scene which has the type
        physicManager.BasicObjectsList.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
