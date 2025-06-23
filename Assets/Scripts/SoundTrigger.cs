using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            soundManager.SetAudio(SOUNDTYPE.indoor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            soundManager.SetAudio(SOUNDTYPE.outdoor);
        }
    }
}
