using UnityEngine;

public enum SlimeEntityAnimState
{
    Idle,
    Move
}

public class SlimeAnimatorControllerSlime2D : MonoBehaviour
{
    [SerializeField] private Animator Animator_Entity;

    private SlimeEntityAnimState _currentAnimState;
    private bool _isFirstInit = true;

    // 외부에서 쉽게 변경을 요청하려고
    // 이 상태에 따른 애니메이션 재생을 여기서만 모아서 해줄려고
    public void SetState(SlimeEntityAnimState newState) // 새로운 상태
    {
        if((_isFirstInit == false) && (newState == _currentAnimState))
        {
            return;
        }

        _isFirstInit = false;
        _currentAnimState = newState;

        switch (_currentAnimState)
        {
            case SlimeEntityAnimState.Idle:
                ResetAllAnimParameters();
                break;
            case SlimeEntityAnimState.Move:
                Animator_Entity.SetBool("IsMove", true);
                break;
            default:
                ResetAllAnimParameters();
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Animator_Entity.SetBool("IsMove", false);
    }
}
