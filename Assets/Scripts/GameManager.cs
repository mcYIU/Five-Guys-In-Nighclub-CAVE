using UnityEngine;
using Votanic.vXR.vGear;

public class GameManager : MonoBehaviour
{
    public static bool isUserInside = false;
    [SerializeField] Animator user;

    private void Update()
    {
        if (vGear.Cmd.Received("Pan") || Input.GetKeyDown(KeyCode.P))
        {
            if (isUserInside) user.SetTrigger("MoveOut");   
            else user.SetTrigger("MoveIn");        
        }
    }
}
