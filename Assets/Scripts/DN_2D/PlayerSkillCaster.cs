using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;

public class PlayerSkillCaster : MonoBehaviour
{
    public static PlayerSkillCaster Inst;

    [Header("프리팹")]
    public GameObject Prefab_LindIndicator;
    public GameObject Prefab_Projectile;

    private GameObject _currentIndicator;
    private LineRenderer _lineRendrerer;
    private bool _isAiming = false;


    private void Awake()
    {
        Inst = this;
    }

    private void Update()
    {
        if((_isAiming == true) && (_lineRendrerer != null))
        {
            UpdateLineIndicator();
        }
    }

    public void StartIndicator()
    {
        _isAiming = true;
        if ((_currentIndicator == null) && (Prefab_LindIndicator != null)) 
        {
            _currentIndicator = Instantiate(Prefab_LindIndicator);
            _lineRendrerer = _currentIndicator.GetComponent<LineRenderer>();
        }
    }


    private void UpdateLineIndicator()
    {
        Vector3 startPos = transform.position;
        startPos.z = 0f;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        _lineRendrerer.SetPosition(0, startPos);
        _lineRendrerer.SetPosition(1, mouseWorldPos);

    }

    public void ClearIndicator()
    {
        _isAiming = false;
        if(_currentIndicator != null)
        {
            Destroy(_currentIndicator);
            _currentIndicator = null;
            _lineRendrerer = null;
        }
    }

    public void CastProjectileSkill()
    {
        ClearIndicator();

        if (Prefab_Projectile == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector3 shootDirection = mouseWorldPos - transform.position;

        // 2. 투사체 생성
        GameObject projGo = Instantiate(Prefab_Projectile);

        // 3. 기존에 사용하던 'SkillProjectile' 컴포넌트로 가져오기
        PlayerProjectile projectile = projGo.GetComponent<PlayerProjectile>();

        if (projectile != null)
        {
            // 4. Launch 대신 정의해둔 InitSkillObject를 호출하여 방향과 데이터를 넘겨줍니다.
            int damage = 10; // 임시 대미지 값 (기존에 관리하던 대미지 변수가 있다면 그것을 넣으세요!)

            projectile.InitSkillObject(
                ownerInstanceId: 0,               // 플레이어가 쏘는 것이므로 0
                targetDirection: shootDirection,  // 마우스 방향 벡터
                playerPos: transform.position,    // 시작 위치 (주인공 위치)
                damage: damage,                   // 스킬 대미지
                parentTag: "Player",              // 부모 태그
                onSkillCollision: (instId, dmg) =>
                {
                    // 필요 시 적중 콜백 로직 추가 (없다면 null 입력 가능)
                    Debug.Log($"적 인스턴스 {instId}에게 {dmg}의 피해를 입힘");
                }
            );
        }
    }
}
