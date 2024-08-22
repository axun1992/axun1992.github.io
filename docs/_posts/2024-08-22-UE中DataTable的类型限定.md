---
layout: post
title:  "UE中DataTable的类型限定"
tags: ue
---
在UE中不同表结构的数据表在类型上都是`UDataTable`，数据行引用的类型都是`FDataTableRowHandle`。如果定义了相应字段，想在蓝图中赋值时，所有的表资产都会出现在备选列表中。
有没有办法可以限制表的类型呢？当然是可以的，以UE5.1为例：  
<!--more-->
分为两种情况：1、变量是数据表；2、变量是数据行。  
假设现在在模块`MyModule`中有一个数据行定义：  
```c++
USTRUCT(BlueprintType)
struct FZoneRow : public FTableRowBase
```
并以它为表结构去创建数据表资产，那么可以通过在`UPROPERTY`中以特定的meta标签来约束类型。  
# 变量是数据表  
```c++
UPROPERTY(meta = (RequiredAssetDataTags="RowStructure=/Script/MyModule.ZoneRow"))
UDataTable DT_Zone;
```

# 变量是数据行
```c++
UPROPERTY(BlueprintReadWrite, EditAnywhere, meta = (RowType = "ZoneRow"))
FDataTableRowHandle ParentZone;
```

---
在meta中添加如上规则的标签后，在蓝图面板上去选择数据表资产时，就只有FZoneRow类型的数据表会显示出来了。  
两种情况都要注意，在meta的标签中，表结构定义的F要去除。