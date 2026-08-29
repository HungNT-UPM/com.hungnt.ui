# com.hungnt.ui

Base UI cho hệ sinh thái HungNT: view base cache sẵn `RectTransform` / `CanvasGroup`, button cơ bản, feedback scale khi nhấn, và anim idle (lắc lư / lơ lửng). Các package UI khác (`com.hungnt.ui.tween`, `com.hungnt.ui.panel`) đều kế thừa từ đây.

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
- Odin Inspector + DOTween — cần cho `UIScaleFeedback` và `UIIdleSwing` / `UIIdleFloat`

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

---

## UIIdleSwing / UIIdleFloat

Anim **lặp lúc UI đứng yên** để thẻ bài / icon vật phẩm không đứng chết. Cùng base `UIIdleLoopBase`, dùng DOTween theo unscaled time.

| Component | Hiệu ứng |
|-----------|----------|
| `UIIdleSwing` | lắc lư quanh trục Z trong một khoảng góc |
| `UIIdleFloat` | trôi lên xuống theo Y trong một khoảng |

Field chung (`UIIdleLoopBase`):

| Field | Mặc định | Ý nghĩa |
|-------|----------|---------|
| `_autoPlay` | `true` | tự chạy khi GameObject bật |
| `_loopCount` | `-1` | `-1` = vô hạn (`0` quy về 1 nhịp) |
| `_loopType` | `Yoyo` | `Yoyo` đi rồi về, `Restart` nhảy về đầu khoảng |
| `_ease` | `InOutSine` | |
| `_duration` | `1.2` | thời gian một nhịp (đầu khoảng → cuối khoảng) |
| `_useRandomDelay` | `true` | bốc trễ ngẫu nhiên trước nhịp đầu |
| `_delay` | `0` | trễ cố định — hiện khi `_useRandomDelay = false` |
| `_delayRange` | `0 – 0.5` | khoảng bốc ngẫu nhiên (bốc lại mỗi lần `Play`), để nhiều thẻ không lắc trùng nhịp |

Field riêng: `UIIdleSwing._angleRange` (mặc định `-4 – 4` độ, lệch so với góc Z gốc) + `_rotateMode`; `UIIdleFloat._offsetRange` (mặc định `-6 – 6` px theo Y, lệch so với `anchoredPosition` gốc).

Trạng thái gốc đọc **một lần** ở `Awake`; `Stop()` mặc định trả object về đúng trạng thái đó.

```csharp
_card.GetComponent<UIIdleSwing>().Play();   // chạy lại từ đầu, bốc random delay mới
_card.GetComponent<UIIdleSwing>().Stop();   // dừng + về góc gốc
```
