using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HungNT.UI
{
    /// <summary>
    /// trôi lên xuống trong một khoảng, lặp mãi — tạo cảm giác UI đang lơ lửng thay vì đứng yên.
    /// </summary>
    public class UIIdleFloat : UIIdleLoopBase
    {
        [Title("Float")]
        [Tooltip("Khoảng trôi theo trục Y (pixel), lệch so với anchoredPosition gốc.")]
        [SerializeField, MinMaxSlider(-200f, 200f, true)]
        private Vector2 _offsetRange = new Vector2(-6f, 6f);

        private Vector2 _originAnchoredPos;

        protected override void ReadOrigin()
        {
            _originAnchoredPos = RectTransform.anchoredPosition;
        }

        protected override Tween CreateTween()
        {
            // đặt sẵn ở đầu khoảng: DOTween chốt giá trị start lúc tween chạy bằng cách đọc
            // vị trí hiện tại, nên phải set trước thì nhịp Yoyo mới quét đủ biên độ
            RectTransform.anchoredPosition = PosWithOffset(_offsetRange.x);

            // package không reference DOTweenModuleUI nên không có DOAnchorPos, tự dựng bằng DOTween.To
            var target = RectTransform;
            var endValue = PosWithOffset(_offsetRange.y);

            return DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, endValue, Duration);
        }

        protected override void ResetToOrigin()
        {
            RectTransform.anchoredPosition = _originAnchoredPos;
        }

        private Vector2 PosWithOffset(float offsetY)
        {
            return new Vector2(_originAnchoredPos.x, _originAnchoredPos.y + offsetY);
        }
    }
}
