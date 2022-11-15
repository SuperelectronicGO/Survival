using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class changeFilter : MonoBehaviour
{
    [SerializeField] private CraftingManager manager;
  public void changeThisFilter()
    {
        manager.currentFilter = this.gameObject.name;
        manager.searchingForSpecific = false;
        manager.searchingForName = "";
        manager.inputBar.GetComponent<InputField>().text = "";
    }
}
