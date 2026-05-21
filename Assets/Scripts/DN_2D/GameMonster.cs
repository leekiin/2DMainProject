using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameMonster : MonsterBase
{
    [Header("몬스터 프리팹에서 미리 세팅할 데이터")]
    public float SkillCoolTime = 5.0f;
    public GameObject Prefab_MonsterSkillObject;
    [SerializeField] private SpriteRenderer SpriteRenderer_Monster;

    public int _instanceId;
    public string _dataId;

    private DNMonsterData _monsterData;
    public int _baseHp;
    public int _baseAtk;
    public bool _isAlive = true;
    public bool _lookRight = true;
    private Vector3 _moveDirection;


    private void OnDisable()
    {
        _isAlive = false;
    }

    public void InitMonster(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _dataId = dataId;   

        var monsterData = DaniTechGameDataManager.Instance.GetDNMonsterData(dataId);
        if(monsterData != null)
        {
            _monsterData = monsterData;
            _baseHp = _monsterData.BaseHp;
            _baseAtk = _monsterData.BaseAtk;
        }
        StartCoroutine(CheckAndUseSkill());
    }

    private int GetFinalNormalAttackDamage(int baseAtk, float normalAttackMultiple)
    {
        return (int)(baseAtk * normalAttackMultiple);
    }

    private int GetFinalSkillDamage(int baseAtk, float skillMultiple)
    {
        return (int)(baseAtk * skillMultiple);
    }

    IEnumerator CheckAndUseSkill()
    {
        while(_isAlive)
        {
            yield return new WaitForSeconds(SkillCoolTime);

            if(_isAlive == false)
            {
                break;
            }

            ChangeMonsterDirection();
            UseSkill();
        }
    }

    void ChangeMonsterDirection()
    {
        _lookRight = !_lookRight;
        _moveDirection = new Vector3(_lookRight ? 1 : -1, 0, 0);
        SetMeshDirectionByMoveDirection((int)_moveDirection.x);
    }

    void SetMeshDirectionByMoveDirection(int x)
    {
        SpriteRenderer_Monster.flipX = x < 0;
    }

    private void UseSkill()
    {
        var gObj = Instantiate(Prefab_MonsterSkillObject, DaniTechGameObjectManager.Inst.transform);
        if (gObj == null) return;

        var skillProjectileComponent = gObj.GetComponent<SkillProjectile>();
        if (skillProjectileComponent == null) return;

        float skillMultiple = _monsterData.SkillAtkMultipleList.Count > 0 ? _monsterData.SkillAtkMultipleList[0] : 0;
        int finalSkillDamage = GetFinalSkillDamage(_monsterData.BaseAtk, skillMultiple);
        skillProjectileComponent.InitSkillObject(_instanceId, _lookRight, this.transform.position, finalSkillDamage);
    }


}
