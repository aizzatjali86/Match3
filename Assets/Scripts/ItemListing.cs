using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListing : MonoBehaviour
{
    public Text itemName;
    public Text price;
    public Button buyButton;

    public string itemId;

    private void Start()
    {
        gameObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { Purchase(itemId); });
    }

    void Purchase(string itemId)
    {
        Debug.Log(itemId);
        PlayFabController.PFC.MakePurchase(itemId, System.Int16.Parse(price.text.Split(' ')[0]));
    }
}
