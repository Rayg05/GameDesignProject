using UnityEngine;
using UnityEngine.UIElements;

public class WindowDragManipulator : PointerManipulator
{
    private Vector2 startPointerPosition;
    private Vector2 startWindowPosition;
    private bool isDrag;
    private VisualElement windowRoot;

    private Vector2 panelSize;
    private float windowwidth;

    private Vector2 currentPosition;

    public WindowDragManipulator(VisualElement dragHandle, VisualElement _windowRoot)
    {
        target = dragHandle;
        windowRoot = _windowRoot;
    }

    public void SetPosition(Vector2 position)
    {
        currentPosition = position;
        windowRoot.style.translate = new Translate(position.x, position.y);
    }

    public Vector2 GetPosition()
    {
        return currentPosition;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(onPointerDown);
        target.RegisterCallback<PointerMoveEvent>(onPointerMove);
        target.RegisterCallback<PointerUpEvent>(onPointerUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(onPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(onPointerMove);
        target.UnregisterCallback<PointerUpEvent>(onPointerUp);
    }
    private void onPointerDown(PointerDownEvent evt)
    {
        if(evt.button != 0) return;


        startPointerPosition = evt.position;
        startWindowPosition = currentPosition;

        panelSize = windowRoot.panel.visualTree.worldBound.size;
        windowwidth = windowRoot.resolvedStyle.width;

        isDrag = true;
        target.CapturePointer(evt.pointerId);
        windowRoot.BringToFront();
        evt.StopPropagation();
    }

    private void onPointerMove(PointerMoveEvent evt)
    {
        if(!isDrag) return;

        Vector2 delta = (Vector2) evt.position - startPointerPosition;
        Vector2 newPosition = startWindowPosition + delta;

        newPosition.x = Mathf.Clamp(newPosition.x, -(windowwidth - 40), panelSize.x - 40);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, panelSize.y - 40);

        currentPosition = newPosition;
        windowRoot.style.translate = new Translate(currentPosition.x, currentPosition.y);

    }

    private void onPointerUp(PointerUpEvent evt)
    {
        if(!isDrag) return;

        isDrag = false;
        target.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
    }
}
