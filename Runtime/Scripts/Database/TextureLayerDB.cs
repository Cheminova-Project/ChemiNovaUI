using System.Collections;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class TextureLayerDB
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
    
    public static IEnumerator DeleteTextureLayerFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of TextureLayer: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetTextureLayerByID(UnityAction<TextureLayerData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("TextureLayer obtained successfully: " + request.downloadHandler.text);
            TextureLayerData textureLayerData = JsonConvert.DeserializeObject<TextureLayerData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(textureLayerData.error))
            {
                Debug.Log("TextureLayer: " + textureLayerData.name);
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
    
    public static IEnumerator PutTextureLayerFromApi(UnityAction<TextureLayerData, bool> onCompleted,
        int id, [CanBeNull] string config = null, [CanBeNull] string description = null,
        int? icon = null, int? id3DInstance = null, int? idGltfFile = null,
        LicenseType? licenseType = null, [CanBeNull] string name = null,
        [CanBeNull] string sensorConfiguration = null, TextureType? textureType = null)
    {
        TextureLayerPost textureLayerPutData = new TextureLayerPost()
        {
            config = config,
            description = description,
            icon = icon,
            id_3Dinstance = id3DInstance,
            id_gltf_file = idGltfFile,
            license_type = licenseType.ToString(),
            name = name,
            sensor_configuration = sensorConfiguration,
            textureType = textureType.ToString()
        };
        
        string jsonData = JsonConvert.SerializeObject(textureLayerPutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("TextureLayer changed successfully: " + request.downloadHandler.text);
            TextureLayerData textureLayerData = JsonConvert.DeserializeObject<TextureLayerData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(textureLayerData.error))
            {
                Debug.Log("TextureLayer changed: " + textureLayerData.name);
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
}