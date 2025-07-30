using System;
using UnityEngine;

public class GlobalManagement : MonoBehaviour
{
    public static GlobalManagement Instance { get; private set; }

    public string token;
    public string username;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

[Serializable]
public class ErrorMessage
{
    public string error;

    public ErrorMessage(string error)
    {
        this.error = error;
    }
}