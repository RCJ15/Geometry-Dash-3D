using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// Creates both the tab categories for the objects as well as the object buttons.
    /// </summary>
    public class LevelEditorObjectButtons : MonoBehaviour
    {
        public static LevelEditorObjectButtons Instance;

        private const int BUTTONS_PER_PAGE = 36;
        private static int _tabIndex;

        [Header("Templates")]
        [SerializeField] private GameObject tabButtonTemplate;
        [SerializeField] private GameObject categoryGroupTemplate;
        [SerializeField] private GameObject categoryPageTemplate;
        [SerializeField] private GameObject objectButtonTemplate;

        [Header("Buttons")]
        [SerializeField] private Button arrowLeftButton;
        [SerializeField] private Button arrowRightButton;

        [Header("Sprites")]
        [SerializeField] private Sprite enabledSprite;
        [SerializeField] private IconData[] iconData;
        private Dictionary<string, Sprite> _iconDataDictionary = new Dictionary<string, Sprite>();

        private GameObject _currentCategory;
        private CanvasGroup _currentSelectedTabCanvasGroup;
        private Image _currentSelectedTabImg;

        private Dictionary<int, int> _pageOpened = new Dictionary<int, int>();
        private Dictionary<int, int> _pageCount = new Dictionary<int, int>();
        private Dictionary<int, List<GameObject>> _categoryPages = new Dictionary<int, List<GameObject>>();

        private void Awake()
        {
            // Set singleton instance
            Instance = this;
        }

        private void Start()
        {
            LevelEditorPlacementFile.ReadFileIfNull();

            // Reset static variable
            _tabIndex = 0;

            // Populate the _iconDataDictionary
            foreach (IconData data in iconData)
            {
                _iconDataDictionary.Add(data.Name, data.Icon);
            }

            int index = 0;

            // Loop through the LevelEditorPlacementFile data
            foreach (var pair in LevelEditorPlacementFile.Data)
            {
                Sprite icon = _iconDataDictionary[pair.Key];

                // Create new category
                GameObject category = Instantiate(categoryGroupTemplate, categoryGroupTemplate.transform.parent);

                // Create new tab
                GameObject tabObj = Instantiate(tabButtonTemplate, tabButtonTemplate.transform.parent);

                CanvasGroup canvasGroup = tabObj.GetComponent<CanvasGroup>();
                Image tabImg = tabObj.transform.GetChild(0).GetComponent<Image>();
                Image tabIcon = tabImg.transform.GetChild(0).GetComponent<Image>();

                tabIcon.sprite = icon;
                tabIcon.SetNativeSize();

                // Subscribe to the click event on the tab
                int tabIndex = index;

                Button tabButton = tabObj.GetComponent<Button>();

                tabButton.onClick.AddListener(() =>
                {
                    _tabIndex = tabIndex;

                    // Disable previous category
                    if (_currentCategory != null)
                    {
                        _currentCategory.SetActive(false);
                    }

                    // Enable this category
                    category.SetActive(true);
                    _currentCategory = category;

                    // Disable previous tab
                    if (_currentSelectedTabCanvasGroup != null)
                    {
                        _currentSelectedTabCanvasGroup.alpha = 0.5f;
                    }
                    if (_currentSelectedTabImg != null)
                    {
                        _currentSelectedTabImg.overrideSprite = null;
                    }

                    // Set alpha to 1
                    canvasGroup.alpha = 1;
                    _currentSelectedTabCanvasGroup = canvasGroup;

                    // Also make sprite enabled
                    tabImg.overrideSprite = enabledSprite;
                    _currentSelectedTabImg = tabImg;

                    UpdateArrows();
                });

                // Loop through all items in the category
                int buttonIndex = 0;

                GameObject page = null;

                foreach (string item in pair.Value)
                {
                    // Create a new page if the current page is null, or the button index exceeds the amount of buttons per page
                    if (page == null || buttonIndex >= BUTTONS_PER_PAGE)
                    {
                        page = Instantiate(categoryPageTemplate, category.transform);

                        buttonIndex = 0;

                        // Create dictionary entries if they don't exist yet
                        if (!_pageOpened.ContainsKey(index) || !_categoryPages.ContainsKey(index) || !_pageCount.ContainsKey(index))
                        {
                            _pageOpened[index] = 0;
                            _pageCount[index] = 0;
                            _categoryPages[index] = new List<GameObject>();
                        }
                        else
                        {
                            page.SetActive(false);
                        }

                        // Add page to dictionary
                        _categoryPages[index].Add(page);
                        _pageCount[index]++;
                    }

                    // Create object button
                    GameObject obj = Instantiate(objectButtonTemplate, page.transform);
                    Image objImg = obj.GetComponent<Image>();
                    Image objIcon = obj.transform.GetChild(0).GetComponent<Image>();

                    Button objButton = obj.GetComponent<Button>();

                    objButton.onClick.AddListener(() =>
                    {

                    });

                    buttonIndex++;
                }

                if (index == 0)
                {
                    tabButton.onClick?.Invoke();
                }
                else
                {
                    category.SetActive(false);
                }

                index++;
            }

            // Delete all templates as they are no longer needed
            Destroy(tabButtonTemplate);
            Destroy(categoryGroupTemplate);
            Destroy(categoryPageTemplate);
            Destroy(objectButtonTemplate);

            arrowLeftButton.onClick.AddListener(() => ChangePage(-1));
            arrowRightButton.onClick.AddListener(() => ChangePage(1));
        }

        private void Update()
        {
            
        }

        public void UpdateArrows()
        {
            // Enable/disable the arrow buttons depending on if this tab has more than 1 pages in it
            bool active = _pageCount[_tabIndex] > 1;

            arrowLeftButton.gameObject.SetActive(active);
            arrowRightButton.gameObject.SetActive(active);
        }

        private void ChangePage(int amount)
        {
            // Only works in build mode :/
            if (LevelEditorBuildModes.Mode != BuildMode.build)
            {
                return;
            }

            // Return if pages are not a thing in this tab index
            if (!_pageOpened.ContainsKey(_tabIndex) || !_categoryPages.ContainsKey(_tabIndex))
            {
                return;
            }

            // Get all pages from this tab
            List<GameObject> pages = _categoryPages[_tabIndex];

            int count = _pageCount[_tabIndex];

            int newPage = _pageOpened[_tabIndex];

            // Disable previous page
            pages[newPage].SetActive(false);

            // Add by amount
            newPage += amount;

            // Loop the value
            newPage = Helpers.LoopValue(newPage, 0, count - 1);

            // Enable new page
            pages[newPage].SetActive(true);

            _pageOpened[_tabIndex] = newPage;
        }

        /// <summary>
        /// Contains the icon for a single category
        /// </summary>
        [System.Serializable]
        private class IconData
        {
            public string Name;
            public Sprite Icon;
        }
    }
}
