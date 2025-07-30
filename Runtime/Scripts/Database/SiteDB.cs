using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class SiteDB
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
    
    public static IEnumerator GetSiteIcon(int? id, UnityAction<Texture2D, bool> onComplete)
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
    
    public static IEnumerator GetSiteIcon(int id, UnityAction<RawFileDownload, bool> onComplete)
    {
        yield return DownloadDB.GetDownloadByID(id, onComplete);
    }

    public static IEnumerator GetSiteList(UnityAction<SiteResponse, bool> onCompleted,
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
            Debug.Log("Sites obtained successfully: " + request.downloadHandler.text);
            SiteResponse siteResponse = JsonConvert.DeserializeObject<SiteResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(siteResponse.error))
            {
                Debug.Log("List of site saved with the size of " + siteResponse.items.Count);
                onCompleted?.Invoke(siteResponse, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + siteResponse.error);
                onCompleted?.Invoke(siteResponse, false);
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
    
    public static IEnumerator PostNewSite(UnityAction<SiteData, bool> onCompleted, string description,
        int idCountry, float latitude, float longitude, string name, int? icon = null)
    {
        SitePost sitePostData = new SitePost()
        {
            description = description,
            icon = icon,
            id_country = idCountry,
            latitude = latitude,
            longitude = longitude,
            name = name
        };

        string jsonData = JsonConvert.SerializeObject(sitePostData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful creation of site: " + request.downloadHandler.text);
            SiteData siteData = JsonConvert.DeserializeObject<SiteData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(siteData.error))
            {
                Debug.Log("Site added " + siteData.name);
                onCompleted?.Invoke(siteData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + siteData.error);
                onCompleted?.Invoke(siteData, false);
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
    
    public static IEnumerator DeleteSiteFromApi(UnityAction<ErrorMessage, bool> onCompleted, int id)
    {
        UnityWebRequest request = UnityWebRequest.Delete(ApiUrl + "/" + id);
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successful elimination of site: " + request.downloadHandler.text);
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
    
    public static IEnumerator GetSiteByID(UnityAction<SiteData, bool> onCompleted, int id)
    {
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Site obtained successfully: " + request.downloadHandler.text);
            SiteData siteData = JsonConvert.DeserializeObject<SiteData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(siteData.error))
            {
                Debug.Log("Site: " + siteData.name);
                onCompleted?.Invoke(siteData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + siteData.error);
                onCompleted?.Invoke(siteData, false);
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
    
    public static IEnumerator PutSiteFromApi(UnityAction<SiteData, bool> onCompleted, int id,
        [CanBeNull] string description = null, int? icon = null, int idCountry = 0,
        float latitude = -90f, float longitude = -180f, [CanBeNull] string name = null)
    {
        SitePost sitePutData = new SitePost()
        {
            description = description,
            icon = icon,
            id_country = idCountry,
            latitude = latitude,
            longitude = longitude,
            name = name
        };
        
        string jsonData = JsonConvert.SerializeObject(sitePutData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id, "PUT");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Site changed successfully: " + request.downloadHandler.text);
            SiteData siteData = JsonConvert.DeserializeObject<SiteData>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(siteData.error))
            {
                Debug.Log("Site changed: " + siteData.name);
                onCompleted?.Invoke(siteData, true);
            }
            else
            {
                Debug.LogWarning("Error in server reply: " + siteData.error);
                onCompleted?.Invoke(siteData, false);
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
    
    public static IEnumerator GetCHElementListFromSite(UnityAction<CHElementResponse, bool> onCompleted,
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
        
        string query = ApiUrl + "/" + id + "/chelements" + QueryManagement.ToQueryString(queryParams);
        Debug.Log("API URL: " + query);
        UnityWebRequest request = new UnityWebRequest(query, "GET");
        request.SetRequestHeader("Authorization", "Bearer " + GlobalManagement.Instance.token);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("CHElements obtained successfully: " + request.downloadHandler.text);
            CHElementResponse chelementResponse = JsonConvert.DeserializeObject<CHElementResponse>(request.downloadHandler.text);
            
            if (string.IsNullOrEmpty(chelementResponse.error))
            {
                //Debug.Log("List of CHElement saved with the size of " + chelementResponse.items.Count);
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
    
    public static IEnumerator PostNewCHElementToSite(UnityAction<CHElementData, bool> onCompleted,
        int id, bool approximateDate, string description, int endYear, HeritageCategory heritageCategory,
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
        
        UnityWebRequest request = new UnityWebRequest(ApiUrl + "/" + id + "/chelements", "POST");
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
    
    public static IEnumerator GetAnnexDataListFromSite(UnityAction<AnnexDataResponse, bool> onCompleted,
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
}