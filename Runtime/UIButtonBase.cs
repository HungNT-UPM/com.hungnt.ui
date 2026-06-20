using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HungNT.UI
{
    /// <summary>
    /// Button UI cơ bản: set sprite / title / highlight, quản lý listener click.
    /// Kế thừa <see cref="UIViewBase"/> — dùng chung RectTransform / CanvasGroup.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIButtonBase : UIViewBase
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_Text _titleTxt;
        [SerializeField] private GameObject _highlightObj;

        private Action _clicked;

        public void AddListener(Action action) => _clicked += action;
        public void RemoveListener(Action action) => _clicked -= action;

        protected virtual void OnValidate()
        {
            if (_button == null)
                _button = GetComponent<Button>();
        }

        protected virtual void Awake()
        {
            _button.onClick.AddListener(OnActionClicked);
            SetHighlight(false);
        }

        protected virtual void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnActionClicked);
        }

        /// <summary>
        /// Đổi sprite hiển thị (bỏ qua nếu không gán Image).
        /// </summary>
        public void SetSprite(Sprite sprite)
        {
            if (_image != null)
                _image.sprite = sprite;
        }

        /// <summary>
        /// Đổi text title (bỏ qua nếu không gán TMP_Text).
        /// </summary>
        public void SetTitle(string title)
        {
            if (_titleTxt != null)
                _titleTxt.SetText(title);
        }

        /// <summary>
        /// Bật/tắt object highlight (selected state).
        /// </summary>
        public void SetHighlight(bool active)
        {
            if (_highlightObj != null)
                _highlightObj.SetActive(active);
        }

        protected virtual void OnActionClicked() => _clicked?.Invoke();
    }
}
