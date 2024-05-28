using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TextHoverEffect : MonoBehaviour
{
    private Color originalColor;
    private TMPro.TextMeshPro textMeshPro;

    void Start()
    {
        // Get the original color of the text
        textMeshPro = GetComponent<TMPro.TextMeshPro>();
        originalColor = textMeshPro.color;

        // Add a Mesh Collider to ensure raycasting works
        AddMeshCollider();
    }

    void Update()
    {
        // Check if the mouse is over the text using raycasting
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            // Change the color when the mouse is over the text
            textMeshPro.color = Color.red; // You can change this to your desired hover color
        }
        else
        {
            // Revert to the original color when the mouse is not over the text
            textMeshPro.color = originalColor;
        }
    }

    void AddMeshCollider()
    {
        // Add a Mesh Collider if not already present
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
            // Configure the Mesh Collider as needed
        }
    }
}