using UnityEngine;

public class DoorRotationTrigger : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && animator != null)
            animator.SetTrigger("Rotate");     
    }

}
