using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardSlotUI : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Hover 세팅")]
    public float hoverScale = 1.1f;      // Hover 시 크기 배율
    public float hoverRiseDistance = 30f; // 부채꼴 바깥쪽으로 떠오를 거리 (UI 좌표 기준)
    public float hoverDuration = 0.2f;     // 연출 시간

    public string skillID;
    public float dragCancelThreshold = 100f; // 이 거리 이상 드래그하면 조준 모드 진입

    [HideInInspector] public Vector2 originAnchoredPosition;
    [HideInInspector] public Quaternion originRotation;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private bool isHovering = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>(); // 페이드 효과용 (없으면 추가)
    }

    // 원래 내 부채꼴 자리 기억하기 (매니저가 배치해줄 때 설정)
    public void SetOriginLayout(Vector2 pos, Quaternion rot)
    {
        originAnchoredPosition = pos;
        originRotation = rot;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 이미 조준 중이거나 드래그 중이면 Hover 효과 무시
        if (CardManager.Instance.aimingCard != null) return;

        isHovering = true;

        // DOTween 애니메이션 중복 실행 방지 (이전 트윈 끄기)
        rectTransform.DOKill();

        // [연출] 1. 크기 확대
        rectTransform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutQuad);

        // [연출] 2. 부채꼴 바깥 방향으로 이동 (떠오름 효과)
        // 카드가 서 있는 로컬 Z축 회전값을 기준으로 윗방향(Up)이 부채꼴 바깥쪽입니다.
        Vector3 riseOffset = rectTransform.up * hoverRiseDistance;
        Vector3 hoverPos = (Vector3)originAnchoredPosition + riseOffset;

        rectTransform.DOAnchorPos(hoverPos, hoverDuration).SetEase(Ease.OutQuad);
    }

    // 2. 마우스가 카드 영역 밖으로 나갔을 때 (Hover End)
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        // 조준 모드로 진입해서 카드가 숨겨지는 상태라면 복귀 연출을 하지 않음
        if (CardManager.Instance.aimingCard == this) return;

        // 원상복귀 연출
        ReturnToHand();
    }

    // 단순히 클릭했을 때도 조준 모드 진입
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            CardManager.Instance.StartAiming(this);
        }
    }

    // 드래그 중일 때 카드가 마우스를 따라오게 함
    public void OnDrag(PointerEventData eventData)
    {
        // 조준 중이 아닐 때만 카드를 마우스로 움직임
        if (CardManager.Instance.aimingCard != this)
        {
            rectTransform.position = eventData.position;
        }
    }

    // 드래그를 끝냈을 때 판정
    public void OnEndDrag(PointerEventData eventData)
    {
        float distance = Vector2.Distance(rectTransform.anchoredPosition, originAnchoredPosition);

        // 일정 범위 밖으로 멀리 드래그했다면 조준 모드로 전환
        if (distance > dragCancelThreshold)
        {
            CardManager.Instance.StartAiming(this);
        }
        else
        {
            // 애매하게 드래그하다 말았으면 제자리로 빽
            ReturnToHand();
        }
    }

    // 조준 시작 시 카드를 연출상 숨김
    public void HideCardForAiming()
    {
        // 툭 사라지는 것보다 DOTween으로 투명도와 크기를 줄여주면 자연스럽습니다.
        rectTransform.DOScale(0f, 0.2f);
        if (canvasGroup != null) canvasGroup.DOFade(0f, 0.2f);
    }

    // 취소되어서 패로 다시 돌아올 때의 연출
    public void ReturnToHand()
    {
        if (isHovering && CardManager.Instance.aimingCard != this) return;

        rectTransform.DOKill(); // 기존 애니메이션 처치

        // 원래 배치 정보로 부드럽게 복귀 (Ease.OutCyan이나 OutQuad 추천)
        //rectTransform.DOAnchorPos(originAnchoredPosition, 0.4f).SetEase(Ease.OutQuad);
        rectTransform.DOLocalMove(originAnchoredPosition, 0.4f).SetEase(Ease.OutQuad);
        rectTransform.DORotateQuaternion(originRotation, 0.4f).SetEase(Ease.OutQuad);
        rectTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutQuad);

        if (canvasGroup != null) canvasGroup.DOFade(1f, hoverDuration);
    }

    // 실제 스킬 로직 발동 (이펙트, 대미지 등)
    public void UseSkill(Vector3 targetWorldPos)
    {
        Debug.Log($"{skillID} 스킬을 {targetWorldPos} 좌표에 발동합니다!");
        // 여기서 실제 스킬 시스템(투사체 발사, 범위 공격 등)을 호출하시면 됩니다.
    }
}
