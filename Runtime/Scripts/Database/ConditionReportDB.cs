using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class ConditionReportDB
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
    
    public static IEnumerator DeleteConditionReportFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Successful elimination of ConditionReport: " + request.downloadHandler.text);
            ErrorMessage errorMessage = JsonConvert.DeserializeObject<ErrorMessage>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(errorMessage.error))
            {
                //Debug.Log("Element deleted");
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
    
    public static IEnumerator GetConditionReportByID(UnityAction<ConditionReportData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ConditionReport obtained successfully: " + request.downloadHandler.text);
            ConditionReportData conditionReportData = JsonConvert.DeserializeObject<ConditionReportData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(conditionReportData.error))
            {
                Debug.Log("ConditionReport: " + conditionReportData.name);
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
    
    public static IEnumerator PutConditionReportFromApi(UnityAction<ConditionReportData, bool> onCompleted,
        int id, [CanBeNull] string address = null, [CanBeNull] string author = null,
        [CanBeNull] List<ChInspectionMode> chInspectionModes = null, [CanBeNull] List<ChUsageDescription> chUsages = null,
        [CanBeNull] string colocationCharacteristics = null, [CanBeNull] string conservationHistory = null,
        [CanBeNull] string culturalSphere = null, [CanBeNull] string date = null, [CanBeNull] string description = null,
        [CanBeNull] string diagnosisDescription = null, [CanBeNull] string environmentalConditionsCurrentWeatherCondition = null,
        [CanBeNull] string environmentalConditionsPastWeekWeatherCondition = null,
        [CanBeNull] string environmentalConditionsRelativeHumidity = null, [CanBeNull] string environmentalConditionsTemperature = null,
        [CanBeNull] List<int> idAnnotations = null, [CanBeNull] string legalStatus = null, [CanBeNull] string municipality = null,
        [CanBeNull] string name = null, [CanBeNull] string objectString = null, [CanBeNull] string parentAssetName = null,
        [CanBeNull] string province = null, [CanBeNull] string referenceCartographyCadastralParcel = null,
        [CanBeNull] string referenceCartographyCadastralSheet = null, [CanBeNull] string referenceCartographyMunicipality = null,
        [CanBeNull] string referenceCartographyOther = null, [CanBeNull] string reportReferences = null,
        [CanBeNull] string specificLocation = null, StateOfConservationType? stateOfConservation = null, StatusType? status = null,
        [CanBeNull] string subject = null, [CanBeNull] string surveyDate = null, [CanBeNull] string surveyResponsible = null,
        [CanBeNull] string techniques = null)
    {
        ConditionReportPost conditionReportPutData = new ConditionReportPost()
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
        
        string jsonData = JsonConvert.SerializeObject(conditionReportPutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ConditionReport changed successfully: " + request.downloadHandler.text);
            ConditionReportData conditionReportData = JsonConvert.DeserializeObject<ConditionReportData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(conditionReportData.error))
            {
                Debug.Log("ConditionReport changed: " + conditionReportData.name);
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
    
    public static IEnumerator GetConditionReportFileListFromConditionReport(UnityAction<ConditionReportFileResponse, bool> onCompleted,
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
        
        string query = ApiUrl + "/" + id + "/files" + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("ConditionReportFiles obtained successfully: " + request.downloadHandler.text);
            ConditionReportFileResponse conditionReportFileResponse = JsonConvert.DeserializeObject<ConditionReportFileResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(conditionReportFileResponse.error))
            {
                Debug.Log("List of ConditionReportFile saved with the size of " + conditionReportFileResponse.items.Count);
                onCompleted?.Invoke(conditionReportFileResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + conditionReportFileResponse.error);
                onCompleted?.Invoke(conditionReportFileResponse, false);
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
    
    public static IEnumerator PostNewConditionReportFileToConditionReport(UnityAction<ConditionReportFileData, bool> onCompleted,
        int id, int idFile)
    {
        ConditionReportFilePost conditionReportFilePostData = new ConditionReportFilePost()
        {
            id_file = idFile
        };

        string jsonData = JsonConvert.SerializeObject(conditionReportFilePostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/e3ds", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of ConditionReportFile: " + request.downloadHandler.text);
            ConditionReportFileData conditionReportFileData = JsonConvert.DeserializeObject<ConditionReportFileData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(conditionReportFileData.error))
            {
                Debug.Log("ConditionReportFile added " + conditionReportFileData.id);
                onCompleted?.Invoke(conditionReportFileData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + conditionReportFileData.error);
                onCompleted?.Invoke(conditionReportFileData, false);
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