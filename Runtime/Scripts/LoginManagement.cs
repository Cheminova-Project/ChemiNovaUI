using UnityEngine;

public class LoginManagement : MonoBehaviour
{
    public bool autoLogin;
    [SerializeField] private string username;
    [SerializeField] private string password;

    public string GetUsername()
    {
        return username;
    }

    public string GetPassword()
    {
        return password;
    }
}
