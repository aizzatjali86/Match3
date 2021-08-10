using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int health;
    public int attack;

    public Slider healthSlider;

    public GameObject inventoryPanel;
    public GameObject itemContainer;
    public GameObject inItem;

    // Start is called before the first frame update
    void Start()
    {
        health = PlayerPrefs.GetInt("HEALTH"); 
        attack = PlayerPrefs.GetInt("ATTACK"); 

        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health;
    }

    public void ConsumeItem(string itemInstanceId, string itemId)
    {
        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
        {
            ConsumeCount = 1,
            // This is a hex-string value from the GetUserInventory result
            ItemInstanceId = itemInstanceId
        }, result =>
        {
            Debug.Log("Used item");
            // Check itemId to see how much to add to health
            switch (itemId)
            {
                case "health1":
                    health += 50;
                    break;
                case "health2":
                    health += 100;
                    break;
                case "attack1":
                    attack += 10;
                    break;
                case "attack2":
                    attack += 20;
                    break;
            }
            ClearInventory();
            GetInventory();
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), 
        result =>
        {
            Debug.Log("Get Inventory Success");
            foreach (var eachItem in result.Inventory)
            {
                GameObject inventoryItemInstance = Instantiate(inItem, itemContainer.transform);
                inventoryItemInstance.GetComponent<UseItemListing>().itemName.text = eachItem.DisplayName;
                inventoryItemInstance.GetComponent<UseItemListing>().itemId = eachItem.ItemId;
                inventoryItemInstance.GetComponent<UseItemListing>().itemInstanceId = eachItem.ItemInstanceId;
            }
        }, (error) => {
            Debug.Log("Got error retrieving inventory:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    void ClearInventory()
    {
        for (int i = 0; i < itemContainer.transform.childCount; i++)
        {
            GameObject inventoryItemInstance = itemContainer.transform.GetChild(i).gameObject;
            Destroy(inventoryItemInstance);
        }
    }

    public void ShowPanel()
    {
        inventoryPanel.transform.localPosition = new Vector3(0, 0, 0);
        GetInventory();
    }

    public void HidePanel()
    {
        inventoryPanel.transform.localPosition = new Vector3(0, -500, 0);
        ClearInventory();
    }
}
