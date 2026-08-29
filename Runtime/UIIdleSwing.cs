using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HungNT.UI
{
    /// <summary>
    /// lắc lư quanh trục Z trong một khoảng góc, lặp mãi — dùng cho thẻ bài, icon vật phẩm,
    /// biển hiệu... để UI không đứng chết.
    /// </summary>
    public class UIIdleSwing : UIIdleLoopBase
    {
        [Title("Swing")]
        [Tooltip("Khoảng góc lắc (độ), lệch so với góc Z gốc của node.")]
        [SerializeField, MinMaxSlider(-90f, 90f, true)]
        private Vector2 _angleRange = new Vector2(-4f, 4f);

        [Tooltip("Fast: đi đường ngắn nhất. FastBeyond360: cho phép quay quá một vòng.")]
        [SerializeField]
        private RotateMode _rotateMode = RotateMode.Fast;

        private Vector3 _originEuler;

        protected override void ReadOrigin()
        {
            _originEuler = RectTransform.localEulerAngles;
        }

        protected override Tween CreateTween()
        {
            // đặt sẵn ở đầu khoảng: DOTween chốt giá trị start lúc tween chạy bằng cách đọc
            // góc hiện tại, nên phải set trước thì nhịp Yoyo mới quét đủ biên độ
            RectTransform.localEulerAngles = EulerWithOffset(_angleRange.x);

            return RectTransform.DOLocalRotate(EulerWithOffset(_angleRange.y), Duration, _rotateMode);
        }

        protected override void ResetToOrigin()
        {
            RectTransform.localEulerAngles = _originEuler;
        }

        private Vector3 EulerWithOffset(float offset)
        {
            return new Vector3(_originEuler.x, _originEuler.y, _originEuler.z + offset);
        }
    }
}
