# LockScreen

一个从 0 到 1 学习和实现 Windows 锁屏应用的示例项目。

## 当前阶段

当前已经完成：

- `WPF + .NET 8` 项目初始化
- 全屏置顶窗口原型
- 点阵屏风格视觉改造
- 自定义 5x7 点阵字符渲染控件
- 时间和日期显示
- 多显示器全覆盖锁屏
- 基于 `MVVM` 的页面状态管理
- 回车键退出流程
- 日志文件输出

## 如何运行

```powershell
dotnet build .\LockScreen.sln
dotnet run --project .\LockScreen.App
```

当前按 `Enter` 会直接退出锁屏。

## 如果你有 Vue 和 Java 背景

可以先这样理解：

- `App.xaml` / `App.xaml.cs`：类似应用启动入口
- `MainWindow.xaml`：类似 Vue 里的页面模板
- `MainWindowViewModel.cs`：类似页面状态 + 交互逻辑
- `AuthService.cs`：类似后端里的 service 层

## 接下来我们会做什么

下一阶段建议按这个顺序推进：

1. 把演示密码改为本地安全存储
2. 增加退出保护
3. 支持开机自启
4. 支持多显示器全覆盖
5. 增加设置页和管理员模式

## 现在先不用碰的内容

这些内容复杂度更高，先不碰：

- Windows 真正的系统登录界面替换
- `Credential Provider`
- `Ctrl+Alt+Del` 级别的安全按键接管
- 驱动或内核层开发
