using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class AlterationUI : BaseUI
{
    [SerializeField] private VisualTreeAsset alterationFormTemplate;
    [SerializeField] private VisualTreeAsset alterationTemplate;
    [SerializeField] private HomePageUI homePageUI;
    
    private string alterationFormFoldoutName = "alteration-form-foldout";
    private string alterationFormScrollViewName = "alteration-scrollview";
    private string alterationToggleName = "alteration-toggle";
    private string alterationFormToggleName = "alterationform-foldout-toggle";
    private string alterationFormsContainerName = "alteration-container";
    
    private void OnAlterationFormListReceived(AlterationFormResponse data, bool success)
    {
        if (success)
        {
            CreateFoldouts(data);
        }
        else
        {
            Debug.LogWarning("Error when obtaining alteration forms list.");
        }
    }

    public void CreateFoldouts(AlterationFormResponse alterationForms)
    {
        var alterationFormsFoldoutRoot = rootElement.Q<VisualElement>(alterationFormsContainerName);
        
        foreach (var alterationForm in alterationForms.items)
        {
            var alterationFormInstance = alterationFormTemplate.CloneTree();
            var alterationFormFoldout = alterationFormInstance.Q<Foldout>(alterationFormFoldoutName);
            alterationFormFoldout.text = alterationForm.name;
            alterationFormFoldout.value = false;
            
            var alterationFormScrollView = alterationFormInstance.Q<ScrollView>(alterationFormScrollViewName);
            var alterationFormFoldoutToggle = alterationFormInstance.Q<Toggle>(alterationFormToggleName);

            foreach (var alteration in alterationForm.alterations)
            {
                var alterationInstance = alterationTemplate.CloneTree();
                var alterationToggle = alterationInstance.Q<Toggle>(alterationToggleName);
                alterationToggle.text = alteration.name;
                alterationToggle.value = false;
                
                alterationFormScrollView.Add(alterationToggle);
                homePageUI.ConfigureToggleEvents(alterationToggle, alteration.name, alteration.id, FilterType.Alteration);
            }
            
            homePageUI.ConfigureToggleEvents(alterationFormFoldoutToggle, alterationForm.name, alterationForm.id, FilterType.AlterationForm, 
                alterationFormScrollView.contentContainer.Children().OfType<Toggle>().ToList());
            
            alterationFormsFoldoutRoot.Add(alterationFormInstance);
        }
    }

    protected override void InitializeUI()
    {
        StartCoroutine(AlterationFormDB.GetAlterationFormList(OnAlterationFormListReceived));
    }
}