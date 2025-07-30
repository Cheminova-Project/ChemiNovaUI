using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class AnnotationGroupDB
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

    public static IEnumerator GetAnnotationGroupList(UnityAction<AnnotationGroupResponse, bool> onCompleted,
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
            Debug.Log("AnnotationGroups obtained successfully: " + request.downloadHandler.text);
            AnnotationGroupResponse annotationGroupResponse = JsonConvert.DeserializeObject<AnnotationGroupResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annotationGroupResponse.error))
            {
                Debug.Log("List of AnnotationGroup saved with the size of " + annotationGroupResponse.items.Count);
                onCompleted?.Invoke(annotationGroupResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + annotationGroupResponse.error);
                onCompleted?.Invoke(annotationGroupResponse, false);
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
    
    public static IEnumerator PostNewAnnotationGroup(UnityAction<AnnotationGroupData, bool> onCompleted,
        string description, List<int> idAnnotations, string name)
    {
        AnnotationGroupPost annotationGroupPostData = new AnnotationGroupPost()
        {
            description = description,
            id_annotations = idAnnotations,
            name = name
        };

        string jsonData = JsonConvert.SerializeObject(annotationGroupPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of annotationGroup: " + request.downloadHandler.text);
            AnnotationGroupData annotationGroupData = JsonConvert.DeserializeObject<AnnotationGroupData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annotationGroupData.error))
            {
                Debug.Log("AnnotationGroup added " + annotationGroupData.name);
                onCompleted?.Invoke(annotationGroupData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + annotationGroupData.error);
                onCompleted?.Invoke(annotationGroupData, false);
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
    
    public static IEnumerator DeleteAnnotationGroupFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of AnnotationGroup: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetAnnotationGroupByID(UnityAction<AnnotationGroupData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("AnnotationGroup obtained successfully: " + request.downloadHandler.text);
            AnnotationGroupData annotationGroupData = JsonConvert.DeserializeObject<AnnotationGroupData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annotationGroupData.error))
            {
                Debug.Log("AnnotationGroup: " + annotationGroupData.name);
                onCompleted?.Invoke(annotationGroupData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + annotationGroupData.error);
                onCompleted?.Invoke(annotationGroupData, false);
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
    
    public static IEnumerator PutAnnotationGroupFromApi(UnityAction<AnnotationGroupData, bool> onCompleted,
        int id, [CanBeNull] string description = null, [CanBeNull] List<int> idAnnotations = null,
        [CanBeNull] string name = null)
    {
        AnnotationGroupPost annotationGroupPutData = new AnnotationGroupPost()
        {
            description = description,
            id_annotations = idAnnotations,
            name = name
        };
        
        string jsonData = JsonConvert.SerializeObject(annotationGroupPutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("AnnotationGroup changed successfully: " + request.downloadHandler.text);
            AnnotationGroupData annotationGroupData = JsonConvert.DeserializeObject<AnnotationGroupData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annotationGroupData.error))
            {
                Debug.Log("AnnotationGroup changed: " + annotationGroupData.name);
                onCompleted?.Invoke(annotationGroupData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + annotationGroupData.error);
                onCompleted?.Invoke(annotationGroupData, false);
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