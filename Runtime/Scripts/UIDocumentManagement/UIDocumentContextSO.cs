using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "UIDocumentContext", menuName = "UI/UIDocumentContext", order = 0)]
public class UIDocumentContextSO : ScriptableObject
{
    public string contextName;
    public VisualTreeAsset visualTreeAsset;
    public GameObject scriptsContainer;
}