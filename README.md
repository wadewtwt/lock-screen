# LockScreen

一个从 0 到 1 学习和实现 Windows 锁屏应用的示例项目。

---

## 项目总览

用 **WPF + .NET 8** 实现的 Windows 锁屏应用，界面风格为**点阵屏（Dot Matrix）复古风**。项目结构精简，适合学习 WPF 自定义控件、MVVM 模式和多显示器管理。

---

## 当前已完成

- `WPF + .NET 8` 项目初始化
- 全屏置顶窗口（无边框、无任务栏图标）
- 点阵屏风格视觉改造（径向光晕 + 点阵网格背景）
- 自定义 5×7 点阵字符渲染控件（`DotMatrixTextBlock`）
- 时间（`HH:mm:ss`）和日期（`yyyy.MM.dd ddd`）显示
- 多显示器全覆盖锁屏
- 基于 `MVVM` 的页面状态管理（CommunityToolkit.Mvvm）
- 回车键退出流程
- 日志文件输出（Serilog）

---

## 如何运行

```powershell
dotnet build .\LockScreen.sln
dotnet run --project .\LockScreen.App
```

当前按 `Enter` 会直接退出锁屏（尚无密码验证）。

---

## 项目结构

```
LockScreen.sln
└── LockScreen.App/
    ├── App.xaml / App.xaml.cs          # 应用入口，多显示器管理
    ├── MainWindow.xaml / .cs           # 主窗口（UI 布局 + 键盘事件）
    ├── Controls/
    │   ├── DotMatrixFont.cs            # 点阵字体数据（5×7 字符位图）
    │   └── DotMatrixTextBlock.cs       # 自定义点阵渲染控件
    ├── ViewModels/
    │   └── MainWindowViewModel.cs      # 时钟更新 + 状态 + 按键处理
    └── Native/
        └── WindowPlacement.cs          # P/Invoke 精确定位窗口到物理坐标
```

---

## 模块说明

### App.xaml.cs — 启动与多显示器管理

- 枚举所有显示器（`System.Windows.Forms.Screen.AllScreens`），**每个屏幕创建一个 `MainWindow`** 实现全覆盖
- 监听 `SystemEvents.DisplaySettingsChanged`，分辨率/显示器变化时自动重建窗口
- `ShutdownMode.OnExplicitShutdown` — 手动控制退出
- 使用 Serilog 写日志到 `logs/lockscreen-日期.log`

### MainWindow.xaml — 纯视觉层

- `WindowStyle="None"` + `Topmost="True"` + `ResizeMode="NoResize"` — 无边框全屏置顶
- 背景：深色底色 `#06090F` + 三个径向渐变光晕 + `DrawingBrush` 点阵网格纹理
- 内容全部由 `DotMatrixTextBlock` 自定义控件渲染，数据绑定到 ViewModel

### DotMatrixFont.cs + DotMatrixTextBlock.cs — 核心创意

**`DotMatrixFont`**：手写 5×7 点阵 bitmap，支持 0-9、A-Z、空格及常用符号。`IsPixelOn(char, x, y)` 查询某坐标是否点亮。

**`DotMatrixTextBlock`**：继承 `FrameworkElement`，完全自绘：
- 支持 `DotSize`、`DotGap`、`CharacterSpacing` 等尺寸属性
- 支持 `OnBrush`（亮点）、`OffBrush`（暗点）、`GlowBrush`（光晕）三层颜色
- `OnRender` 中逐字符、逐像素调用 `DrawEllipse` 绘制

### MainWindowViewModel.cs — MVVM 状态

- `DispatcherTimer` 每 100ms 触发，但只在**秒级变化时**更新 UI（性能优化）
- `[ObservableProperty]` 源生成：`CurrentTime`、`CurrentDate`、`StatusMessage`
- `[RelayCommand]` 源生成：`HandleKeyPressCommand` — 仅响应 Enter 键，调用 `Application.Current.Shutdown()`

### WindowPlacement.cs — Native P/Invoke

- 调用 `user32.dll` 的 `SetWindowPos` 精确定位窗口到对应显示器的物理像素坐标，解决 WPF DPI 缩放的偏移问题

---

## 依赖

| 包 | 版本 | 用途 |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.4.2 | ObservableObject、RelayCommand、属性源生成 |
| `Serilog.Sinks.File` | 7.0.0 | 日志写入文件 |

---

## 如果你有 Vue 和 Java 背景

| WPF | 类比 |
|---|---|
| `App.xaml / App.xaml.cs` | 应用启动入口（main） |
| `MainWindow.xaml` | Vue 页面模板（template） |
| `MainWindowViewModel.cs` | Vue data + methods / Controller |
| `DotMatrixTextBlock.cs` | 自定义 Vue 组件（完全自绘版） |
| `AuthService.cs`（待实现） | 后端 Service 层 |

---

## 当前局限

