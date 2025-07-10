using UnityEngine;

public class ManagementDoor : MonoBehaviour, KeyListener.IUseKey
{
    public Animator animator;
    public void UseKey()
    {
        animator.SetBool("Open", true);
    }
}
