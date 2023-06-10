using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private GameObject horizontalCamera;
    [SerializeField] private GameObject verticalCamera;

    [Header("Camera Settings")]
    [SerializeField] private float maxHorizCamValue;
    [SerializeField] private float maxVertZCamValue;
    [SerializeField] private float maxVertXCamValue;
    [SerializeField] private float cameraStepValue;
    private float minHorizCamValue;
    private float minVertZCamValue;
    private float minVertXCamValue;

    private void Start()
    {
        SetMinCameraValues();
    }

    private void Update()
    {
        if (CheckIfCameraIsEnabled(horizontalCamera))
        {
            CheckIfHorizontalCameraTooFar();
        } else
        {
            CheckIfVerticalCameraTooFar();
        }

        UpdateCameraPositions();
    }

    private void SetMinCameraValues()
    {
        minHorizCamValue = horizontalCamera.transform.position.x;
        minVertZCamValue = verticalCamera.transform.position.y;
        minVertXCamValue = verticalCamera.transform.position.x;
    }

    private void CheckIfHorizontalCameraTooFar()
    {
        if (horizontalCamera.transform.localPosition.x >= maxHorizCamValue)
        {
            SwapToOtherCamera();
            horizontalCamera.transform.position = new Vector3(minHorizCamValue, horizontalCamera.transform.position.y, horizontalCamera.transform.position.z);
        }
    }
    private void CheckIfVerticalCameraTooFar()
    {
        if (verticalCamera.transform.localPosition.z >= maxVertZCamValue || Mathf.Abs(verticalCamera.transform.localPosition.x) >= Mathf.Abs(maxVertXCamValue))
        {
            SwapToOtherCamera();
            verticalCamera.transform.position = new Vector3(minVertXCamValue, verticalCamera.transform.position.y, minVertZCamValue);
        }
    }

    private void UpdateCameraPositions()
    {
        if (CheckIfCameraIsEnabled(horizontalCamera))
        {
            horizontalCamera.transform.position = new Vector3(horizontalCamera.transform.position.x + cameraStepValue * Time.deltaTime, horizontalCamera.transform.position.y, horizontalCamera.transform.position.z);
        }
        if (CheckIfCameraIsEnabled(verticalCamera))
        { 
            verticalCamera.transform.position = new Vector3(verticalCamera.transform.position.x - cameraStepValue * Time.deltaTime, verticalCamera.transform.position.y, verticalCamera.transform.position.z + cameraStepValue * Time.deltaTime);
        }
    }
    
    private bool CheckIfCameraIsEnabled(GameObject camera)
    {
        if (camera.activeSelf)
        {
            return true;
        }
        return false;
    }

    private void SwapToOtherCamera()
    {
        if (CheckIfCameraIsEnabled(horizontalCamera))
        {
            verticalCamera.SetActive(true);
            horizontalCamera.SetActive(false);
        } else // if vertical camera is enabled
        {
            horizontalCamera.SetActive(true);
            verticalCamera.SetActive(false);
        }
    }
}
