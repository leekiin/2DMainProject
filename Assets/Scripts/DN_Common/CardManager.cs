using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [Header("Prefabs & Canvas")]
    public GameObject Prefab_Card;
    public Transform Transform_CardParent;

    [Header("Layout")]
    public RectTransform centerPoint; // 카드가 정렬될 중심점
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

    void Update()
    {
        HandleCardHotKeys();
        
        if(aimingCard != null)
        {
            UpdateAiming();
        }
    }

    private void HandleCardHotKeys()
    {
        if(Input.anyKeyDown == false) return;

        for(int i = 0; i < handCards.Count; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                CardSlotUI selectedEventCard = handCards[i];

                if (aimingCard == null) // 아직 조준 중이 아닐 때만
                {
                    StartAiming(selectedEventCard);
                }
                else
                {
                    if (aimingCard == selectedEventCard) // 이미 조준 중인 카드와 같은 번호를 눌렀을 때는 조준 취소
                    {
                        CancelAiming();
                    }
                    else // 다른 카드 번호를 눌렀을 때는 새로운 카드로 조준 시작
                    {
                        aimingCard.ReturnToHand(); // 기존 조준 카드 원위치
                        aimingCard = selectedEventCard; // 새로운 카드로 교체
                        aimingCard.HideCardForAiming(); // 새 카드 숨김
                    }
                }

                break;
            }
        }
    }

    public void AddCardToHand(string skillID)
    {
        if (Prefab_Card == null)
        {
            Debug.LogError("Card Prefab이 CardManager에 할당되지 않았습니다!");
            return;
        }

        // 1. 프리팹을 지정된 부모(Canvas) 밑에 생성
        GameObject newCardObj = Instantiate(Prefab_Card, Transform_CardParent);

        // 2. 생성 시점의 스케일 초기화 (DOTween 연출 유연성을 위해)
        newCardObj.transform.localScale = Vector3.zero;

        // 3. CardSlotUI 컴포넌트 획득 및 Skill ID 세팅
        CardSlotUI newCardUI = newCardObj.GetComponent<CardSlotUI>();
        if (newCardUI != null)
        {
            newCardUI.skillID = skillID;

            // 4. 매니저의 리스트에 추가
            handCards.Add(newCardUI);

            // 5. 리스트가 갱신되었으므로 부채꼴 레이아웃 재정렬
            AlignCards();
        }
    }

    public void InitCardLayout(RectTransform targetCenter, Transform targetParent)
    {
        centerPoint = targetCenter;
        Transform_CardParent = targetParent; // 외부에서 넘겨받은 메인 UI 오브젝트를 부모로 설정
        DrawDefaultCards(3); 
    }

    private void DrawDefaultCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddCardToHand($"Default_Skill_{i}");
        }
    }

    // 조준 상태 시작
    public void StartAiming(CardSlotUI card)
    {
        if (aimingCard != null) CancelAiming(); // 이미 다른 거 조준 중이면 취소

        aimingCard = card;
        aimingCard.HideCardForAiming(); // 카드는 잠시 숨김

        PlayerSkillCaster.Inst.StartIndicator();
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
        // 1. 먼저 주인공 컴포넌트를 가져와 스킬을 발사시킵니다.
        var localPlayer = DaniTechGameObjectManager.Inst.GetLocalPlayer();
        if (localPlayer != null)
        {
            int actualDamage = localPlayer.GetPlayerATK(); // 람다 없이 구현한 프로퍼티/함수
            string playerTag = localPlayer.gameObject.tag;

            PlayerSkillCaster.Inst.CastProjectileSkill(actualDamage, playerTag);
        }
        else
        {
            PlayerSkillCaster.Inst.CastProjectileSkill(100, "Player");
        }

        // 2. 조준 중이던 카드를 리스트에서 먼저 제외합니다.
        CardSlotUI cardToDestroy = aimingCard;
        handCards.Remove(cardToDestroy);
        aimingCard = null;

        // 3. 남은 카드들만 가지고 부채꼴 재정렬을 수행합니다. (이제 에러가 나지 않습니다!)
        AlignCards();

        // 4. 안전하게 애니메이션을 완전히 끄고(Kill) 오브젝트를 파괴합니다.
        if (cardToDestroy != null)
        {
            cardToDestroy.GetComponent<RectTransform>().DOKill();
            Destroy(cardToDestroy.gameObject);
        }
    }

    public void CancelAiming()
    {
        if (aimingCard == null) return;

        aimingCard.ReturnToHand(); // 카드를 다시 부채꼴 위치로 돌려놓음
        aimingCard = null;

        PlayerSkillCaster.Inst.ClearIndicator();
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
        // 1. 삼각함수로 중심점(0,0) 기준의 부채꼴 로컬 오프셋 계산
        float rad = angle * Mathf.Deg2Rad;
        float offsetX = radius * Mathf.Cos(rad);
        float offsetY = radius * Mathf.Sin(rad);
        Vector3 cardOffset = new Vector3(offsetX, offsetY, 0);

        // 2. 공격 버튼(centerPoint)의 월드 좌표를 가져옵니다.
        Vector3 buttonWorldPos = centerPoint.position;

        // 3. 공격 버튼 월드 좌표에 부채꼴 오프셋을 더해, 카드가 가야 할 '월드 좌표'를 만듭니다.
        Vector3 targetWorldPos = buttonWorldPos + cardOffset;

        // 4. 이 월드 좌표를 카드의 부모(Transform_CardParent)의 로컬 좌표계로 변환합니다.
        // 이렇게 하면 카드의 앵커가 Center든 어디든 상관없이 정확한 위치가 계산됩니다.
        Vector2 targetPos = Transform_CardParent.InverseTransformPoint(targetWorldPos);

        // 5. 회전각 계산 (기존 유지)
        float lookAngle = angle - 90f;
        Quaternion targetRot = Quaternion.Euler(0, 0, lookAngle);

        // 원래 자리 기억하기
        card.SetOriginLayout(targetPos, targetRot);

        // 조준 중이 아니라면 패 정렬 이동 (LocalMove 사용)
        if (aimingCard != card)
        {
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.DOKill();

            cardRect.DOLocalMove(targetPos, alignDuration).SetEase(Ease.OutQuad);
            cardRect.DORotateQuaternion(targetRot, alignDuration).SetEase(Ease.OutQuad);
            cardRect.DOScale(Vector3.one, alignDuration).SetEase(Ease.OutQuad);
        }

        //// 삼각함수 계산을 위해 도(Degree)를 라디안(Radian)으로 변환
        //float rad = angle * Mathf.Deg2Rad;

        //float targetX = /*centerPoint.anchoredPosition.x*/ + radius * Mathf.Cos(rad);
        //float targetY = /*centerPoint.anchoredPosition.y*/ + radius * Mathf.Sin(rad);
        //Vector2 targetPos = new Vector2(targetX, targetY);

        //// 카드가 부채꼴 원주를 따라 자연스럽게 회전하도록 Z축 회전각 계산
        //// 90도를 빼주는 이유는 유니티 UI 0도가 우측(3시 방향) 기준이기 때문에, 윗방향(12시)을 정면으로 맞추기 위함입니다.
        //float lookAngle = angle - 90f;
        //Quaternion targetRot = Quaternion.Euler(0, 0, lookAngle);

        //// [중요] 카드가 기억할 '원래 자리' 데이터 업데이트
        //// 나중에 조준 취소(ReturnToHand)할 때 이 좌표를 보고 돌아옵니다.
        //card.SetOriginLayout(targetPos, targetRot);

        //// 만약 이 카드가 현재 조준 중인 카드가 아니라면 (패에 남아있는 상태라면) 즉시 DOTween 이동
        //if (aimingCard != card)
        //{
        //    RectTransform cardRect = card.GetComponent<RectTransform>();

        //    cardRect.DOKill(); // 기존 움직임 애니메이션이 있다면 씹히지 않게 초기화

        //    //cardRect.DOAnchorPos(targetPos, alignDuration).SetEase(Ease.OutQuad);
        //    cardRect.DOLocalMove(targetPos, alignDuration).SetEase(Ease.OutQuad);
        //    cardRect.DORotateQuaternion(targetRot, alignDuration).SetEase(Ease.OutQuad);
        //    cardRect.DOScale(Vector3.one, alignDuration).SetEase(Ease.OutQuad);
        //}

    }
}