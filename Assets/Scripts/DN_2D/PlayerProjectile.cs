using System;
using System.Collections;
using UnityEngine;

public class PlayerProjectile : SkillBase
{
    [SerializeField] private SpriteRenderer SpriteRenderer_Effect;
    [SerializeField] private float ProjecTileSpeed = 10.0f;
    [SerializeField] private float ProjecTileSecond = 10.0f;

    private int _damage;
    private int _ownerInstanceId;

    private Vector3 _moveDirection = Vector3.right;

    private event Action<int, int> _onSkillColision;

    private void OnDisable()
    {
        _onSkillColision = null;
    }

    public void InitSkillObject(int ownerInstanceId, Vector3 targetDirection, Vector3 playerPos, int damage, string parentTag, Action<int, int> onSkillCollision = null)
    {
        this.transform.position = playerPos;

        // 1. 전달받은 방향을 정규화(크기를 1로 만듦)하여 이동 방향으로 설정
        _moveDirection = targetDirection.normalized;

        // 2. 2D 투사체가 마우스 방향을 자연스럽게 바라보도록 Z축 회전 계산
        // 아크탄젠트(Atan2)를 사용해 Y와 X값으로 라디안 각도를 구하고 도(Degree) 단위로 변환합니다.
        float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0, 0, angle);

        SpriteRenderer_Effect.flipX = false;
        SpriteRenderer_Effect.flipY = false;

        _damage = damage;
        _ownerInstanceId = ownerInstanceId;
        _onSkillColision = onSkillCollision;

        this.gameObject.tag = parentTag;
        StartCoroutine(DestroyAfterSeconds(ProjecTileSecond));
    }

    private void Update()
    {
        // 변함없이 설정된 마우스 방향(_moveDirection)으로 매 프레임 직진합니다.
        transform.position += _moveDirection * ProjecTileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision.collider);
    }

    private void CheckCollision(Collider2D collision)
    {
        bool isOwnerPlayer = (_ownerInstanceId == 0);

        if (collision.CompareTag("Player") && (isOwnerPlayer == false))
        {
            _onSkillColision?.Invoke(0, _damage);
            Destroy(this.gameObject);
        }
        else if (collision.CompareTag("Enemy") && (isOwnerPlayer == true))
        {
            var gObj = collision.gameObject;
            if (gObj == null) return;

            var monsterComponent = gObj.GetComponent<GameMonster>();
            if (monsterComponent == null) return;

            int instId = monsterComponent.GetMonsterInstanceId();
            _onSkillColision?.Invoke(instId, _damage*3);

            Destroy(this.gameObject);
        }
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
