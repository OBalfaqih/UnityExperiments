using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    // Bones Details
    private Rigidbody[] bones;
    private Quaternion[] rotations;

    // Hips
    private Vector3 hipsPosition;
    public Transform hips;

    // Animator
    private Animator animator;

    // Ragdoll Status
    private bool activateRagdoll;
    public float rotationSpeed = 3, movementSpeed = 0.33f;
    
    void Start()
    {
        // Get all the bones by getting the rigidbody component in the children
        bones = GetComponentsInChildren<Rigidbody>();
        // Create an empty array of Quaternion with the length of the bones
        rotations = new Quaternion[bones.Length];
        // Check if there's an attached Animator component then assign it the animator variable
        if (GetComponent<Animator>())
            animator = GetComponent<Animator>();
    }

    [ContextMenu("EnableRagdoll")]
    public void EnableRagdoll()
    {
        UpdateRagdollBones();
        activateRagdoll = true;
        if (animator)
            StartCoroutine(ToggleAnimator(false, 0)); // Disable animator after 0s
        else
            Debug.LogWarning("There's no Animator component assigned.");
    }

    [ContextMenu("DisableRagdoll")]
    public void DisableRagdoll()
    {
        activateRagdoll = false;
        if(animator)
            StartCoroutine(ToggleAnimator(true, 1.5f)); // Enable animator in 1.5s
        else
            Debug.LogWarning("There's no Animator component assigned.");
    }

    private void UpdateRagdollBones()
    {
        // Set the hips position to the current character's position
        hipsPosition = transform.position;
        // And get the Y position from the current hips position
        hipsPosition.y = hips.position.y;
        // Update the rotations array
        for (int i = 0; i < bones.Length; i++)
            rotations[i] = bones[i].transform.rotation;
    }

    private IEnumerator ToggleAnimator(bool actv, float time)
    {
        // Wait for "time" seconds and then set animator to "actv"
        yield return new WaitForSeconds(time);
        animator.enabled = actv;
    }

    void FixedUpdate()
    {
        if (activateRagdoll)
        {
            // Set isKinematic to false in all bones to activated the Ragdoll effect
            for (int i = 0; i < bones.Length; i++)
                if (bones[i].isKinematic)
                    bones[i].isKinematic = false;
        }
        else
        {
            for (int i = 0; i < bones.Length; i++)
            {
                // Set each bone's isKinematic to true so it won't be affected by gravity and collision
                if (!bones[i].isKinematic)
                    bones[i].isKinematic = true;
                // Then we update the rotation of each bone to its previous rotation
                bones[i].transform.rotation = Quaternion.Lerp(bones[i].transform.rotation, rotations[i], Time.deltaTime * rotationSpeed);
                // And move the hips to the last position before Ragdoll
                hips.position = Vector3.MoveTowards(hips.position, hipsPosition, Time.deltaTime * movementSpeed);
            }
        }
    }

    // Public method to check if the Ragdoll is active
    public bool isRagdollActive()
    {
        return this.activateRagdoll;
    }
}
