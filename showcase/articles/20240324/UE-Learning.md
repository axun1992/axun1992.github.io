[toc]

# UE学习记录

## Timeline
`Timeline`节点是一个自动随时间流逝对曲线求值的异步执行块。
它内部包含着曲线，曲线横轴是时间，纵轴是输出值。
**在蓝图中，`Timeline`只可用于`Event`，不可用于`Function`内部。（可能是因为它是异步的）**
当在`EventGraph`中创建了一个`Timeline`节点时，将自动生成一个`Timeline`变量。此变量代表节点所用的`Timeline`实例，无法直接删除。双击变量可打开编辑内部的曲线。
一般用法上，连接节点的`Update`输出引脚，并获取其曲线输出引脚。就可以得到基于时间增加的值用于其它用途，例如用来做数据插值的比例。（节点内可创建多条曲线，每条曲线会生成一个输出引脚）
选中曲线的关键点可以设置点两侧线段的连接方式，一般选`Auto`即可形成较流畅的曲线。

## 核心重定向
因路径改变、文件重命名等原因，导致对资源或代码的引用被破坏。可以通过`核心重定向`功能修复引用关系，并随后修复重定向以去除重定向配置。
配置在项目的`DefaultEngine.ini`文件中，
在`[CoreRedirects]`节中添加，例如下面是一个把旧项目资源迁移到新项目之后的配置：
```ini
[CoreRedirects]
+PackageRedirects=(OldName=“/Script/ChuanSongJiHua”,NewName="/Script/HuaGongChangProj")
+ClassRedirects=(OldName="/Script/ChuanSongJiHua",NewName="/Script/HuaGongChangProj", MatchSubstring=true)
+StructRedirects=(OldName="/Script/ChuanSongJiHua",NewName="/Script/HuaGongChangProj", MatchSubstring=true)
+EnumRedirects=(OldName="/Script/ChuanSongJiHua",NewName="/Script/HuaGongChangProj", MatchSubstring=true)
```
注意：对于类、结构体、枚举的重定向时，可以设置`MatchSubstring=true`，这样会重定向目录下所有类型，不需要单独为每个类型配置。
加上这样的配置之后，之前显示为被破坏的蓝图现在应该可以正常打开了，然后重新保存之后就会修复重定向，不再需要在ini中的配置。
一个自动化的全部重新配置bat脚本可能是这样：
```shell
set UEPath="D:\Program Files\Epic Games\UE_4.27\Engine\Binaries\Win64\UE4Editor.exe"
set ProjPath=F:\work\led_huagongchang\HuaGongChangProj
%UEPath% "%ProjPath%\HuaGongChangProj.uproject" -run=ResavePackages -projectonly
```
`PackageRedirects`仅对资源的重定向起作用。蓝图类或枚举需要用对应的重定向类型。
如果是Plugin，例如叫A，则需要在Plugin的Config目录下建立`DefaultA.ini`文件，并如上述般进行配置。
关于路径：正确的路径可以通过右键资源,点击复制引用来得到。
名称规则：
|类别|规则|
|:-|:-|
|蓝图|`/Game/[相对于Content的路径]`|
|源代码|`/Script/[项目名].[类名]`|
|插件蓝图|`/[插件名]/[相对于插件Content的路径]`|
|插件源代码|`/Script/[插件名].[类名]`|

## 蓝图的多态
两个蓝图之间为继承关系的话，经测试有这些结论：
- 子类重载的函数具有虚函数的行为。
- 对于预设的Event，子类的Event会覆盖父类的同名Event。
- 对于用户自定义Event，子类不能再重新定义，可以直接使用父类Event，具有虚函数行为。