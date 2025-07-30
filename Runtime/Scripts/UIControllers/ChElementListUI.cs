using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UIControllers
{
    public class ChElementListUI : BaseUI
    {
        // Map things
        [SerializeField] private VisualTreeAsset itemTemplate;
        [SerializeField] private HomePageUI homePageUI;
        [SerializeField] private Texture2D defaultIcon;
    
        // UI Elements
        private string itemsContainerName = "chelements-container";
        private string prevPageButtonName = "prev-page-button";
        private string nextPageButtonName = "next-page-button";
        private string currentPageLabelName = "current-page-label"; // Añadimos un label para mostrar la página actual
        private string siteNameName = "site-name";
        private string chElementNameName = "ch-element-name";
        private string chElementDescriptionName = "ch-element-description";
        private string filtersCountName = "filters-count";
        private string lastUpdateName = "last-update";
        private string authorName = "author";
        private string chElementImageName = "chelement-image";
        private string usernameName = "username-text";
    
        // Internal variables
        private int currentPage = 1;
        private int perPage = 10;
        private int? page;
        private int? nextPage;
        private int? prevPage;
        private int parentID = -1;

        private void OnChElementListReceived(CHElementResponse chElementResponse, bool success)
        {
            if (success)
            {
                ClearItems();
                page = chElementResponse.page;
                prevPage = chElementResponse.prev_page;
                nextPage = chElementResponse.next_page;
                InsertItems(chElementResponse.items);
            }
            else
            {
                Debug.LogWarning("Error when obtaining CH elements list.");
            }
        }

        private void InsertItems(List<CHElementData> chElementDatas)
        {
            var itemsContainer = rootElement.Q<ScrollView>(itemsContainerName);
            itemsContainer.mode = ScrollViewMode.Vertical;

            foreach (var chElement in chElementDatas)
            {
                var element = itemTemplate.CloneTree();
                itemsContainer.Add(element);
  
                var siteLabel = element.Q<Label>(siteNameName);
                if (siteLabel != null)
                {
                    if (chElement.parent != null)
                        siteLabel.text = chElement.parent.name;
                    else
                        siteLabel.text = "No collection associated";
                }

                var chElementNameLabel = element.Q<Label>(chElementNameName);
                if (chElementNameLabel != null)
                    chElementNameLabel.text = chElement.name;
        
                var chElementDescriptionLabel = element.Q<Label>(chElementDescriptionName);
                if (chElementDescriptionLabel != null)
                    chElementDescriptionLabel.text = chElement.description;
        
                var lastUpdateLabel = element.Q<Label>(lastUpdateName);
                if (lastUpdateLabel != null)
                    lastUpdateLabel.text = HelpFunctions.FormatDateTo_DD_MM_YYYY(chElement.created_on);
        
                var authorLabel = element.Q<Label>(authorName);
                if (authorLabel != null)
                    authorLabel.text = chElement.username;
        
                var chElementImage = element.Q<VisualElement>(chElementImageName);
        
                int chElementID = chElement.id;
                string chElementName = chElement.name;
                element.RegisterCallback<ClickEvent>(ev =>
                {
                    Debug.Log($"Element clicked: {chElement.name}");
                    if (ev.button == 0)
                    {
                        UIDocumentManager.Instance.SwitchContext("e3ds", scripts =>
                        {
                            foreach (var script in scripts)
                                if(script is E3DListUI e3DListUI)
                                    e3DListUI.SetParentCHElement(chElementID, chElementName);
                        });
                    }
                });
                DownloadAndFillCHelementImage(chElementImage, chElement.icon);
        
                //Debug.Log($"Element added: {chElement.name}");
            }
        
            var itemsCountLabel = rootElement.Q<Label>(filtersCountName);
            if (itemsCountLabel != null)
                itemsCountLabel.text = chElementDatas.Count + " results";
            
            UpdatePagination();

        }
    
        private void DownloadAndFillCHelementImage(VisualElement image, int? chElementIconID)
        {
            StartCoroutine(CHElementDB.GetCHElementIcon(chElementIconID, FillImage(image)));
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
                    Debug.LogWarning("Couldn't load CH element image.");
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
                // Actualizamos la página actual
                currentPageLabel.text = $"Page {page}";

                // Desactivamos o activamos la flecha izquierda según si hay una página previa
                prevPageButton.SetEnabled(prevPage != null);

                // Desactivamos o activamos la flecha derecha según si hay una página siguiente
                nextPageButton.SetEnabled(nextPage != null);
            }
            else
            {
                Debug.LogWarning("ERROR: Pagination buttons or current page label not found in UI Document.");
            }
        }

        private void ClearItems()
        {
            var itemsContainer = rootElement.Q<ScrollView>(itemsContainerName);
            itemsContainer.Clear();
        }

        private void InitializeButtons()
        {
            // Paginación: botones de "anterior" y "siguiente"
            var prevPageButton = rootElement.Q<Button>(prevPageButtonName);
            if (prevPageButton != null)
                prevPageButton.clicked += () => ChangePage(false);
            else
                Debug.LogWarning("ERROR: 'prev-page-button' not found in UI Document.");

            var nextPageButton = rootElement.Q<Button>(nextPageButtonName);
            if (nextPageButton != null)
                nextPageButton.clicked += () => ChangePage(true);
            else
                Debug.LogWarning("ERROR: 'next-page-button' not found in UI Document.");
        
            var usernameLabel = rootElement.Q<Label>(usernameName);
            if (usernameLabel != null)
                usernameLabel.text = GlobalManagement.Instance.username;
        }
    
        protected override void InitializeUI()
        {
            InitializeButtons();
        }
    
        public void SetParentSite(int id)
        {
            parentID = id;
            
            ListChElements();
        }

        public void ListChElements()
        {
            if (parentID == -1)
                ListAllChElements();
            else
                ListChElementsFromSite();
        }
    
        private void ListAllChElements()
        {
            ClearItems();
            List<int> materialFilters = homePageUI.GetActiveMaterials();
            StartCoroutine(CHElementDB.GetCHElementList(OnChElementListReceived, currentPage, perPage, homePageUI.GetSearchText(),
                homePageUI.GetSortBy(), homePageUI.GetOrderDirection(), null, materialFilters));
        }
        
        private void ListChElementsFromSite()
        {
            ClearItems();
            StartCoroutine(SiteDB.GetCHElementListFromSite(OnChElementListReceived, parentID, currentPage, perPage, homePageUI.GetSearchText(),
                homePageUI.GetSortBy(), homePageUI.GetOrderDirection()));
        }

        private void ChangePage(bool isNextPage)
        {
            if (isNextPage)
                currentPage++;
            else
                currentPage--;

            ListChElements();
        }
    }
}