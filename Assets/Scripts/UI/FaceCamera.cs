// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    /// <summary> method <c>RotateToFaceCamera</c> rotates the obj to face the main camera in the scene. </summary>
    public void RotateToFaceCamera()
    {
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0; // This will make the object only rotate around the y-axis
        Quaternion rotation = Quaternion.LookRotation(-direction);
        transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update()
    {
        RotateToFaceCamera();
    }
}
