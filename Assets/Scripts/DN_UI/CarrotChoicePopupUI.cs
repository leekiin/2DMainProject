using NUnit.Framework;
using System.Collections.Generic;
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

    public void InitCarrotChoicePopup(List<string> selectableBuffIdList)
    {
        foreach(string buffId in selectableBuffIdList)
        {

        }
    }

    public void OnClick_SelectHp()
    {
        Debug.Log("체력 선택");
        DaniTechGameManager.Inst.UseCarrotItemFunction("StatChangeHp", 70);
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.CarrotChoicePopupUI);
    }

    public void OnClick_SelectAtk()
    {
        Debug.Log("공격력 선택");
        DaniTechGameManager.Inst.UseCarrotItemFunction("StatChangeAtk", 20);
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.CarrotChoicePopupUI);
    }

    public void OnClick_SelectSkillCard()
    {
        Debug.Log("스킬 카드 선택");
        DaniTechUIManager.Instance.ClosePopupUI(DaniTechUIType.CarrotChoicePopupUI);
    }


}
