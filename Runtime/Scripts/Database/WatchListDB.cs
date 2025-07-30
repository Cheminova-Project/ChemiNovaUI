using System.Collections;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class WatchListDB
{
    private static APIBaseConfig config;
    public static string ApiUrl => Config != null ? Config.baseURL : "";
    private static APIBaseConfig Config
    {
        get
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            if(className == null)
            {
                Debug.LogWarning("Couldn't obtained class name.");
                return null;
            }
            else
            {
                //Debug.Log("Name of the class: " + className);
            }
     
            config = Resources.Load<APIBaseConfig>("API_URLS/" + className);
            if (config == null)
            {
                Debug.LogWarning("No APIBaseConfig found in Resources. Create an asset with that name in Resources.");
            }
            return config;
        }
    }
    
    public static IEnumerator GetWatchListList(UnityAction<WatchListResponse, bool> onCompleted)
    {
        //Debug.Log("API URL: " + ApiUrl);
        string auxiliarApiUrl = ApiUrl;
        auxiliarApiUrl += "?per_page=1000000";
        UnityWebRequest request = new UnityWebRequest(auxiliarApiUrl, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("WatchLists obtained successfully: " + request.downloadHandler.text);
            WatchListResponse watchListResponse = JsonConvert.DeserializeObject<WatchListResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(watchListResponse.error))
            {
                Debug.Log("List of WatchList saved with the size of " + watchListResponse.items.Count);
                onCompleted?.Invoke(watchListResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + watchListResponse.error);
                onCompleted?.Invoke(watchListResponse, false);
            }
        }
        else
        {
            Debug.LogWarning("Error: " + request.error);
            Debug.LogWarning("Server reply: " + request.downloadHandler.text);
            onCompleted?.Invoke(null, false);
        }
        
        request.Dispose();
    }
    
    public static IEnumerator DeleteWatchListFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of watchList: " + request.downloadHandler.text);
            ErrorMessage errorMessage = JsonConvert.DeserializeObject<ErrorMessage>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(errorMessage.error))
            {
                Debug.Log("Element deleted");
                onCompleted?.Invoke(null, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + errorMessage.error);
                onCompleted?.Invoke(errorMessage, false);
            }
        }
        else
        {
            Debug.LogWarning("Error: " + request.error);
            Debug.LogWarning("Server reply: " + request.downloadHandler.text);
            onCompleted?.Invoke(null, false);
        }
        
        request.Dispose();
    }
    
    public static IEnumerator PostNewWatchList(UnityAction<WatchListData, bool> onCompleted, int id,
        string entityType, string guid)
    {
        WatchListPost watchListPostData = new WatchListPost()
        {
            entity_type = entityType,
            guid = guid
        };

        string jsonData = JsonConvert.SerializeObject(watchListPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of WatchList: " + request.downloadHandler.text);
            WatchListData watchListData = JsonConvert.DeserializeObject<WatchListData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(watchListData.error))
            {
                Debug.Log("WatchList added " + watchListData.entity_type);
                onCompleted?.Invoke(watchListData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + watchListData.error);
                onCompleted?.Invoke(watchListData, false);
            }
        }
        else
        {
            Debug.LogWarning("Error: " + request.error);
            Debug.LogWarning("Server reply: " + request.downloadHandler.text);
            onCompleted?.Invoke(null, false);
        }
        
        request.Dispose();
    }
}