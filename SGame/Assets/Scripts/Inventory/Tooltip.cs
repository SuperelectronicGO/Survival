using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;
    public TextMeshProUGUI tooltipText;
    public RectTransform background;
    public RectTransform tooltipRect;
    [SerializeField] private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        tooltipText.text = string.Empty;
        tooltipRect = tooltipText.GetComponent<RectTransform>();
        instance = this;
    }
    public  void updateTooltip(bool settingActive, string text, RectTransform anchor, bool displayOnTop)
    {
        StartCoroutine(updateTooltipRect(settingActive, text, anchor, displayOnTop));
    }
    private IEnumerator updateTooltipRect(bool settingActive, string text, RectTransform anchor, bool displayOnTop)
    {
        if (settingActive)
        {
            //Update the text, then wait for the next frame so we have the calculated data
            tooltipText.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
            tooltipText.text = text;
            tooltipText.ForceMeshUpdate();
            
            if (!displayOnTop)
            {

                transform.position = new Vector3(anchor.position.x + (anchor.rect.width * anchor.localScale.x / 2) + 2, anchor.position.y - ((tooltipRect.rect.height / 3) * canvas.scaleFactor), 0);
            }
            else
            {
                transform.position = new Vector3(anchor.position.x + (anchor.rect.width * anchor.localScale.x / 2) + 2, anchor.position.y + ((tooltipRect.rect.height) * canvas.scaleFactor), 0);

            }
            Vector2 tooltipSizeModified = new Vector2(154, tooltipText.textBounds.size.y+4);
            //Vector2 tooltipSizeModified = new Vector2(tooltipRect.rect.width + 2, tooltipRect.rect.height + 2);
            background.sizeDelta = tooltipSizeModified;
            background.transform.position = new Vector3(tooltipText.transform.position.x - 2, tooltipText.transform.position.y - 2, 0);
        }
        else
        {
            background.gameObject.SetActive(false);
            tooltipText.gameObject.SetActive(false);
            tooltipText.text = string.Empty;

        }
        yield break;
    }



}
