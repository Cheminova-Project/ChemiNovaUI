using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class AnnotationDB
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
    
    public static IEnumerator DeleteAnnotationFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of annotation: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetAnnotationByID(UnityAction<AnnotationData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Annotation obtained successfully: " + request.downloadHandler.text);
            AnnotationData annotationData = JsonConvert.DeserializeObject<AnnotationData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annotationData.error))
            {
                Debug.Log("Annotation: " + annotationData.name);
                onCompleted?.Invoke(annotationData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + annotationData.error);
                onCompleted?.Invoke(annotationData, false);
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
    
    public static IEnumerator PutAnnotationFromApi(UnityAction<AnnotationData, bool> onCompleted,
        int id, [CanBeNull] string description = null, int? icon = null, int? idGroup = null,
        [CanBeNull] List<int> idTextures = null, [CanBeNull] string name = null,
        int? shapeAndContext = null, AnnotationType? type = null)
    {
        AnnotationPost annotationPutData = new AnnotationPost()
        {
            description = description,
            icon = icon,
            id_group = idGroup,
            id_textures = idTextures,
            name = name,
            shape_and_context = shapeAndContext,
            type = type.ToString()
        };
        
        string jsonData = JsonConvert.SerializeObject(annotationPutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Annotation changed successfully: " + request.downloadHandler.text);
            AnnotationData annotationData = JsonConvert.DeserializeObject<AnnotationData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annotationData.error))
            {
                Debug.Log("Annotation changed: " + annotationData.name);
                onCompleted?.Invoke(annotationData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + annotationData.error);
                onCompleted?.Invoke(annotationData, false);
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
    
    public static IEnumerator GetAlterationEventListFromAnnotation(UnityAction<AlterationEventResponse, bool> onCompleted,
        int id, int? page = null, int? perPage = null, [CanBeNull] string search = null,
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
        
        string query = ApiUrl + "/" + id + "/alterationevents" + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("AlterationEvents obtained successfully: " + request.downloadHandler.text);
            AlterationEventResponse alterationEventResponse = JsonConvert.DeserializeObject<AlterationEventResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationEventResponse.error))
            {
                Debug.Log("List of AlterationEvent saved with the size of " + alterationEventResponse.items.Count);
                onCompleted?.Invoke(alterationEventResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationEventResponse.error);
                onCompleted?.Invoke(alterationEventResponse, false);
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
    
    public static IEnumerator PostNewAlterationEventToAnnotation(UnityAction<AlterationEventData, bool> onCompleted,
        int id, string description, List<int> idAlterations, List<int> idAnnexData, int idMaterial,
        string name, int? idAlterationAgent = null)
    {
        AlterationEventPost alterationEventPostData = new AlterationEventPost()
        {
            description = description,
            id_alteration_agent = idAlterationAgent,
            id_alterations = idAlterations,
            id_annex_data = idAnnexData,
            id_material = idMaterial,
            name = name
        };
        
        string jsonData = JsonConvert.SerializeObject(alterationEventPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/alterationevents", "POST");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of AlterationEvent: " + request.downloadHandler.text);
            AlterationEventData alterationEventData = JsonConvert.DeserializeObject<AlterationEventData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(alterationEventData.error))
            {
                Debug.Log("AlterationEvent added " + alterationEventData.name);
                onCompleted?.Invoke(alterationEventData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationEventData.error);
                onCompleted?.Invoke(alterationEventData, false);
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