using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class AlterationAgentTypeDB
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

    public static IEnumerator GetAlterationAgentTypeList(UnityAction<AlterationAgentTypeResponse, bool> onCompleted,
        int? page = null, int? perPage = null, [CanBeNull] string search = null,
        [CanBeNull] string sort = null, [CanBeNull] string order = null, [CanBeNull] List<int> alterationId = null,
        [CanBeNull] List<int> materialId = null, int? yearFrom = null, int? yearTo = null)
    {
        QueryParams queryParams = new QueryParams()
        {
            page = page,
            per_page = perPage,
            search = search,
            sort = sort,
            order = order,
            alteration_id = alterationId,
            material_id = materialId,
            year_from = yearFrom,
            year_to = yearTo
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
            Debug.Log("Alteration agent types obtained successfully: " + request.downloadHandler.text);
            AlterationAgentTypeResponse alterationAgentTypeResponse = JsonConvert.DeserializeObject<AlterationAgentTypeResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationAgentTypeResponse.error))
            {
                Debug.Log("List of alteration agent type saved with the size of " + alterationAgentTypeResponse.items.Count);
                onCompleted?.Invoke(alterationAgentTypeResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationAgentTypeResponse.error);
                onCompleted?.Invoke(alterationAgentTypeResponse, false);
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
    
    public static IEnumerator PostNewAlterationAgentType(UnityAction<AlterationAgentTypeData, bool> onCompleted, string name)
    {
        AlterationAgentTypePost alterationAgentTypePostData = new AlterationAgentTypePost()
        {
            name = name
        };

        string jsonData = JsonConvert.SerializeObject(alterationAgentTypePostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of alteration agent type: " + request.downloadHandler.text);
            AlterationAgentTypeData alterationAgentTypeData = JsonConvert.DeserializeObject<AlterationAgentTypeData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationAgentTypeData.error))
            {
                Debug.Log("Alteration agent type added " + alterationAgentTypeData.name);
                onCompleted?.Invoke(alterationAgentTypeData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationAgentTypeData.error);
                onCompleted?.Invoke(alterationAgentTypeData, false);
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
    
    public static IEnumerator DeleteAlterationAgentTypeFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of alteration agent: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetAlterationAgentTypeByID(UnityAction<AlterationAgentTypeData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Alteration agent type obtained successfully: " + request.downloadHandler.text);
            AlterationAgentTypeData alterationAgentTypeData = JsonConvert.DeserializeObject<AlterationAgentTypeData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationAgentTypeData.error))
            {
                Debug.Log("Alteration agent type: " + alterationAgentTypeData.name);
                onCompleted?.Invoke(alterationAgentTypeData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationAgentTypeData.error);
                onCompleted?.Invoke(alterationAgentTypeData, false);
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
    
    public static IEnumerator PutAlterationAgentTypeFromApi(UnityAction<AlterationAgentTypeData, bool> onCompleted,
        int id, [CanBeNull] string name = null)
    {
        AlterationAgentTypePost alterationAgentTypePutData = new AlterationAgentTypePost()
        {
            name = name
        };
        
        string jsonData = JsonConvert.SerializeObject(alterationAgentTypePutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Alteration agent type changed successfully: " + request.downloadHandler.text);
            AlterationAgentTypeData alterationAgentTypeData = JsonConvert.DeserializeObject<AlterationAgentTypeData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationAgentTypeData.error))
            {
                Debug.Log("Alteration agent type changed: " + alterationAgentTypeData.name);
                onCompleted?.Invoke(alterationAgentTypeData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationAgentTypeData.error);
                onCompleted?.Invoke(alterationAgentTypeData, false);
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