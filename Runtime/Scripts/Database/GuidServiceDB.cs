using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class GuidServiceDB
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
    
    public static IEnumerator GetGuidServiceByID(UnityAction<GuidServiceData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("GuidService obtained successfully: " + request.downloadHandler.text);
            GuidServiceData guidServiceData = JsonConvert.DeserializeObject<GuidServiceData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(guidServiceData.error))
            {
                Debug.Log("GuidService: " + guidServiceData.entity_type);
                onCompleted?.Invoke(guidServiceData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + guidServiceData.error);
                onCompleted?.Invoke(guidServiceData, false);
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

public class GuidServiceData
{
    public object entity;
    public int entity_id;
    public string entity_type;
    public string error;
}