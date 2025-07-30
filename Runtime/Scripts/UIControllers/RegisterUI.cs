using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class RegisterUI : MonoBehaviour
{
    [SerializeField] private List<string> roleOptions = new List<string>();
    
    private IntegerField activeField;
    private IntegerField activityStartField;
    private TextField affiliationField;
    private TextField emailField;
    private IntegerField idCountryField;
    private TextField nameField;
    private TextField passwordField;
    private TextField professionField;
    private DropdownField roleField;
    private TextField surnameField;
    private TextField usernameField;
    private Button registerButton;
    private Label passwordErrorLabel;
    
    private void OnRegister(UserData data, bool success)
    {
        if (success)
        {
            // User data
        }
        else
        {
            Debug.LogWarning("Error when registering.");
        }
    }
    
    void OnEnable()
    {
        var loginUIDocumentRoot = GetComponent<UIDocument>().rootVisualElement;

        activeField = loginUIDocumentRoot.Q<IntegerField>("activeField");
        activityStartField = loginUIDocumentRoot.Q<IntegerField>("activity-start-field");
        affiliationField = loginUIDocumentRoot.Q<TextField>("affiliation-field");
        nameField = loginUIDocumentRoot.Q<TextField>("name-field");
        emailField = loginUIDocumentRoot.Q<TextField>("email-field");
        idCountryField = loginUIDocumentRoot.Q<IntegerField>("id-country-field");
        passwordField = loginUIDocumentRoot.Q<TextField>("password-field");
        professionField = loginUIDocumentRoot.Q<TextField>("profession-field");
        roleField = loginUIDocumentRoot.Q<DropdownField>("role-field");
        surnameField = loginUIDocumentRoot.Q<TextField>("surname-field");
        usernameField = loginUIDocumentRoot.Q<TextField>("username-field");
        registerButton = loginUIDocumentRoot.Q<Button>("continue-button");
        passwordErrorLabel = loginUIDocumentRoot.Q<Label>("password-error");
        
        // Get custom labels from EnumMember attribute
        roleOptions = HelpFunctions.GetEnumMemberLabels<UserRole>();

        // Set the dropdown's choices to the custom labels
        roleField.choices = roleOptions;
        
        ShowCredentialsError(false);
        
        if (registerButton != null)
            registerButton.clicked += OnRegisterClicked;
        else
            Debug.LogWarning("Continue button not found.");
    }
    
    void OnRegisterClicked()
    {
        int active = activeField.value;
        int activityStart = activityStartField.value;
        string affiliation = affiliationField?.value;
        string name = nameField?.value;
        string email = emailField?.value;
        int idCountry = idCountryField.value;
        string password = passwordField?.value;
        string profession = professionField?.value;
        UserRole role = GetUserRoleFromLabel(roleField?.value);
        string surname = surnameField?.value;
        string username = usernameField?.value;

        RegisterWithCredentials(active, activityStart, affiliation, email, idCountry,
            name, password, profession, role, surname, username);
    }
    
    public void ShowCredentialsError(bool visible, string err = "Unrecognized error")
    {
        passwordErrorLabel.text = err;
        passwordErrorLabel.style.display =visible? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    public void RegisterWithCredentials(int active, int activityStart, string affiliation, string email,
        int idCountry, string name, string password, string profession, UserRole role, string surname, string username)
    {
        Debug.Log("Trying to register user");
        StartCoroutine(RegisterDB.Register(OnRegister, active, activityStart, affiliation, email,
            idCountry, name, password, profession, role, surname, username));
    }
    
    public void OnSuccessfulRegister()
    {
        Debug.Log("Successful register.");
        ShowCredentialsError(false);
    }
    
    public void OnFailedRegister(string err = "Unrecognized error")
    {
        Debug.Log("User register failed.");
        ShowCredentialsError(true, err);
    }
    
    // Convert the selected label back to UserRole enum
    private UserRole GetUserRoleFromLabel(string selectedLabel)
    {
        var customLabels = HelpFunctions.GetEnumMemberLabels<UserRole>();

        // Find the corresponding enum value
        for (int i = 0; i < customLabels.Count; i++)
        {
            if (customLabels[i] == selectedLabel)
            {
                return (UserRole)Enum.GetValues(typeof(UserRole)).GetValue(i);
            }
        }

        return UserRole.admin; // Default if no match is found
    }
}