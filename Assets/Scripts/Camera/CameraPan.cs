// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    // Speed of the camera panning.
    [SerializeField] private float panSpeed = 20f;

    // Last position of the cursor.
    private Vector3 lastMousePos;

    /// <summary> method <c>PanCamera</c> uses mouse position on button hold to determine new camera pan position. </summary>
    void PanCamera()
    {
        // Saves current mouse position on button click.
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }

        // If held, move.
        if (Input.GetMouseButton(1))
        {
            // Diff in mouse pos & new camera move vector.
            Vector3 delta = Input.mousePosition - lastMousePos;
            Vector3 move = new Vector3(-delta.x * panSpeed, 0, -delta.y * panSpeed);

            // Converts move to world space, prevents Y axis issues.
            move = transform.TransformDirection(move);
            move.y = 0;

            // Actually moves the camera.
            transform.parent.Translate(move * Time.deltaTime, Space.World);

            // Updates last mouse pos.
            lastMousePos = Input.mousePosition;

            // RaycastAll, checks for within level bounds.
            RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down);
            bool isOutsideBounds = true;
            foreach (var hit in hits)
            {
                // If hit found, enable camera movement.
                if (hit.collider.tag == "LevelBound")
                {
                    isOutsideBounds = false;
                    break;
                }
            }

            // Revert movement if outside bounds.
            if (isOutsideBounds)
            {
                ResetToLevelBounds();
            }
        }
    }

    /// <summary> method <c>ResetToLevelBounds</c> if the camera position is outside the level bounds, re-center it inside. </summary>
    public void ResetToLevelBounds()
    {
        // Get all colliders in the scene
        BoxCollider[] allColliders = FindObjectsOfType<BoxCollider>();

        // Collider closest to the camera + in bounds already check.
        BoxCollider closestLevelBoundCollider = null;
        Vector3 closestPoint = Vector3.zero;

        // Loop through all colliders.
        float minDistance = float.MaxValue;
        foreach (var collider in allColliders)
        {
            // If bound found, check if it's the closest one
            if (collider.tag == "LevelBound")
            {
                // Calculate the closest point on the bounds of the collider
                Vector3 point = collider.bounds.ClosestPoint(transform.parent.position);

                // Check if the point is within the collider's bounds
                if (collider.bounds.Contains(point))
                {
                    // Calculate the distance from the camera to the point
                    float distance = Vector3.Distance(transform.parent.position, point);

                    // Update distance IF current bound is closer.
                    if (distance < minDistance)
                    {
                        closestLevelBoundCollider = collider;
                        closestPoint = point;
                        minDistance = distance;
                    }
                }
            }
        }

        // Moves camera inside bounds if not already.
        if (closestLevelBoundCollider != null)
        {
            // Move the camera to the closest point
            transform.parent.position = new Vector3(closestPoint.x, transform.parent.position.y, closestPoint.z);
        }
    }

    // Called once on script initlization.
    void OnEnable()
    {
        // Prevents player being unable to pan camera.
        ResetToLevelBounds();
    }

    // Called once every frame.
    void Update()
    {
        // Allows camera to be panned.
        if (!BattleInfo.camBehind) { PanCamera(); }
    }
}