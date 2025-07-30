using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class MunicipalityDB
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

    public static IEnumerator GetMunicipalityList(UnityAction<MunicipalityResponse, bool> onCompleted,
        int? page = null, int? perPage = null, [CanBeNull] string search = null,
        [CanBeNull] string sort = null, [CanBeNull] string order = null,
        [CanBeNull] string country = null, [CanBeNull] string admin1 = null)
    {
        QueryParams queryParams = new QueryParams()
        {
            page = page,
            per_page = perPage,
            search = search,
            sort = sort,
            order = order,
            country = country,
            admin1 = admin1
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
            Debug.Log("Municipalities obtained successfully: " + request.downloadHandler.text);
            MunicipalityResponse municipalityResponse = JsonConvert.DeserializeObject<MunicipalityResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(municipalityResponse.error))
            {
                Debug.Log("List of municipality saved with the size of " + municipalityResponse.items.Count);
                onCompleted?.Invoke(municipalityResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + municipalityResponse.error);
                onCompleted?.Invoke(municipalityResponse, false);
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
    
    public static IEnumerator GetMunicipalityByID(UnityAction<GeoData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Municipality obtained successfully: " + request.downloadHandler.text);
            GeoData municipalityData = JsonConvert.DeserializeObject<GeoData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(municipalityData.error))
            {
                Debug.Log("Municipality: " + municipalityData.name);
                onCompleted?.Invoke(municipalityData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + municipalityData.error);
                onCompleted?.Invoke(municipalityData, false);
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