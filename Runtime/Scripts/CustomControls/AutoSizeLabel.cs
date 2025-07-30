using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[UxmlElement("AutoSizeLabel")]
public partial class AutoSizeLabel : Label
{
    [UxmlAttribute]
    public string group { get; set; }

    public AutoSizeLabel()
    {
        RegisterCallback<GeometryChangedEvent>(_ => OnLayoutReady());
        OnLayoutReady();
    }
    
    private void OnLayoutReady()
    {
        if (!string.IsNullOrEmpty(group))
        {
            AutoSizeLabelGroups.Register(this, group);
            AutoSizeLabelGroups.ScheduleRecalculate(group); // ← ahora usa planificación
        }
    }

    public float ComputeBestFitFontSize()
    {
        if (string.IsNullOrEmpty(text) || contentRect.width <= 0 || contentRect.height <= 0)
        {
            Debug.LogWarning($"[AutoSizeLabel] Group: {group} → No text or invalid dimensions for label: {text}");
            return resolvedStyle.fontSize;
        }

        float fontSize = resolvedStyle.fontSize;
        float minFontSize = 4f;
        float maxFontSize = 100f;

        // Reducir si no cabe
        while (fontSize > minFontSize && MeasureTextWidth(fontSize) > contentRect.width)
            fontSize--;
        
        // Aumentar si cabe
        while (fontSize < maxFontSize && 
               MeasureTextWidth(fontSize + 1) < contentRect.width &&
               MeasureTextHeight(fontSize + 1) < contentRect.height)
        {
            fontSize++;
        }
       
        return fontSize;
    }

    public void ApplyFontSize(float size)
    {
        style.fontSize = size;
    }

    private float MeasureTextWidth(float fontSize)
    {
        return MeasureTextSize(text, fontSize).x;
    }

    private float MeasureTextHeight(float fontSize)
    {
        return MeasureTextSize(text, fontSize).y;
    }

    private Vector2 MeasureTextSize(string textstring, float fontSize)
    {
        style.fontSize = fontSize;
        return MeasureTextSize(textstring, contentRect.width, MeasureMode.AtMost, contentRect.height, MeasureMode.AtMost);
    }
}

public static class AutoSizeLabelGroups
{
    private static Dictionary<string, List<AutoSizeLabel>> _groups;
    private static readonly HashSet<string> _pendingGroups = new();
    public static void Register(AutoSizeLabel label, string group)
    {
        if(_groups == null)
            _groups = new Dictionary<string, List<AutoSizeLabel>>();
        if (!_groups.ContainsKey(group))
            _groups[group] = new List<AutoSizeLabel>();

        if (!_groups[group].Contains(label))
            _groups[group].Add(label);
    }
    public static void ScheduleRecalculate(string group)
    {
        if (_pendingGroups.Contains(group)) return;

        _pendingGroups.Add(group);

        // Usa el UIDocument activo como contexto para ejecutar en el próximo frame
        // Si estás usando varios UIDocuments, adapta esto según tu arquitectura
        var root = UnityEngine.Object.FindObjectOfType<UIDocument>()?.rootVisualElement;
        if (root != null)
        {
            root.schedule.Execute(() =>
            {
                Recalculate(group);
                _pendingGroups.Remove(group);
            }).ExecuteLater(1); // Espera un frame
        }
        else
        {
            Debug.LogWarning("[AutoSizeLabelGroups] No UIDocument encontrado para programar recálculo.");
        }
    }
    
    public static void Recalculate(string group)
    {
        if (!_groups.ContainsKey(group)) return;

        var labels = _groups[group];
        float minSize = float.MaxValue;

        foreach (var label in labels)
        {
            float size = label.ComputeBestFitFontSize();
            if (size < minSize)
                minSize = size;
        }

        foreach (var label in labels)
            label.ApplyFontSize(minSize);
    }
    
}