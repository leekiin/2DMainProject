using System;
using UnityEngine;
using UnityEngine.UI;

public class DaniTechUIButton : MonoBehaviour
{
    [SerializeField] private Button Button_Base;
    [SerializeField] private Text Text_Base;
    [SerializeField] private Image Image_Base;
    [SerializeField] private Image Image_Select;

    //수동으로 끊어주기 위함.
    private bool _isSlotMenualUnbindEvent;

    private void Awake()
    {
        // 1-2) 이 오브젝트가 생성될 때, 한번 컴포넌트를 찾아서 캐싱하자
        InitUIButton();
        SetDefaultUI();
    }

    private void OnEnable()
    {
        UnBindOnClickButtonEvent(OnClickSetSelectUI);
        BindOnClickButtonEvent(OnClickSetSelectUI);
    }

    private void OnDisable()
    {
        // 컴포넌트가 꺼질 때 내부 연출용 이벤트는 확실히 해제해 줍니다.
        UnBindOnClickButtonEvent(OnClickSetSelectUI);

        // 만약 매뉴얼 언바인드 모드가 아니라면 안전하게 전체 해제하는 것도 방법입니다.
        if (_isSlotMenualUnbindEvent == false && Button_Base != null)
        {
            Button_Base.onClick.RemoveAllListeners();
        }
    }


    private void SetDefaultUI()
    {
        if(Image_Select != null)
        {
            Image_Select.gameObject.SetActive(false);
        }
    }

    private void InitUIButton()
    {
        if(Button_Base != null)
        {
            return;
        }

        // 1-1) 외부에서도 등록할 수 있고,
            // 누군가 누락했다면 등록안해도 알아서 찾아주도록 로직을 넣어 놨다
        var button = this.gameObject.GetComponentInChildren<Button>();
        if(button != null)
        {
            this.Button_Base = button;
        }
    }

    public void BindOnClickButtonEvent(Action onClickCallback, bool isMenualUnbindEvent = false)
    {
        //if(Button_Base == null) return;

        //Button_Base.onClick.AddListener(new UnityEngine.Events.UnityAction(onClickCallback));
        //_isSlotMenualUnbindEvent = isMenualUnbindEvent;

        if (Button_Base == null || onClickCallback == null) return;

        // 유니티 4.x/5.x 이후 버전부터는 UnityAction 구문을 생략하고 
        // Action 델리게이트 자체를 인자로 던지면 컴파일러가 참조를 올바르게 캐싱합니다.
        Button_Base.onClick.AddListener(onClickCallback.Invoke);
        _isSlotMenualUnbindEvent = isMenualUnbindEvent;
    }

    public void UnBindOnClickButtonEvent(Action onClickCallback)
    {
        //if (Button_Base == null) return;

        //Button_Base.onClick.RemoveListener(new UnityEngine.Events.UnityAction(onClickCallback));
        if (Button_Base == null || onClickCallback == null) return;

        Button_Base.onClick.RemoveListener(onClickCallback.Invoke);
    }

    public void ChangeButtonText(string buttonStr)
    {
        // 혹시 이버튼을 동적으로, 코드에서 텍스트를 수정해야할 때 사용
        if (Text_Base == null) return;

        Text_Base.text = buttonStr;
    }

    private void OnClickSetSelectUI()
    {
        if(Image_Select != null)
        {
            bool currentActive = Image_Select.gameObject.activeSelf;
            Image_Select.gameObject.SetActive(!currentActive);
        }
    }
}
