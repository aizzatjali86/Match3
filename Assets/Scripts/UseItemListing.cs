using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseItemListing : MonoBehaviour
{
    public Text itemName;
    public string itemId;
    public string itemInstanceId;

    private void Start()
    {
        gameObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { UseItem(itemInstanceId, itemId); });
    }

    void UseItem(string itemInstanceId, string itemId)
    {
        Debug.Log(itemId);
        GameObject.Find("Player").GetComponent<PlayerManager>().ConsumeItem(itemInstanceId, itemId);
    }
}
