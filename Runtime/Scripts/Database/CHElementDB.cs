using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class CHElementDB
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
    
    public static IEnumerator GetCHElementList(UnityAction<CHElementResponse, bool> onCompleted,
        int? page = null, int? perPage = null, [CanBeNull] string search = null,
        [CanBeNull] string sort = null, [CanBeNull] string order = null, [CanBeNull] List<int> alterationId = null,
        [CanBeNull] List<int> materialId = null, int? yearFrom = null, int? yearTo = null, ScaleType? scale = null,
        HeritageCategory? category = null, StateOfConservationType? stateOfConservation = null,
        int? countryId = null, int? provinceId = null, int? municipalityId = null)
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
            year_to = yearTo,
            scale = scale.ToString(),
            category = category.ToString(),
            state_of_conservation = stateOfConservation.ToString(),
            country_id = countryId,
            province_id = provinceId,
            municipality_id = municipalityId
        };
        
        string query = ApiUrl + QueryManagement.ToQueryString(queryParams);
        Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("CHElements obtained successfully: " + request.downloadHandler.text);
            CHElementResponse chelementResponse = JsonConvert.DeserializeObject<CHElementResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(chelementResponse.error))
            {
                Debug.Log("List of CHElement saved with the size of " + chelementResponse.items.Count);
                onCompleted?.Invoke(chelementResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + chelementResponse.error);
                onCompleted?.Invoke(chelementResponse, false);
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
    
    public static IEnumerator PostNewCHElement(UnityAction<CHElementData, bool> onCompleted,
        bool approximateDate, string description, int endYear, HeritageCategory heritageCategory,
        string name, ScaleType scale, SpatialDimension spatialDimension, int startYear,
        [CanBeNull] string address = null, [CanBeNull] string author = null,
        [CanBeNull] List<ChElementMaterial> chElementMaterials = null, [CanBeNull] string culturalSphere = null,
        int? icon = null, int? idCountry = null, int? idMunicipality = null, int? idProvince = null,
        int? idSite = null, float? latitude = null, float? longitude = null)
    {
        CHElementPost chElementPostData = new CHElementPost()
        {
            address = address,
            approximate_date = approximateDate,
            author = author,
            chelement_materials = chElementMaterials,
            cultural_sphere = culturalSphere,
            description = description,
            end_year = endYear,
            heritage_category = heritageCategory.ToString(),
            icon = icon,
            id_country = idCountry,
            id_municipality = idMunicipality,
            id_province = idProvince,
            id_site = idSite,
            latitude = latitude,
            longitude = longitude,
            name = name,
            scale = scale.ToString(),
            spatial_dimension = spatialDimension.ToString(),
            start_year = startYear,
        };

        string jsonData = JsonConvert.SerializeObject(chElementPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of CHElement: " + request.downloadHandler.text);
            CHElementData chElementData = JsonConvert.DeserializeObject<CHElementData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(chElementData.error))
            {
                Debug.Log("CHElement added " + chElementData.name);
                onCompleted?.Invoke(chElementData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + chElementData.error);
                onCompleted?.Invoke(chElementData, false);
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
    
    public static IEnumerator GetCHElementByIDList(List<int> ids, UnityAction<List<CHElementData>, bool> onCompleted)
    {
        List<CHElementData> results = new List<CHElementData>();
        bool hasError = false;
        int completedCount = 0;

        foreach (int id in ids)
        {
            yield return GetCHElementByID((data, success) =>
            {
                completedCount++;
                if (success && data != null)
                {
                    results.Add(data);
                }
                else
                {
                    hasError = true;
                }
            }, id);
        }

        onCompleted?.Invoke(results, !hasError);
    }
    
    public static IEnumerator GetCHElementIcon(int? id, UnityAction<Texture2D, bool> onComplete)
    {
        yield return DownloadDB.GetDownloadByID(id, (rawDownload, success) =>
        {
            if (success && rawDownload.data != null && rawDownload.data.Length > 0)
            {
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                bool loaded = tex.LoadImage(rawDownload.data); // Esto convierte los bytes en textura

                if (loaded)
                {
                    onComplete?.Invoke(tex, true);
                }
                else
                {
                    Debug.LogWarning("Texture couldn't be created from downloaded data.");
                    onComplete?.Invoke(null, false);
                }
            }
            else
            {
                Debug.LogWarning("Empty data or error in the download.");
                onComplete?.Invoke(null, false);
            }
        });
    }
    
    public static IEnumerator DeleteCHElementFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of CHElement: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetCHElementByID(UnityAction<CHElementData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("CHElement obtained successfully: " + request.downloadHandler.text);
            CHElementData chElementData = JsonConvert.DeserializeObject<CHElementData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(chElementData.error))
            {
                //Debug.Log("CHElement: " + chElementData.name);
                onCompleted?.Invoke(chElementData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + chElementData.error);
                onCompleted?.Invoke(chElementData, false);
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
    
    public static IEnumerator PutCHElementFromApi(UnityAction<CHElementData, bool> onCompleted, int id,
        bool? approximateDate = null, [CanBeNull] string description = null, int? endYear = null,
        HeritageCategory? heritageCategory = null, [CanBeNull] string name = null, ScaleType? scale = null,
        SpatialDimension? spatialDimension = null, int? startYear = null, [CanBeNull] string address = null,
        [CanBeNull] string author = null, [CanBeNull] List<ChElementMaterial> chElementMaterials = null,
        [CanBeNull] string culturalSphere = null, int? icon = null, int? idCountry = null,
        int? idMunicipality = null, int? idProvince = null, int? idSite = null, float? latitude = null, float? longitude = null)
    {
        CHElementPost chElementPutData = new CHElementPost()
        {
            address = address,
            approximate_date = approximateDate,
            author = author,
            chelement_materials = chElementMaterials,
            cultural_sphere = culturalSphere,
            description = description,
            end_year = endYear,
            heritage_category = heritageCategory.ToString(),
            icon = icon,
            id_country = idCountry,
            id_municipality = idMunicipality,
            id_province = idProvince,
            id_site = idSite,
            latitude = latitude,
            longitude = longitude,
            name = name,
            scale = scale.ToString(),
            spatial_dimension = spatialDimension.ToString(),
            start_year = startYear,
        };
        
        string jsonData = JsonConvert.SerializeObject(chElementPutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("CHElement changed successfully: " + request.downloadHandler.text);
            CHElementData chElementData = JsonConvert.DeserializeObject<CHElementData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(chElementData.error))
            {
                Debug.Log("CHElement changed: " + chElementData.name);
                onCompleted?.Invoke(chElementData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + chElementData.error);
                onCompleted?.Invoke(chElementData, false);
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
    
    public static IEnumerator GetE3DModelListFromCHElement(UnityAction<E3DModelResponse, bool> onCompleted,
        int id, int? page = null, int? perPage = null, [CanBeNull] string search = null,
        [CanBeNull] string sort = null, [CanBeNull] string order = null, [CanBeNull] List<int> alterationId = null,
        [CanBeNull] List<int> materialId = null, [CanBeNull] string referenceDateFrom = null,
        [CanBeNull] string referenceDateTo = null)
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
            reference_date_from = referenceDateFrom,
            reference_date_to = referenceDateTo
        };
        
        string query = ApiUrl + "/" + id + "/e3ds" + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("E3DModels obtained successfully: " + request.downloadHandler.text);
            E3DModelResponse e3DModelResponse = JsonConvert.DeserializeObject<E3DModelResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(e3DModelResponse.error))
            {
                //Debug.Log("List of E3DModel saved with the size of " + e3DModelResponse.items.Count);
                onCompleted?.Invoke(e3DModelResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + e3DModelResponse.error);
                onCompleted?.Invoke(e3DModelResponse, false);
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
    
    public static IEnumerator PostNewE3DModelToCHElement(UnityAction<E3DModelData, bool> onCompleted,
        int id, string description, float height, float length, string name,
        string referenceDate, float scale, float width, int? icon = null)
    {
        E3DModelPost alterationFormPostData = new E3DModelPost()
        {
            description = description,
            height = height,
            length = length,
            name = name,
            reference_date = referenceDate,
            scale = scale,
            width = width,
            icon = icon
        };

        string jsonData = JsonConvert.SerializeObject(alterationFormPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/e3ds", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of E3DModel: " + request.downloadHandler.text);
            E3DModelData e3DModelData = JsonConvert.DeserializeObject<E3DModelData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(e3DModelData.error))
            {
                Debug.Log("E3DModel added " + e3DModelData.name);
                onCompleted?.Invoke(e3DModelData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + e3DModelData.error);
                onCompleted?.Invoke(e3DModelData, false);
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
    
    public static IEnumerator GetAnnexDataListFromCHElement(UnityAction<AnnexDataResponse, bool> onCompleted,
        int id, int? page = null, int? perPage = null, [CanBeNull] string search = null,
        [CanBeNull] string sort = null, [CanBeNull] string order = null,
        [CanBeNull] List<string> mimeType = null)
    {
        QueryParams queryParams = new QueryParams()
        {
            page = page,
            per_page = perPage,
            search = search,
            sort = sort,
            order = order,
            mime_type = mimeType
        };
        
        string query = ApiUrl + "/" + id + "/annexdata" + QueryManagement.ToQueryString(queryParams);
        Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("AnnexDatas obtained successfully: " + request.downloadHandler.text);
            AnnexDataResponse annexDataResponse = JsonConvert.DeserializeObject<AnnexDataResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(annexDataResponse.error))
            {
                Debug.Log("List of AnnexData saved with the size of " + annexDataResponse.items.Count);
                onCompleted?.Invoke(annexDataResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + annexDataResponse.error);
                onCompleted?.Invoke(annexDataResponse, false);
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
    
    public static IEnumerator GetAnnotationListFromCHElement(UnityAction<AnnotationResponse, bool> onCompleted,
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
    
    public static IEnumerator GetConditionReportListFromCHElement(UnityAction<ConditionReportResponse, bool> onCompleted,
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
        
        string query = ApiUrl + "/" + id + "/conditionreports" + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ConditionReports obtained successfully: " + request.downloadHandler.text);
            ConditionReportResponse conditionReportResponse = JsonConvert.DeserializeObject<ConditionReportResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(conditionReportResponse.error))
            {
                Debug.Log("List of ConditionReport saved with the size of " + conditionReportResponse.items.Count);
                onCompleted?.Invoke(conditionReportResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + conditionReportResponse.error);
                onCompleted?.Invoke(conditionReportResponse, false);
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