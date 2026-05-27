using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [Header("Layout")]
    public RectTransform centerPoint; // 카드가 정렬될 중심점 (구 전투버튼 위치)
    public List<CardSlotUI> handCards = new List<CardSlotUI>(); // 현재 들고 있는 카드들

    [Header("Aiming")]
    public CardSlotUI aimingCard = null; // 현재 조준 중인 카드
    public GameObject rangeIndicator;   // 마우스를 따라다닐 조준선/장판 프리팹 (선택)

    [Header("부채꼴 레이아웃 세팅")]
    public float radius = 250f;        // 부채꼴 반지름 (카드가 중심점에서 떨어질 거리)
    public float startAngle = 60f;     // 카드가 배치될 시작 각도 (우측)
    public float endAngle = 120f;      // 카드가 배치될 끝 각도 (좌측)
    public float alignDuration = 0.4f; // 정렬되는 데 걸리는 시간

    private GameObject currentIndicator;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AlignCards();
    }

    void Update()
    {
        // 1. 단축키 입력 처리 (1, 2, 3번 키로 카드 선택)
        if (aimingCard == null) // 이미 조준 중이 아닐 때만
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && handCards.Count > 0) StartAiming(handCards[0]);
            if (Input.GetKeyDown(KeyCode.Alpha2) && handCards.Count > 1) StartAiming(handCards[1]);
            if (Input.GetKeyDown(KeyCode.Alpha3) && handCards.Count > 2) StartAiming(handCards[2]);
        }
        else
        {
            // 2. 조준 중일 때의 처리
            UpdateAiming();
        }
    }

    // 조준 상태 시작
    public void StartAiming(CardSlotUI card)
    {
        if (aimingCard != null) CancelAiming(); // 이미 다른 거 조준 중이면 취소

        aimingCard = card;
        aimingCard.HideCardForAiming(); // 카드는 잠시 숨김

        // 조준 인디케이터 생성
        if (rangeIndicator != null)
        {
            currentIndicator = Instantiate(rangeIndicator);
        }
    }

    // 조준 중 마우스 이동 및 클릭 감지
    void UpdateAiming()
    {
        // 마우스 위치를 월드 좌표로 변환하여 인디케이터 이동 (2D 탑다운/사이드뷰에 맞게 조절 필요)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        if (currentIndicator != null)
        {
            currentIndicator.transform.position = mouseWorldPos;
            // 필요하다면 캐릭터와 마우스 사이의 각도를 계산해 방향을 회전시킵니다.
        }

        // 마우스 좌클릭 -> 스킬 실행
        if (Input.GetMouseButtonDown(0))
        {
            ExecuteSkill(mouseWorldPos);
        }
        // 마우스 우클릭 또는 ESC -> 취소
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelAiming();
        }
    }

    void ExecuteSkill(Vector3 targetPos)
    {
        aimingCard.UseSkill(targetPos);

        // 사용한 카드를 리스트에서 제거하고 파괴
        handCards.Remove(aimingCard);
        Destroy(aimingCard.gameObject);

        aimingCard = null;
        if (currentIndicator != null) Destroy(currentIndicator);

        // 남은 카드들 정렬 상태 재업데이트
        AlignCards();
    }

    public void CancelAiming()
    {
        if (aimingCard == null) return;

        aimingCard.ReturnToHand(); // 카드를 다시 부채꼴 위치로 돌려놓음
        aimingCard = null;

        if (currentIndicator != null) Destroy(currentIndicator);
    }

    // 카드들을 부채꼴로 예쁘게 재정렬하는 함수 (카드가 사용되어 개수가 줄었을 때도 대응)
    public void AlignCards()
    {
        // 지난번에 작성한 수학 공식(Mathf.Cos, Sin) 기반 레이아웃 로직을 
        // handCards 리스트 기준으로 돌려서 각 카드의 원래 자리를 찾아 짚어주고 DOMove 시킵니다.
        int cardCount = handCards.Count;
        if (cardCount == 0) return;

        // 1. 카드가 1개만 남았을 때는 양 끝 각도의 딱 중간(90도)에 배치
        if (cardCount == 1)
        {
            float centerAngle = (startAngle + endAngle) / 2f;
            MoveCardToAngle(handCards[0], centerAngle);
            return;
        }

        // 2. 카드가 여러 개일 때는 시작 각도와 끝 각도 사이를 균등하게 분할
        // 예: 3개면 angleStep은 (120 - 60) / 2 = 30도씩 (60도, 90도, 120도)
        float angleStep = (endAngle - startAngle) / (cardCount - 1);

        for (int i = 0; i < cardCount; i++)
        {
            // 각 카드가 가야 할 고유한 각도 계산
            float currentAngle = startAngle + (angleStep * i);
            MoveCardToAngle(handCards[i], currentAngle);
        }
    }

    private void MoveCardToAngle(CardSlotUI card, float angle)
    {
        // 삼각함수 계산을 위해 도(Degree)를 라디안(Radian)으로 변환
        float rad = angle * Mathf.Deg2Rad;

        // CenterPoint(기준점)의 anchoredPosition을 원점으로 잡고 목표 UI 좌표 계산
        float targetX = centerPoint.anchoredPosition.x + radius * Mathf.Cos(rad);
        float targetY = centerPoint.anchoredPosition.y + radius * Mathf.Sin(rad);
        Vector2 targetPos = new Vector2(targetX, targetY);

        // 카드가 부채꼴 원주를 따라 자연스럽게 회전하도록 Z축 회전각 계산
        // 90도를 빼주는 이유는 유니티 UI 0도가 우측(3시 방향) 기준이기 때문에, 윗방향(12시)을 정면으로 맞추기 위함입니다.
        float lookAngle = angle - 90f;
        Quaternion targetRot = Quaternion.Euler(0, 0, lookAngle);

        // [중요] 카드가 기억할 '원래 자리' 데이터 업데이트
        // 나중에 조준 취소(ReturnToHand)할 때 이 좌표를 보고 돌아옵니다.
        card.SetOriginLayout(targetPos, targetRot);

        // 만약 이 카드가 현재 조준 중인 카드가 아니라면 (패에 남아있는 상태라면) 즉시 DOTween 이동
        if (aimingCard != card)
        {
            RectTransform cardRect = card.GetComponent<RectTransform>();

            cardRect.DOKill(); // 기존 움직임 애니메이션이 있다면 씹히지 않게 초기화
            cardRect.DOAnchorPos(targetPos, alignDuration).SetEase(Ease.OutQuad);
            cardRect.DORotateQuaternion(targetRot, alignDuration).SetEase(Ease.OutQuad);
            cardRect.DOScale(Vector3.one, alignDuration).SetEase(Ease.OutQuad);
        }
    }
}