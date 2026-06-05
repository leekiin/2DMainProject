using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HudUI : DaniTechUIBase
{
    [SerializeField] private GameObject Prefab_HudSlot;
    //[SerializeField] private GameObject Prefab_InteractionSlot;

    [SerializeField] private Transform Transform_SlotRoot;

    //[SerializeField] private GameObject Prefab_HudSlot_Monster;

    private Dictionary<int, HudUISlot> _hudSlotList = new Dictionary<int, HudUISlot>();
    private Dictionary<int, InteractionSlotUI> _interactionSlotList = new Dictionary<int, InteractionSlotUI>();


    public void AddHudSlot(int instanceId, Transform targetTransform, string characterName, int maxHp)
    {
        CreateHudSlot(instanceId, targetTransform, characterName, maxHp);
    }

    private void CreateHudSlot(int instanceId, Transform targetTransform, string characterName, int maxHp)
    {
        var gObj = Instantiate(Prefab_HudSlot, Transform_SlotRoot);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<HudUISlot>();
        if (slotComponent == null) return;

        slotComponent.InitSlot(instanceId, targetTransform, characterName, maxHp);

        _hudSlotList.Add(instanceId, slotComponent);
    }


    //public void AddInteractiondSlot(int instanceId, Transform targetTransform, string characterName)
    //{
    //    CreateHudSlot(instanceId, targetTransform, characterName);
    //}

    //private void CreateInteractionSlot(int instanceId, string interactionTitle, string interactionkey
    //    , Transform targetTransform
    //    , Action<string> onClockCallback = null)
    //{
    //    var gObj = Instantiate(Prefab_HudSlot, Transform_SlotRoot);
    //    if (gObj == null) return;

    //    var slotComponent = gObj.GetComponent<InteractionSlotUI>();
    //    if (slotComponent == null) return;

    //    slotComponent.InitSlot(instanceId, interactionTitle, interactionkey
    //        , targetTransform
    //        , onClockCallback);

    //    _interactionSlotList.Add(instanceId, slotComponent);
    //}

    
    public void RemoveHudSlot(int instanceId)
    {
        if(_hudSlotList.ContainsKey(instanceId) == true)
        {
            var slot = _hudSlotList[instanceId];
            
            Destroy(slot.gameObject);

            _hudSlotList.Remove(instanceId);
        }
    }










}
