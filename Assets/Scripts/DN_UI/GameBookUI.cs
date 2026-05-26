using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public enum EGameBookCategory
{
    None= 0,
    ItemCategory,
    MonsterCategory,
    HarvestCategory
}

public class GameBookUI : DaniTechUIBase
{
    [Header("프리팹")]
    [SerializeField] private GameObject Prefab_Slot;

    [Header("디테일 정보 영역")]
    [SerializeField] private Image Image_MainIcoin;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Desc;

    //[Header("부가 정보")]
    //[SerializeField] private GameObject Layout_SubInfoSkill;

    [Header("상단 카테고리")]
    [SerializeField] private DaniTechUIButton Btn_ItemCategory;
    [SerializeField] private DaniTechUIButton Btn_MonsterCategory;
    [SerializeField] private DaniTechUIButton Btn_HarvestCategory;

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_SlotRoot;

    [Header("버튼")]
    [SerializeField] private DaniTechUIButton Btn_CloseSelf;
    [SerializeField] private DaniTechUIButton Btn_CloseAll;

    private Dictionary<string, GameBookSlotUI> _slotList = new Dictionary<string, GameBookSlotUI>();

    private void OnEnable()
    {
        Btn_CloseSelf.BindOnClickButtonEvent(OnClick_CloseGameBookUI);
        Btn_CloseAll.BindOnClickButtonEvent(OnClick_CloseGameBookUI);
        Btn_ItemCategory.BindOnClickButtonEvent(OnClick_ItemCategory);
        Btn_MonsterCategory.BindOnClickButtonEvent(OnClick_MonsterCategory);
        Btn_HarvestCategory.BindOnClickButtonEvent(OnClick_HarvestCategory);

        ReadItemListAndCreateSlot();
    }

    private void OnDisable()
    {
        if(_slotList.Count > 0)
        {
            foreach(var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }
            _slotList.Clear();
        }
    }

    public void OnClick_CloseGameBookUI()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.GameBookUI);
    }

    public void OnClick_ItemCategory()
    {
        SetGameBookLayoutByCatrgory(EGameBookCategory.ItemCategory);
    }

    public void OnClick_MonsterCategory()
    {
        SetGameBookLayoutByCatrgory(EGameBookCategory.MonsterCategory);
    }

    public void OnClick_HarvestCategory()
    {
        SetGameBookLayoutByCatrgory(EGameBookCategory.HarvestCategory);
    }

    private void SetGameBookLayoutByCatrgory(EGameBookCategory category)
    {
        switch (category)
        {
            case EGameBookCategory.ItemCategory:
                //아이템 카테고리 레이아웃 설정 로직 작성
                break;
            case EGameBookCategory.MonsterCategory:
                //몬스터 카테고리 레이아웃 설정 로직 작성
                break;
            case EGameBookCategory.HarvestCategory:
                //채집 카테고리 레이아웃 설정 로직 작성
                break;
            default:
                break;
        }
    }

    private void ReadItemListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.ItemDataList;
        foreach(var dataKv in dataList)
        {
            var data = dataKv.Value;
            if(data == null) continue;

            CreateGameBookSlot(data.Id);
        }

        //if (_slotList.Count > 0)
        //{
        //    foreach (var slotKv in _slotList)
        //    {
        //        var slot = slotKv.Value;
        //        slot.OnClick_GameBookSlot();
        //    }
        //}
    }

    private void CreateGameBookSlot(string dataId)
    {
        var gObj = Instantiate(Prefab_Slot, Transform_SlotRoot);
        if (gObj == null) return;

        var slotComponment = gObj.GetComponent<GameBookSlotUI>();
        if (slotComponment == null) return;

        slotComponment.InitSlot(dataId, OnClickChildSlotSelected);
        _slotList.Add(dataId, slotComponment);
    }

    private void OnClickChildSlotSelected(string slotDataId)
    {
        var currentSelectedData = DaniTechGameDataManager.Instance.GetDNItemData(slotDataId);
        if(currentSelectedData == null) return;

        Text_MainName.text = currentSelectedData.Name;
        Text_Desc.text = currentSelectedData.Description;
        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcoin, currentSelectedData.IconPath).Forget();

        foreach(var slotKv in _slotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSeletedUI(slotDataId == dataId);
        }
    }
}
