using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    // Will be updated a bit later when networking is introduced, for now it's a good medium between tiles, cameras, etc.
    [Header("Flag Properties")]
    public Vector3 flagPosition;

    [Header("Player ID")]
    public int id; // For now will be '0' and '1' and will be accessed publicly, later will be more private.

    [Header("Scripts")]
    CameraController cameraController;


    public GameObject gridLayoutObject;

    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
    }

    private void Start()
    {
        flagPosition = gridLayoutObject.GetComponent<HexGridLayout>().GetStartingPosition(id);
    }


}
