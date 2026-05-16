using UnityEngine;
using UnityEngine.UI;

public class CarrotScoreUI : MonoBehaviour
{
    [SerializeField] private Text Text_CurrentScore;

    public void AddCarrotScore(int currentCarrot)
    {
        Text_CurrentScore.text = $"얻은 당근 수 : {currentCarrot}";
    }
 
}
