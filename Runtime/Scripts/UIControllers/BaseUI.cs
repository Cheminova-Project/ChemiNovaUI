using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseUI : MonoBehaviour
{
    protected UIDocument uiDocument;
    protected VisualElement rootElement;
    private void OnEnable()
    {
        uiDocument = GetComponentInParent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogWarning("UIDocument component not found on this GameObject.");
            return;
        }

        rootElement = uiDocument.rootVisualElement;
        InitializeUI();
    }

    protected abstract void InitializeUI();
}
