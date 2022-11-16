using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODmanager : MonoBehaviour
{
    [SerializeField]
    private int distanceFromPlayer;
    public GameObject player;
    public List<ActivatorItem> activatorItems;
    // Start is called before the first frame update
    void Start()
    {

        activatorItems = new List<ActivatorItem>();

        StartCoroutine("CheckActivation");
    }
    void Update()
    {

    }
    // Update is called once per frame
    IEnumerator CheckActivation()
    {
        List<ActivatorItem> removeList = new List<ActivatorItem>();
        if (activatorItems.Count > 0)
        {
            foreach (ActivatorItem item in activatorItems)
            {
                if (Vector3.Distance(player.transform.position, item.itempos) > distanceFromPlayer)
                {
                    if (item.item == null)
                    {
                        removeList.Add(item);
                    }
                    else
                    {

                        item.item.SetActive(false);
                    }


                }
                else
                {
                    if (item.item == null)
                    {
                        removeList.Add(item);
                    }
                    else
                    {
                        item.item.SetActive(true);
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.01f);


        if (removeList.Count > 0)
        {
            foreach (ActivatorItem item in removeList)
            {
                activatorItems.Remove(item);
            }
        }
        yield return new WaitForSeconds(0.01f);
        StartCoroutine("CheckActivation");
    }
}

public class ActivatorItem
{
    public GameObject item;
    public Vector3 itempos;



}
