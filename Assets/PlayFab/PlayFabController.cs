using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections.Generic;
using UnityEngine;

public class PlayFabController: MonoBehaviour
{
    public static PlayFabController PFC;

    private string userEmail;
    private string userPassword;
    private string username;
    public GameObject loadingPanel;
    public GameObject loginPanel;
    public GameObject addLoginPanel;
    public GameObject recoverButton;

    public int level;
    public List<string> itemList = new List<string>();
    public List<string[]> catalogList = new List<string[]>();
    public string userDataData;

    private void OnEnable()
    {
        if (PlayFabController.PFC == null)
        {
            PlayFabController.PFC = this;
        }
        else
        {
            if(PlayFabController.PFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }


    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "CE46F";
        }
        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest
            {
                Email = userEmail,
                Password = userPassword,
            };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), CreateAccount = true};
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
        }

        loadingPanel.transform.localPosition = Vector3.zero;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful login Email API call!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        loginPanel.SetActive(false);
        recoverButton.SetActive(false);
        loadingPanel.SetActive(false);

        StartCloudGetPlayerStats();
        GameObject.Find("Menu").GetComponent<MenuManager>().UpdateCurrency();
        GameObject.Find("Menu").GetComponent<MenuManager>().GetHealth();
        GameObject.Find("Menu").GetComponent<MenuManager>().GetAttack();
    }

    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful login Mobile API call!");
        loginPanel.SetActive(false);
        loadingPanel.SetActive(false);

        StartCloudGetPlayerStats();
        GameObject.Find("Menu").GetComponent<MenuManager>().UpdateCurrency();
        GameObject.Find("Menu").GetComponent<MenuManager>().GetHealth();
        GameObject.Find("Menu").GetComponent<MenuManager>().GetAttack();
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Congratulations, you made your first successful Register Email API call!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        loginPanel.SetActive(false);

        SetUserData("Health", "200");
        SetUserData("Attack", "20");
        StartCloudUpdatePlayerStats(1);
        StartCloudGetPlayerStats();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnLoginMobileFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void GetUserName(string usernameIn)
    {
        username = usernameIn;
    }

    public void OnClickLogin()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = userEmail,
            Password = userPassword,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    public void OpenAddLogin()
    {
        addLoginPanel.SetActive(true);
    }

    public void OnClickAddLogin()
    {
        var addLoginRequest = new AddUsernamePasswordRequest { Email = userEmail, Password = userPassword, Username = username };
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSuccess, OnRegisterFailure);
    }

    private void OnAddLoginSuccess(AddUsernamePasswordResult result)
    {
        Debug.Log("Congratulations, you made your first successful Register Email API call!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        addLoginPanel.SetActive(false);
    }

    public void StartCloudUpdatePlayerStats(int level)
    {
        string levelStr = level.ToString();
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "updatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { gameLevel = levelStr }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, 
        result =>
        {
            Debug.Log("Cloud Script call success: Update stats");
            Debug.Log(result.FunctionResult);
            StartCloudGetPlayerStats();
        },
        error =>
        {
            Debug.Log("Cloud Script call failed");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private static void OnCloudUpdatePlayerStats(ExecuteCloudScriptResult result)
    {
        // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log(PlayFabSimpleJson.SerializeObject(result.FunctionResult));
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript
        Debug.Log((string)messageValue);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void StartCloudGetPlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "getPlayerStatisticByName",
            FunctionParameter = new { Name = "gameLevel" }, // The parameter provided to your function
            GeneratePlayStreamEvent = true,
        },
        result =>
        {
            try
            {
                Debug.Log("Cloud Script call success: Get stats");
                if (short.Parse(result.FunctionResult.ToString()) >= 1)
                {
                    level = short.Parse(result.FunctionResult.ToString());
                }
                else
                {
                    StartCloudUpdatePlayerStats(1);
                }
                Debug.Log(result.FunctionResult);
            }
            catch
            {
                StartCloudUpdatePlayerStats(1);
            }
        },
        error =>
        {
            Debug.Log("Cloud Script call failed");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void GrantItem()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "grantItemsToUser",
            //FunctionParameter = new { Name = "gameLevel" }, // The parameter provided to your function
            GeneratePlayStreamEvent = true,
        },
        result =>
        {
            Debug.Log("Cloud Script call success: Get items");
            Debug.Log(result.FunctionResult);
        },
        error =>
        {
            Debug.Log("Cloud Script call failed");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    // temporary way to get gold. need to work on real payments
    public void GrantGold()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "grantGoldToUser",
            //FunctionParameter = new { Name = "gameLevel" }, // The parameter provided to your function
            GeneratePlayStreamEvent = true,
        },
        result =>
        {
            Debug.Log("Cloud Script call success: Get gold");
            Debug.Log(result.FunctionResult);
            GameObject.Find("Menu").GetComponent<MenuManager>().UpdateCurrency();
        },
        error =>
        {
            Debug.Log("Cloud Script call failed");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void MakePurchase(string itemId, int price)
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            // In your game, this should just be a constant matching your primary catalog
            CatalogVersion = "test1",
            ItemId = itemId,
            Price = price,
            VirtualCurrency = "MO"
        }, OnPurchaseSuccess , LogFailure);
    }

    void OnPurchaseSuccess (PurchaseItemResult result)
    {
        Debug.Log("Purchase Success");
        GameObject.Find("Menu").GetComponent<MenuManager>().UpdateCurrency();
    }

    void LogFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetInventorySuccess, LogFailure);
    }

    void OnGetInventorySuccess(GetUserInventoryResult result)
    {
        itemList.Clear();
        Debug.Log("Get Inventory Success");
        foreach (var eachItem in result.Inventory)
        {
            itemList.Add(eachItem.DisplayName);
        }
    }

    public void GetCatalogItems()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest { CatalogVersion = "test1" }, OnGetCatalogSuccess, LogFailure);
    }

    void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        catalogList.Clear();
        Debug.Log("Get Catalog Success");
        foreach (var eachItem in result.Catalog)
        {
            if (eachItem.Tags[0] != "unusable")
            {
                string[] item = new string[] { eachItem.DisplayName, eachItem.VirtualCurrencyPrices["MO"].ToString(), eachItem.ItemId };
                catalogList.Add(item);
            }
        }
    }

    public void SetUserData(string userDataName, string userDataData)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {userDataName, userDataData},
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void GetUserData(string userDataName)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey(userDataName)) Debug.Log("No needed data");
            else
            {
                Debug.Log("Data: " + result.Data[userDataName].Value);
                userDataData = result.Data[userDataName].Value;
            }
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }


}
