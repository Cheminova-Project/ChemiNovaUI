using UIControllers;
using UnityEngine;
using UnityEngine.UIElements;

public class SigninUI : BaseUI
{
    private LoginManagement loginManagement;
    private TextField usernameField;
    private TextField passwordField;
    private Button loginButton;
    private Label passwordErrorLabel;
    
    private void OnSignin(SigninResponse data, bool success)
    {
        if (success)
        {
            GlobalManagement.Instance.token = data.access_token;
            OnSuccessfulLogin();
        }
        else
        {
            Debug.LogWarning("Error when signin.");
            string error;
            if (data == null)
                error = "Unrecognized data";
            else
                error = data.error;
            
            OnFailedLogin(error);  
        }
    }

    protected override void InitializeUI()
    {
        if (loginManagement == null)
            loginManagement = FindAnyObjectByType<LoginManagement>();
        
        usernameField = rootElement.Q<TextField>("username-field");
        passwordField = rootElement.Q<TextField>("password-field");
        loginButton = rootElement.Q<Button>("continue-button");
        passwordErrorLabel = rootElement.Q<Label>("password-error");
        
        ShowCredentialsError(false);
        
        if (loginButton != null)
            loginButton.clicked += OnLoginClicked;
        else
            Debug.LogWarning("The continue button was not found.");
        
        if (loginManagement.autoLogin)
            LoginWithCredentials(loginManagement.GetPassword(),loginManagement.GetUsername());
    }

    void OnLoginClicked()
    {
        string password = passwordField?.value;
        string username = usernameField?.value;

        LoginWithCredentials(password, username);
    }

    public void ShowCredentialsError(bool visible, string err = "Unrecognized error")
    {
        passwordErrorLabel.text = err;
        passwordErrorLabel.style.display =visible? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    public void LoginWithCredentials(string password, string username)
    {
        Debug.Log("Trying to log in with credentials: " + password + ", " + username);
        GlobalManagement.Instance.username = username;
        StartCoroutine(SigninDB.Signin(OnSignin, password, username));
    }
    
    public void OnSuccessfulLogin()
    {
        // Here you can manage the logic after a successful login
        Debug.Log("Login successful.");
        ShowCredentialsError(false);
        //LoadHomePage();
        LoadCHElementsList();
    }
    
    public void OnFailedLogin(string err = "Unrecognized error")
    {
        // Here you can handle the logic after a failed login.
        ShowCredentialsError(true, err);
        Debug.Log("Login failed.");
    }
    
    void LoadHomePage()
    {
        UIDocumentManager.Instance.SwitchContext("search_page");
    }

    void LoadCHElementsList()
    {
        UIDocumentManager.Instance.SwitchContext("chElementList", scripts =>
        {
            foreach (var script in scripts)
                if(script is ChElementListUI chElementListUI)
                    chElementListUI.SetParentSite(-1);
        });
    }
}