| 功能 | 现状 |
|---|---|
| 解锁验证 | 无 — 按 Enter 直接退出，无密码校验 |
| `Services/` 目录 | 空目录，AuthService 尚未实现 |
| 开机自启 | 未实现 |
| 退出保护 | 未实现（任务管理器可强杀） |
| Ctrl+Alt+Del 接管 | 不支持（需要 Credential Provider / 驱动层） |

---

## 接下来建议推进的顺序

1. 实现密码验证（本地安全存储，避免明文）
2. 增加退出保护（拦截系统热键、任务管理器）
3. 支持开机自启（注册表 / 任务计划程序）
4. 增加设置页和管理员模式
5. 支持更多点阵字符（小写字母、中文扩展等）

---

## 暂不涉及的复杂内容

- Windows 真正的系统登录界面替换
- `Credential Provider`
- `Ctrl+Alt+Del` 级别的安全按键接管
- 驱动或内核层开发

---

## 开发避坑指南

### ⚠️ WPF + WindowsForms 同时引用时的命名冲突

本项目为了实现**多显示器枚举**，在 `.csproj` 中同时启用了：

```xml
<UseWPF>true</UseWPF>
<UseWindowsForms>true</UseWindowsForms>
```

这导致多个常用类型在 `System.Drawing`（Forms）和 `System.Windows.Media`（WPF）之间产生歧义，编译器会报 `error CS0104: 'Xxx' is an ambiguous reference`。

#### 受影响的类型及解决方法

| 有歧义的类型 | 来源冲突 | 解决方式 |
|---|---|---|
| `Brush` | `System.Drawing.Brush` vs `System.Windows.Media.Brush` | 使用全限定名 `System.Windows.Media.Brush` |
| `Brushes` | `System.Drawing.Brushes` vs `System.Windows.Media.Brushes` | 使用全限定名 `System.Windows.Media.Brushes.Transparent` |
| `Color` | `System.Drawing.Color` vs `System.Windows.Media.Color` | 用 `using WpfColor = System.Windows.Media.Color;` 别名 |
| `Colors` | `System.Drawing.Color`（静态）vs `System.Windows.Media.Colors` | 使用全限定名 `System.Windows.Media.Colors.Transparent` |
| `Point` | `System.Drawing.Point` vs `System.Windows.Point` | 使用全限定名 `new System.Windows.Point(x, y)` |

#### 推荐的处理模板

在 ViewModel 或其他 C# 文件顶部，针对频繁使用的 WPF 类型统一加别名：

```csharp
using WpfColor = System.Windows.Media.Color;
// 对于 Brush / Brushes / Colors / Point，直接用全限定名更清晰
// 避免用 using static 引入 System.Windows.Media，会和 Forms 冲突更严重
```

对于字段/属性声明，不能用别名的地方直接写全限定名：

```csharp
// ✅ 正确
private System.Windows.Media.Brush myBrush = System.Windows.Media.Brushes.Transparent;

// ❌ 错误（歧义，编译失败）
private Brush myBrush = Brushes.Transparent;
```

#### 根本原因

`[UseWindowsForms]` 会隐式引入 `System.Drawing`，而 `[UseWPF]` 引入 `System.Windows.Media`，两个命名空间里有大量同名类。C# implicit usings 不会自动处理这种冲突。

#### 更彻底的替代方案

如果不想处处写全限定名，可以**把多显示器逻辑单独抽到一个只引用 WindowsForms 的项目**，主 WPF 项目通过接口调用。但对当前项目规模来说，直接用全限定名更简单。

---

### ⚠️ XAML 中 Style 重复设置会报错

在 WPF 中，不能同时在元素属性和子元素中设置同一个属性：

```xml
<!-- ❌ 错误：Style 被设置了两次 -->
<Button Style="{StaticResource ThemeBtn}">
    <Button.Style>
        <Style ...> ... </Style>
    </Button.Style>
</Button>

<!-- ✅ 正确：只用 Button.Style 子元素，在 Style 里通过 BasedOn 继承 -->
<Button>
    <Button.Style>
        <Style TargetType="Button" BasedOn="{StaticResource ThemeBtn}">
            <Style.Triggers> ... </Style.Triggers>
        </Style>
    </Button.Style>
</Button>
```

`BasedOn="{StaticResource ThemeBtn}"` 可以继承基础样式，同时添加 DataTrigger 等扩展逻辑。

---

### ⚠️ WPF TextBox 没有 LetterSpacing 属性

`LetterSpacing` 是 WinUI 3 / CSS 的属性，**WPF 的 `TextBlock` 没有这个属性**。

```xml
<!-- ❌ 在 WPF 中会报编译错误 -->
<TextBlock LetterSpacing="3" ... />

<!-- ✅ WPF 的替代方案：用 Typography 或者接受无字间距 -->
<TextBlock Typography.Kerning="False" ... />
```

WPF 没有原生字间距控件，如果需要精确控制，只能自己用 `ItemsControl` 拆分字符或自定义渲染。
