using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Events;
using Button = UnityEngine.UIElements.Button;
using Toggle = UnityEngine.UIElements.Toggle;

public enum FilterType
{
    Material,
    Alteration,
    AlterationForm,
}

public enum OrderType
{
    None,
    NameAsc,
    NameDesc,
    TimeAsc,
    TimeDesc
}

public class HomePageUI : BaseUI
{
    private readonly Dictionary<string, string> displayLabels = new()
    {
        { "None", OrderType.None.ToString() },
        { "A-Z", OrderType.NameAsc.ToString() },
        { "Z-A", OrderType.NameDesc.ToString() },
        { "Creation date asc.", OrderType.TimeAsc.ToString() },
        { "Creation date desc.", OrderType.TimeDesc.ToString() }
    };

    private VisualElement filtersList;
    private DropdownField orderDropdown;
    private DropdownField mimeTypeDropdown;

    private Dictionary<int, string> activeMaterialFilters = new Dictionary<int, string>();
    private Dictionary<int, string> activeAlterationFilters = new Dictionary<int, string>();
    
    private string searchButtonName = "search-button";
    private string searchFieldName = "search-bar";
    private string filterListName = "filters-list";
    private string orderingOptionsName = "ordering-options";
    private string mimeTypeOptionsName = "type-options";
    private string mapButtonName = "map-search-button";
    private string sitesListButtonName = "sites-button";

    private string searchValue = "";
    private string sortBy = "";
    private string orderDirection = "";
    private List<string> mimeType = new List<string>();

    public string nameOrderingValue;
    public string timeOrderingValue;
    
    public UnityEvent ListChange;
    
    private bool alterationFormChecked = false;
    
    protected override void InitializeUI()
    {
        // Input text field
        var searchField = rootElement.Q<TextField>(searchFieldName);
        if (searchField != null)
        {
            searchField.RegisterValueChangedCallback(evt =>
            {
                searchValue = evt.newValue;
            });
        }
        else
            Debug.LogWarning("ERROR: 'search-bar' not found in UI Document.");
        
        var searchButton = rootElement.Q<Button>(searchButtonName);
        if (searchButton != null)
            searchButton.clicked += OnSearchChangeEvent;
        else
            Debug.LogWarning("ERROR: 'search-button' not found in UI Document.");
        
        // Get the filter container
        filtersList = rootElement.Q<VisualElement>(filterListName);
        if (filtersList == null)
        {
            Debug.LogWarning("ERROR: 'filters-list' not found in UI Document.");
        }

        // Get the DropdownField by its name (from UXML)
        orderDropdown = rootElement.Q<DropdownField>(orderingOptionsName);

        if (orderDropdown != null)
        {
            // Set display values (user-friendly)
            orderDropdown.choices = new List<string>(displayLabels.Keys);

            // Register for change callback
            orderDropdown.RegisterValueChangedCallback(OnSortChanged);
        }
        
        // Get the DropdownField by its name (from UXML)
        mimeTypeDropdown = rootElement.Q<DropdownField>(mimeTypeOptionsName);
        
        if (mimeTypeDropdown != null)
        {
            // Register for change callback
            Debug.Log(mimeTypeDropdown.value);
            mimeTypeDropdown.RegisterValueChangedCallback(OnMimeTypeChanged);
        }
        
        InitializeButtons();
    }
    
    private void OnSortChanged(ChangeEvent<string> evt)
    {
        string selectedDisplay = evt.newValue;
        if (displayLabels.TryGetValue(selectedDisplay, out string internalValue))
        {
            Debug.Log("Selected internal value: " + internalValue);

            // Use internalValue (e.g., "nameAsc", "timeDesc", etc.) in your logic
            switch (internalValue)
            {
                case "None":
                    sortBy = null;
                    orderDirection = null;
                    break;
                case "NameAsc":
                    sortBy = nameOrderingValue;
                    orderDirection = "asc";
                    break;
                case "NameDesc":
                    sortBy = nameOrderingValue;
                    orderDirection = "desc";
                    break;
                case "TimeAsc":
                    sortBy = timeOrderingValue;
                    orderDirection = "asc";
                    break;
                case "TimeDesc":
                    sortBy = timeOrderingValue;
                    orderDirection = "desc";
                    break;
                default:
                    sortBy = null;
                    orderDirection = null;
                    break;
            }
            
            ListChange.Invoke();
        }
    }

    private void OnMimeTypeChanged(ChangeEvent<string> evt)
    {
        mimeType.Clear();
        
        switch (evt.newValue)
        {
            case "All":
                mimeType = null;
                break;
            case "PDF":
                mimeType.Add("application/pdf");
                break;
            case "Image":
            case "Audio":
            case "Video":
                mimeType = HelpFunctions.BuildMediaQuery(evt.newValue);
                break;
            default:
                mimeType = null;
                break;
        }
        
        ListChange.Invoke();
    }

