using UnityEngine;

public class CarrotChoicePopupUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_SelectHp;
    [SerializeField] private DaniTechUIButton Btn_SelectAtk;
    [SerializeField] private DaniTechUIButton Btn_SelectSkillCard;

    private void OnEnable()
    {
        Btn_SelectHp.BindOnClickButtonEvent(OnClick_SelectHp);
        Btn_SelectAtk.BindOnClickButtonEvent(OnClick_SelectAtk);
        Btn_SelectSkillCard.BindOnClickButtonEvent(OnClick_SelectSkillCard);
    }

    private void OnDisable()
    {
        Btn_SelectHp.UnBindOnClickButtonEvent(OnClick_SelectHp);
        Btn_SelectAtk.UnBindOnClickButtonEvent(OnClick_SelectAtk);
        Btn_SelectSkillCard.UnBindOnClickButtonEvent(OnClick_SelectSkillCard);
    }

    public void OnClick_SelectHp()
    {
        Debug.LogWarning("체력 선택");
    }

    public void OnClick_SelectAtk()
    {
        Debug.LogWarning("공격력 선택");
    }

    public void OnClick_SelectSkillCard()
    {
        Debug.LogWarning("스킬 카드 선택");
    }


}
