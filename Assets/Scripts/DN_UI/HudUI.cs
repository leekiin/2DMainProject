using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class HudUI : DaniTechUIBase
{
    [SerializeField] private GameObject Prefab_HudSlot;
    [SerializeField] private Transform Transform_SlotRoot;

    //[SerializeField] private GameObject Prefab_HudSlot_Monster;

    private Dictionary<int, HudUISlot> _hudSlotList = new Dictionary<int, HudUISlot>();


    public void AddHudSlot(int instanceId, Transform targetTransform, string characterName)
    {
        CreateHudSlot(instanceId, targetTransform, characterName);
    }
    
    private void CreateHudSlot(int instanceId, Transform targetTransform, string characterName)
    {
        var gObj = Instantiate(Prefab_HudSlot, Transform_SlotRoot);
        if(gObj == null) return;

        var slotComponent = gObj.GetComponent<HudUISlot>();
        if(slotComponent == null) return;

        slotComponent.InitSlot(instanceId, targetTransform, characterName);

        _hudSlotList.Add(instanceId, slotComponent);
    }

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
