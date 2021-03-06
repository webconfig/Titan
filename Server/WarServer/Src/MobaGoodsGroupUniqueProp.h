/*******************************************************************
** 文件名: e:\CoGame\SchemeEngine\include\MobaGoodsProp.h
** 版  权: (C) 深圳冰川网络技术有限公司(www.weimingtech.com)
** 创建人: 秦其勇
** 日  期: 2015/3/11
** 版  本: 1.0
** 描  述: 战场属性管理信息 组唯一属性
** 应  用: 
**************************** 修改记录 ******************************/
#pragma once

#include "stdafx.h"
#include "IEntity.h"
#include "MobaGoodsDef.h"
#include "MobaGoodsProp.h"

class CMobaGoodsGroupUniqueProp: public CMobaGoodsProp
{
public:
	CMobaGoodsGroupUniqueProp();

	~CMobaGoodsGroupUniqueProp();

	virtual bool				AddEffect(int nSlotID, int nEffectID, UID OperatorUID, UID TargetUID);

	virtual bool				RemoveEffect(int nSlotID, int nEffectID);

	virtual void				RemoveAllEffect();

	virtual void				Release();

private:

	// 组ID 对应的效果结构
	typedef map<int,SPropInfo> T_MAP_GROUP_EFFECT_ALL;

	T_MAP_GROUP_EFFECT_ALL m_EffectAll;
};