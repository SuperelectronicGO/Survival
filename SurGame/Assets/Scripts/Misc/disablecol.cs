using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disablecol : MonoBehaviour
{
    public Animator anim;
    public BoxCollider coll;
    // Start is called before the first frame update
    void Start()
    {
      
        coll = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("swing")&& anim.GetCurrentAnimatorStateInfo(0).normalizedTime<=0.8f&& anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.62f)
        {
            coll.enabled = true;
        }
        else
        {
            coll.enabled = false;
        }
    }
}
