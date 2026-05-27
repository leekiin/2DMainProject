using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class CardUI : MonoBehaviour
{
    [Header("조절 매개변수")]
    public RectTransform centerButton; // 전투 버튼
    public List<RectTransform> cards;  // 배치할 카드 3개
    public float radius = 200f;        // 부채꼴 반지름
    public float startAngle = 60f;     // 시작 각도 (우측)
    public float endAngle = 120f;      // 끝 각도 (좌측)
    public float duration = 0.5f;      // 펼쳐지는 시간

    private bool isOpened = false;
    private List<Vector2> targetPositions = new List<Vector2>();
    private List<Quaternion> targetRotations = new List<Quaternion>();

    void Start()
    {
        CalculateLayout();
        ResetCardsPosition();
    }

    // 1. 카드들의 목표 위치와 회전각을 미리 계산
    void CalculateLayout()
    {
        int cardCount = cards.Count;
        if (cardCount == 0) return;

        targetPositions.Clear();
        targetRotations.Clear();

        // 카드가 1개일 때 예외 처리 및 각도 분할
        float angleStep = (cardCount > 1) ? (endAngle - startAngle) / (cardCount - 1) : 0;

        for (int i = 0; i < cardCount; i++)
        {
            // 각 카드의 각도 계산
            float currentAngle = startAngle + (angleStep * i);
            float rad = currentAngle * Mathf.Deg2Rad;

            // 원점(전투버튼) 기준 목적지 좌표 계산
            float x = centerButton.anchoredPosition.x + radius * Mathf.Cos(rad);
            float y = centerButton.anchoredPosition.y + radius * Mathf.Sin(rad);
            targetPositions.Add(new Vector2(x, y));

            // 카드가 자연스럽게 눕도록 회전값 계산 (취향에 따라 -90은 조절 가능)
            float lookAngle = currentAngle - 90f;
            targetRotations.Add(Quaternion.Euler(0, 0, lookAngle));
        }
    }

    // 2. 카드를 초기 상태(전투 버튼 뒤에 숨김)로 세팅
    void ResetCardsPosition()
    {
        foreach (var card in cards)
        {
            card.anchoredPosition = centerButton.anchoredPosition;
            card.rotation = Quaternion.identity;
            card.localScale = Vector3.zero; // 크기도 0으로 시작
            card.gameObject.SetActive(false);
        }
    }

    // 3. 전투 버튼을 눌렀을 때 실행할 토글 함수
    public void ToggleMenu()
    {
        isOpened = !isOpened;

        for (int i = 0; i < cards.Count; i++)
        {
            if (isOpened)
            {
                cards[i].gameObject.SetActive(true);
                // DOTween을 이용해 부채꼴로 펼치기 (Ease.OutBack으로 탄력 효과)
                cards[i].DOAnchorPos(targetPositions[i], duration).SetEase(Ease.OutBack);
                cards[i].DORotateQuaternion(targetRotations[i], duration).SetEase(Ease.OutBack);
                cards[i].DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
            }
            else
            {
                // 다시 전투 버튼 안으로 복귀
                cards[i].DOAnchorPos(centerButton.anchoredPosition, duration).SetEase(Ease.InBack);
                cards[i].DORotateQuaternion(Quaternion.identity, duration).SetEase(Ease.InBack);
                cards[i].DOScale(Vector3.zero, duration).SetEase(Ease.InBack)
                        .OnComplete(() => ResetCardsPosition()); // 연출 종료 후 비활성화
            }
        }
    }

}
