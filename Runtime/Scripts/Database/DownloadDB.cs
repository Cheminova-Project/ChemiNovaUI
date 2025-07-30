using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class DownloadDB
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
    
    public static IEnumerator GetDownloadByID(int? id, UnityAction<RawFileDownload, bool> onCompleted)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] data = request.downloadHandler.data;

            // Creamos RawFileDownload con error = null (porque no se espera JSON de error en este endpoint)
            RawFileDownload rawFileDownload = new RawFileDownload(null, data);
            onCompleted?.Invoke(rawFileDownload, true);
        }
        else
        {
            Debug.LogWarning("Error: " + request.error);
            onCompleted?.Invoke(null, false);
        }
        
        request.Dispose();
    }
    
    public static IEnumerator GetGenerateDownloadCodeByID(UnityAction<RawFileDownload, bool> onCompleted, int id)
    {
        string url = "https://cgisdev.utcluj.ro/cheminovaStagingAPI/generate_download_code/" + id;
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            ErrorMessage errorMessage = JsonConvert.DeserializeObject<ErrorMessage>(request.downloadHandler.text);
            
            if (errorMessage == null || string.IsNullOrEmpty(errorMessage.error))
            {
                byte[] data = request.downloadHandler.data;
                
                onCompleted?.Invoke(new RawFileDownload(errorMessage, data), true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + errorMessage.error);
                onCompleted?.Invoke(new RawFileDownload(errorMessage, null), false);
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

public class RawFileDownload
{
    public ErrorMessage error;
    public byte[] data;
    
    public RawFileDownload(ErrorMessage error, byte[] data)
    {
        this.error = error;
        this.data = data;
    }
}

public class GeneratedDownloadCode
{
    public ErrorMessage error;
    public string download_code;

    public GeneratedDownloadCode(string download_code, ErrorMessage error = null)
    {
        this.error = error;
        this.download_code = download_code;
    }
}