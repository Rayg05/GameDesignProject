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
        windowRoot = template.Instantiate().ExtractRoot("gamewindow");

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
        dragManipulator.SetPosition(new Vector2(x,y));
    }

    public Vector2 GetPosition()
    {
        return dragManipulator.GetPosition();
    }
}
