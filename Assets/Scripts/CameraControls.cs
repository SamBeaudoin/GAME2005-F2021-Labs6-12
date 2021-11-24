using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{

    [Header("Controls")]
    public float sensitivity = 1000.0f;
    public float maxSpeed = 10.0f;
    public Vector3 velocity;
    public GameObject panel;

    private float XAxisRotaion = 0.0f;
    private float YAxisRotaion = 0.0f;
    private Vector2 mouse;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(!panel.activeInHierarchy)
        {
            mouse.x = Input.GetAxis("Mouse X") * sensitivity;
            mouse.y = Input.GetAxis("Mouse Y") * sensitivity;

            XAxisRotaion -= mouse.y;
            XAxisRotaion = Mathf.Clamp(XAxisRotaion, -90.0f, 90.0f);
            YAxisRotaion += mouse.x;
            transform.localRotation = Quaternion.Euler(XAxisRotaion, YAxisRotaion, 0.0f);

            Move();
        }
        else
        {

        }
        
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float z = Input.GetAxis("Up");

        Vector3 newPositionY = Vector3.MoveTowards(Vector3.zero, transform.forward * maxSpeed, y * maxSpeed * Time.deltaTime);
        Vector3 newPositionX = Vector3.MoveTowards(Vector3.zero, transform.right * maxSpeed, x * maxSpeed * Time.deltaTime);
        Vector3 newPositionZ = Vector3.MoveTowards(Vector3.zero, transform.up * maxSpeed, z * maxSpeed * Time.deltaTime);
        transform.position += newPositionX + newPositionY + newPositionZ;

    }
}
