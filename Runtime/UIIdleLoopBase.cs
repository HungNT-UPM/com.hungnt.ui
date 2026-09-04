using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HungNT.UI
{
    /// <summary>
    /// base cho anim idle chạy vòng lặp (lắc lư, lơ lửng...) lúc UI đứng yên.
    /// lớp con chỉ cần đọc trạng thái gốc và dựng tween một nhịp.
    /// </summary>
    public abstract class UIIdleLoopBase : UIViewBase
    {
        #region === Config ===

        [Title("Idle Loop")]
        [Tooltip("Tự cache vị trí On Awake")]
        [SerializeField]
        private bool _autoCapture = true;

        [Tooltip("Tự chạy khi GameObject được bật.")]
        [SerializeField]
        private bool _autoPlay = true;

        [Tooltip("-1 = lặp vô hạn.")]
        [SerializeField, MinValue(-1)]
        private int _loopCount = -1;

        [Tooltip("Yoyo: đi rồi về. Restart: về đầu khoảng rồi chạy lại.")]
        [SerializeField]
        private LoopType _loopType = LoopType.Yoyo;

        [SerializeField]
        private Ease _ease = Ease.InOutSine;

        [Tooltip("Thời gian một nhịp (đi từ đầu khoảng tới cuối khoảng).")]
        [SerializeField, MinValue(0.01f)]
        private float _duration = 1.2f;

        [Title("Delay")]
        [Tooltip("Bốc trễ ngẫu nhiên trước nhịp đầu, để nhiều thẻ cùng loại không lắc trùng nhịp.")]
        [SerializeField]
        private bool _useRandomDelay = true;

        [Tooltip("Trễ cố định trước nhịp đầu.")]
        [HideIf(nameof(_useRandomDelay))]
        [SerializeField, MinValue(0f)]
        private float _delay;

        [Tooltip("Khoảng bốc trễ ngẫu nhiên, bốc lại mỗi lần Play.")]
        [ShowIf(nameof(_useRandomDelay))]
        [SerializeField, MinMaxSlider(0f, 3f, true)]
        private Vector2 _delayRange = new Vector2(0f, 0.5f);

        #endregion

        private Tween _tween;
        private bool _originCaptured;

        /// <summary>
        /// vòng lặp đang chạy hay không (tính cả lúc đang chờ delay).
        /// </summary>
        public bool IsPlaying
        {
            get { return _tween != null && _tween.IsActive() && _tween.IsPlaying(); }
        }

        /// <summary>
        /// thời gian một nhịp, cho lớp con dựng tween.
        /// </summary>
        protected float Duration
        {
            get { return _duration; }
        }

        // ── Vòng đời ─────────────────────────────────────────────

        protected virtual void Awake()
        {
            if (_autoCapture)
            {
                CaptureOrigin();
            }
        }

        private void OnEnable()
        {
            if (_autoPlay)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        private void OnDestroy()
        {
            KillTween();
        }

        // ── API ──────────────────────────────────────────────────

        /// <summary>
        /// chạy vòng lặp từ đầu; đang chạy thì kill nhịp cũ rồi bắt đầu lại.
        /// </summary>
        public void Play()
        {
            CaptureOrigin();
            KillTween();

            _tween = CreateTween()
                .SetEase(_ease)
                .SetLoops(NormalizedLoopCount(), _loopType)
                .SetDelay(ResolveDelay())
                .SetUpdate(true)
                .SetTarget(this);
        }

        /// <summary>
        /// dừng vòng lặp; mặc định trả object về đúng trạng thái trước khi anim chạy.
        /// </summary>
        public void Stop(bool resetToOrigin = true)
        {
            KillTween();

            if (resetToOrigin && _originCaptured)
            {
                ResetToOrigin();
            }
        }

        // ── Lớp con cài đặt ──────────────────────────────────────

        /// <summary>
        /// đọc và nhớ trạng thái gốc (góc, vị trí...) để tween quanh nó. gọi đúng một lần.
        /// </summary>
        protected abstract void ReadOrigin();

        /// <summary>
        /// dựng tween một nhịp đi từ đầu khoảng tới cuối khoảng; chưa cần gắn ease/loop/delay.
        /// lớp con tự đặt object về đầu khoảng trước khi tạo tween.
        /// </summary>
        protected abstract Tween CreateTween();

        /// <summary>
        /// đưa object về trạng thái gốc đã đọc ở ReadOrigin.
        /// </summary>
        protected abstract void ResetToOrigin();

        // ── Nội bộ ───────────────────────────────────────────────

        private void CaptureOrigin()
        {
            if (_originCaptured)
            {
                return;
            }

            ReadOrigin();
            _originCaptured = true;
        }

        private void KillTween()
        {
            if (_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }
        }

        /// <summary>
        /// DOTween coi 0 vòng là không hợp lệ, quy về 1 nhịp.
        /// </summary>
        private int NormalizedLoopCount()
        {
            return _loopCount == 0 ? 1 : _loopCount;
        }

        private float ResolveDelay()
        {
            if (!_useRandomDelay)
            {
                return _delay;
            }

            return Random.Range(_delayRange.x, _delayRange.y);
        }
    }
}
