using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSlotUI : MonoBehaviour
{
    [SerializeField] private Text Text_InteractionTitle;
    [SerializeField] private Text Text_KeyName;
    [SerializeField] private DaniTechUIButton Btn_OnClickInteraction;



    private int _instanceId;

    private Transform _targetTransform;
    private string _interactionCallbackMsg;

    private event Action<string> _onClickCallback;

    private void OnEnable()
    {
        Btn_OnClickInteraction.BindOnClickButtonEvent(OnClick_Interaction);
    }

    private void OnDisable()
    {
        _onClickCallback = null;
    }

    public void OnClick_Interaction()
    {
        _onClickCallback?.Invoke(_interactionCallbackMsg);
    }

    public void InitSlot(int instanceId, string interactionTitle, string interactionkey
        , Transform targetTransform
        , Action<string> onClockCallback = null)
    {
        _instanceId = instanceId;
        _targetTransform = targetTransform;

        Text_KeyName.text = interactionkey;
        Text_InteractionTitle.text = interactionTitle;

        _interactionCallbackMsg = interactionkey;
        _onClickCallback = onClockCallback;
    }


































































}
