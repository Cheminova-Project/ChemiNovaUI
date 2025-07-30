using System.Collections.Generic;
using UIControllers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class E3DListUI : BaseUI
{
    // Map things
    public VisualTreeAsset e3DTemplate;
    public HomePageUI homePageUI;
    public Texture2D defaultIcon;
    
    // UI Elements
    private string returnButtonName = "back-button";
    private string prevPageButtonName = "prev-page-button";
    private string nextPageButtonName = "next-page-button";
    private string currentPageLabelName = "current-page-label"; // Añadimos un label para mostrar la página actual
    private string chElementName = "chelement-name";
    private string filtersCountName = "filters-count";
    private string itemsContainerName = "e3d-container";
    private string itemLabelName = "e3d-name";
    private string itemLabelDescriptionName = "e3d-description";
    private string itemImageName = "e3d-image";
    private string itemAuthorName = "e3d-author";
    private string usernameName = "username-text";
    
    // Internal variables
    private int currentPage = 1;
    private int perPage = 10;
    private int? page;
    private int? nextPage;
    private int? prevPage;
    private int parentID;
    private string parentName;
    
    private void OnE3DModelListReceived(E3DModelResponse e3DModelResponse, bool success)
    {
        if (success)
        {
            ClearItems();
            page = e3DModelResponse.page;
            prevPage = e3DModelResponse.prev_page;
            nextPage = e3DModelResponse.next_page;
            InsertItems(e3DModelResponse.items);
        }
        else
        {
            Debug.LogWarning("Error when obtaining E3D elements list.");
        }
    }
    
    private void InsertItems(List<E3DModelData> e3dItems)
    {
        var itemsContainer = rootElement.Q<ScrollView>(itemsContainerName);
        itemsContainer.mode = ScrollViewMode.Vertical;
        
        var itemsCountLabel = rootElement.Q<Label>(filtersCountName);
        if (itemsCountLabel != null)
            itemsCountLabel.text = e3dItems.Count + " results";

        foreach (var e3D in e3dItems)
        {
            var element = e3DTemplate.CloneTree();
            itemsContainer.Add(element);
            var nameLabel = element.Q<Label>(itemLabelName);
            if (nameLabel != null)
                nameLabel.text = e3D.name;
            var descriptionLabel = element.Q<Label>(itemLabelDescriptionName);
            if (descriptionLabel != null)
                descriptionLabel.text = e3D.description;
            int e3DModelID = e3D.id;
            element.RegisterCallback<ClickEvent>(ev =>
            {
                //Debug.Log($"Element clicked: {e3D.name}");
                if (ev.button == 0)
                {
                    Debug.Log("ED3Model selected name: " + e3D.name);
                }
            });
            var image = element.Q<VisualElement>(itemImageName);
            if (image != null)
                DownloadAndFillE3DIcon(image, e3D.icon);
            else
                Debug.LogWarning("ERROR: "+ itemImageName +" not found in UI Document.");
            
            var authorLabel = element.Q<Label>(itemAuthorName);
            if (authorLabel != null)
                authorLabel.text = e3D.username;
        }
        
        UpdatePagination();
    }

    private void DownloadAndFillE3DIcon(VisualElement image, int? e3dIcon)
    {
        StartCoroutine(E3DModelDB.GetE3DIcon(e3dIcon, FillImage(image)));
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
                Debug.LogWarning("Couldn't load E3D image.");
            }
        };
    }

    private void UpdatePagination()
    {
        var prevPageButton = rootElement.Q<Button>(prevPageButtonName);
        var nextPageButton = rootElement.Q<Button>(nextPageButtonName);
        var currentPageLabel = rootElement.Q<Label>(currentPageLabelName);

        if (prevPageButton != null && nextPageButton != null && currentPageLabel != null)
        {
            currentPageLabel.text = $"Page {page}";
            prevPageButton.SetEnabled(prevPage != null);
            nextPageButton.SetEnabled(nextPage != null);
        }
        else
        {
            Debug.LogWarning("ERROR: Pagination buttons or current page label not found in UI Document.");
        }
    }
    
    protected override void InitializeUI()
    {
        InitializeButtons();
    }
    
    private void ClearItems()
    {
        var itemsContainer = rootElement.Q<ScrollView>(itemsContainerName);
        itemsContainer.Clear();
    }

    private void InitializeButtons()
    {
        var returnButton = rootElement.Q<Button>(returnButtonName);
        if (returnButton != null)
            returnButton.clicked += ReturnChElementList;
    }
    
    public void ListE3DModels()
    {
        ClearItems();
        List<int> alterationFilters = homePageUI.GetActiveAlterations();
        StartCoroutine(CHElementDB.GetE3DModelListFromCHElement(OnE3DModelListReceived, parentID, currentPage, perPage, homePageUI.GetSearchText(),
            homePageUI.GetSortBy(), homePageUI.GetOrderDirection(), alterationFilters, null));
    }

    public void SetParentCHElement(int id, string name)
    {
        parentID = id;
        parentName = name;
        
        var usernameLabel = rootElement.Q<Label>(usernameName);
        if (usernameLabel != null)
            usernameLabel.text = GlobalManagement.Instance.username;
        
        var chElementNameElement = rootElement.Q<Label>(chElementName);
        if (chElementNameElement != null)
            chElementNameElement.text = parentName;
        
        ListE3DModels();
    }
    
    private void ChangePage(bool isNextPage)
    {
        if (isNextPage)
            currentPage++;
        else
            currentPage--;

        ListE3DModels();
    }
    
    private void ReturnChElementList()
    {
        Debug.Log("Return ch element list");
        UIDocumentManager.Instance.SwitchContext("chElementList", scripts =>
        {
            foreach (var script in scripts)
                if(script is ChElementListUI chElementListUI)
                    chElementListUI.SetParentSite(-1);
        });
    }
}