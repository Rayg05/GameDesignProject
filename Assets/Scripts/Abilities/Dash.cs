using System.Runtime.CompilerServices;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private EquipmentData ability;
    private float duration = .2f;
    private int speedMultiplier = 2;
    private Basic_Movement2 speed;
    private float cooldown = 4f;
    private bool dash_cooldown = false;
    //private float time = Time.deltaTime;
    private float dash_timer = 0;

    //private void HandleDash()
    //{

    //}

        void Awake()
    {
        //speed = GetComponent<Basic_Movement2>();
    }

    void OnEnable()
    {
        //Loadout.Changed += OnLoadoutChanged;
        //Apply();   // covers the case where it's already equipped on load
    }

    void OnDisable()
    {
        //Loadout.Changed -= OnLoadoutChanged;
        //if (speed != null) speed.SetSpeedMultiplier(1f);
    }

    private void OnLoadoutChanged(SlotType type, EquipmentData data)
    {
        //if (ability != null && type == ability.slotType) Apply();
    }

    private void Apply()
    {
        //Debug.Log("here0");
        //if (!Loadout.IsEquipped(ability))
        //{
        //    speed.SetSpeedMultiplier(1f);
        //    Debug.Log("here");
        //}

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    Debug.Log("here2");
        //    dash_timer += Time.time;
        //    if (dash_timer < duration && !dash_cooldown)
        //    {
        //        Debug.Log("here3");
        //        //Debug.Log("dash_timer val: " + dash_timer);
        //        speed.SetSpeedMultiplier(speedMultiplier);
        //    }
        //    else
        //    {
        //        Debug.Log("here4");
        //        speed.SetSpeedMultiplier(1f);
        //        dash_cooldown = true;
        //        dash_timer = 0;
        //    }
        //    if (dash_timer >= cooldown)
        //    {
        //        Debug.Log("here5");
        //        dash_cooldown = false;
        //        dash_timer = 0;
        //    }

        //}
    }
}