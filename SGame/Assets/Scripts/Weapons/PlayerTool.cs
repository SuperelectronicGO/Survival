using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTool : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private void OnEnable()
    {
        anim.Play("PullOut");
    }
}
