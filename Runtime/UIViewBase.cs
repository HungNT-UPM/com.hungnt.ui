using UnityEngine;

namespace HungNT.UI
{
    /// <summary>
    /// Base UI view: cache <see cref="RectTransform"/> và <see cref="CanvasGroup"/> (lazy, tự add nếu thiếu).
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIViewBase : MonoBehaviour
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        [SerializeField, HideInInspector] private CanvasGroup _canvasGroup;
        [SerializeField, HideInInspector] private RectTransform _parent;

        /// <summary>
        /// RectTransform của view (bắt buộc có trên GameObject).
        /// </summary>
        public virtual RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        /// <summary>
        /// CanvasGroup dùng fade / chặn tương tác; tự thêm component nếu chưa có.
        /// </summary>
        public virtual CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetOrAddComponent<CanvasGroup>();

                return _canvasGroup;
            }
        }

        /// <summary>
        /// RectTransform của parent trực tiếp (cache lần đọc đầu).
        /// </summary>
        public virtual RectTransform Parent
        {
            get
            {
                if (_parent == null)
                    _parent = RectTransform.parent as RectTransform;

                return _parent;
            }
        }

        /// <summary>
        /// Đọc/ghi <see cref="CanvasGroup.interactable"/>; mặc định true nếu chưa có CanvasGroup.
        /// </summary>
        public virtual bool Interactable
        {
            get
            {
                if (CanvasGroup != null)
                    return CanvasGroup.interactable;

                return true;
            }

            set
            {
                if (CanvasGroup != null)
                    CanvasGroup.interactable = value;
            }
        }

        private T GetOrAddComponent<T>() where T : Component
        {
            if (TryGetComponent(out T existing))
                return existing;

            return gameObject.AddComponent<T>();
        }
    }
}
