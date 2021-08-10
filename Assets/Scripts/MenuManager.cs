using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject panelInFront;
    public GameObject otherPanels;

    public GameObject mainPanel;
    public GameObject mapPanel;
    public GameObject missionPanel;
    public GameObject playerPanel;
    public GameObject shopPanel;
    public GameObject settingsPanel;
    public GameObject inventoryPanel;

    int level;
    public int waitTime;
    public int baseHealth;
    public int baseAttack;
    public Text healthText;
    public Text attackText;
    public Text waitTimeStr;
    public string lastUpgradeTime;

    public Text money;
    public Text buildingMaterial;
    public Text gold;

    public GameObject waitPanel;

    public GameObject itemContainer;
    public GameObject inItem;

    public GameObject shopContainer;
    public GameObject catalogItem;

    // Start is called before the first frame update
    void Start()
    {
        panelInFront.transform.position = new Vector3(0, 0, 0);
        UpdateCurrency();
        GetHealth();
        GetAttack();
    }

    // Update is called once per frame
    void Update()
    {
        EnableLevelButtons(PlayFabController.PFC.level);
        healthText.text = baseHealth.ToString();
        attackText.text = baseAttack.ToString();

        int waitTimeCounter = waitTime - (CurrentTime() - int.Parse(lastUpgradeTime));
        waitTimeStr.text = waitTimeCounter.ToString() + " s";
        // Set wait panel position
        if (waitTimeCounter > 0)
        {
            waitPanel.transform.localPosition = new Vector3(42, 50, 0);
        }
        else { waitPanel.transform.localPosition = new Vector3(42, 320, 0); }
    }

    public void OnClickPanelToFront(string panel)
    {
        OnClickBackToMain();
        switch (panel)
        {
            case "shop":
                StartCoroutine(ShowCatalog());
                UpdateCurrency();
                shopPanel.transform.SetParent(panelInFront.transform);
                shopPanel.transform.localPosition = Vector3.zero;
                break;
            case "player":
                GetLastUpgradeTime();
                GetHealth();
                GetAttack();
                playerPanel.transform.SetParent(panelInFront.transform);
                playerPanel.transform.localPosition = Vector3.zero;
                break;
            case "map":
                mapPanel.transform.SetParent(panelInFront.transform);
                mapPanel.transform.localPosition = Vector3.zero;
                break;
            case "settings":
                settingsPanel.transform.SetParent(panelInFront.transform);
                settingsPanel.transform.localPosition = Vector3.zero;
                break;
            case "mission":
                missionPanel.transform.SetParent(panelInFront.transform);
                missionPanel.transform.localPosition = Vector3.zero;
                break;
            case "inventory":
                StartCoroutine(ShowInventory());
                inventoryPanel.transform.SetParent(panelInFront.transform);
                inventoryPanel.transform.localPosition = Vector3.zero;
                break;
        }
    }
    
    public void OnClickBackToMain()
    {
        panelInFront.transform.GetChild(0).localPosition = new Vector3(255, 0, 0);
        panelInFront.transform.GetChild(0).SetParent(otherPanels.transform);
        mainPanel.transform.SetParent(panelInFront.transform);
        mainPanel.transform.localPosition = Vector3.zero;

        ClearCatalog();
        ClearInventory();
    }

    public void EnableLevelButtons(int level)
    {
        GameObject mapLayout = GameObject.Find("MapLayout");
        for (int i = 0; i < level; i++)
        {
            mapLayout.transform.GetChild(i).GetChild(0).GetComponent<Button>().interactable = true;
        }
    }

    IEnumerator ShowCatalog()
    {
        PlayFabController.PFC.GetCatalogItems();
        List<string[]> catalogList = PlayFabController.PFC.catalogList;
        yield return new WaitUntil(() => catalogList.Count > 0); // wait until list is populated
        foreach (string[] item in catalogList)
        {
            GameObject catalogItemInstance = Instantiate(catalogItem, shopContainer.transform);
            catalogItemInstance.GetComponent<ItemListing>().itemName.text = item[0];
            catalogItemInstance.GetComponent<ItemListing>().price.text = item[1] + " MO";
            catalogItemInstance.GetComponent<ItemListing>().itemId = item[2];
        }
    }

    void ClearCatalog()
    {
        for (int i = 0; i < shopContainer.transform.childCount; i++)
        {
            GameObject catalogItemInstance = shopContainer.transform.GetChild(i).gameObject;
            Destroy(catalogItemInstance);
        }
    }

    IEnumerator ShowInventory()
    {
        PlayFabController.PFC.GetInventory();
        List<string> inventoryList = PlayFabController.PFC.itemList;
        yield return new WaitUntil(() => inventoryList.Count > 0); // wait until list is populated
        foreach (string item in inventoryList)
        {
            GameObject inventoryItemInstance = Instantiate(inItem, itemContainer.transform);
            inventoryItemInstance.GetComponent<InventoryListing>().itemName.text = item;
        }
    }

    void ClearInventory()
    {
        for (int i = 0; i < itemContainer.transform.childCount; i++)
        {
            GameObject inventoryItemInstance = itemContainer.transform.GetChild(i).gameObject;
            Destroy(inventoryItemInstance);
        }
    }

    void SetLastUpgradeTime()
    {
        lastUpgradeTime = CurrentTime().ToString();
        PlayFabController.PFC.SetUserData("lastUpgrade", lastUpgradeTime);
    }

    public void UpdateCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result => {
            List<int> currencies = new List<int>();
            foreach (var pair in result.VirtualCurrency)
            {
                Debug.Log(pair.Value);
                currencies.Add(pair.Value);
            }
            buildingMaterial.text = currencies[0].ToString();
            gold.text = currencies[1].ToString();
            money.text = currencies[2].ToString();
        }, (error) => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void SetHealth()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            // In your game, this should just be a constant matching your primary catalog
            CatalogVersion = "test1",
            ItemId = "upgrade",
            Price = 50,
            VirtualCurrency = "BM"
        }, result => {
            Debug.Log("Upgraded");
            SetLastUpgradeTime();
            baseHealth += 50;
            PlayFabController.PFC.SetUserData("Health", baseHealth.ToString());
            UpdateCurrency();
        }, (error) => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void SetAttack()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            // In your game, this should just be a constant matching your primary catalog
            CatalogVersion = "test1",
            ItemId = "upgrade",
            Price = 50,
            VirtualCurrency = "BM"
        }, result => {
            Debug.Log("Upgraded");
            SetLastUpgradeTime();
            baseAttack += 10;
            PlayFabController.PFC.SetUserData("Attack", baseAttack.ToString());
            UpdateCurrency();
        }, (error) => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    void GetLastUpgradeTime()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("lastUpgrade")) Debug.Log("No needed data");
            else
            {
                Debug.Log("Data: " + result.Data["lastUpgrade"].Value);
                lastUpgradeTime = result.Data["lastUpgrade"].Value;
            }
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void SkipUpdateWait()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            // In your game, this should just be a constant matching your primary catalog
            CatalogVersion = "test1",
            ItemId = "skip",
            Price = 1,
            VirtualCurrency = "GD"
        }, result => {
            Debug.Log("Skipped");
            lastUpgradeTime = "0";
            PlayFabController.PFC.SetUserData("lastUpgrade", lastUpgradeTime);
            UpdateCurrency();
        }, (error) => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void GetHealth()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("Health"))
            {
                Debug.Log("No needed data");
                PlayFabController.PFC.SetUserData("Health", "200");
                GetHealth();
            }
            else
            {
                Debug.Log("Data: " + result.Data["Health"].Value);
                baseHealth = short.Parse(result.Data["Health"].Value);
            }
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void GetAttack()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("Attack"))
            {
                Debug.Log("No needed data");
                PlayFabController.PFC.SetUserData("Attack", "20");
                GetAttack();
            }
            else
            {
                Debug.Log("Data: " + result.Data["Attack"].Value);
                baseAttack = short.Parse(result.Data["Attack"].Value);
            }
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public static int CurrentTime()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

        return currentEpochTime;
    }
}
