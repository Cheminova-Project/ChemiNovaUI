using UnityEngine;

[CreateAssetMenu(menuName = "Config/API Base Config")]
public class APIBaseConfig : ScriptableObject
{
    [Header("API URL")]
    public string baseURL = "";
}