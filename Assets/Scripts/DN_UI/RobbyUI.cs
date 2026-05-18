using UnityEngine;

public class RobbyUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_GameStart;
    [SerializeField] private DaniTechUIButton Btn_QuitGame;


    private void OnEnable()
    {
        Btn_GameStart.BindOnClickButtonEvent(OnClick_StartGame);
        Btn_QuitGame.BindOnClickButtonEvent(OnClick_QuitGame);
    }

    public void OnClick_StartGame()
    {
        //DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.RobbyUI);
        DaniTechUIManager.Instance.CloseRobbyUI();
        DaniTechUIManager.Instance.OpenLoadingUI();
    }

    public void OnClick_QuitGame()
    {
        DaniTechGameManager.Inst.SaveAndEndGame();
    }

}
