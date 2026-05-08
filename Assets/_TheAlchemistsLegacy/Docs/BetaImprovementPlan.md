# The Alchemist's Legacy — Beta Improvement Plan

**Status:** 记录中 — 全部四关完成后实施  
**来源：** Tutor 评测反馈 + GDD Known Flaws 整合

---

## 优先级排序（按评分影响）

### P1 — 必须修（直接影响 Game Quality 评分）

| # | 问题 | 具体位置 | 方案 |
|---|---|---|---|
| 1 | **物品放下闪退 / 出界消失** | `ItemPickup.cs` DropItem() | ✅ 已修：collisionDetectionMode 顺序、ContinuousDynamic→Discrete、自动 convex MeshCollider、墙壁 raycast 安全落点 |
| 1.5 | **道具召回键 (Recall)** | `ItemPickup.cs` | ✅ 已加：按 R 键召回最近拾起且仍存在的道具；已被消费(destroyed/SetActive false)的不可召回，避免破坏关卡设计 |
| 2 | **鼠标灵敏度过高（多人报告头晕）** | `PlayerController.cs` mouseSensitivity | 默认值从 0.8 降至 0.3；Pause 菜单加灵敏度 Slider |
| 3 | **没有重开关卡入口（只能刷新页面）** | Pause 菜单 | 加 "Restart Level" 按钮 → `SceneManager.LoadScene` |

### P2 — 重要（影响 Individual Level Quality）

| # | 问题 | 具体位置 | 方案 |
|---|---|---|---|
| 4 | **可交互物体视觉引导不足** | 全关卡 | 统一 Outline + Emission 发光系统；关键道具换饱和金/橙色材质 |
| 5 | **L4 底座符号看不清** | Level4 scene | ○▢△ 底座符号放大，发同色脉冲光；relic 也发配套颜色微光 |
| 6 | **L2 火把无法拾取** | Level2 / ItemPickup | 检查 layer/tag/collider 设置；确认 TorchGiver 脚本逻辑 |
| 7 | **L3 架子颜色和墙融合** | Level3 scene | 架子材质加 emissive 边缘高亮或用对比色木纹 |

### P3 — 体验优化（有时间再做）

| # | 问题 | 方案 |
|---|---|---|
| 8 | **目标不清晰** | HUD 左上角加 objective 文字，`TheAlchemistsLegacyLevelHud.cs` 已存在 |
| 9 | **Idle 提示** | 玩家卡住 > 2min 触发大师低语文字提示（符合"Set in Stone"叙事） |
| 10 | **菜单体验** | 主菜单加关卡说明；Level Select 显示关卡子标题 |

---

## 业界视觉引导手法（适配中世纪炼金主题）

选以下五种 — 全部 diegetic，不破坏神秘氛围：

1. **石纹发光 (Stone Emissive)** — 符文/钥匙/relic 用低饱和暖橙 emission，像内嵌炼金光
2. **火光指向 (Torch Beam)** — 一束火把光打在关键道具上（L3 的灯笼揭示楼梯已是此手法）
3. **靠近描边 (Proximity Outline)** — 玩家进入交互范围时亮金色轮廓线，拿起后消失
4. **大师低语 (Idle Whisper)** — 卡住 > 2min 显示一句大师台词，方向性暗示
5. **底座大符号 (Pedestal Symbol)** — L4 的 ○▢△ 必须从院子另一头可见，有脉冲动画

---

## 叙事完整性检查（Beta 前确认）

每关结束必须有大师结语：

| 关卡 | 大师台词 |
|---|---|
| L0 | *"Good. You remember how to see."* |
| L1 | *"Good. You remember how to make."* |
| L2 | *"Good. You remember how to wait."* |
| L3 | *"Good. You remember how to reveal."* |
| L4 | *"Now you remember who you are. The legacy is yours."* |

---

## 注：GameDesignDocument.md 状态

原始 GDD 在工作区被删除，仍可从 git 恢复：
```
git checkout 0bc82b4 -- Assets/_TheAlchemistsLegacy/Docs/GameDesignDocument.md
```
