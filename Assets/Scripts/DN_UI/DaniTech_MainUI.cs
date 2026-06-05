using UnityEngine;

public class DaniTech_MainUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_MyProfile;
    [SerializeField] private DaniTechUIButton Btn_StartBattle;
    [SerializeField] private DaniTechUIButton Btn_MonsterSpawn;
    [SerializeField] private DaniTechUIButton Btn_OpenInventory;
    [SerializeField] private DaniTechUIButton Btn_OpenGameBook;

    [Header("스킬 버튼")]
    [SerializeField] private DaniTechUIButton Btn_NormalAttack;
    [SerializeField] private DaniTechUIButton Btn_FirstSkill;
    [SerializeField] private DaniTechUIButton Btn_SecondSkill;
    [SerializeField] private DaniTechUIButton Btn_ThirdSkill;



    private void OnEnable()
    {
        Btn_MyProfile.BindOnClickButtonEvent(OnClick_OpenMyProfile);
        Btn_StartBattle.BindOnClickButtonEvent(OnClick_StartBattle);
        Btn_MonsterSpawn.BindOnClickButtonEvent(OnClicK_MonsterSpawn);
        Btn_OpenInventory.BindOnClickButtonEvent(OnClick_OpenInventory);
        Btn_OpenGameBook.BindOnClickButtonEvent(OnClick_OpenGameBook);

        Btn_NormalAttack.BindOnClickButtonEvent(OnClick_UseNormalAttack);
        Btn_FirstSkill.BindOnClickButtonEvent(OnClick_UseFirstSkill);
        Btn_SecondSkill.BindOnClickButtonEvent(OnClick_UseSecondSkill);
        Btn_ThirdSkill.BindOnClickButtonEvent(OnClick_UseThirdSkill);

        if(CardManager.Instance != null)
        {
            RectTransform attackBtnRect = Btn_NormalAttack.GetComponent<RectTransform>();
            if(attackBtnRect != null)
            {
                CardManager.Instance.InitCardLayout(attackBtnRect, this.transform);
            }
            else
            {
                Debug.LogError("Btn_NormalAttack에 RectTransform 컴포넌트가 없습니다.");
            }
        }
    }

    public void OnClick_OpenInventory()
    {
        DaniTechUIManager.Instance.OpenInventoryPopup();
        DaniTechGameManager.Inst.SaveData();
    }

    public void OnClick_OpenMyProfile()
    {
        //UIManager.Instance.OpenMyProfilePopup("character_hellena_01");
        DaniTechUIManager.Instance.OpenInventoryPopup();
        Debug.LogWarning("프로필 오픈");
    }

    public void OnClick_StartBattle()
    {
        DaniTechUIManager.Instance.OpenSimplePopup("배틀 스타트!");
        Debug.LogWarning("배틀 스타트");
    }

    public void OnClicK_MonsterSpawn()
    {
        Debug.LogWarning("몬스터 스폰");
    }


    public void OnClick_OpenGameBook()
    {
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.GameBookUI);
    }

    public void OnClick_UseNormalAttack()
    {
        var localPlayer = DaniTechGameManager.Inst.GetLocalPlayer();
        localPlayer.UseNormalAttack();
    }

    public void OnClick_UseFirstSkill()
    {
        var localPlayer = DaniTechGameManager.Inst.GetLocalPlayer();
        localPlayer.UseFirstSkill();
    }

    public void OnClick_UseSecondSkill()
    {
        var localPlayer = DaniTechGameManager.Inst.GetLocalPlayer();
        localPlayer.UseSecondSkill();
    }

    public void OnClick_UseThirdSkill()
    {
        var localPlayer = DaniTechGameManager.Inst.GetLocalPlayer();
        localPlayer.UseThirdSkill();
    }

}
