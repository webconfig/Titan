-------------------------------------------------------------------
-- ��  Ȩ:	(C) ���ڱ������缼�����޹�˾ 2008 - All Rights Reserved
-- ������:	��ҫ��
-- ��  ��:	3/31/2017
-- ��  ��:	1.0
-- ��  ��:	���һ������
-------------------------------------------------------------------

require "TaskFrameActorData"

-------------------------------------------------------------------
-- ���һ������
-------------------------------------------------------------------
Task_10_CompleteGame = TaskFrameActorData:new
{
	tFinishCondition = { ConditionCheckNewPlayerGuideStep:new{count=4} },	
}