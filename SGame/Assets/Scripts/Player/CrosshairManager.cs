using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public Canvas canvas;
    public static CrosshairManager instance;
    public enum CrosshairType
    {
        Default,
        Fireball
    }
    public Image crosshair;
    public Image secondaryCrosshairComponent;
    [Space]
    [Header("Crosshair Sprites")]
    public Sprite defaultCrosshairSprite;
    public Sprite fireballCrosshairLeftSprite;
    public Sprite fireballCrosshairRightSprite;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        mainCrosshairRect = crosshair.GetComponent<RectTransform>();
        secondaryCrosshairRect = secondaryCrosshairComponent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetCrosshair(CrosshairType.Default);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetCrosshair(CrosshairType.Fireball);
        }
    }
    private RectTransform mainCrosshairRect;
    private RectTransform secondaryCrosshairRect;
    private Vector3 defaultCrosshairAnchor = new Vector3(0, 0, 0);
    private Vector3 fireballLeftAnchor = new Vector3(-12, 0, 0), fireballRightAnchor = new Vector3(12, 0, 0);
    public void SetCrosshair(CrosshairType type)
    {
        switch (type)
        {
            case CrosshairType.Default:
                //Set images
                crosshair.sprite = defaultCrosshairSprite;
                secondaryCrosshairComponent.gameObject.SetActive(false);
                //Set position
                mainCrosshairRect.localPosition = defaultCrosshairAnchor;
                mainCrosshairRect.sizeDelta = new Vector2(12, 12);
                break;
            case CrosshairType.Fireball:
                //Set images
                crosshair.sprite = fireballCrosshairLeftSprite;
                secondaryCrosshairComponent.sprite = fireballCrosshairRightSprite;
                mainCrosshairRect.localPosition = fireballLeftAnchor;
                secondaryCrosshairRect.localPosition = fireballRightAnchor;
                mainCrosshairRect.sizeDelta = new Vector2(14 , 14);
                secondaryCrosshairRect.sizeDelta = new Vector2(14, 14 );
                secondaryCrosshairComponent.gameObject.SetActive(true);
                //Set position
                break;
        }
    }
    public enum ChangeMode
    {
        Reset,
        Lerp
       
    }
    public void updateCrosshair(CrosshairType type, ChangeMode changeMode, float value, float finalPositionOffset)
    {
        switch (type)
        {
            default:
                break;
            case CrosshairType.Fireball:
                switch (changeMode)
                {
                   
                    case ChangeMode.Reset:
                        mainCrosshairRect.transform.localPosition = fireballLeftAnchor;
                        secondaryCrosshairRect.transform.localPosition = fireballRightAnchor;
                        break;
                    case ChangeMode.Lerp:
                        mainCrosshairRect.transform.localPosition = fireballLeftAnchor - (new Vector3(Mathf.Lerp(0, 1, value/100), 0, 0)*finalPositionOffset);
                        secondaryCrosshairRect.transform.localPosition = fireballRightAnchor + (new Vector3(Mathf.Lerp(0, 1, value / 100), 0, 0) * finalPositionOffset);
                        break;
                }
                break;
        }
    }
}


