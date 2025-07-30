using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIDocumentManager : MonoBehaviourSingleton<UIDocumentManager>
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private List<UIDocumentContextSO> contexts;
    [SerializeField] private UIDocumentContextSO startContext;

    private UIDocumentContextSO _currentContext;
    private UIDocumentContextSO _secondContext;
    private GameObject currentContextScriptsContainer;
    private GameObject currentContextSecondScriptsContainer;
    protected override void SingletonStarted()
    {
        base.SingletonStarted();
        SwitchContext(startContext.contextName, null);
    }

    public void SwitchContext(string contextName, UnityAction<List<MonoBehaviour>>onComplete = null)
    {
        var newContext = contexts.Find(c => c.contextName == contextName);
        if (newContext == null)
        {
            Debug.LogWarning($"[UIDocumentManager] Context '{contextName}' not found.");
            return;
        }

        if(currentContextScriptsContainer != null)
            Destroy(currentContextScriptsContainer);

        _currentContext = newContext;
        uiDocument.visualTreeAsset = _currentContext.visualTreeAsset;

        // Wait a frame before initializing behaviours
        StartCoroutine(DeferredInitializeBehaviours(onComplete));
    }

    public void CloseSecondContext()
    {
        if(currentContextSecondScriptsContainer != null)
            Destroy(currentContextSecondScriptsContainer);

        _secondContext = null;
    }

    public void ClearAndInstantiateByElementID(string contextName, string elementID, bool instantiate, UnityAction<List<MonoBehaviour>> onComplete = null)
    {
        if(currentContextSecondScriptsContainer != null)
            Destroy(currentContextSecondScriptsContainer);
        
        var element = uiDocument.rootVisualElement.Q(elementID);
        
        if (element == null)
        {
            Debug.LogWarning($"[UIDocumentManager] Element '{elementID}' not found.");
            return;
        }
        
        element.Clear();
        
        var newContext = contexts.Find(c => c.contextName == contextName);
        if (newContext == null)
        {
            Debug.LogWarning($"[UIDocumentManager] Context '{contextName}' not found.");
            return;
        }
        
        _secondContext = newContext;
        
        if (_secondContext.visualTreeAsset != null && instantiate)
            element.Add(_secondContext.visualTreeAsset.Instantiate());
        
        // Wait a frame before initializing behaviours
        StartCoroutine(DeferredInitializeSecondBehaviours(onComplete));
    }

    private IEnumerator DeferredInitializeBehaviours(UnityAction<List<MonoBehaviour>> onComplete)
    {
        yield return null; // Wait one frame
        var scriptsPrefab = _currentContext.scriptsContainer;
        if (scriptsPrefab == null)
        {
            Debug.LogWarning($"[UIDocumentManager] No scripts container found for context '{_currentContext.contextName}'.");
            yield break;
        }
        
        // Load the scripts from the prefab
        currentContextScriptsContainer = Instantiate(scriptsPrefab, uiDocument.transform); // Instantiate the prefab to get the scripts

        Debug.Log($"[UIDocumentManager] Initialized context: {_currentContext.contextName}");
        var behaviours = currentContextScriptsContainer.GetComponentsInChildren<MonoBehaviour>(true);
        if(onComplete != null) onComplete?.Invoke(behaviours.ToList());
    }
    
    private IEnumerator DeferredInitializeSecondBehaviours(UnityAction<List<MonoBehaviour>> onComplete)
    {
        yield return null; // Wait one frame
        var scriptsPrefab = _secondContext.scriptsContainer;
        if (scriptsPrefab == null)
        {
            Debug.LogWarning($"[UIDocumentManager] No scripts container found for context '{_secondContext.contextName}'.");
            yield break;
        }
        
        // Load the scripts from the prefab
        currentContextSecondScriptsContainer = Instantiate(scriptsPrefab, uiDocument.transform); // Instantiate the prefab to get the scripts

        Debug.Log($"[UIDocumentManager] Initialized context: {_secondContext.contextName}");
        var behaviours = currentContextSecondScriptsContainer.GetComponentsInChildren<MonoBehaviour>(true);
        if(onComplete != null) onComplete?.Invoke(behaviours.ToList());
    }

    public string CurrentContextName => _currentContext != null ? _currentContext.contextName : "(None)";
}
