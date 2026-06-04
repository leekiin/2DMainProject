using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemUsePopupUI : DaniTechUIBase
{

    [Header("UI 기본 구성요소")]
    [SerializeField] private Image Image_ItemIcon;
    [SerializeField] private Text Text_ItemName;
    [SerializeField] private Text Text_ItemDesc;
    [SerializeField] private DaniTechUIButton Btn_CloseSelf;
    [SerializeField] private DaniTechUIButton Btn_CloseAll;
    [SerializeField] private DaniTechUIButton Btn_UseItem;

    public long SlotItemUniqueId { get; private set; } // 인벤토리 슬롯의 UniqueId를 보관할 변수
    public bool IsUsableItem { get; private set; }

    private void OnEnable()
    {
        Btn_CloseSelf.BindOnClickButtonEvent(OnClick_CloseItemUsePopupUI);
        Btn_CloseAll.BindOnClickButtonEvent(OnClick_CloseItemUsePopupUI);
        Btn_UseItem.BindOnClickButtonEvent(OnClick_UseItem);
    }

    private void OnDisable()
    {
        Btn_CloseSelf.UnBindOnClickButtonEvent(OnClick_CloseItemUsePopupUI);
        Btn_CloseAll.UnBindOnClickButtonEvent(OnClick_CloseItemUsePopupUI);
        Btn_UseItem.UnBindOnClickButtonEvent(OnClick_UseItem);
    }

    public void OnClick_CloseItemUsePopupUI()
    {
        var inventoryUI = DaniTechUIManager.Instance.GetOpenedUI(DaniTechUIRootType.ContentUI, DaniTechUIType.DNInventory) as DaniTech_SampleInventoryUI;
        if (inventoryUI == null) return;

        var currentSlotUI = inventoryUI.GetCurrentSlotUI();
        
        DaniTechUIManager.Instance.CloseUI(DaniTechUIRootType.PopupUI, DaniTechUIType.ItemUsePopupUI);

        if (currentSlotUI != null) 
        {
            currentSlotUI.ChangeSelectedState(false);
        }
    }

    public void OnClick_UseItem()
    {
        Debug.Log($"팝업에서 아이템 사용 요청 : {SlotItemUniqueId}");

        bool isItemRemoved = DaniTechGameManager.Inst.RequestUseItem(SlotItemUniqueId);
        if (isItemRemoved == true)
        {
            // 인벤토리 화면에 있는 슬롯도 실시간으로 갱신되어야 하므로, 
            // 현재 열려있는 인벤토리 UI를 찾아서 슬롯을 지워주거나 인벤토리를 리프레시해줍니다.
            var inventoryUI = DaniTechUIManager.Instance.GetOpenedUI(DaniTechUIRootType.ContentUI, DaniTechUIType.DNInventory) as DaniTech_SampleInventoryUI;
            if (inventoryUI != null)
            {
                inventoryUI.HandleItemRemoved(SlotItemUniqueId);
            }

            OnClick_CloseItemUsePopupUI();
        }
    }

    public void InitItemUsePopupUI(long itemId, string itemName, string itemDesc)
    {
        SlotItemUniqueId = itemId; 
        Text_ItemName.text = itemName;
        Text_ItemDesc.text = itemDesc;
    }

    public void SetIcon(string itemDataId)
    {
        var itemData = DaniTechGameDataManager.Instance.GetDNItemData(itemDataId);
        if (itemData == null) return;

        string iconPath = itemData.IconPath;
        if (string.IsNullOrEmpty(iconPath) == true) return;


        IsUsableItem = (itemData.UseItemType != null && itemData.UseItemType.Count > 0);

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_ItemIcon, iconPath).Forget();
        Btn_UseItem.gameObject.SetActive(IsUsableItem);
    }
}
