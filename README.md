# com.hungnt.ui

Base UI cho hệ sinh thái HungNT: view base cache sẵn `RectTransform` / `CanvasGroup`, button cơ bản, và feedback scale khi nhấn. Các package UI khác (`com.hungnt.ui.tween`, `com.hungnt.ui.panel`) đều kế thừa từ đây.

Namespace: **`HungNT.UI`**.

---

## Cài đặt

`Packages/manifest.json`:

```json
"com.hungnt.ui": "https://github.com/HungNT-UPM/com.hungnt.ui.git#1.0.3"
```

Hoặc **Package Manager → Add package from git URL**:

```
https://github.com/HungNT-UPM/com.hungnt.ui.git#1.0.3
```

### Yêu cầu
- Unity 2022.3+
- TextMeshPro
- Odin Inspector + DOTween — chỉ cần cho `UIScaleFeedback`

---

## UIViewBase

Base cho mọi UI view. Cache **lazy** `RectTransform` và `CanvasGroup` (tự `AddComponent` nếu thiếu), expose `Interactable`. Reference bị destroy sẽ được lấy lại ở lần truy cập sau (Unity fake-null), nên cache an toàn qua domain reload / re-enable.

```csharp
using HungNT.UI;

public class HealthBar : UIViewBase
{
    public void SetVisible(bool show)
    {
        CanvasGroup.alpha = show ? 1f : 0f;   // CanvasGroup tự được add nếu chưa có
        Interactable = show;
    }
}
```

| Thành phần | Mô tả |
|-----------|-------|
| `RectTransform` | Cache lazy (bắt buộc có trên GameObject). |
| `CanvasGroup` | Cache lazy, tự add nếu thiếu — dùng fade / chặn tương tác. |
| `Parent` | `RectTransform` của parent trực tiếp. |
| `Interactable` | Đọc/ghi `CanvasGroup.interactable`. |

---

## UIButtonBase

`[RequireComponent(Button)]`. Quản lý sprite / title / highlight và listener click qua `Action` (tự `AddListener`/`RemoveListener` vào `Button.onClick` trong Awake/OnDestroy — không cần wire trong Inspector).

```csharp
var btn = GetComponent<UIButtonBase>();

btn.AddListener(OnClick);
btn.SetTitle("Bắt đầu");
btn.SetSprite(activeSprite);
btn.SetHighlight(true);          // bật object selected-state

void OnClick() => Debug.Log("clicked");
```

`SetSprite` / `SetTitle` / `SetHighlight` bỏ qua an toàn nếu field tương ứng (Image / TMP_Text / highlight object) chưa gán.

---

## UIScaleFeedback

Hiệu ứng scale khi nhấn/thả, gắn lên bất kỳ UI có raycast target (Image, Button…). Dùng DOTween và chạy cả khi `Time.timeScale = 0` (`SetUpdate(true)`).

- **Scale On Press** — scale xuống khi nhấn, nảy nhẹ khi thả.
- **Scale In Idle** *(tùy chọn)* — loop Yoyo nhẹ lúc đứng yên.

```
Button
├── Button (component)
└── UIScaleFeedback (component)   ← thêm vào là có feedback nhấn
```

> `_originScale` lấy từ `localScale` hiện tại — bấm **Refresh** trong Inspector nếu đổi scale gốc của object.
