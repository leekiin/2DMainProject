using UnityEngine;
using UnityEngine.UI;

public class HudUISlot : MonoBehaviour
{
    [SerializeField] private int SlotOffsetX;
    [SerializeField] private int SlotOffsetY;

    [SerializeField] private GameObject Layout_TextArea;
    [SerializeField] private Text Text_Name;
    [SerializeField] private Slider Slider_Hp;
    [SerializeField] private Slider Slider_Mp;


    private int _instanceId;

    private Transform _targetTransform;

    public void InitSlot(int instanceId, Transform targetTransform, string characterName)
    {
        _instanceId = instanceId;
        _targetTransform = targetTransform;
        SlotOffsetX = -20;
        SlotOffsetY = 120;
        Text_Name.text = characterName;

        TryBindStatEvent(targetTransform.gameObject);
    }

    private void TryBindStatEvent(GameObject gObj)
    {
        var player = gObj.GetComponent<DaniTech_2DPlayer>();
        if (player != null)
        {
            player.BindOnStatChangedEvent(OnTargetEntityHpChanged, OnTargetEntityMpChanged);
            return;
        }

        var monster = gObj.GetComponent<GameMonster>();
        if (monster != null)
        {
            monster.BindOnStatChangedEvent(OnTargetEntityHpChanged, OnTargetEntityMpChanged);
            return;
        }
    }

    private void OnTargetEntityHpChanged(int curHp, int maxHp)
    {
        Slider_Hp.value = (curHp / (float)maxHp);
    }

    private void OnTargetEntityMpChanged(int curMp, int maxMp)
    {
        Slider_Mp.value = (curMp / (float)maxMp);
    }

    private void Update()
    {
        if (_targetTransform != null)
        {
            ////this.gameObject.transform.position = _targetTransform.position;

            Vector2 screenPos = Camera.main.WorldToScreenPoint(_targetTransform.position);

            var rectTransform = this.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 finalScreenPos = new Vector2(screenPos.x - SlotOffsetX, screenPos.y - SlotOffsetY);
                rectTransform.position = finalScreenPos;
            }
        }
    }
}
