# HungNT UI (`com.hungnt.ui`)

Base UI và feedback tương tác nhẹ.

## Components

| Component | Mô tả |
|-----------|--------|
| `UIViewBase` | Cache `RectTransform`, `CanvasGroup` (lazy add), `Interactable` |
| `UIButtonBase` | Button + sprite/title/highlight, listener click sync |
| `UIScaleFeedback` | Scale khi nhấn/thả — dùng với Image, Button, hoặc bất kỳ UI có raycast target |

## Ví dụ

```csharp
using HungNT.UI;

public class MyButton : UIViewBase { }
```
