using UnityEngine.UIElements;

[UxmlElement]
public partial class ToggleWithInternalValue : Toggle
{
    [UxmlAttribute("internal-value")]
    public string InternalValue { get; set; } = string.Empty;

    public ToggleWithInternalValue()
    {
    }
    
    public string GetInternalValue()
    {
        return InternalValue;
    }
}