    private void InitializeButtons()
    {
        var logoutButton = rootElement.Q<Button>("logout");
        if (logoutButton != null)
            logoutButton.clicked += OnLogoutClicked;

        var mapButton = rootElement.Q<Button>(mapButtonName);
        if (mapButton != null)
            mapButton.clicked += OnMapButtonClicked;
        
        var sitesListButton = rootElement.Q<Button>(sitesListButtonName);
        if (sitesListButton != null)
            sitesListButton.clicked += () => UIDocumentManager.Instance.SwitchContext("sitesList");
    }
    
    public void ConfigureToggleEvents(Toggle toggle, string text, int id, FilterType filterType,
        [CanBeNull] List<Toggle> childToggles = null)
    {
        if (filterType.Equals(FilterType.AlterationForm))
        {
            toggle.RegisterValueChangedCallback(evt =>
            {
                alterationFormChecked = true;
                SyncMasterToggle(childToggles, toggle);
            });
        }
        else
        {
            toggle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    AddFilter(toggle, text, id, filterType);
                }
                else
                {
                    RemoveFilter(id, filterType);
                }
                
                if (!alterationFormChecked)
                    ListChange.Invoke();
            });
        }
    }
    
    public void SyncMasterToggle(List<Toggle> toggles, Toggle master)
    {
        bool state = master.value;
        foreach (var toggle in toggles)
        {
            toggle.value = state;
        }
        
        master.schedule.Execute(() =>
        {
            ListChange.Invoke();
            alterationFormChecked = false;
        }).ExecuteLater(1);
    }

    public void OnSearchChangeEvent()
    {
        ListChange.Invoke();
    }
    
    /*[Button("Change to CHElementList")]
    public void ChangeToCHElementList()
    {
        Debug.Log("Changing to CHElementList");
        UIDocumentManager.Instance.SwitchContext("chElementList", scripts =>
        {
            foreach (var script in scripts)
                if(script is ChElementListUI chElementListUI)
                    chElementListUI.SetParentSite(-1);
        });
    }*/
    
    public void OnMapButtonClicked()
    {
        Debug.Log("Map button clicked.");
        UIDocumentManager.Instance.SwitchContext("sitesMap");
    }
    
    private void AddFilter(Toggle toggle, string toggleText, int id, FilterType filterType)
    {
        if (filterType == FilterType.Material)
        {
            if (activeMaterialFilters.ContainsKey(id))
                return;
            activeMaterialFilters[id] = toggleText;
        }
        
        else if (filterType == FilterType.Alteration)
        {
            if (activeAlterationFilters.ContainsKey(id))
                return;
            activeAlterationFilters[id] = toggleText;
        }
        
        else if (filterType == FilterType.AlterationForm)
        {
            if (filtersList.Q<VisualElement>(toggleText) != null)
                return;
        }
        
        var filterLabel = new Label(toggleText)
        {
            name = $"{toggleText}-label"
        };
        filterLabel.AddToClassList("filter-chip");

        var closeButton = new Button(() =>
        {
            RemoveFilter(id, filterType);
            toggle.value = false;
        })
        {
            text = "x"
        };
        closeButton.AddToClassList("filter-close");

        var filterContainer = new VisualElement
        {
            name = $"filter-{id}"
        };
        filterContainer.AddToClassList("filter-container");
        filterContainer.Add(closeButton);
        filterContainer.Add(filterLabel);

        filtersList.Add(filterContainer);
    }

    private void RemoveFilter(int id, FilterType filterType)
    {
        if (filterType == FilterType.Material)
        {
            if (activeMaterialFilters.ContainsKey(id))
                activeMaterialFilters.Remove(id);
        }
            
        else if (filterType == FilterType.Alteration)
        {
            if (activeAlterationFilters.ContainsKey(id))
                activeAlterationFilters.Remove(id);
        }

        var filterContainer = filtersList.Q<VisualElement>($"filter-{id}");
        if (filterContainer != null)
        {
            filtersList.Remove(filterContainer);
        }
    }

    public string GetSearchText()
    {
        return searchValue;
    }

    public string GetSortBy()
    {
        return sortBy;
    }

    public string GetOrderDirection()
    {
        return orderDirection;
    }

    public List<string> GetMimeType()
    {
        return mimeType;
    }

    public List<int> GetActiveMaterials()
    {
        List<int> activeMaterials = new List<int>();
        foreach (var kvp in activeMaterialFilters)
        {
            activeMaterials.Add(kvp.Key);
        }
        return activeMaterials;
    }
    
    public List<int> GetActiveAlterations()
    {
        List<int> activeAlterationForms = new List<int>();
        foreach (var kvp in activeAlterationFilters)
        {
            activeAlterationForms.Add(kvp.Key);
        }
        return activeAlterationForms;
    }

    void OnLogoutClicked()
    {
        Debug.Log("Logout pressed. Redirecting to the login page...");
        UIDocumentManager.Instance.SwitchContext("login");
    }
}