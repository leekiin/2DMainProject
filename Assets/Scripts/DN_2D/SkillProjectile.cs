using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SkillProjectile : SkillBase
{
    [SerializeField] private SpriteRenderer SpriteRenderer_Effect;
    [SerializeField] private float ProjecTileSpeed = 10.0f;
    [SerializeField] private float ProjecTileLifetime = 4.5f;

    private int _damage;
    private int _ownerInstanceId;

    private Vector3 _moveDirection = new Vector3(1, 0, 0);

    private event Action<int, int> _onSkillColision;

    private void OnDisable()
    {
        _onSkillColision = null;
    }

    public void InitSkillObject(int ownerInstanceId, bool isDirRight, Vector3 playerPos, int damage, string parentTag, Action<int, int> onSkillCollision = null)
    {
        this.transform.position = playerPos;
        _moveDirection = isDirRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
        SpriteRenderer_Effect.flipX = !isDirRight;
        SpriteRenderer_Effect.flipY = !isDirRight;

        _damage = damage;
        _ownerInstanceId = ownerInstanceId;

        _onSkillColision = onSkillCollision;

        this.gameObject.tag = parentTag;
        StartCoroutine(DestroyAfterSeconds(ProjecTileLifetime));
    }

    private void Update()
    {
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

        if(collision.CompareTag("Player") && (isOwnerPlayer == false))
        {
            _onSkillColision?.Invoke(0, _damage);

            //투사체가 자체 데미지 주는 로직
            //var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();
            //player.TakeDamage(_damage);

            Destroy(this.gameObject);
        }
        else if(collision.CompareTag("Enemy") && (isOwnerPlayer == true))
        {
            var gObj = collision.gameObject;
            if (gObj == null) return;

            var monsterComponent = gObj.GetComponent<GameMonster>();
            if(monsterComponent == null) return;

            //투사체가 자체 데미지 주는 로직
            //monsterComponent.TakeDamage(_damage);

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
