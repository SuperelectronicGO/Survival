using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToolManager : MonoBehaviour
{
    [SerializeField] private Animator toolAnim;
    public GameObject activeTool;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            toolAnim.SetFloat("ActionState", 1);
        }
        else
        {
            toolAnim.SetFloat("ActionState", 0);
        }
        /*
        if (activeTool != null)
        {
            activeTool.transform.rotation = transform.rotation;
            activeTool.transform.position = transform.position;
        }
        */
    }
}
