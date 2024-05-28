// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float rotSpeed;

    [SerializeField] private GameObject rotObject;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.RotateAround(rotObject.transform.position, Vector3.up, rotSpeed);
    }
}
