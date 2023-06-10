using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public float normalSpeed;
    public float fastspeed;
    private float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public float maxZoom;
    public float minZoom;

    //public float maxY;
    //public float minY;

    //public float maxX;
    //public float minX;


    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;



    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;


    private bool allowMovement;

    public static int playerID;
    // Start is called before the first frame update
    void Start()
    {
       

        newPosition = GetComponent<PlayerProperties>().flagPosition; // Where the specific player's flag is.
        if (GetComponent<PlayerProperties>().id == 1) // If player two, reverse camera Y rotation.
        {
            transform.rotation = Quaternion.Euler(Vector3.up * -45f);
        }

        newRotation = transform.rotation;

        newZoom = cameraTransform.localPosition;

        allowMovement = true;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Match.Instance.getCurrentPlayer() == playerID && Match.Instance.currentCamera != null)
        {

            HandleMovementInput();
             HandleMouseInput();

           
            
            
            
        }
        playerID = GetComponent<PlayerProperties>().id;

    }

    void HandleMouseInput()
    {
        
        // Zooming 
        if (Input.mouseScrollDelta.y != 0 )
        {
            if(newZoom.y > minZoom && newZoom.y < maxZoom)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount * 5;
            }
            else if(newZoom.y < minZoom && Input.mouseScrollDelta.y < 0)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount * 5;
            }
            else if(newZoom.y > maxZoom && Input.mouseScrollDelta.y > 0)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount * 5;
            }
            
        }

        // Moving 
        if (Input.GetMouseButtonDown(0) && allowMovement)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Match.Instance.currentCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0) && allowMovement)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Match.Instance.currentCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        //Rotating

        if (Input.GetMouseButtonDown(1))
        {
            rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }

       
    }

    void HandleMovementInput()
    {
        
        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastspeed;
        }
        else
        {
            movementSpeed = normalSpeed;

        }

        // Moving
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        
        if (transform.position.x > 14 && newPosition.x > 14)
            newPosition.x = 14;
        if (transform.position.x < 0 && newPosition.x < 0)
            newPosition.x = 0;
        if (transform.position.z > 2 && newPosition.z > 2)
            newPosition.z = 2;
        if (transform.position.z < -14 && newPosition.z < -14)
            newPosition.z = -14;

        //Rotating
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        //Zooming
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }

        // Applying change
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);

    }

    public void Movement(bool choice)
    {
        allowMovement = choice;
    }

    public void MoveToFlag()
    {
        newPosition = GetComponent<PlayerProperties>().flagPosition; // Where the specific player's flag is.

        allowMovement = true;
    }
}
