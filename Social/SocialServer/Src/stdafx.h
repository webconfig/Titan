
// stdafx.h : 标准系统包含文件的包含文件，
// 或是经常使用但不常更改的
// 特定于项目的包含文件

#pragma once

#ifndef _SECURE_ATL
#define _SECURE_ATL 1
#endif

#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN            // 从 Windows 头中排除极少使用的资料
#endif

#include "targetver.h"

#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS      // 某些 CString 构造函数将是显式的

// 关闭 MFC 对某些常见但经常可放心忽略的警告消息的隐藏
#define _AFX_ALL_WARNINGS

#include <afxwin.h>         // MFC 核心组件和标准组件
#include <afxext.h>         // MFC 扩展


#include <afxdisp.h>        // MFC 自动化类



#ifndef _AFX_NO_OLE_SUPPORT
#include <afxdtctl.h>           // MFC 对 Internet Explorer 4 公共控件的支持
#endif
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>             // MFC 对 Windows 公共控件的支持
#endif // _AFX_NO_AFXCMN_SUPPORT

#include <afxcontrolbars.h>     // 功能区和控件条的 MFC 支持


///////////////////////////////////////////////////////////////////////////////////////////
#define USE_SYSTEM_MEMORY

#include "Base.h"
#pragma comment(lib, MAKE_DLL_LIB_NAME_BY_ARCH(Base))
using namespace rkt;
#include "GameDef.h"

#include "ICenterServerProtocol.h"
#include "ICenterServerConnector.h"
using namespace CenterServer;


#ifdef _UNICODE
#if defined _M_IX86
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_X64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='amd64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#else
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif
#endif

//////////////////////////////////////////////////////////////////////////////////////////////
#define BEGIN_WINDOW()      CWnd  *pTempWnd = NULL;

#define ENABLE_WINDOW(itemid, flag)											\
    pTempWnd = GetDlgItem(itemid);											\
    if(pTempWnd != NULL)													\
{																		\
    pTempWnd->EnableWindow(flag);										\
}

#define SET_WINDOW_NAME(itemid, name)                                       \
    pTempWnd = GetDlgItem(itemid);                                          \
    if (pTempWnd != NULL)                                                   \
{                                                                       \
    pTempWnd->SetWindowText(name);                                      \
}                                                                       \


#define ENABLE_WINDOW_EX(dialog, itemid, flag)								\
    pTempWnd = dialog->GetDlgItem(itemid);									\
    if(pTempWnd != NULL)													\
{																		\
    pTempWnd->EnableWindow(flag);										\
}


#define VISIBLE_WINDOW(itemid, flag)										\
    pTempWnd = GetDlgItem(itemid);											\
    if(pTempWnd != NULL)													\
{																		\
    if(flag)															\
{																	\
    pTempWnd->ShowWindow(SW_SHOW);									\
}																	\
        else																\
{																	\
    pTempWnd->ShowWindow(SW_HIDE);									\
}																	\
}


#define SAFE_DELETEARRAY(data) if(data) { delete[] (data); data=NULL;};
