// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterAnim : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    // Called once when active & enabled.
    void OnEnable()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the animation has finished playing
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            // Disable this script
            this.gameObject.SetActive(false);
        }
    }
}
