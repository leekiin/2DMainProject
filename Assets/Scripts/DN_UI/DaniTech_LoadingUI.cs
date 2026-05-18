using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DaniTech_LoadingUI : DaniTechUIBase
{
    [SerializeField] private RawImage RawImage_LoadingImg;
    [SerializeField] private Slider Slider_LoadingBar;
    [SerializeField] private Image Image_SliderColor;

    private CancellationTokenSource _cancelToken;
    float[] _pausePoints = { 0.1f, 0.1f, 0.1f };
    int _pauseIndex = 0;

    private void OnEnable()
    {
        LoadAndSetLoadingImg();

    }

    private void LoadAndSetLoadingImg()
    {
        string texturePath = string.Empty;
        
        texturePath = "Texture2D/Texture2D_Loading_3";

        DaniTechGameUtil.LoadAndSetTexture(RawImage_LoadingImg, texturePath).Forget();
        StartLoadingResouce(1.0f).Forget();
    }

    private async UniTaskVoid StartLoadingResouce(float duration)
    {
        _cancelToken = new CancellationTokenSource();

        float elapsed = 0f;
        Slider_LoadingBar.value = 0f;

        // 1. 지정된 시간(duration) 동안 반복
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 2. 진행률 계산 (0.0 ~ 1.0)
            float progress = Mathf.Clamp01(elapsed / duration);

            // 가짜 연출용 ====
            if (_pauseIndex < _pausePoints.Length && progress >= _pausePoints[_pauseIndex])
            {
                float pausePointValue = _pausePoints[_pauseIndex];
                Slider_LoadingBar.value = pausePointValue;
                // 1초간 대기 (비동기)
                await UniTask.Delay(TimeSpan.FromSeconds(pausePointValue), cancellationToken: _cancelToken.Token);
                _pauseIndex++;
            }

            Slider_LoadingBar.value = progress;

            // 3. 다음 프레임까지 대기 (매 프레임 갱신)
            await UniTask.Yield(PlayerLoopTiming.Update, _cancelToken.Token);
        }

        // 4. 완료 처리
        Slider_LoadingBar.value = 1.0f;
        DaniTechUIManager.Instance.CloseLoadingUI();
    }

}
