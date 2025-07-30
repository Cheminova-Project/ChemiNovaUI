using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class AlterationDB
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

    public static IEnumerator GetAlterationList(UnityAction<AlterationResponse, bool> onCompleted,
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
            Debug.Log("Alterations obtained successfully: " + request.downloadHandler.text);
            AlterationResponse alterationResponse = JsonConvert.DeserializeObject<AlterationResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationResponse.error))
            {
                Debug.Log("List of alteration saved with the size of " + alterationResponse.items.Count);
                onCompleted?.Invoke(alterationResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationResponse.error);
                onCompleted?.Invoke(alterationResponse, false);
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
    
    public static IEnumerator PostNewAlteration(UnityAction<AlterationData, bool> onCompleted,
        int idAlterationForm, string name)
    {
        AlterationPost alterationPostData = new AlterationPost()
        {
            id_alteration_form = idAlterationForm,
            name = name
        };

        string jsonData = JsonConvert.SerializeObject(alterationPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of alteration: " + request.downloadHandler.text);
            AlterationData alterationData = JsonConvert.DeserializeObject<AlterationData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationData.error))
            {
                Debug.Log("Alteration added " + alterationData.name);
                onCompleted?.Invoke(alterationData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationData.error);
                onCompleted?.Invoke(alterationData, false);
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
    
    public static IEnumerator DeleteAlterationFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of alteration: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetAlterationByID(UnityAction<AlterationData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Alteration obtained successfully: " + request.downloadHandler.text);
            AlterationData alterationData = JsonConvert.DeserializeObject<AlterationData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationData.error))
            {
                Debug.Log("Alteration: " + alterationData.name);
                onCompleted?.Invoke(alterationData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationData.error);
                onCompleted?.Invoke(alterationData, false);
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
    
    public static IEnumerator PutAlterationFromApi(UnityAction<AlterationData, bool> onCompleted,
        int id, int? idAlterationForm = null, [CanBeNull] string name = null)
    {
        AlterationPost alterationPutData = new AlterationPost()
        {
            id_alteration_form = idAlterationForm,
            name = name
        };
        
        string jsonData = JsonConvert.SerializeObject(alterationPutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Alteration changed successfully: " + request.downloadHandler.text);
            AlterationData alterationData = JsonConvert.DeserializeObject<AlterationData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationData.error))
            {
                Debug.Log("Alteration changed: " + alterationData.name);
                onCompleted?.Invoke(alterationData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationData.error);
                onCompleted?.Invoke(alterationData, false);
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