using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    private DaniTech_2DPlayer Player;
    
    private List<int> _hitMonsterIdList = new List<int>();

    private void Awake()
    {
        Player = GetComponentInParent<DaniTech_2DPlayer>();
    }

    public void OnEnable()
    {
        _hitMonsterIdList.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameMonster monster = collision.GetComponent<GameMonster>();
            if(monster != null)
            {
                int monsterId = monster.GetMonsterInstanceId();

                if (_hitMonsterIdList.Contains(monsterId)) return;

                _hitMonsterIdList.Add(monsterId);

                int damage = Player.GetPlayerATK();
                var monsterTarget = DaniTechGameObjectManager.Inst.GetMonsterObjectByInstanceId(monsterId);
                if(monsterTarget != null)
                {
                    Debug.Log($"일반 공격 적중. {damage}의 피해를 가함");
                    monsterTarget.TakeDamage(damage);
                }
            }
        }
    }


}
