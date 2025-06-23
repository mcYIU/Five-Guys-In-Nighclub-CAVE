using UnityEngine;
using Votanic.vXR.vCast;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField] vCast_Fader fader;
    [SerializeField] Animator user;
    [SerializeField] Animator textCanvas;

    public void FadeOut() { fader.FadeOut(); }

    public void FadeIn() { fader.FadeIn(); GameManager.isUserInside = true; }

    public void TextIn() { textCanvas.SetTrigger("Fade"); }
}
