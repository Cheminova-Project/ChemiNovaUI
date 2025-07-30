using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class UserDB
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

    public static IEnumerator GetUserList(UnityAction<UserResponse, bool> onCompleted)
    {
        string query = ApiUrl;
        //Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Users obtained successfully: " + request.downloadHandler.text);
            UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(userResponse.error))
            {
                Debug.Log("List of user saved with the size of " + userResponse.items.Count);
                onCompleted?.Invoke(userResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + userResponse.error);
                onCompleted?.Invoke(userResponse, false);
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
    
    public static IEnumerator PostNewUser(UnityAction<UserData, bool> onCompleted, int active,
        int activityStart, string affiliation, string email, int idCountry, string name,
        string password, string profession, UserRole role, string surname, string username)
    {
        UserPost userDataPost = new UserPost {
            active = active,
            activity_start = activityStart,
            affiliation = affiliation,
            email = email,
            id_country = idCountry,
            name = name,
            password = password,
            profession = profession,
            role = role.ToString(), // Convert the enum to string
            surname = surname,
            username = username
        };

        string jsonData = JsonConvert.SerializeObject(userDataPost);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of user: " + request.downloadHandler.text);
            UserData userData = JsonConvert.DeserializeObject<UserData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(userData.error))
            {
                Debug.Log("User added " + userData.username);
                onCompleted?.Invoke(userData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + userData.error);
                onCompleted?.Invoke(userData, false);
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
    
    public static IEnumerator DeleteUserFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of user: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetUserByID(UnityAction<UserData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("User obtained successfully: " + request.downloadHandler.text);
            UserData userData = JsonConvert.DeserializeObject<UserData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(userData.error))
            {
                //Debug.Log("User: " + userData.username);
                onCompleted?.Invoke(userData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + userData.error);
                onCompleted?.Invoke(userData, false);
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
    
    public static IEnumerator PutUserFromApi(UnityAction<UserData, bool> onCompleted, int id,
        int? active = null, int? activityStart = null, [CanBeNull] string affiliation = null,
        [CanBeNull] string email = null, int? idCountry = null, [CanBeNull] string name = null,
        [CanBeNull] string password = null, [CanBeNull] string profession = null,
        UserRole? role = null, [CanBeNull] string surname = null, [CanBeNull] string username = null)
    {
        UserPost userDataPut = new UserPost {
            active = active,
            activity_start = activityStart,
            affiliation = affiliation,
            email = email,
            id_country = idCountry,
            name = name,
            password = password,
            profession = profession,
            role = role.ToString(), // Convert the enum to string
            surname = surname,
            username = username
        };
        
        string jsonData = JsonConvert.SerializeObject(userDataPut);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("User changed successfully: " + request.downloadHandler.text);
            UserData userData = JsonConvert.DeserializeObject<UserData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(userData.error))
            {
                //Debug.Log("User changed: " + userData.username);
                onCompleted?.Invoke(userData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + userData.error);
                onCompleted?.Invoke(userData, false);
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