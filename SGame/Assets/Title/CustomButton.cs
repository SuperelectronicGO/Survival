using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
public class CustomButton : MonoBehaviour
{
    [SerializeField] private buttonType type;
    

    private Button_UI button_UI;
    private GameObject button;
    private bool playAnimRetract = false; 
    [SerializeField] private Animator anim;

    
    private enum buttonType  {
    Standard,
    Slide,
    Grow,
    Spin,

        }
    // Start is called before the first frame update
    void Start()
    {
        button_UI = GetComponent<Button_UI>();
        button = this.gameObject;
        if (GetComponent<Animator>() != null)
        {
            anim = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (type == buttonType.Slide)
        {
            GetComponent<Button_UI>().MouseOverOnceFunc = () =>
            {

                anim.Play("Out");

            };
            GetComponent<Button_UI>().MouseOutOnceFunc = () =>
            {

                anim.Play("In");

            };
        }

        if (type == buttonType.Grow)
        {
            GetComponent<Button_UI>().MouseOverOnceFunc = () =>
            {

                anim.Play("Grow");

            };
            GetComponent<Button_UI>().MouseOutOnceFunc = () =>
            {

                anim.Play("Shrink");

            };
        }

        if (type == buttonType.Spin)
        {
           
          GetComponent<Button_UI>().MouseOverFunc = () =>
            {
                playAnimRetract = false;
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("IdleIn"))
                {
                 
                
                anim.Play("SpinOut");
            }

            };




           
            GetComponent<Button_UI>().MouseOutOnceFunc = () =>
            {
                playAnimRetract = true;

                
            };
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("IdleOut")&&playAnimRetract)
            {
                anim.Play("SpinIn");
            }
        }

    }
}
