using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TooltipManager : MonoBehaviour
{
    [SerializeField] private string[] tooltips;
    [SerializeField] private float tooltipDuration;
    [Header("")]
    [SerializeField] private Text tooltipText;
    [SerializeField] private Image tooltipTimer;

    private Animator anim;
    private float currrentTooltipLength;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        selectRandomTooltip();
    }

    // Update is called once per frame
    void Update()
    {
        currrentTooltipLength += Time.deltaTime;
        tooltipTimer.fillAmount =currrentTooltipLength/tooltipDuration;
        if (currrentTooltipLength >= tooltipDuration)
        {
            selectRandomTooltip();
        }
    }
    private void selectRandomTooltip()
    {
        anim.Play("PopIn");
        tooltipTimer.fillAmount = 0;
        tooltipText.text = tooltips[Random.Range(0, tooltips.Length)];
        currrentTooltipLength = 0;
    }
}
