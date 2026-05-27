using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string skillName;       // 스킬 이름 또는 ID
    public CardUI menuManager; // 부모 매니저 참조
    public GameObject skillEffectPrefab; // 발생시킬 이펙트 프리팹

    // 마우스 올렸을 때 강조 (선택 사항)
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.15f, 0.2f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1.0f, 0.2f).SetUpdate(true);
    }

    // 카드를 클릭했을 때 실행 (스킬 발동)
    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. 클릭 연출: 카드가 살짝 커졌다가 사라지는 Sequence 생성
        Sequence selectSequence = DOTween.Sequence();

        selectSequence.Append(transform.DOScale(1.3f, 0.15f)) // 확 커졌다가
                      .Append(transform.DOScale(0f, 0.15f))  // 소멸
                      .OnComplete(() =>
                      {
                          ExecuteSkill();
                          menuManager.ToggleMenu(); // 스킬 사용 후 나머지 카드 복귀
                      });
    }

    void ExecuteSkill()
    {
        Debug.Log($"{skillName} 스킬 사용!");

        // 2. 이펙트 생성 (예시: 화면 중앙이나 몬스터 위치 등)
        if (skillEffectPrefab != null)
        {
            Instantiate(skillEffectPrefab, Vector3.zero, Quaternion.identity);
        }

        // TODO: 실제 데미지를 주거나 턴을 넘기는 전투 매니저 연동 로직 구현
    }
}
