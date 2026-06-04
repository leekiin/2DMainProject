using System.Collections.Generic;
using UnityEngine;

// 관리주체 역할
public class DaniTech_SampleInventoryUI : DaniTechUIBase
{
    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Transform_UISlotRoot;
    [SerializeField] private DaniTechUIButton Button_UseSelectItem;
    [SerializeField] private DaniTechUIButton Button_CloseSelf;
    [SerializeField] private DaniTechUIButton Button_CloseSelfAllArea;

    private Dictionary<long, DaniTech_SampleInventorySlotUI> _itemSlotList = new Dictionary<long, DaniTech_SampleInventorySlotUI>();

    private long _currentSelectedItemUniqueId;

    private void OnEnable()
    {
        //Button_UseSelectItem.BindOnClickButtonEvent(OnClick_UseSelectItem, true);
        Button_CloseSelf.BindOnClickButtonEvent(OnClick_ClosePopup);
        Button_CloseSelfAllArea.BindOnClickButtonEvent(OnClick_ClosePopup);
        SetInventoryItemSlotOnEnable();

        ActiveUseSelectItemButton(false);
    }

    private void SetInventoryItemSlotOnEnable()
    {
        // 슬롯 정리 - 혹시 오픈 시점에 다른 슬롯들이 있다면 제거하자
        if(_itemSlotList.Count > 0)
        {
            foreach(var slot in _itemSlotList){
                DestroyImmediate(slot.Value.gameObject);
            }
            _itemSlotList.Clear();
        }

        //인벤오픈 1-1) 인벤토리가 열릴때 플레이어가 보유한 모든 아이템을 출력하는 로직을 넣어봅시다
        var itemList = DaniTechGameManager.Inst.GetPlayerItemList();
        if(itemList == null || itemList.Count == 0)
        {
            Debug.LogWarning("보유한 아이템이 없습니다!");
            return;
        }

        foreach (var itemModel in itemList) 
        {
            CreateSlot(itemModel.ItemUniqueId, itemModel.ItemDataId, itemModel.ItemStackCount);
        }
    }

    private void OnDisable()
    {
        //Button_UseSelectItem.UnBindOnClickButtonEvent(OnClick_UseSelectItem);
    }

    
    public void OnClick_ClosePopup()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNInventory);
    }

    public void OnClick_UseSelectItem()
    {
        RequestSelectedUseItem();
    }

    public DaniTech_SampleInventorySlotUI GetCurrentSlotUI()
    {
        if(_itemSlotList.ContainsKey(_currentSelectedItemUniqueId) == false)
        {
            return null;
        }
        return _itemSlotList[_currentSelectedItemUniqueId];
    }

    private void RequestSelectedUseItem()
    {
        bool isItemRemoved = DaniTechGameManager.Inst.RequestUseItem(_currentSelectedItemUniqueId);  
        if(isItemRemoved == true)
        {
            RemoveItemSlot(_currentSelectedItemUniqueId);
            _currentSelectedItemUniqueId = 0;
            ActiveUseSelectItemButton(false);
        }
    }

    private void ActiveUseSelectItemButton(bool isActive)
    {
        //Button_UseSelectItem.gameObject.SetActive(isActive);
    }

    private void RemoveItemSlot(long removedItemUniqueId)
    {
        if(_itemSlotList.ContainsKey(removedItemUniqueId) == false)
        {
            //Debug.LogError("제거하려는 아이템 슬롯을 찾을 수 없음!");
            return;
        }

        var slotComponent = _itemSlotList[removedItemUniqueId];
        slotComponent.ChangeSelectedState(false);
        _itemSlotList.Remove(removedItemUniqueId);
        Destroy(slotComponent.gameObject);
    }


    private void CreateSlot(long itemUniqueId, string itemDataId, int itemStackCount)
    {
        // 1-1 수동 SetParant가 뒤에 지금은 자동으로 해주고 있다
        var gObj = Instantiate(Prefab_Slot, Transform_UISlotRoot);
        if (gObj == null) return;

        // 1-2 자식 슬롯의 컴포넌트를 가져온다 -> 위에 게임오브젝트는 스크립트가 아직 아니므로
        var slotComponent = gObj.GetComponent<DaniTech_SampleInventorySlotUI>();
        if(slotComponent == null) return;

        // 1-3 여기서 slotComponent가지고 뭔가를 하는 겁니다!
        slotComponent.InitSlot(itemUniqueId, itemDataId, itemStackCount);
        slotComponent.gameObject.name = $"ItemSlot : {slotComponent.SlotItemUniqueId}";

        // 1-4 중복체크 해주면 좋긴 하지만, 일단 쉽게 컴포넌트(컴포넌트로 게임오브젝트는 받을 수 있으므로)를 보관해보자
        _itemSlotList.Add(slotComponent.SlotItemUniqueId, slotComponent);

        slotComponent.BindSlotSelectEvent(OnChildSlotSelected);
    }

    private void OnChildSlotSelected(long selectedItemUniqueId)
    {
        foreach(var slotKv in _itemSlotList)
        {
            var slot = slotKv.Value;
            bool isSlotSelected = (selectedItemUniqueId == slot.SlotItemUniqueId);
            slot.ChangeSelectedState(isSlotSelected);

            if (isSlotSelected == true)
            {
                _currentSelectedItemUniqueId = slot.SlotItemUniqueId;
                ActiveUseSelectItemButton(slot.IsUsableItem);
                OpenItemUsePopup(slot);
            }
        }
    }

    private void OpenItemUsePopup(DaniTech_SampleInventorySlotUI selectedSlot)
    {
        // 1. 데이터 매니저에서 상세 아이템 정보 가져오기
        var itemData = DaniTechGameDataManager.Instance.GetDNItemData(selectedSlot.ItemDataId);
        if (itemData == null) return;

        // 2. UI 매니저를 통해 팝업 컴포넌트 받아오기 
        // (DaniTechUIManager.Instance.OpenUI가 제네릭 형태 <T>를 지원한다고 가정하거나, GetComponent를 해야합니다)
        var popup = DaniTechUIManager.Instance.OpenUI(DaniTechUIRootType.PopupUI, DaniTechUIType.ItemUsePopupUI) as ItemUsePopupUI;

        if (popup != null)
        {
            // 3. 팝업 컴포넌트의 초기화 함수들을 호출하며 정보 전달!
            // (itemData 내부의 변수명은 기획 데이터 구조인 Name, Desc 등에 맞게 수정해 주세요)
            popup.InitItemUsePopupUI(selectedSlot.SlotItemUniqueId, itemData.Name, itemData.Description);
            popup.SetIcon(selectedSlot.ItemDataId);
            Debug.Log($"아이템 사용 팝업 열기! 아이템 이름 : {itemData.Name}, 설명 : {itemData.Description}");
        }
    }

    public void HandleItemRemoved(long removedItemUniqueId)
    {
        // 1. 선택되어 있던 ID와 제거된 ID가 같다면 선택 상태 초기화
        if (_currentSelectedItemUniqueId == removedItemUniqueId)
        {
            _currentSelectedItemUniqueId = 0;
            ActiveUseSelectItemButton(false);
        }

        // 2. 이미 가지고 계신 슬롯 삭제 함수 호출 (화면에서 제거)
        RemoveItemSlot(removedItemUniqueId);
    }
}
