using UIControllers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SiteListUI : BaseUI
{
    // Map things
    public VisualTreeAsset siteTemplate;
    public HomePageUI homePageUI;
    public Texture2D defaultIcon;
    
    // UI Elements
    private string returnButtonName = "back-button";
    private string prevPageButtonName = "prev-page-button";
    private string nextPageButtonName = "next-page-button";
    private string currentPageLabelName = "current-page-label";
    private string usernameName = "username-text";
    private string filtersCountName = "filters-count";
    private string itemsContainerName = "sites-container";
    private string itemLabelName = "site-name";
    private string itemLabelDescriptionName = "site-description";
    private string itemImageName = "site-image";
    
    // Internal variables
    private int currentPage = 1;
    private int perPage = 10;
    private int? page;
    private int? nextPage;
    private int? prevPage;
    
    private void OnSiteListReceived(SiteResponse sites, bool success)
    {
        if (success)
        {
            InsertItems(sites);
        }
        else
        {
            Debug.LogWarning("Error when obtaining sites list.");
        }
    }

    private void InsertItems(SiteResponse siteResponse)
    { 
        var itemsContainer = rootElement.Q<ScrollView>(itemsContainerName);
        var siteItems = siteResponse.items;

        foreach (var site in siteItems)
        {
            var element = siteTemplate.CloneTree();
            itemsContainer.Add(element);
            Debug.Log("Site added");
            // Asignar el nombre y la descripción al elemento
            var nameLabel = element.Q<Label>(itemLabelName);
            if (nameLabel != null)
                nameLabel.text = site.name;
            var descriptionLabel = element.Q<Label>(itemLabelDescriptionName);
            if (descriptionLabel != null)
                descriptionLabel.text = site.description;
            element.RegisterCallback<ClickEvent>(ev =>
            {
                Debug.Log($"Element clicked: {site.name}");
                if (ev.button == 0)
                {
                    UIDocumentManager.Instance.SwitchContext("chElementList", scripts =>
                    {
                        foreach (var script in scripts)
                            if(script is ChElementListUI chElementListUI)
                                chElementListUI.SetParentSite(site.id);
                    });
                }
            });
            var image = element.Q<VisualElement>(itemImageName);
            if (image != null)
                DownloadAndFillSiteIcon(image, site.icon);
            else
            {
                Debug.LogWarning("ERROR: "+ itemImageName +" not found in UI Document.");
            }
        }
        
        var itemsCountLabel = rootElement.Q<Label>(filtersCountName);
        if (itemsCountLabel != null)
            itemsCountLabel.text = siteItems.Count + " results";
        
        UpdatePagination(siteResponse);
    }

    private void DownloadAndFillSiteIcon(VisualElement image, int? siteIcon)
    {
        StartCoroutine(SiteDB.GetSiteIcon(siteIcon, FillImage(image)));
    }

    private UnityAction<Texture2D, bool> FillImage(VisualElement imageToFill)
    {
        return (texture, success) =>
        {
            if (success && texture != null)
            {
                imageToFill.style.backgroundImage = texture;
            }
            else
            {
                imageToFill.style.backgroundImage = defaultIcon;
                Debug.LogWarning("Couldn't load site image.");
            }
        };
    }
    
    private void UpdatePagination(SiteResponse siteResponse)
    {
        var prevPageButton = rootElement.Q<Button>(prevPageButtonName);
        var nextPageButton = rootElement.Q<Button>(nextPageButtonName);
        var currentPageLabel = rootElement.Q<Label>(currentPageLabelName);

        if (prevPageButton != null && nextPageButton != null && currentPageLabel != null)
        {
            currentPageLabel.text = $"Page {siteResponse.page ?? 0}";
            prevPageButton.SetEnabled(siteResponse.prev_page != null);
            nextPageButton.SetEnabled(siteResponse.next_page != null);
        }
        else
        {
            Debug.LogWarning("ERROR: Pagination buttons or current page label not found in UI Document.");
        }
    }
    
    protected override void InitializeUI()
    {
        var username = rootElement.Q<Label>(usernameName);
        if (username != null)
            username.text = GlobalManagement.Instance.username;
        
        InitializeButtons();
        
        ExecuteSearch();
    }

    private void InitializeButtons()
    {
        var returnButton = rootElement.Q<Button>(returnButtonName);
        if (returnButton != null)
        {
            returnButton.clicked += ReturnHome;
        }
    }
    
    public void ExecuteSearch()
    {
        ClearItems();

        StartCoroutine(SiteDB.GetSiteList(OnSiteListReceived, currentPage, perPage, homePageUI.GetSearchText(),
            homePageUI.GetSortBy(), homePageUI.GetOrderDirection()));
    }

    private void ClearItems()
    {
        var itemsContainer = rootElement.Q<ScrollView>(itemsContainerName);
        itemsContainer.Clear();
    }

    private void ReturnHome()
    {
        UIDocumentManager.Instance.SwitchContext("chElementList", scripts =>
        {
            foreach (var script in scripts)
                if(script is ChElementListUI chElementListUI)
                    chElementListUI.SetParentSite(-1);
        });
    }
}