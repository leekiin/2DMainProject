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
        // 현재 씬의 활성화된 메인 카메라를 안전하게 가져옵니다.
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 startPos = transform.position;
        startPos.z = 0f; // 주인공 위치 Z축 고정

        // 마우스 스크린 좌표를 월드 좌표로 변환
        Vector3 mouseScreenPos = Input.mousePosition;

        // Z축 값을 카메라와 2D 맵 사이의 적절한 거리(보통 10f)로 명시해 주어야 대칭 왜곡이 안 생깁니다.
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f; // 월드 좌표 Z축 고정

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

    public void CastProjectileSkill(int damage, string ownerTag)
    {
        ClearIndicator();

        if (Prefab_Projectile == null) return;

        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        // 발사할 때도 동일한 방식으로 마우스 좌표 완전 보정
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z);

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // 주인공 중심(transform.position)에서 마우스를 향하는 순수한 방향 벡터 계산
        Vector3 shootDirection = (mouseWorldPos - transform.position).normalized;

        // 투사체 생성 (주인공 발사 루트 위치가 있다면 거기서 생성해도 좋습니다)
        GameObject projGo = Instantiate(Prefab_Projectile);

        PlayerProjectile projectile = projGo.GetComponent<PlayerProjectile>();

        if (projectile != null)
        {
            projectile.InitSkillObject(
                ownerInstanceId: 0,
                targetDirection: shootDirection,
                playerPos: transform.position, // 시작점 고정
                damage: damage,
                parentTag: ownerTag,
                onSkillCollision: (instId, dmg) =>
                {
                    var monsterComponent = DaniTechGameObjectManager.Inst.GetMonsterObjectByInstanceId(instId);
                    if (monsterComponent != null)
                    {
                        monsterComponent.TakeDamage(dmg);
                    }
                }
            );
        }
    }
}
