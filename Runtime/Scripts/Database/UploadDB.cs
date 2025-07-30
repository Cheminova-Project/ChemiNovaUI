using System.Collections;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class UploadDB
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
    
    public static IEnumerator PostNewUpload(UnityAction<UploadData, bool> onCompleted, string fileChecksum,
        string fileName, FileTypeCategory fileType, int? id = null, int? totalChunks = null)
    {
        UploadDataPost uploadDataPost = new UploadDataPost()
        {
            file_checksum = fileChecksum,
            file_name = fileName,
            file_type = fileType.ToString(),
            id = id,
            total_chunks = totalChunks
        };
        
        string jsonData = JsonConvert.SerializeObject(uploadDataPost);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of upload: " + request.downloadHandler.text);
            UploadData uploadData = JsonConvert.DeserializeObject<UploadData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(uploadData.error))
            {
                Debug.Log("Upload added " + uploadData.username);
                onCompleted?.Invoke(uploadData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + uploadData.error);
                onCompleted?.Invoke(uploadData, false);
            }
        }
        else
        {
            Debug.LogWarning("Error: " + request.error);
            Debug.LogWarning("Server reply: " + request.downloadHandler.text);
            onCompleted?.Invoke(null, false);
        }
    }
    
    public static IEnumerator DeleteUploadFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of upload: " + request.downloadHandler.text);
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
    
    // todo: request body schema -> multipart/form-data
    /*public static IEnumerator PostNewUploadChunks(int id, UnityAction<UploadData, bool> onCompleted)
    {
        UploadDataPost uploadDataPost = new UploadDataPost(fileChecksum, fileName, fileType, id, totalChunks);
        
        string jsonData = JsonConvert.SerializeObject(uploadDataPost);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of upload: " + request.downloadHandler.text);
            UploadData uploadData = JsonConvert.DeserializeObject<UploadData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(uploadData.error))
            {
                Debug.Log("Upload added " + uploadData.username);
                onCompleted?.Invoke(uploadData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + uploadData.error);
                onCompleted?.Invoke(uploadData, false);
            }
        }
        else
        {
            Debug.LogWarning("Error: " + request.error);
            Debug.LogWarning("Server reply: " + request.downloadHandler.text);
            onCompleted?.Invoke(null, false);
        }
        
        request.Dispose();
    }*/
    
    public static IEnumerator PostNewUploadComplete(UnityAction<UploadData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/complete", "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of upload complete: " + request.downloadHandler.text);
            UploadData uploadData = JsonConvert.DeserializeObject<UploadData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(uploadData.error))
            {
                Debug.Log("Upload complete added " + uploadData.username);
                onCompleted?.Invoke(uploadData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + uploadData.error);
                onCompleted?.Invoke(uploadData, false);
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