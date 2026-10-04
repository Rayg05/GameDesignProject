using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class GameWindow //represents single window instance
{
    private VisualElement windowRoot;
    private Label titleLabel;
    private VisualElement contentArea;
    private WindowDragManipulator dragManipulator;

    public bool isVisible => windowRoot.resolvedStyle.display == DisplayStyle.Flex;

    public GameWindow(VisualTreeAsset template, VisualElement parent, string title)
    {
        var templateContainer = template.Instantiate();
        windowRoot = templateContainer.Q<VisualElement>("gamewindow");

        for (int i = 0; i<templateContainer.styleSheets.count; i++)
        {
            windowRoot.styleSheets.Add(templateContainer.styleSheets[i]);
        }

        titleLabel = windowRoot.Q<Label>("title-label");
        contentArea = windowRoot.Q<VisualElement>("content");

        titleLabel.text = title;

        var titleBar = windowRoot.Q<VisualElement>("title-bar");

        dragManipulator = new WindowDragManipulator(titleBar, windowRoot);
        titleBar.AddManipulator(dragManipulator);

        windowRoot.RegisterCallback<PointerDownEvent>(evt => windowRoot.BringToFront());

        parent.Add(windowRoot);

    }

    public void Show()
    {
        windowRoot.style.display = DisplayStyle.Flex;
        windowRoot.BringToFront();
    }

    public void Hide()
    {
        windowRoot.style.display = DisplayStyle.None;
    }

    public void SetPosition(float x, float y)
    {
        windowRoot.style.left = x;
        windowRoot.style.top = y;
    }

    public Vector2 GetPostion()
    {
        return new Vector2(windowRoot.resolvedStyle.left, windowRoot.resolvedStyle.top);
    }
}
