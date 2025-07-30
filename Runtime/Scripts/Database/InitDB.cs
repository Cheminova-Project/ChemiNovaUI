using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class InitDB
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
    
    public static IEnumerator PostInitAlterationForms(UnityAction<InitResponse, bool> onCompleted)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/alterationforms", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of init alteration forms: " + request.downloadHandler.text);
            InitResponse initResponseData = JsonConvert.DeserializeObject<InitResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(initResponseData.error))
            {
                Debug.Log("Init alteration form added " + initResponseData.name);
                onCompleted?.Invoke(initResponseData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + initResponseData.error);
                onCompleted?.Invoke(initResponseData, false);
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
    
    public static IEnumerator PostInitAlterations(UnityAction<InitResponse, bool> onCompleted)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/alterations", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of init alterations: " + request.downloadHandler.text);
            InitResponse initResponseData = JsonConvert.DeserializeObject<InitResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(initResponseData.error))
            {
                Debug.Log("Init alteration added " + initResponseData.name);
                onCompleted?.Invoke(initResponseData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + initResponseData.error);
                onCompleted?.Invoke(initResponseData, false);
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
    
    public static IEnumerator PostInitChUsages(UnityAction<InitResponse, bool> onCompleted)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/chusages", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of init ChUsages: " + request.downloadHandler.text);
            InitResponse initResponseData = JsonConvert.DeserializeObject<InitResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(initResponseData.error))
            {
                Debug.Log("Init ChUsage added " + initResponseData.name);
                onCompleted?.Invoke(initResponseData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + initResponseData.error);
                onCompleted?.Invoke(initResponseData, false);
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
    
    public static IEnumerator PostInitCountries(UnityAction<InitResponse, bool> onCompleted)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/countries", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of init countries: " + request.downloadHandler.text);
            InitResponse initResponseData = JsonConvert.DeserializeObject<InitResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(initResponseData.error))
            {
                Debug.Log("Init country added " + initResponseData.name);
                onCompleted?.Invoke(initResponseData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + initResponseData.error);
                onCompleted?.Invoke(initResponseData, false);
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

    public static IEnumerator PostInitInspectionModes(UnityAction<InitResponse, bool> onCompleted)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/inspectionmodes", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of init inspection modes: " + request.downloadHandler.text);
            InitResponse initResponseData = JsonConvert.DeserializeObject<InitResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(initResponseData.error))
            {
                Debug.Log("Init inspection mode added " + initResponseData.name);
                onCompleted?.Invoke(initResponseData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + initResponseData.error);
                onCompleted?.Invoke(initResponseData, false);
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
    
    public static IEnumerator PostInitMaterials(UnityAction<InitResponse, bool> onCompleted)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/materials", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of init materials: " + request.downloadHandler.text);
            InitResponse initResponseData = JsonConvert.DeserializeObject<InitResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(initResponseData.error))
            {
                Debug.Log("Init material added " + initResponseData.name);
                onCompleted?.Invoke(initResponseData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + initResponseData.error);
                onCompleted?.Invoke(initResponseData, false);
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
    
    public static IEnumerator PostInitUsers(UnityAction<InitResponse, bool> onCompleted)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/users", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of init users: " + request.downloadHandler.text);
            InitResponse initResponseData = JsonConvert.DeserializeObject<InitResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(initResponseData.error))
            {
                Debug.Log("Init user added " + initResponseData.name);
                onCompleted?.Invoke(initResponseData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + initResponseData.error);
                onCompleted?.Invoke(initResponseData, false);
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