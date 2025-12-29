# TaskBarHideSwitch

一个极简的 Windows 任务栏自动隐藏切换工具。

## 功能

- **双击托盘图标**：快速切换任务栏自动隐藏开/关
- **右键菜单**：
  - 开机启动（可勾选）
  - 退出程序
- **状态提示**：鼠标悬停显示当前任务栏状态

## 安装

从 [Releases](https://github.com/1PercentSync/TaskBarHideSwitch/releases) 下载最新版本的 `TaskBarHideSwitch-vX.X.X-win-x64.exe`，双击运行即可。

> 程序为单文件自包含版本，无需安装 .NET 运行时。

## 构建

```bash
# 克隆仓库
git clone https://github.com/1PercentSync/TaskBarHideSwitch.git
cd TaskBarHideSwitch

# 构建
dotnet build

# 发布单文件版本
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 要求

- Windows 10/11
- 构建需要 .NET 10 SDK

## 许可

MIT
