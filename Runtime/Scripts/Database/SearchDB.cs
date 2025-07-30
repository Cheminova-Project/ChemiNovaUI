using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class SearchDB
{
    private static APIBaseConfig config;
    private static string ApiUrl => Config != null ? Config.baseURL : "";
    private static APIBaseConfig Config
    {
        get
        {
            if (config == null)
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
            }
            return config;
        }
    }

    public static IEnumerator Search(UnityAction<SearchResponse, bool> onCompleted,
        [CanBeNull] List<int> alterations = null,
        [CanBeNull] List<int> materials = null,
        [CanBeNull] List<SearchType> types = null,
        int page = 0,
        int perPage = 10,
        string searchText = null,
        string sortBy = null,
        string order = null)
    {
        string[] typesString = SearchPost.SearchTypeListToStringArray(types);
        yield return Search(onCompleted, alterations, materials, typesString.ToList(), page, perPage, searchText, sortBy, order);
    }
    
    public static IEnumerator Search(UnityAction<SearchResponse, bool> onCompleted, 
        [CanBeNull] List<int> alterations = null, 
        [CanBeNull] List<int> materials = null, 
        [CanBeNull] List<string> types = null,
        int page = 0,
        int perPage = 10,
        string searchText = null,
        string sortBy = null,
        string order = null)
    {
        int[] alterationsArray = alterations?.ToArray();
        int[] materialsArray = materials?.ToArray();
        string[] typesArray = types?.ToArray();
        
        string typesArrayString = string.Join(",", typesArray);
        Debug.Log("Types of search: " + typesArrayString);
        
        SearchPost searchPost = new SearchPost(alterationsArray, materialsArray, typesArray);

        string jsonData = JsonUtility.ToJson(searchPost);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        
        QueryParams queryParams = new QueryParams()
        {
            page = page,
            per_page = perPage,
            search = searchText,
            sort = sortBy,
            order = order,
        };

        string sb = QueryManagement.ToQueryString(queryParams);
        Debug.Log("Query ->" + sb);
        request.url += sb;
        
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Search results: " + request.downloadHandler.text);
            SearchResponse searchResponse = JsonConvert.DeserializeObject<SearchResponse>(request.downloadHandler.text);
            Debug.Log("Search response obtained successfully: " + request.downloadHandler.text);
            onCompleted?.Invoke(searchResponse, true);
        }
        else
        {
            Debug.LogWarning("Error: " + request.error);
            Debug.LogWarning("Server reply: " + request.downloadHandler.text);
            onCompleted?.Invoke(null, false);
        }
        
        request.Dispose();
    }
    public static SearchType TypedStringToSearchType(string typedString)
    {
        if (string.IsNullOrEmpty(typedString))
        {
            return SearchType.Unknown; // Valor por defecto
        }

        switch (typedString)
        {
            case "e3d_model":
                return SearchType.E3DModel;
            case "e3d_instance":
                return SearchType.E3DInstance;
            case "annotation":
                return SearchType.Annotation;
            case "ch_element":
                return SearchType.CHElement;
            default:
                return SearchType.Unknown; // Valor por defecto
        }
    }
}

[Serializable]
public class SearchPost
{
    public int[] alteration_id;
    public int[] material_id;
    public string[] types;
    // Posibilidades: "e3d_model", "e3d_instance", "annotation", "ch_element"
    public SearchPost(int[] alteration_id, int[] material_id, string[] type)
    {
        this.alteration_id = alteration_id;
        this.material_id = material_id;
        this.types = type;
    }
    public static string[] SearchTypeListToStringArray(List<SearchType> searchTypes)
    {
        List<string> result = new List<string>();
        if (searchTypes == null || searchTypes.Count == 0)
        {
            return result.ToArray();
        }
        foreach (SearchType searchType in searchTypes)
        {
            switch (searchType)
            {
                case SearchType.E3DModel:
                    result.Add("e3d_model");
                    break;
                case SearchType.E3DInstance:
                    result.Add("e3d_instance");
                    break;
                case SearchType.Annotation:
                    result.Add("annotation");
                    break;
                case SearchType.CHElement:
                    result.Add("ch_element");
                    break;
            }
        }
        return result.ToArray();
    }
}