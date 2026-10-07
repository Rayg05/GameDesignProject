using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [SerializeField] private WindowManager windowManager;

    private InputAction toggleInventory = new InputAction ("Inventory", binding: "<Keyboard>/i");

    private InputAction toggleEquipment = new InputAction ("Equipment", binding: "<Keyboard>/h");

    private InputAction closeAll = new InputAction ("CloseAll", binding: "<Keyboard>/escape");

    private void Start()
    {
        windowManager.CreateWindow("inventory", "INVENTORY", new Vector2(50, 50));
        windowManager.CreateWindow("equipment", "EQUIPMENT", new Vector2(200, 100));
    }

    private void OnEnable()
    {
        toggleInventory.Enable();
        toggleEquipment.Enable();
        closeAll.Enable();
    }

    private void OnDisable()
    {
        toggleInventory.Disable();
        toggleInventory.Disable();
        closeAll.Disable();
    }

    private void Update()
    {
        if(toggleInventory.WasPressedThisFrame())
        {
            windowManager.ToggleWindow("inventory");
        }
        if(toggleEquipment.WasPressedThisFrame())
        {
            windowManager.ToggleWindow("equipment");
        }
        if(closeAll.WasPressedThisFrame())
        {
            windowManager.CloseAllWindows();
        }
    }
}
