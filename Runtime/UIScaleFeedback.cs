using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HungNT.UI
{
    /// <summary>
    /// Phản hồi scale khi nhấn/thả — gắn lên Image, Button, hoặc UI bất kỳ có raycast target.
    /// </summary>
    public class UIScaleFeedback : UIViewBase, IPointerDownHandler, IPointerUpHandler
    {
        [Title("Scale On Press")]
        [SerializeField, InlineButton(nameof(RefreshScale))] private float _originScale = 1f;
        [SerializeField] private float _pressScaleMultiplier = 0.9f;
        [SerializeField] private float _releaseScaleMultiplier = 1.1f;
        [SerializeField] private float _pressAnimDuration = 0.15f;

        [Title("Scale In Idle")]
        [SerializeField] private bool _scaleInIdle = false;
        [SerializeField, ShowIf(nameof(_scaleInIdle))] private float _idleScaleMultiplier = 1.05f;
        [SerializeField, ShowIf(nameof(_scaleInIdle))] private float _idleAnimDuration = 0.75f;
        [SerializeField, ShowIf(nameof(_scaleInIdle))] private Ease _idleAnimEase = Ease.InOutSine;

        private DG.Tweening.Tween _pressTween;
        private DG.Tweening.Tween _releaseTween;
        private DG.Tweening.Tween _idleTween;

        private void Reset() => RefreshScale();

        private void RefreshScale()
        {
            _originScale = RectTransform.localScale.x;
        }

        private void OnEnable()
        {
            if (_scaleInIdle)
                PlayIdleAnim();
        }

        private void OnDisable() => KillAllTweens();

        private void OnDestroy() => KillAllTweens();

        public void OnPointerDown(PointerEventData eventData)
        {
            KillAllTweens();

            _pressTween = RectTransform.DOScale(_originScale * _pressScaleMultiplier, _pressAnimDuration)
                .SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            KillAllTweens();

            _releaseTween = DOTween.Sequence()
                .Append(RectTransform.DOScale(_originScale * _releaseScaleMultiplier, _pressAnimDuration))
                .Append(RectTransform.DOScale(_originScale, _pressAnimDuration))
                .OnComplete(() =>
                {
                    if (_scaleInIdle)
                        PlayIdleAnim();
                })
                .SetUpdate(true);
        }

        private void PlayIdleAnim()
        {
            _idleTween?.Kill();
            _idleTween = RectTransform.DOScale(_originScale * _idleScaleMultiplier, _idleAnimDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(_idleAnimEase)
                .SetUpdate(true);
        }

        private void KillAllTweens()
        {
            _pressTween?.Kill();
            _releaseTween?.Kill();
            _idleTween?.Kill();

            _pressTween = null;
            _releaseTween = null;
            _idleTween = null;
        }
    }
}
