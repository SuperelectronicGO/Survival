using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class ModifiyVFXGraphProperty : MonoBehaviour
{
    public VisualEffect asset;
    [Header("Fireball")]
    [SerializeField] private bool changeOnMove = false;
    private bool moving = false;
    
    // Start is called before the first frame update
    void Start()
    {
        asset = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    public void changeMoveAllow(bool setTo)
    {
        changeOnMove = setTo;
    }
    void Update()
    {
        if (changeOnMove)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                if (!moving)
                {
                    asset.SetFloat("MovingModifier", 2);
                    moving = true;
                }

            }
            else
            {
                if (moving)
                {
                    asset.SetFloat("MovingModifier", 1);
                    moving = false;
                }
            }
        }
        
    }
    public void SetGraphProperty(string property, float value)
    {
        asset.SetFloat(property, value);
    }
    public void SendVFXGraphEvent(string eventName)
    {
        asset.SendEvent(eventName);
    }

}
