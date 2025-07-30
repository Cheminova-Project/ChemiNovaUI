using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class ThreeDInstanceDB
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

    public static IEnumerator GetThreeDInstanceList(UnityAction<ThreeDInstanceResponse, bool> onCompleted,
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
            Debug.Log("ThreeDInstances obtained successfully: " + request.downloadHandler.text);
            ThreeDInstanceResponse threeDInstanceResponse = JsonConvert.DeserializeObject<ThreeDInstanceResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(threeDInstanceResponse.error))
            {
                Debug.Log("List of ThreeDInstance saved with the size of " + threeDInstanceResponse.items.Count);
                onCompleted?.Invoke(threeDInstanceResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + threeDInstanceResponse.error);
                onCompleted?.Invoke(threeDInstanceResponse, false);
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
    
    public static IEnumerator DeleteThreeDInstanceFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of ThreeDInstance: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetThreeDInstanceByID(UnityAction<ThreeDInstanceData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ThreeDInstance obtained successfully: " + request.downloadHandler.text);
            ThreeDInstanceData threeDInstanceData = JsonConvert.DeserializeObject<ThreeDInstanceData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(threeDInstanceData.error))
            {
                Debug.Log("ThreeDInstance: " + threeDInstanceData.name);
                onCompleted?.Invoke(threeDInstanceData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + threeDInstanceData.error);
                onCompleted?.Invoke(threeDInstanceData, false);
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
    
    public static IEnumerator PutThreeDInstanceFromApi(UnityAction<ThreeDInstanceData, bool> onCompleted,
        int id, [CanBeNull] string description = null, DetailLevel? detailLevel = null,
        Format? format = null, int? icon = null, int? idE3dmodel = null, int? idFile = null,
        LicenseType? licenseType = null, [CanBeNull] string name = null,
        [CanBeNull] string sensorConfiguration = null)
    {
        ThreeDInstancePost threeDInstancePutData = new ThreeDInstancePost()
        {
            description = description,
            detail_level = detailLevel.ToString(),
            format = format.ToString(),
            icon = icon,
            id_e3dmodel = idE3dmodel,
            id_file = idFile,
            license_type = licenseType.ToString(),
            name = name,
            sensor_configuration = sensorConfiguration
        };
        
        string jsonData = JsonConvert.SerializeObject(threeDInstancePutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ThreeDInstance changed successfully: " + request.downloadHandler.text);
            ThreeDInstanceData threeDInstanceData = JsonConvert.DeserializeObject<ThreeDInstanceData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(threeDInstanceData.error))
            {
                Debug.Log("ThreeDInstance changed: " + threeDInstanceData.name);
                onCompleted?.Invoke(threeDInstanceData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + threeDInstanceData.error);
                onCompleted?.Invoke(threeDInstanceData, false);
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
    
    public static IEnumerator LoadThreeDInstance(UnityAction<ErrorMessage, bool> onCompleted, int? id)
    {
        string query = ApiUrl + id + "/load/";
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ThreeDInstance loaded: " + request.downloadHandler.text);
            ErrorMessage errorMessage = JsonConvert.DeserializeObject<ErrorMessage>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(errorMessage.error))
            {
                Debug.Log("ThreeDInstance loaded successfully");
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
    
    public static IEnumerator LoadThreeDInstanceWithFileName(UnityAction<ErrorMessage, bool> onCompleted,
        int id, string fileName)
    {
        string query = ApiUrl + id + "/load/" + fileName;
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ThreeDInstance loaded: " + request.downloadHandler.text);
            ErrorMessage errorMessage = JsonConvert.DeserializeObject<ErrorMessage>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(errorMessage.error))
            {
                Debug.Log("ThreeDInstance loaded successfully");
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
    
    public static IEnumerator GetTextureLayerListFromThreeDInstance(UnityAction<TextureLayerResponse, bool> onCompleted,
        int id, int? page = null, int? perPage = null, [CanBeNull] string search = null,
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
        
        string query = ApiUrl + "/" + id + "/textures" + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("TextureLayers obtained successfully: " + request.downloadHandler.text);
            TextureLayerResponse textureLayerResponse = JsonConvert.DeserializeObject<TextureLayerResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(textureLayerResponse.error))
            {
                Debug.Log("List of TextureLayer saved with the size of " + textureLayerResponse.items.Count);
                onCompleted?.Invoke(textureLayerResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + textureLayerResponse.error);
                onCompleted?.Invoke(textureLayerResponse, false);
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
    
    public static IEnumerator PostNewTextureLayerToThreeDInstance(UnityAction<TextureLayerData, bool> onCompleted,
        int id, string config, string description, int idGltfFile, LicenseType licenseType, string name,
        string sensorConfiguration, TextureType textureType, int? icon = null, [CanBeNull] List<int> idTextures = null)
    {
        TextureLayerPost textureLayerPostData = new TextureLayerPost()
        {
            config = config,
            description = description,
            icon = icon,
            id_gltf_file = idGltfFile,
            id_textures = idTextures,
            license_type = licenseType.ToString(),
            name = name,
            sensor_configuration = sensorConfiguration,
            textureType = textureType.ToString()
        };

        string jsonData = JsonConvert.SerializeObject(textureLayerPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/textures", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of TextureLayer: " + request.downloadHandler.text);
            TextureLayerData textureLayerData = JsonConvert.DeserializeObject<TextureLayerData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(textureLayerData.error))
            {
                Debug.Log("TextureLayer added " + textureLayerData.name);
                onCompleted?.Invoke(textureLayerData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + textureLayerData.error);
                onCompleted?.Invoke(textureLayerData, false);
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
    
    public static IEnumerator GetAnnotationListFromThreeDInstance(UnityAction<AnnotationResponse, bool> onCompleted,
        int id, int? page = null, int? perPage = null, [CanBeNull] string search = null,
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
        
        string query = ApiUrl + "/" + id + "/annotations" + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Annotations obtained successfully: " + request.downloadHandler.text);
            AnnotationResponse annotationResponse = JsonConvert.DeserializeObject<AnnotationResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annotationResponse.error))
            {
                Debug.Log("List of Annotation saved with the size of " + annotationResponse.items.Count);
                onCompleted?.Invoke(annotationResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + annotationResponse.error);
                onCompleted?.Invoke(annotationResponse, false);
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
    
    public static IEnumerator PostNewAnnotationToThreeDInstance(UnityAction<AnnotationData, bool> onCompleted,
        int id, List<AnnotationInfo> annotationInfos, string description, string name, AnnotationType type,
        int? icon = null, int? idGroup = null, [CanBeNull] List<int> idTextures = null, int? shapeAndContext = null)
    {
        AnnotationPost annotationPostData = new AnnotationPost()
        {
            annotation_infos = annotationInfos,
            description = description,
            icon = icon,
            id_group = idGroup,
            id_textures = idTextures,
            name = name,
            shape_and_context = shapeAndContext,
            type = type.ToString()
        };

        string jsonData = JsonConvert.SerializeObject(annotationPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/annotations", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of Annotation: " + request.downloadHandler.text);
            AnnotationData annotationData = JsonConvert.DeserializeObject<AnnotationData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annotationData.error))
            {
                Debug.Log("Annotation added " + annotationData.name);
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
    
    public static IEnumerator GetAlterationEventListFromThreeDInstance(UnityAction<AlterationEventResponse, bool> onCompleted,
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
    
    public static IEnumerator GetSpatialRelationListFromThreeDInstance(UnityAction<SpatialRelationResponse, bool> onCompleted,
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
        
        string query = ApiUrl + "/" + id + "/spatialrelations" + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("SpatialRelations obtained successfully: " + request.downloadHandler.text);
            SpatialRelationResponse spatialRelationResponse = JsonConvert.DeserializeObject<SpatialRelationResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(spatialRelationResponse.error))
            {
                //Debug.Log("List of SpatialRelation saved with the size of " + spatialRelationResponse.items.Count);
                onCompleted?.Invoke(spatialRelationResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + spatialRelationResponse.error);
                onCompleted?.Invoke(spatialRelationResponse, false);
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
    
    public static IEnumerator PostNewSpatialRelationToThreeDInstance(UnityAction<SpatialRelationData, bool> onCompleted,
        int id, int idIncluded, InclusionType inclusionType, int transformation)
    {
        SpatialRelationPost spatialRelationPostData = new SpatialRelationPost()
        {
            id_included = idIncluded,
            inclusion_type = inclusionType.ToString(),
            transformation = transformation
        };

        string jsonData = JsonConvert.SerializeObject(spatialRelationPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/spatialrelations", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of SpatialRelation: " + request.downloadHandler.text);
            SpatialRelationData spatialRelationData = JsonConvert.DeserializeObject<SpatialRelationData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(spatialRelationData.error))
            {
                Debug.Log("SpatialRelation added " + spatialRelationData.id);
                onCompleted?.Invoke(spatialRelationData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + spatialRelationData.error);
                onCompleted?.Invoke(spatialRelationData, false);
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