using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static VisualElement ExtractRoot(this VisualElement container, string rootName)
    {
        var root = container.Q<VisualElement>(rootName);

        if(root == null)
        {
            Debug.LogError($"ExtractRoot: element '{rootName}' not found in container");
            return null;
        }

        for (int i = 0; i < container.styleSheets.count; i++)
        {
            root.styleSheets.Add(container.styleSheets[i]);
        }

        return root;
    }
}