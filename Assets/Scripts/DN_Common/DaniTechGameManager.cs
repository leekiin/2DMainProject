using System.Collections.Generic;
using UnityEngine;

public class DaniTechGameManager : MonoBehaviour
{
    public static DaniTechGameManager Inst { get; set; }

    // 플레이 중에 저장되어야 하는 정보들이 있는 위치
    private DaniTechPlayerModel _playerModel = new DaniTechPlayerModel();

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        LoadSaveData();
    }

    public void SaveData()
    {
        DaniTechNetworkManager.Inst.RequstSaveData(_playerModel);
    }

    public void SaveAndEndGame()
    {
        SaveData();
        Application.Quit();
    }

    private void LoadSaveData()
    {
        _playerModel = DaniTechNetworkManager.Inst.RequstLoadSaveData();
    }

    public void IncreasePlayerExp(int exp)
    {
        // 추후에 한곳에서 관리할 수 있게 익스텐션으로 빼도 된다
        _playerModel.PlayerTotalExp += exp;
    }

    public void AddItem(string itemDataId, int addItemCount)
    {
        // 저장할때 고유값 ID를 부여하기 위해 사용
        long uniqueId = DaniTechGameUtil.GenerateUniqueId();

        // TODO : 우선 쉽게 사용할 수 있도록 중복 처리는 빼두었다. 습득할때마다 아이템이 하나씩 추가되도록 해두고
        // 추후에 중복값은 StackCount가 다 찰때까지 누적해줄 수 있도록 로직을 추가하자
        var newItem = new DaniTechItemModel();
        newItem.ItemUniqueId = uniqueId;
        newItem.ItemDataId = itemDataId;
        newItem.ItemStackCount = addItemCount;

        _playerModel.ItemList.Add(newItem);
    }

    public bool RequestUseItem(long requestUseTargetItemUniqueId)
    {
        //int removeTargetIdx = 0;
        //bool isRemoveItemExist = false;
        //foreach (var itemModel in _playerModel.ItemList)
        //{
        //    if (itemModel.ItemUniqueId == requestUseTargetItemUniqueId)
        //    {
        //        isRemoveItemExist = true;

        //        string itemDataId = itemModel.ItemDataId;
        //        var itemData = DaniTechGameDataManager.Instance.GetDNItemData(itemDataId);

        //        if(itemData.UseItemType != null && itemData.UseItemType.Count > 0)
        //        {
        //            if (itemData.ItemType == "Carrot")
        //            {
        //                DaniTechUIManager.Instance.OpenPopupUI(DaniTechUIType.CarrotChoicePopupUI);
        //                break;
        //            }
        //            UseItemFunction(itemData.UseItemType[0], itemData.UseItemParameterList);
        //        }
        //        break;
        //    }
        //    removeTargetIdx++;
        //}

        //RequestRemoveItem(isRemoveItemExist, removeTargetIdx);
        //return true;

        // UniqueId로 리스트에서 인덱스를 정확하게 찾습니다.
        int targetIdx = _playerModel.ItemList.FindIndex(item => item.ItemUniqueId == requestUseTargetItemUniqueId);

        if (targetIdx == -1)
        {
            Debug.LogError("사용하려는 아이템을 인벤토리에서 찾을 수 없습니다.");
            return false;
        }

        var itemModel = _playerModel.ItemList[targetIdx];
        var itemData = DaniTechGameDataManager.Instance.GetDNItemData(itemModel.ItemDataId);

        if (itemData == null) return false;

        // 아이템 타입 체크 (아이템 데이터에 정의된 Type 혹은 Model의 Type)
        // 기획 데이터(itemData)의 ItemType이 "Carrot"이거나, 사용 타입이 특수 팝업을 띄우는 형태라면
        if (itemData.ItemType == "Carrot")
        {
            // 당근 선택 팝업 오픈
            DaniTechUIManager.Instance.OpenUI(DaniTechUIRootType.PopupUI, DaniTechUIType.CarrotChoicePopupUI);

            // 아이템 소모 처리
            _playerModel.ItemList.RemoveAt(targetIdx);
            SaveData();
            return true;
        }

        // 일반 아이템 사용 로직
        if (itemData.UseItemType != null && itemData.UseItemType.Count > 0)
        {
            UseItemFunction(itemData.UseItemType[0], itemData.UseItemParameterList);

            // 아이템 소모 처리
            _playerModel.ItemList.RemoveAt(targetIdx);
            SaveData();
            return true;
        }

        return false;
    }

    //일단은 하드코딩으로 돌아가게 만듦.
    public void UseCarrotItemFunction(string itemUseType, int useItemParam)
    {
        if(itemUseType == "StatChangeHp")
        {
            var playerComponent = GetLocalPlayer();
            playerComponent.AddHp(useItemParam);
        }
        else if(itemUseType == "StatChangeAtk")
        {
            var playerComponent = GetLocalPlayer();
            playerComponent.AddAtk(useItemParam);
        }
        else if(itemUseType == "AddSkillCard")
        {
            //스킬 카드 추가 로직
            CardManager.Instance.AddCardToHand("Default_Skill_1");
        }
    }

    private void UseItemFunction(string itemUseType, List<string> useItemParamList)
    {
        if (useItemParamList == null || useItemParamList.Count == 0) return;

        if(itemUseType == "RandomItemBox")
        {

        }
        else if(itemUseType == "StatChangeAtk")
        {
            if (useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                int statChangeVal = int.Parse(str);
                var playerComponent = GetLocalPlayer();
                playerComponent.AddAtk(statChangeVal);
            }
        }
        else if(itemUseType == "StatChangeHp")
        {
            if (useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                int statChangeVal = int.Parse(str);
                var playerComponent = GetLocalPlayer();
                playerComponent.AddHp(statChangeVal);
            }
        }
        else if(itemUseType == "SummonMonster")
        {
            if (useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                var strArr = str.Split(":");
                if(strArr.Length > 1)
                {
                    string monsterDataId = strArr[0];
                    int monsterSummonCount = int.Parse(strArr[1]);

                    for(int i = 0; i < monsterSummonCount; i++)
                    {
                        var playerComponent = GetLocalPlayer();
                        DaniTechGameObjectManager.Inst.CreateMonsterObject(monsterDataId, playerComponent.transform).Forget();
                    }
                }
            }
        }
    }

    public void RequestClearItemList()
    {
        _playerModel.ItemList.Clear();
        SaveData();
    }

    private bool RequestRemoveItem(bool isRemoveItemExist, int removeRatgetIdx)
    {
        if(isRemoveItemExist == true)
        {
            _playerModel.ItemList.RemoveAt(removeRatgetIdx);
            SaveData();
            return true;
        }
        return false;
    }

    public List<DaniTechItemModel> GetPlayerItemList()
    {
        // _playerModel이 Private이므로 외부에서 ItemList를 받아올 수 있게 Get함수를 사용한다
        return _playerModel.ItemList;
    }

    public DaniTech_2DPlayer GetLocalPlayer()
    {
        return DaniTechGameObjectManager.Inst.GetLocalPlayer();
    }
}
