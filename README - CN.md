# **MidiBard 2**

**如果需要支持，请加入我们的 ✅[Discord 服务器](https://discord.gg/sMUACqqKzF)!**
**如果不方便使用Discord，请加入QQ群：260985966（由原作者akira0245创建）**

MidiBard是基于[卫月框架](https://bbs.tggfl.com/topic/32/dalamud-%E5%8D%AB%E6%9C%88%E6%A1%86%E6%9E%B6)的强大且易用的诗人midi演奏插件。目前版本兼容国服及国际服。

原作者[akira0245](https://github.com/akira0245/MidiBard)贡献了绝大部分代码。游戏6.1更新以后目前由[Ori](https://github.com/reckhou/MidiBard) 以及其他开发者提供更新和维护。



# 主要特性
* 无需繁琐的键位配置，打开即用
* 使用合奏助手进行一键高精度合奏
* 音轨可视化，直观查看乐谱效果
* 毫秒精度的MIDI回放和零按键延迟，最大程度还原乐曲细节
* 超出音域音符的自适应功能，节省适配midi文件的时间
* 内置播放列表并支持搜索，一次导入演奏你收藏的所有乐曲
* 通过插件界面/文本指令一键切换乐器，或随乐曲播放自动切换对应的乐器
* 可选择任意个音轨同时演奏或合奏，允许你在任何时候切换/静音/独奏/移调任意音轨
* 良好的MIDI设备支持，自动扫描可用MIDI设备并连接，最佳的MIDI键盘演奏体验
* 为每个音轨指定不同的电吉他音色并单人演奏！


# 安装方法
> MidiBard 2需要[卫月框架](https://bbs.tggfl.com/topic/32/dalamud-%E5%8D%AB%E6%9C%88%E6%A1%86%E6%9E%B6)，如未安装请参考[原帖](https://bbs.tggfl.com/topic/32/dalamud-%E5%8D%AB%E6%9C%88%E6%A1%86%E6%9E%B6)安装后继续。

正确安装卫月框架并注入后在游戏聊天框中输入`/xlsettings`打开Dalamud 设置窗口，复制该源  
`https://raw.githubusercontent.com/reckhou/DalamudPlugins-Ori/api6/pluginmaster.json` 并将其添加到插件仓库  


成功添加后在`/xlplugins`中搜索MidiBard 2并安装即可。

 ![DD5JTJH.png](https://i.imgur.com/DD5JTJH.png)](https://i.imgur.com/DD5JTJH.png)
![enter image description here](https://i.imgur.com/4BH682e.png)

# 建议配置
新手建议使用以下配置：
![image](https://i.imgur.com/wFsYNDy.png)


# 使用FAQ
* **如何开始使用MIDIBARD演奏？**  
MIDIBARD窗口默认在角色进入演奏模式后自动弹出。点击窗口左上角的“+”按钮来将乐曲文件导入到播放列表。仅支持.mid格式的乐曲。导入时按Ctrl或Shift可以选择多个文件一同导入。双击播放列表中要演奏的乐曲后点击播放按钮开始演奏。

* **如何使用MIDIBARD进行多人合奏？**  
MIDIBARD使用游戏中的合奏助手来完成合奏，请在合奏时打开游戏的节拍器窗口。合奏前在播放列表中双击要合奏的乐曲，播放器下方会出现可供演奏的所有音轨，请为每位合奏成员分别选择其需要演奏的音轨。选择音轨后队长点击节拍器窗口的“合奏准备确认”按钮，并确保合奏准备确认窗口中已勾选“使用合奏助手”选项后点击开始即可开始合奏。  
注：节拍器前两小节为准备时间，从第1小节开始会正式开始合奏。考虑到不同使用环境乐曲加载速度可能不一致，为了避免切换乐曲导致的不同步，在乐曲结束时合奏会自动停止。

* **如何让MIDIBARD为不同乐曲自动切换音调和乐器？**  
方法一（推荐）：打开`BMP Track Name Compatible`选项，将MIDI音轨命名为游戏中乐器的名字，并且在名称后使用+1，-1进行移调。乐器的名称见本文档附录。该方法简单直接，目前[BMP 曲库](https://bmp.trotlinebeercan.com/)中所有曲目都使用该标准。
方法二（原方案）：在导入前把要指定乐器和移调的乐曲文件名前加入“#<乐器名><移调的半音数量>#”。例如：原乐曲文件名为“demo.mid”，将其重命名为“#中提琴+12#demo.mid”可在演奏到该乐曲时自动切换到中提琴并升调1个八度演奏。将其重命名为“#长笛-24#demo.mid”可在演奏到该乐曲时切换到长笛并降调2个八度演奏。  
注：可以只添加#+12#或#竖琴#或#harp#，也会有对应的升降调或切换乐器效果。

* **如何为MIDIBARD配置外部Midi输入（如虚拟Midi接口或Midi键盘）？**  
在“输入设备”下拉菜单中选择你的Midi设备，窗口顶端出现“正在监听Midi输入”信息后即可使用外部输入。

* **后台演奏时有轻微卡顿不流畅怎么办？**  
在游戏内 *系统设置→显示设置→帧数限制* 中取消勾选 *“程序在游戏窗口处于非激活状态时限制帧数”* 并应用设置。

* **如何获得技术支持？**
如果方便使用Discord，可以加入我们的 ✅[Discord 服务器](https://discord.gg/sMUACqqKzF)。
如果不方便使用Discord，请加入QQ群：260985966（由原作者akira0245创建）

加入Discord服务器后，进入 `role-assign` 频道并点击图中这条信息的“中”字按钮。之后就可以看到`MidiBard中文支持`频道。

![enter image description here](https://i.imgur.com/VK8Iz0S.png)
![enter image description here](https://i.imgur.com/6mEBWmY.png)

# 小队聊天命令

在小队聊天中输入以下命令，以控制同一小队中所有安装了MidiBard插件的诗人。

* **switchto [序号]**

切换到播放列表上的第X首曲目。例如， `switchto 2` 切换到第二首。
* **close**

停止演奏并退出演奏模式。

*  **reloadplaylist**

重新加载播放列表。在改动播放列表后可以使用此命令将其同步到所有同一电脑上的客户端。

# BMP标准 - 根据音轨名称自动切换乐器

以下是所有游戏中目前支持乐器的名称：

>Piano
>Harp
>Fiddle
>Lute
>Fife
>Flute
>Oboe
>Panpipes
>Clarinet
>Trumpet
>Saxophone
>Trombone
>Horn
>Tuba
>Violin
>Viola
>Cello
>Double Bass
>Timpani
>ElectricGuitarClean 
>ElectricGuitarMuted 
>ElectricGuitarOverdriven
>ElectricGuitarPowerChords 
>ElectricGuitarSpecial

在乐器名称之后加入`+X` or `-X`进行移调。例如，`Piano-1`表示将钢琴音轨下降一个八度进行演奏。对于使用`MuseScore`等作曲软件的用户，这么做可以使得软件中的播放效果和游戏中保持一致。

**Octave Ranges**

>Piano-1: C4-C7 
>Harp: C3-C6 
>Fiddle+1: C2-C5 
>Lute+1: C2-C5
>Fife-2 C5-C8 
>Flute-1: C4-C7 
>Oboe-1: C4-C7 
>Panpipes-1: C4-C7 
>Clarinet: C3-C6
>Trumpet: C3-C6 
>Saxophone: C3-C6
>Trombone+1: C2-C5
>Horn+1: C2-C5
>Tuba+1: C1-C4
>Violin: C3-C6
>Viola: C3-C6
>Cello+1: C2-C5
>Double Bass+2: C1-C4
>Timpani+1: C2-C5
>All Guitars+1: C2-C5


---
> 本项目遵循 GNU Affero General Public License v3.0 协议开源。  
