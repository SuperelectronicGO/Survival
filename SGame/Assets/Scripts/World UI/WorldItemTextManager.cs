using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WorldItemTextManager : MonoBehaviour
{
    //The head of the player to follow
    [SerializeField] private Transform playerHead;
    //The world text
    private TextMeshPro textMesh;
    //The item currently being followed
    private GameObject currentFollowingObject;
    //ff the text needs to be updated
    private bool updateItemText = false;
    //The GameObject reference to the text
    [SerializeField] private GameObject itemText;

    //Start caches components
    private void Start()
    {
        textMesh = itemText.GetComponent<TextMeshPro>();
    }

    # region Event subscribing
    //When enabled, subscribe to the Set Text CameraRaycast event
    private void OnEnable()
    {
        CameraRaycast.onSetWorldText += UpdateWorldItemText;
    }
    //When disabled, unsubscribe from the Set Text CameraRaycast event
    private void OnDisable()
    {
        CameraRaycast.onSetWorldText -= UpdateWorldItemText;
    }
    #endregion

    //Update sets the text position if text is active
    private void Update()
    {
        //Update the position if we are updating the item text
        if (updateItemText)
        {
            UpdateItemTextPosition();
        }
    }
    //LateUpdate sets the text rotation if the text is active
    private void LateUpdate()
    {
        //Update the rotation if we are updating the item text
        if (updateItemText)
        {
            UpdateItemTextRotation();
        }
    }
    /// <summary>
    /// Method that enables/disables the world text and sets values
    /// </summary>
    /// <param name="displayText">The text that should be displayed</param>
    /// <param name="itemToFollow">The item the text should follow</param>
    /// <param name="enableText">If the text should be displayed</param>
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
    /// <summary>
    /// Method that updates the text position to match its followed item
    /// </summary>
    private void UpdateItemTextPosition()
    {
        //Return if object is null
        if (currentFollowingObject == null) return;
        Vector3 positionToDisplay = currentFollowingObject.transform.position;
        positionToDisplay.y += 0.75f;
        itemText.transform.position = positionToDisplay;
        
    }
    /// <summary>
    /// Method that updates the text rotation to look at the head of the player. Should be called in LateUpdate to avoid jittering.
    /// </summary>
    private void UpdateItemTextRotation()
    {
        var targetRotation = Quaternion.LookRotation(playerHead.position - itemText.transform.position);
        itemText.transform.rotation = Quaternion.Slerp(itemText.transform.rotation, targetRotation, 8 * Time.deltaTime);
    }
    /// <summary>
    /// Method that sets the reference to the 'head' of the player
    /// </summary>
    /// <param name="t">The transform reference to set as</param>
    public void SetPlayerHead(Transform t)
    {
        playerHead = t;
    }
    
}
