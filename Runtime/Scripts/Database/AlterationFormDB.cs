using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class AlterationFormDB
{
    private static APIBaseConfig config;
    public static string ApiUrl => Config != null ? Config.baseURL : "";

    private static APIBaseConfig Config
    {
        get
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            if (className == null)
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

    public static IEnumerator GetAlterationFormList(UnityAction<AlterationFormResponse, bool> onCompleted,
        int? page = null, int? perPage = null, [CanBeNull] string search = null,
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

        string query = ApiUrl + QueryManagement.ToQueryString(queryParams);
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Alteration forms obtained successfully: " + request.downloadHandler.text);
            AlterationFormResponse alterationFormResponse =
                JsonConvert.DeserializeObject<AlterationFormResponse>(request.downloadHandler.text);

            if (string.IsNullOrEmpty(alterationFormResponse.error))
            {
                //Debug.Log("List of alteration form saved with the size of " + alterationFormResponse.items.Count);
                onCompleted?.Invoke(alterationFormResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationFormResponse.error);
                onCompleted?.Invoke(alterationFormResponse, false);
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

    public static IEnumerator PostNewAlterationForm(UnityAction<AlterationFormData, bool> onCompleted, string name)
    {
        AlterationFormPost alterationFormPostData = new AlterationFormPost()
        {
            name = name
        };

        string jsonData = JsonConvert.SerializeObject(alterationFormPostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of alteration form: " + request.downloadHandler.text);
            AlterationFormData alterationFormData =
                JsonConvert.DeserializeObject<AlterationFormData>(request.downloadHandler.text);

            if (string.IsNullOrEmpty(alterationFormData.error))
            {
                Debug.Log("Alteration form added " + alterationFormData.name);
                onCompleted?.Invoke(alterationFormData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationFormData.error);
                onCompleted?.Invoke(alterationFormData, false);
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

    public static IEnumerator DeleteAlterationFormFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of alteration form: " + request.downloadHandler.text);
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

    public static IEnumerator GetAlterationFormByID(UnityAction<AlterationFormData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Alteration form obtained successfully: " + request.downloadHandler.text);
            AlterationFormData alterationFormData =
                JsonConvert.DeserializeObject<AlterationFormData>(request.downloadHandler.text);

            if (string.IsNullOrEmpty(alterationFormData.error))
            {
                Debug.Log("Alteration form: " + alterationFormData.name);
                onCompleted?.Invoke(alterationFormData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationFormData.error);
                onCompleted?.Invoke(alterationFormData, false);
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

    public static IEnumerator PutAlterationFormFromApi(UnityAction<AlterationFormData, bool> onCompleted,
        int id, [CanBeNull] string name = null)
    {
        AlterationFormPost alterationFormPutData = new AlterationFormPost()
        {
            name = name
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
            Debug.Log("Alteration form changed successfully: " + request.downloadHandler.text);
            AlterationFormData alterationFormData =
                JsonConvert.DeserializeObject<AlterationFormData>(request.downloadHandler.text);

            if (string.IsNullOrEmpty(alterationFormData.error))
            {
                Debug.Log("Alteration form changed: " + alterationFormData.name);
                onCompleted?.Invoke(alterationFormData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + alterationFormData.error);
                onCompleted?.Invoke(alterationFormData, false);
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