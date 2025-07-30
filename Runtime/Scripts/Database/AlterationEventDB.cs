using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class AlterationEventDB
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
    
    public static IEnumerator DeleteAlterationEventFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of alteration event: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetAlterationEventByID(UnityAction<AlterationEventData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Alteration event obtained successfully: " + request.downloadHandler.text);
            
            var alterationEventData = JsonConvert.DeserializeObject<AlterationEventData>(request.downloadHandler.text);
            if (string.IsNullOrEmpty(alterationEventData.error))
            {
                Debug.Log("Alteration event: " + alterationEventData.name);
                onCompleted?.Invoke(alterationEventData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationEventData.error);
                onCompleted?.Invoke(null, false);
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
    
    public static IEnumerator PutAlterationEventFromApi(UnityAction<AlterationEventData, bool> onCompleted,
        int id, [CanBeNull] string description = null, int? idAlterationAgent = null,
        [CanBeNull] List<int> idAlterations = null, [CanBeNull] List<int> idAnnexData = null,
        int? idMaterial = null, [CanBeNull] string name = null)
    {
        AlterationEventPost alterationEventPutData = new AlterationEventPost()
        {
            description = description,
            id_alteration_agent = idAlterationAgent,
            id_alterations = idAlterations,
            id_annex_data = idAnnexData,
            id_material = idMaterial,
            name = name
        };
        
        string jsonData = JsonConvert.SerializeObject(alterationEventPutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Alteration event changed successfully: " + request.downloadHandler.text);
            var alterationEventData = JsonConvert.DeserializeObject<AlterationEventData>(request.downloadHandler.text);
            if (string.IsNullOrEmpty(alterationEventData.error))
            {
                Debug.Log("Alteration event changed: " + alterationEventData.name);
                onCompleted?.Invoke(alterationEventData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationEventData.error);
                onCompleted?.Invoke(null, false);
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