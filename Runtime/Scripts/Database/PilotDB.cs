using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class PilotDB
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

    public static IEnumerator GetPilotList(UnityAction<PilotResponse, bool> onCompleted,
        int? page = null, int? perPage = null, [CanBeNull] string search = null,
        [CanBeNull] string sort = null, [CanBeNull] string order = null)
    {
        QueryParams queryParams = new QueryParams()
        {
            page = page,
            per_page = perPage,
            search = search,
            sort = sort,
            order = order
        };
        
        string query = ApiUrl + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Pilots obtained successfully: " + request.downloadHandler.text);
            PilotResponse pilotResponse = JsonConvert.DeserializeObject<PilotResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(pilotResponse.error))
            {
                Debug.Log("List of pilot saved with the size of " + pilotResponse.items.Count);
                onCompleted?.Invoke(pilotResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + pilotResponse.error);
                onCompleted?.Invoke(pilotResponse, false);
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
    
    public static IEnumerator PostNewPilot(UnityAction<PilotData, bool> onCompleted, string description,
        List<int> idSites, string name, int? icon = null)
    {
        PilotPost pilotDataPost = new PilotPost()
        {
            description = description,
            icon = icon,
            id_sites = idSites,
            name = name
        };

        string jsonData = JsonConvert.SerializeObject(pilotDataPost);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of pilot: " + request.downloadHandler.text);
            PilotData pilotData = JsonConvert.DeserializeObject<PilotData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(pilotData.error))
            {
                Debug.Log("Pilot added " + pilotData.name);
                onCompleted?.Invoke(pilotData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + pilotData.error);
                onCompleted?.Invoke(pilotData, false);
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
    
    public static IEnumerator DeletePilotFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of pilot: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetPilotByID(UnityAction<PilotData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Pilot obtained successfully: " + request.downloadHandler.text);
            PilotData pilotData = JsonConvert.DeserializeObject<PilotData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(pilotData.error))
            {
                Debug.Log("Pilot: " + pilotData.name);
                onCompleted?.Invoke(pilotData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + pilotData.error);
                onCompleted?.Invoke(pilotData, false);
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
    
    public static IEnumerator PutPilotFromApi(UnityAction<PilotData, bool> onCompleted, int id,
        [CanBeNull] string description = null, int? icon = null, [CanBeNull] List<int> idSites = null,
        [CanBeNull] string name = null)
    {
        PilotPost pilotDataPut = new PilotPost()
        {
            description = description,
            icon = icon,
            id_sites = idSites,
            name = name
        };
        
        string jsonData = JsonConvert.SerializeObject(pilotDataPut);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Pilot changed successfully: " + request.downloadHandler.text);
            PilotData pilotData = JsonConvert.DeserializeObject<PilotData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(pilotData.error))
            {
                Debug.Log("Pilot changed: " + pilotData.name);
                onCompleted?.Invoke(pilotData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + pilotData.error);
                onCompleted?.Invoke(pilotData, false);
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