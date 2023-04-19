using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingCircle : MonoBehaviour
{
    private Image circle;
    // Start is called before the first frame update
    void Start()
    {
        circle = GetComponent<Image>();
        StartCoroutine(changeFillAmount());
    }
    private void OnEnable()
    {
        StartCoroutine(changeFillAmount());
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, -250*Time.deltaTime));
        if (transform.rotation.z < -360)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

    }

    public IEnumerator changeFillAmount()
    {
        yield return new WaitForEndOfFrame();
        float speedmodifier = 1.2f;
        circle.fillClockwise = true;

        while (circle.fillAmount < 1) {
            speedmodifier += 0.08f;
            circle.fillAmount += 0.16f*speedmodifier*Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(0.25f);
            circle.fillClockwise = false;
            speedmodifier = 1.2f;
            while (circle.fillAmount > 0)
            {
                speedmodifier += 0.08f;
                circle.fillAmount -= 0.16f * speedmodifier * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        
        yield return new WaitForSecondsRealtime(0.25f);
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(changeFillAmount());
        }
        yield break;
    }
}
