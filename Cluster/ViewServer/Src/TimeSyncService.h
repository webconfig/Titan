#pragma once
#include "TimeSyncClient.h"
#include "ITimeSyncService.h"


class TimeSyncService : public Singleton<TimeSyncService>, public ITimeSyncService
{
public:
	virtual ~TimeSyncService();

	virtual	DWORD GetTick();

	// 取服务器的 从1970年1月1日00:00:00到现在总共的秒数
	virtual	inline STime GetTime() { return m_pSyncClient->GetTime(); }

	virtual	bool Start(const char * szIP = "127.0.0.1", int nUDPPort = 6600, int nTCPPort = 6601);

	virtual	void Release();

private:
	TimeSyncClient*		m_pSyncClient;
};
