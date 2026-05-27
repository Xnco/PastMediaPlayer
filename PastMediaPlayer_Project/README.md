# PastMediaPlayer - 媒体播放器项目

## 项目概述

PastMediaPlayer是一个基于C# WPF开发的轻量级媒体播放器应用，支持MP4视频文件的播放和管理。

## 技术栈

- **框架**: .NET Core 3.1
- **UI技术**: WPF (Windows Presentation Foundation)
- **目标平台**: Windows桌面应用
- **构建配置**: Debug, Release, Shipping

## 项目结构

```
PastMediaPlayer_Project/
├── App.xaml/.cs              # 应用入口点
├── MainWindow.xaml/.cs        # 主窗口界面
├── LocalInfo.cs               # 本地配置管理
├── MediaControl/              # 媒体控制组件
│   ├── FolderTree.cs          # 文件夹树数据结构
│   ├── FolderTreeCtrl.xaml/.cs # 文件夹树控件
│   ├── MediaCtrl.xaml/.cs     # 媒体播放控件
│   ├── MySlider.xaml/.cs      # 自定义滑块控件
│   └── TreeViewHeader.xaml/.cs # 树视图头部控件
├── Images/                    # 资源图
├── bin/                       # 编译输出目录
└── PastMediaPlayer_Project.csproj # 项目配置文件
```

## 核心功能

### 媒体播放
- MP4视频文件播放支持
- 播放/暂停控制
- 进度条拖拽定位
- 音量控制
- 自动播放功能

### 文件管理
- 文件夹树形导航
- 支持拖拽打开文件夹
- 右键菜单播放
- 路径记忆功能（保存在用户文档目录）

### 快捷键支持
- **右方向键**: 快进5秒
- **左方向键**: 快退5秒

## 界面布局

应用采用左右分栏设计：
- **左侧**: 媒体播放区域
- **右侧**: 文件夹树导航区域
- 支持窗口大小调整和最小尺寸限制

## 快速开始

### 环境要求
- .NET Core 3.1 SDK
- Visual Studio 2019或更高版本

### 编译运行
```bash
# 克隆项目
git clone <repository-url>
cd PastMediaPlayer_Project

# 编译项目
dotnet build

# 运行应用
dotnet run
```

### 使用方法
1. 启动应用后，默认会加载上次打开的文件夹路径
2. 可以通过拖拽文件夹到窗口来切换目录
3. 在右侧文件夹树中浏览和选择视频文件
4. 使用界面控件或键盘快捷键控制播放

## 配置说明

应用配置保存在用户文档目录：
`%USERPROFILE%\Documents\PastMediaPlayer\config.txt`

该文件记录最后一次打开的文件夹路径。

## 开发说明

### 主要组件

#### FolderTree
- 实现文件夹树形数据结构
- 支持递归文件搜索
- 支持INotifyPropertyChanged接口

#### MediaCtrl
- WPF MediaElement封装
- 播放状态管理
- 定时器更新播放进度
- URI缓存机制

#### FolderTreeCtrl
- TreeView控件封装
- 动态树形结构生成
- 右键菜单支持
- 事件委托机制

### 扩展建议
- 支持更多视频格式（AVI, MKV等）
- 添加播放列表功能
- 实现字幕支持
- 添加截图和录制功能

## 许可证

MIT License

## 作者

mx