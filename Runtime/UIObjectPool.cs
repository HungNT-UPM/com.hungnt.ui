using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HungNT.UI
{
    /// <summary>
    /// pool cục bộ gắn lên node cha chứa danh sách, giữ ref prefab con.
    /// mỗi lượt render gọi RecallAll rồi Get đủ số cần — thiếu thì sinh thêm, thừa nằm im chờ lượt sau.
    /// sống theo vòng đời của panel chứa nó, không static và không DontDestroyOnLoad.
    /// </summary>
    [DisallowMultipleComponent]
    public class UIObjectPool : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Nơi gắn instance. Để trống = dùng chính object này.")]
        private Transform _root;

        [SerializeField]
        [Tooltip("Prefab con để clone. Trỏ prefab asset, hoặc 1 object mẫu dựng sẵn ngay trong panel.")]
        private GameObject _prefab;

        [SerializeField]
        [Tooltip("Tự Init trong Awake. Tắt khi muốn tự gọi Init theo thời điểm của mình.")]
        private bool _autoInit = true;

        [SerializeField]
        [Tooltip("Gom các object con dựng sẵn trong prefab vào pool. Chỉ bật khi container chỉ chứa element lặp.")]
        private bool _adoptChildren = true;

        [SerializeField]
        [Tooltip("Sinh sẵn bấy nhiêu instance lúc init.")]
        private int _prewarmCount;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private List<GameObject> _actives = new();

        [ShowInInspector, ReadOnly]
        private List<GameObject> _idles = new();

        private bool _initialized;

        /// <summary>
        /// các instance đang hiển thị, theo đúng thứ tự đã Get trong lượt render hiện tại
        /// </summary>
        public IReadOnlyList<GameObject> Actives
        {
            get { return _actives; }
        }

        public int ActiveCount
        {
            get { return _actives.Count; }
        }

        public GameObject Prefab
        {
            get { return _prefab; }
            set { _prefab = value; }
        }

        private void Awake()
        {
            if (_autoInit)
            {
                Init();
            }
        }

        /// <summary>
        /// gom object con dựng sẵn và sinh trước _prewarmCount instance. gọi lại nhiều lần không sao,
        /// lần thứ hai trở đi bỏ qua
        /// </summary>
        [Button]
        public void Init()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            if (_root == null)
            {
                _root = transform;
            }

            // object mẫu dựng ngay trong panel: tắt đi để nó không nằm lẫn trong danh sách hiển thị
            if (_prefab != null && _prefab.transform.parent == _root)
            {
                _prefab.SetActive(false);
            }

            if (_adoptChildren)
            {
                AdoptChildren();
            }

            for (int i = 0; i < _prewarmCount; i++)
            {
                _idles.Add(CreateInstance());
            }
        }

        /// <summary>
        /// lấy 1 instance đang bật, xếp cuối danh sách để thứ tự hiển thị khớp thứ tự gọi
        /// </summary>
        public GameObject Get()
        {
            Init();

            GameObject obj = TakeIdle() ?? CreateInstance();
            obj.transform.SetAsLastSibling();
            obj.SetActive(true);
            _actives.Add(obj);
            return obj;
        }

        /// <summary>
        /// lấy 1 instance và trả luôn script cần dùng trên đó
        /// </summary>
        public T Get<T>() where T : Component
        {
            return Get().GetComponent<T>();
        }

        /// <summary>
        /// trả 1 instance về pool, phần còn lại giữ nguyên
        /// </summary>
        public void Recall(GameObject obj)
        {
            if (!_actives.Remove(obj))
            {
                return;
            }

            obj.SetActive(false);
            _idles.Add(obj);
        }

        public void Recall(Component component)
        {
            Recall(component.gameObject);
        }

        /// <summary>
        /// tắt hết instance đang hiển thị, giữ lại cho lượt render sau
        /// </summary>
        [Button]
        public void RecallAll()
        {
            Init();

            for (int i = 0; i < _actives.Count; i++)
            {
                _actives[i].SetActive(false);
                _idles.Add(_actives[i]);
            }

            _actives.Clear();
        }

        /// <summary>
        /// hủy sạch instance kể cả object dựng sẵn đã gom vào pool. hiếm dùng vì pool chết theo panel
        /// </summary>
        public void Clear()
        {
            DestroyAll(_actives);
            DestroyAll(_idles);
            _actives.Clear();
            _idles.Clear();
        }

        /// <summary>
        /// gom element dựng sẵn trong prefab vào pool thay vì hủy đi rồi Instantiate lại
        /// </summary>
        private void AdoptChildren()
        {
            for (int i = 0; i < _root.childCount; i++)
            {
                GameObject child = _root.GetChild(i).gameObject;
                if (child == _prefab)
                {
                    continue;
                }

                child.SetActive(false);
                _idles.Add(child);
            }
        }

        private GameObject TakeIdle()
        {
            while (_idles.Count > 0)
            {
                int last = _idles.Count - 1;
                GameObject obj = _idles[last];
                _idles.RemoveAt(last);

                // instance có thể đã bị code khác Destroy -> bỏ qua, lấy cái kế tiếp
                if (obj != null)
                {
                    return obj;
                }
            }

            return null;
        }

        private GameObject CreateInstance()
        {
            GameObject obj = Instantiate(_prefab, _root);
            obj.SetActive(false);
            return obj;
        }

        private void DestroyAll(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Destroy(list[i]);
            }
        }
    }
}
