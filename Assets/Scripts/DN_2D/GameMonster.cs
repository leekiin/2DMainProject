using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameMonster : MonsterBase
{
    [Header("몬스터 프리팹에서 미리 세팅할 데이터")]
    public float SkillCoolTime = 6.0f;
    public GameObject Prefab_MonsterSkillObject;
    [SerializeField] private SpriteRenderer SpriteRenderer_Monster;

    public int _instanceId;
    public string _dataId;

    private DNMonsterData _monsterData;
    public int _baseHp;
    public int _baseAtk;
    public bool _isAlive = true;
    private bool _lookRight = true;
    private int _maxHp; 
    private string _monsterName = "몬스터";

    private Vector3 _moveDirection;

    private event Action<int, int> _onHpChanged;
    private event Action<int, int> _onMpChanged;


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
            _maxHp = _baseHp;
            _baseAtk = _monsterData.BaseAtk;
            _monsterName = _monsterData.Name;
        }

        DaniTechUIManager.Instance.AddHudSlot(_instanceId, this.gameObject.transform, _monsterName, _maxHp);
        StartCoroutine(CheckAndUseSkill());
    }

    public int GetMonsterInstanceId()
    {
        return _instanceId;
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
        var tag = this.gameObject.tag;

        skillProjectileComponent.InitSkillObject(_instanceId, _lookRight, this.transform.position, finalSkillDamage, tag, OnSkillCollision);
    }

    private void OnSkillCollision(int colliedObjectInstanceId, int damage)
    {
        if(colliedObjectInstanceId == 0)
        {
            var localPlayer = DaniTechGameObjectManager.Inst.GetLocalPlayer();

            //float skillMultiple = _monsterData.SkillAtkMultipleList.Count > 0 ? _monsterData.SkillAtkMultipleList[0] : 0;
            //int finalSkillDamage = GetFinalSkillDamage(_monsterData.BaseAtk, skillMultiple);
            //localPlayer.TakeDamage(finalSkillDamage);

            localPlayer.TakeDamage(damage);
        }
    }

    public void TakeDamage(int playerDamage)
    {
        _baseHp -= playerDamage;

        Debug.LogWarning($"몬스터가 {playerDamage}의 데미지를 입었습니다. 남은 체력: {_baseHp}");

        if (_baseHp <= 0)
        {
            _baseHp = 0;
            OnBattleUnitDie();
        }

        InvokeStatChangedEvent();
    }

    private void OnBattleUnitDie()
    {
        DaniTechUIManager.Instance.RemoveHudSlot(_instanceId);
        ResetStatChangedEvent();
        Destroy(this.gameObject);
    }

    public void BindOnStatChangedEvent(Action<int, int> hpChangeCallback, Action<int, int> mpChangeCallback)
    {
        _onHpChanged += hpChangeCallback;
        _onMpChanged += mpChangeCallback;
    }

    public void ResetStatChangedEvent()
    {
        _onHpChanged = null;
        _onMpChanged = null;
    }

    private void InvokeStatChangedEvent()
    {
        _onHpChanged?.Invoke(_baseHp, _maxHp);
        //_onMpChanged?.Invoke(_playerMp);
    }

}
