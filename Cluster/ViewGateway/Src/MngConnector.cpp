#include "stdafx.h"
#include "MngConnector.h"
#include "ViewDef_.h"
#include "MsgFactory.h"
#include "Setting.h"
//#include "HandshakeResponseHandler.h"
#include "ViewMsgDef_Server.h"


MngConnector::MngConnector(Processer<MngConnector>& processer, ByteRecord& br, TimerAxis& timerAxis) :
	ConnectorWithProcess(processer, br, timerAxis)
	, m_dwID(0)
	, m_dwServerID(0)
{
}

bool MngConnector::SendHandshake()
{
	if (m_bIsConnected == false || m_pConnection == NULL)
	{
		WarningLn(_GT("还未连接，稍后发送握手信息！"));
		return false;
	}

	// 观战服与管理服握手
	SMsgView_GM_Handshake sendData;
	sendData.wClientPort = gSetting.m_dwClientPort;
	sendData.wViewPort = gSetting.m_dwViewPort;
	sendData.dwMaxCount = gSetting.m_dwMaxConnection;	// 最多人数
	sendData.dwCurrCount = 0;	// TODO 以后赋值

	this->SendMsg(sendData);

	TraceLn(_GT("发送握手信息! ") << a2utf8(ToString().data()));

	return true;
}


const obuf& MngConnector::GetKeepAliveMsg()
{
	// 心跳检查
	static bool bFlag = true;
	static obuf obufData;
	if (bFlag)
	{
		// 适用于单线程
		SGameMsgHead header;
		header.SrcEndPoint = MSG_ENDPOINT_VIEWGATE;
		header.DestEndPoint = MSG_ENDPOINT_VIEWMNG;
		header.byKeyModule = MSG_MODULEID_VIEW;
		header.byKeyAction = ENUM_MSG_VIEW_KEEPALIVE;

		SMsgView_VM_KeepAlive msg;
		
		obufData.push_back(&header, sizeof(SGameMsgHead));
		obufData.push_back(&msg, sizeof(SMsgView_VM_KeepAlive));
		bFlag = false;
	}

	return obufData;
}

WORD MngConnector::GetAnswerKeepAliveID()
{
	static MsgKey msgKey(MSG_MODULEID_VIEW, ENUM_MSG_VIEW_ANSWER_KEEPALIVE);
	return msgKey.key;
}

//取得目标节点的名称
const char* MngConnector::GetDestPointName()
{
	static const char* szDestPointName = _GT("观战管理服");
	return szDestPointName;
}

void MngConnector::OnDisconnected(IConnection * conn, DWORD reason)
{
	Connector::OnDisconnected(conn, reason);
	ResetID();
}

void MngConnector::OnError(IConnection * conn, DWORD dwErrorCode)
{
	Connector::OnError(conn, dwErrorCode);
	ResetID();
}