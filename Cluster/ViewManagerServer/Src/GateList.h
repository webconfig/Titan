#pragma once
#include "UserList.h"

class GateUser;
struct SMsgView_MV_GatesInfo;

class GateList : public UserList<GateUser, GateID>, public Singleton<GateList>
{
public:
	GateList();
	virtual ~GateList() {};

	int		GetReadyCount();

	void	BuildGatesInfo(SMsgView_MV_GatesInfo& gatesInfo);

	//************************************
	// Returns:   GateID
	// Qualifier: 查找负载最轻的Gate，返回其ID；如果找不到，则返回0 (按照“剩余承载能力”来比较)
	// Parameter: GateID wExceptID	要排除的GateID
	//************************************
	GateID	GetIdleGate(GateID wExceptID = 0);

protected:
	virtual void OnDelUser(ISessionUser<GateID>* pUser) {};

	//注册这里要处理的所有消息，到m_Processer中
	virtual void RegisterHandlers();

	virtual GateID GetMaxID() { return MAX_GATEWAY_COUNT; }

};
extern GateList& gGateList;