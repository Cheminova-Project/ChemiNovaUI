using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class E3DModelDB
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
    
    public static IEnumerator DeleteE3DModelFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of E3DModel: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetE3DModelByID(UnityAction<E3DModelData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("E3DModel obtained successfully: " + request.downloadHandler.text);
            E3DModelData e3DModelData = JsonConvert.DeserializeObject<E3DModelData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(e3DModelData.error))
            {
                //Debug.Log("E3DModel: " + e3DModelData.name);
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
    
    public static IEnumerator PutE3DModelFromApi(UnityAction<E3DModelData, bool> onCompleted, int id,
        [CanBeNull] string description = null, float? height = null, int? icon = null,
        int? idChelement = null, float? length = null, [CanBeNull] string name = null,
        [CanBeNull] string referenceDate = null, float? scale = null, float? width = null)
    {
        E3DModelPost alterationFormPutData = new E3DModelPost()
        {
            description = description,
            height = height,
            icon = icon,
            id_chelement = idChelement,
            length = length,
            name = name,
            reference_date = referenceDate,
            scale = scale,
            width = width
        };
        
        string jsonData = JsonConvert.SerializeObject(alterationFormPutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("E3DModel changed successfully: " + request.downloadHandler.text);
            E3DModelData e3DModelData = JsonConvert.DeserializeObject<E3DModelData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(e3DModelData.error))
            {
                Debug.Log("E3DModel changed: " + e3DModelData.name);
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
    
    public static IEnumerator GetE3DModelByIDList(List<int> ids, int idChElement, UnityAction<List<E3DModelData>, bool> onCompleted)
    {
        List<E3DModelData> results = new List<E3DModelData>();
        bool hasError = false;
        int completedCount = 0;

        foreach (int id in ids)
        {
            yield return GetE3DModelByID((data, success) =>
            {
                completedCount++;
                if (success && data != null)
                {
                    if (data.id_chelement == idChElement)
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
    
    public static IEnumerator GetAnnexDataListFromE3DModel(UnityAction<AnnexDataResponse, bool> onCompleted,
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
        //Debug.Log("API URL: " + query);
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
    
    public static IEnumerator GetThreeDInstanceListFromE3DModel(UnityAction<ThreeDInstanceResponse, bool> onCompleted,
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
        
        string query = ApiUrl + "/" + id + "/e3dinstances" + QueryManagement.ToQueryString(queryParams);
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
    
    public static IEnumerator PostNewThreeDInstanceToE3DModel(UnityAction<ThreeDInstanceData, bool> onCompleted,
        int id, string description, DetailLevel detailLevel, Format format, int idFile,
        LicenseType licenseType, string name, string sensorConfiguration, int? icon = null)
    {
        ThreeDInstancePost threeDInstancePostData = new ThreeDInstancePost()
        {
            description = description,
            detail_level = detailLevel.ToString(),
            format = format.ToString(),
            icon = icon,
            id_file = idFile,
            license_type = licenseType.ToString(),
            name = name,
            sensor_configuration = sensorConfiguration
        };

        string jsonData = JsonConvert.SerializeObject(threeDInstancePostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/e3dinstances", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of ThreeDInstance: " + request.downloadHandler.text);
            ThreeDInstanceData threeDInstanceData = JsonConvert.DeserializeObject<ThreeDInstanceData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(threeDInstanceData.error))
            {
                Debug.Log("ThreeDInstance added " + threeDInstanceData.name);
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
    
    public static IEnumerator GetAnnotationListFromE3DModel(UnityAction<AnnotationResponse, bool> onCompleted,
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
    
    public static IEnumerator GetConditionReportListFromE3DModel(UnityAction<ConditionReportResponse, bool> onCompleted,
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
    
    public static IEnumerator PostNewConditionReportToE3DModel(UnityAction<ConditionReportData, bool> onCompleted,
        int id, [CanBeNull] string address = null, [CanBeNull] string author = null,
        [CanBeNull] List<ChInspectionMode> chInspectionModes = null, [CanBeNull] List<ChUsageDescription> chUsages = null,
        [CanBeNull] string colocationCharacteristics = null, [CanBeNull] string conservationHistory = null,
        [CanBeNull] string culturalSphere = null, [CanBeNull] string date = null, [CanBeNull] string description = null,
        string diagnosisDescription = null, [CanBeNull] string environmentalConditionsCurrentWeatherCondition = null,
        [CanBeNull] string environmentalConditionsPastWeekWeatherCondition = null,
        [CanBeNull] string environmentalConditionsRelativeHumidity = null, [CanBeNull] string environmentalConditionsTemperature = null,
        [CanBeNull] List<int> idAnnotations = null, [CanBeNull] string legalStatus = null, [CanBeNull] string municipality = null,
        string name = null, [CanBeNull] string objectString = null, [CanBeNull] string parentAssetName = null,
        [CanBeNull] string province = null, [CanBeNull] string referenceCartographyCadastralParcel = null,
        [CanBeNull] string referenceCartographyCadastralSheet = null, [CanBeNull] string referenceCartographyMunicipality = null,
        [CanBeNull] string referenceCartographyOther = null, [CanBeNull] string reportReferences = null,
        [CanBeNull] string specificLocation = null, StateOfConservationType? stateOfConservation = null, StatusType? status = null,
        [CanBeNull] string subject = null, [CanBeNull] string surveyDate = null, [CanBeNull] string surveyResponsible = null,
        [CanBeNull] string techniques = null)
    {
        ConditionReportPost conditionReportPostData = new ConditionReportPost()
        {
            address = address,
            author = author,
            ch_inspection_modes = chInspectionModes,
            ch_usages = chUsages,
            colocation_characteristics = colocationCharacteristics,
            conservation_history = conservationHistory,
            cultural_sphere = culturalSphere,
            date = date,
            description = description,
            diagnosis_description = diagnosisDescription,
            environmental_conditions_current_weather_condition = environmentalConditionsCurrentWeatherCondition,
            environmental_conditions_past_week_weather_condition = environmentalConditionsPastWeekWeatherCondition,
            environmental_conditions_relative_humidity = environmentalConditionsRelativeHumidity,
            environmental_conditions_temperature = environmentalConditionsTemperature,
            id_annotations = idAnnotations,
            legal_status = legalStatus,
            municipality = municipality,
            name = name,
            object_string = objectString,
            parent_asset_name = parentAssetName,
            province = province,
            reference_cartography_cadastral_parcel = referenceCartographyCadastralParcel,
            reference_cartography_cadastral_sheet = referenceCartographyCadastralSheet,
            reference_cartography_municipality = referenceCartographyMunicipality,
            reference_cartography_other = referenceCartographyOther,
            report_references = reportReferences,
            specific_location = specificLocation,
            state_of_conservation = stateOfConservation.ToString(),
            status = status.ToString(),
            subject = subject,
            survey_date = surveyDate,
            survey_responsible = surveyResponsible,
            techniques = techniques
        };
        
        string jsonData = JsonConvert.SerializeObject(conditionReportPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/conditionreports", "POST");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of ConditionReport: " + request.downloadHandler.text);
            ConditionReportData conditionReportData = JsonConvert.DeserializeObject<ConditionReportData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(conditionReportData.error))
            {
                Debug.Log("ConditionReport added " + conditionReportData.name);
                onCompleted?.Invoke(conditionReportData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + conditionReportData.error);
                onCompleted?.Invoke(conditionReportData, false);
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
    
    public static IEnumerator GetAlterationEventListFromE3DModel(UnityAction<AlterationEventResponse, bool> onCompleted,
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

    public static IEnumerator GetE3DIcon(int? id, UnityAction<Texture2D, bool> onComplete)
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
                    Debug.LogWarning("No se pudo crear la textura desde los datos descargados.");
                    onComplete?.Invoke(null, false);
                }
            }
            else
            {
                Debug.LogWarning("Error en la descarga o datos vacíos.");
                onComplete?.Invoke(null, false);
            }
        });
    }
}