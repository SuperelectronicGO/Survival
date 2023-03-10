using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WorldItemTextManager : MonoBehaviour
{
    [SerializeField] private Transform playerHead;
    private TextMeshPro textMesh;
    private GameObject currentFollowingObject;
    private bool updateItemText = false;
    [SerializeField] private GameObject itemText;

    private void Start()
    {
        textMesh = itemText.GetComponent<TextMeshPro>();
    }
    # region Event subscribing
    private void OnEnable()
    {
        CameraRaycast.onSetWorldText += UpdateWorldItemText;
    }
    private void OnDisable()
    {
        CameraRaycast.onSetWorldText -= UpdateWorldItemText;
    }
    #endregion
    private void Update()
    {
        //Update the position if we are updating the item text
        if (updateItemText)
        {
            UpdateItemTextPosition();
        }
    }
    private void LateUpdate()
    {
        //Update the rotation if we are updating the item text
        if (updateItemText)
        {
            UpdateItemTextRotation();
        }
    }
    public void UpdateWorldItemText(string displayText, GameObject itemToFollow, bool enableText)
    {
        updateItemText = enableText;
        if (updateItemText)
        {
            currentFollowingObject = itemToFollow;
            textMesh.text = displayText;
            itemText.SetActive(true);
        }
        else
        {
            itemText.SetActive(false);
        }
       
        
        

    }
    private void UpdateItemTextPosition()
    {
        Vector3 positionToDisplay = currentFollowingObject.transform.position;
        positionToDisplay.y += 0.75f;
        itemText.transform.position = positionToDisplay;
        
    }
    private void UpdateItemTextRotation()
    {
        var targetRotation = Quaternion.LookRotation(playerHead.position - itemText.transform.position);
        itemText.transform.rotation = Quaternion.Slerp(itemText.transform.rotation, targetRotation, 8 * Time.deltaTime);
    }
}
