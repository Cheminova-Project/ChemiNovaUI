using UnityEngine;
using UnityEngine.UIElements;

public class MaterialUI : BaseUI
{
    [SerializeField] private VisualTreeAsset materialToggleTemplate;
    [SerializeField] private HomePageUI homePageUI;
    
    private string materialsTogglesContainerName = "material-container";
    private string materialToggleName = "material-toggle";
    
    private void OnMaterialListReceived(MaterialResponse materials, bool success)
    {
        if (success)
        {
            InsertToggles(materials);
        }
        else
        {
            Debug.LogWarning("Error when obtaining material list.");
        }
    }

    private void InsertToggles(MaterialResponse materials)
    {
        var materialsFoldoutRoot = rootElement.Q<VisualElement>(materialsTogglesContainerName);
        
        foreach (var material in materials.items)
        {
            var materialToggleInstance = materialToggleTemplate.CloneTree();
            var materialToggle = materialToggleInstance.Q<Toggle>(materialToggleName);
            materialToggle.text = material.name;
            materialToggle.value = false;
            materialsFoldoutRoot.Add(materialToggleInstance);
            homePageUI.ConfigureToggleEvents(materialToggle, material.name, material.id, FilterType.Material);
        }
    }

    protected override void InitializeUI()
    {
        StartCoroutine(MaterialDB.GetMaterialList(OnMaterialListReceived));
    }
}