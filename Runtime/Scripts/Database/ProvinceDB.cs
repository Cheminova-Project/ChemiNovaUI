using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class ProvinceDB
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

    public static IEnumerator GetProvinceList(UnityAction<ProvinceResponse, bool> onCompleted,
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
            Debug.Log("Provinces obtained successfully: " + request.downloadHandler.text);
            ProvinceResponse provinceResponse = JsonConvert.DeserializeObject<ProvinceResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(provinceResponse.error))
            {
                Debug.Log("List of province saved with the size of " + provinceResponse.items.Count);
                onCompleted?.Invoke(provinceResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + provinceResponse.error);
                onCompleted?.Invoke(provinceResponse, false);
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
    
    public static IEnumerator GetProvinceByID(UnityAction<GeoData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Province obtained successfully: " + request.downloadHandler.text);
            GeoData provinceData = JsonConvert.DeserializeObject<GeoData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(provinceData.error))
            {
                Debug.Log("Province: " + provinceData.name);
                onCompleted?.Invoke(provinceData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + provinceData.error);
                onCompleted?.Invoke(provinceData, false);
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