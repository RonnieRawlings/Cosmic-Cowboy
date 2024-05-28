using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamagePopup : MonoBehaviour
{
    public float shrinkSpeed = 1f;  // Speed at which the object shrinks
    public float minSize = 0.0f;  // Smallest size the object can be


    private TextMeshPro Textmesh;
    [SerializeField]
    public Transform pfDamagePopup;


    public float moveSpeed = 12f; // Speed at which the object moves
    private Vector3 moveDirection; // Random direction for the object to move in
    private void Awake()
    {
        Textmesh = transform.GetComponent<TextMeshPro>();
        // Generate a random direction for the object to move in
        moveDirection = new Vector3(Random.Range(-2f, 2f), Random.Range(2f, 2f), 0f).normalized;
    }

    public void Setup(int damageAmount)
    {
        Textmesh.SetText(damageAmount.ToString());
        Destroy(this.gameObject, 1f);
    }

  
    private void Update()
    {
        // Shrink the object over time
        transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;

        // Clamp the size to the minimum value
        if (transform.localScale.x < minSize)
        {
            transform.localScale = Vector3.one * minSize;
        }

        // Move the object in a random direction
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}


