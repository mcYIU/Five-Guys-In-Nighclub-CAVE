using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    [Tooltip("The GameObject to enable/disable.")]
    public GameObject targetGameObject;

    [Tooltip("The KeyCode to press to toggle the GameObject.")]
    public KeyCode toggleKey = KeyCode.Space; // Default to Spacebar

    private bool isEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the targetGameObject is assigned in the Inspector.  If not, log an error.
        if (targetGameObject == null)
        {
            Debug.LogError("Target GameObject is not assigned!  Please assign a GameObject in the Inspector.");
            // Disable this script to prevent further errors if the target is not assigned.
            enabled = false;
            return;
        }

        // Initialize the GameObject's state based on the initial isEnabled value.
        targetGameObject.SetActive(isEnabled);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the specified key is pressed.
        if (Input.GetKeyDown(toggleKey))
        {
            // Toggle the isEnabled flag.
            isEnabled = !isEnabled;

            // Set the active state of the target GameObject.
            targetGameObject.SetActive(isEnabled);
        }
    }
}