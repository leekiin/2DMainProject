using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class GameBookSlotUI : MonoBehaviour
{
    [Header("슬롯 기본 정보")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private GameObject Gobj_Selected;
    [SerializeField] private DaniTechUIButton Btn_SlotClick;

    private event Action<string> _onClickSlot;

    private string _slotDataId;

    public string GetSlotDataId()
    {
        return _slotDataId;
    }

    private void OnEnable()
    {
        Btn_SlotClick.BindOnClickButtonEvent(OnClick_GameBookSlot);
    }

    private void OnDisable()
    {
        _onClickSlot = null;
    }

    public void OnClick_GameBookSlot()
    {
        _onClickSlot?.Invoke(_slotDataId);
    }

    public void InitSlot(string dataId, Action<string> onClickCallback)
    {
        var itemData = DaniTechGameDataManager.Instance.GetDNItemData(dataId);
        if(itemData == null) return;

        Text_MainName.text = itemData.Name;

        string iconPath = itemData.IconPath;
        if(string.IsNullOrEmpty(iconPath) == true) return;

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();

        _slotDataId = dataId;

        _onClickSlot += onClickCallback;
    }

    public void SetSeletedUI(bool isSelect)
    {
        Gobj_Selected.SetActive(isSelect);
    }
}
