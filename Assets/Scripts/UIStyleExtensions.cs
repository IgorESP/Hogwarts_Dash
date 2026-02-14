using UnityEngine;
using UnityEngine.UIElements;

public static class UIStyleExtensions
{
    /// <summary>
    /// Cambia el radio de todos los bordes de un VisualElement
    /// </summary>
    public static void SetBorderRadius(this IStyle style, float radius)
    {
        style.borderTopLeftRadius = radius;
        style.borderTopRightRadius = radius;
        style.borderBottomLeftRadius = radius;
        style.borderBottomRightRadius = radius;
    }
}
