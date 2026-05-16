using UnityEngine;

public class EndingUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_EndGame;

    private void OnEnable()
    {
        Btn_EndGame.BindOnClickButtonEvent(OnClick_EndGame);
    }

    public void OnClick_EndGame()
    {
        Application.Quit();
    }
}
