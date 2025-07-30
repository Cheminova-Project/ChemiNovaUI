using System.Collections;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class CountryDB
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

    public static IEnumerator GetCountryList(UnityAction<CountryResponse, bool> onCompleted,
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
            Debug.Log("Countries obtained successfully: " + request.downloadHandler.text);
            CountryResponse countryResponse = JsonConvert.DeserializeObject<CountryResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(countryResponse.error))
            {
                Debug.Log("List of country saved with the size of " + countryResponse.items.Count);
                onCompleted?.Invoke(countryResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + countryResponse.error);
                onCompleted?.Invoke(countryResponse, false);
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
    
    public static IEnumerator GetCountryByID(UnityAction<GeoData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Country obtained successfully: " + request.downloadHandler.text);
            GeoData countryData = JsonConvert.DeserializeObject<GeoData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(countryData.error))
            {
                Debug.Log("Country: " + countryData.name);
                onCompleted?.Invoke(countryData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + countryData.error);
                onCompleted?.Invoke(countryData, false);
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