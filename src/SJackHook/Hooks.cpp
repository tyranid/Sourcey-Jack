/***********************************************************************

Sourcey Jack - A simple SOCKSifying application for Windows
Copyright (C) 2011 James Forshaw

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

***********************************************************************/

#include "stdafx.h"
#include <detours.h>
#include <map>

#pragma comment(lib, "ws2_32.lib")

struct SocketConnection
{
	bool ipv6;
	bool nonblocking;
};

typedef std::map<SOCKET, SocketConnection> SocketConnectionMap;

static CRITICAL_SECTION mapLock;

static SocketConnectionMap socketMap;
static sockaddr_in serverAddr = {0};

SOCKET (__stdcall * Real_WSASocket)(
  __in  int af,
  __in  int type,
  __in  int protocol,
  __in  LPWSAPROTOCOL_INFO lpProtocolInfo,
  __in  GROUP g,
  __in  DWORD dwFlags
) = WSASocket;

int (__stdcall * Real_connect)(SOCKET s,
                               CONST sockaddr* addr,
                               int addrlen)
    = connect;

int (__stdcall * Real_WSAIoctl)(
  __in   SOCKET s,
  __in   DWORD dwIoControlCode,
  __in   LPVOID lpvInBuffer,
  __in   DWORD cbInBuffer,
  __out  LPVOID lpvOutBuffer,
  __in   DWORD cbOutBuffer,
  __out  LPDWORD lpcbBytesReturned,
  __in   LPWSAOVERLAPPED lpOverlapped,
  __in   LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
) = WSAIoctl;

int (__stdcall * Real_WSAConnect)(
  __in   SOCKET s,
  __in   const struct sockaddr *name,
  __in   int namelen,
  __in   LPWSABUF lpCallerData,
  __out  LPWSABUF lpCalleeData,
  __in   LPQOS lpSQOS,
  __in   LPQOS lpGQOS
) = WSAConnect;

enum ConnectEvent
{
	ReturnSuccess,
	ContinueConnect,
	ReturnError	
};

bool ConnectSocks(SOCKET s, const sockaddr* addr, int addrlen, bool ipv6)
{
	bool ret = false;

	OutputDebugString(L"In connectsocks");
	if(addrlen == sizeof(sockaddr_in))
	{
		const sockaddr_in* in = (const sockaddr_in*)addr;

		unsigned char req[9] = { 4, 1 };
		memcpy(&req[2], &in->sin_port, sizeof(in->sin_port));
		memcpy(&req[4], &in->sin_addr.S_un.S_addr, sizeof(&in->sin_addr.S_un.S_addr));

		// Build SOCKS address
		if(Real_connect(s, (const sockaddr*) &serverAddr, sizeof(serverAddr)) == 0)
		{
			if(send(s, (const char*)req, sizeof(req), 0) == sizeof(req))
			{
				char resp[8];
				if((recv(s, resp, sizeof(resp), 0) == sizeof(resp)) && (resp[1] == 0x5a))
				{
					ret = true;
				}
				else
				{
					OutputDebugString(L"Failed in receive");
				}
			}
			else
			{
				OutputDebugString(L"Failed to send data");
			}
		}		
		else
		{
			OutputDebugString(L"Couldn't connect");
		}
	}
	else
	{
		OutputDebugString(L"Don't support IPv6 atm");		
	}		

	return ret;
}

ConnectEvent DoConnect(SOCKET s, const sockaddr* addr, int addrlen)
{
	ConnectEvent ret = ContinueConnect;

	EnterCriticalSection(&mapLock);

	auto it = socketMap.find(s);

	if(it != socketMap.end())
	{
		OutputDebugString(L"In DoConnect");
		if(!ConnectSocks(s, addr, addrlen, it->second.ipv6))
		{
			ret = ReturnError;
		}
		else
		{
			ret = ReturnSuccess;
			// Reenable blocking mode if necessary
			if(it->second.nonblocking)
			{
				unsigned long block = 1;
				DWORD bytesReturned = 0;

				OutputDebugString(L"Reenabling non-blocking\n");
				
				if(Real_WSAIoctl(s, FIONBIO, &block, sizeof(block), NULL, 0, &bytesReturned, NULL, NULL) != 0)
				{
					OutputDebugString(L"IoCtl failed");
				}

				WSASetLastError(0);
			}	
		}
	
		socketMap.erase(it);
	}

	LeaveCriticalSection(&mapLock);

	return ret;
}

int __stdcall Mine_connect(SOCKET s,
                           sockaddr* addr,
                           int addrlen)
{    
	OutputDebugString(L"connect\n");

	ConnectEvent e = DoConnect(s, addr, addrlen);

	if(e == ContinueConnect)
	{
		return Real_connect(s, addr, addrlen);    
	}
	else if(e == ReturnSuccess)
	{
		return 0;
	}
	else
	{
		return SOCKET_ERROR;
	}
}

int __stdcall Mine_WSAIoctl(
  __in   SOCKET s,
  __in   DWORD dwIoControlCode,
  __in   LPVOID lpvInBuffer,
  __in   DWORD cbInBuffer,
  __out  LPVOID lpvOutBuffer,
  __in   DWORD cbOutBuffer,
  __out  LPDWORD lpcbBytesReturned,
  __in   LPWSAOVERLAPPED lpOverlapped,
  __in   LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
)
{
	bool doIoctl = true;

	OutputDebugString(L"ioctl\n");
	if(dwIoControlCode == FIONBIO)
	{
		EnterCriticalSection(&mapLock);
		auto it = socketMap.find(s);

		if(it != socketMap.end())		
		{			
			unsigned long val;

			if((cbInBuffer == sizeof(val)) && (lpvInBuffer != nullptr))
			{
				memcpy(&val, lpvInBuffer, sizeof(val));				
				if(val)
				{
					OutputDebugString(L"Setting Socket NonBlocking");
					it->second.nonblocking = true;
					doIoctl = false;
				}
			}
		}

		LeaveCriticalSection(&mapLock);
	}

	if(doIoctl)
	{
		return Real_WSAIoctl(s, dwIoControlCode, lpvInBuffer, cbInBuffer, 
			lpvOutBuffer, cbOutBuffer, lpcbBytesReturned, lpOverlapped, lpCompletionRoutine);
	}
	else
	{
		return 0;
	}
}

SOCKET __stdcall Mine_WSASocket(
  __in  int af,
  __in  int type,
  __in  int protocol,
  __in  LPWSAPROTOCOL_INFO lpProtocolInfo,
  __in  GROUP g,
  __in  DWORD dwFlags
)
{
	OutputDebugString(L"socket\n");	
	SOCKET s = Real_WSASocket(af, type, protocol, lpProtocolInfo, g, dwFlags);

	if(s != INVALID_SOCKET)
	{
		if(((af == AF_INET) || (af == AF_INET6)) && (type == SOCK_STREAM))
		{			
			SocketConnection conn;

			OutputDebugString(L"Capturing socket\n");
			EnterCriticalSection(&mapLock);			
			conn.nonblocking = false;
			conn.ipv6 = (af == AF_INET6);

			socketMap[s] = conn;

			LeaveCriticalSection(&mapLock);
		}
	}

	return s;
}

int __stdcall Mine_WSAConnect(
  __in   SOCKET s,
  __in   const struct sockaddr *name,
  __in   int namelen,
  __in   LPWSABUF lpCallerData,
  __out  LPWSABUF lpCalleeData,
  __in   LPQOS lpSQOS,
  __in   LPQOS lpGQOS
)
{
	OutputDebugString(L"WSAConnect\n");

	ConnectEvent e = DoConnect(s, name, namelen);

	if(e == ContinueConnect)
	{
		return Real_WSAConnect(s, name, namelen, lpCallerData, lpCalleeData, lpSQOS, lpGQOS);
	}
	else if(e == ReturnSuccess)
	{
		return 0;
	}
	else
	{
		return SOCKET_ERROR;
	}	
}

static VOID Decode(PBYTE pbCode, LONG nInst)
{
    PBYTE pbSrc = pbCode;
    PBYTE pbEnd;
    PBYTE pbTarget;
    for (LONG n = 0; n < nInst; n++) {
        pbTarget = NULL;
        pbEnd = (PBYTE)DetourCopyInstruction(NULL, (PVOID)pbSrc, (PVOID*)&pbTarget);       
        pbSrc = pbEnd;

        if (pbTarget != NULL) {
            break;
        }
    }
}

VOID DetAttach(PVOID *ppvReal, PVOID pvMine, PCHAR psz)
{
    DetourAttach(ppvReal, pvMine);
}

VOID DetDetach(PVOID *ppvReal, PVOID pvMine, PCHAR psz)
{
    LONG l = DetourDetach(ppvReal, pvMine);
    if (l != 0) {
#if 0
        Syelog(SYELOG_SEVERITY_NOTICE,
               "Detach failed: `%s': error %d\n", DetRealName(psz), l);
#else
        (void)psz;
#endif
    }
}

#define ATTACH(x,y)   DetAttach(x,y,#x)
#define DETACH(x,y)   DetDetach(x,y,#x)

void UpdateServer()
{
	HANDLE hMap = OpenFileMapping(FILE_MAP_READ, FALSE, L"SJackConfig");
	if(hMap != NULL)
	{
		LPVOID pData = MapViewOfFile(hMap, FILE_MAP_READ, 0, 0, 4096);
		if(pData != NULL)
		{
			serverAddr.sin_family = AF_INET;
			serverAddr.sin_addr.S_un.S_addr = *(unsigned int*)pData;
			serverAddr.sin_port = htons(*((unsigned short*)pData+2));
			UnmapViewOfFile(pData);
		}

		CloseHandle(hMap);
	}
}

LONG AttachDetours(VOID)
{
	UpdateServer();
	InitializeCriticalSection(&mapLock);

    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());

    // For this many APIs, we'll ignore one or two can't be detoured.
    DetourSetIgnoreTooSmall(TRUE);

	ATTACH(&(PVOID&)Real_connect, Mine_connect);
	ATTACH(&(PVOID&)Real_WSAIoctl, Mine_WSAIoctl);
	ATTACH(&(PVOID&)Real_WSASocket, Mine_WSASocket);
	ATTACH(&(PVOID&)Real_WSAConnect, Mine_WSAConnect);
	
    if (DetourTransactionCommit() != 0) {
        PVOID *ppbFailedPointer = NULL;
        LONG error = DetourTransactionCommitEx(&ppbFailedPointer);

		OutputDebugString(L"Attach detours failed\n");

        return error;
    }
    return 0;
}

LONG DetachDetours(VOID)
{
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());

    // For this many APIs, we'll ignore one or two can't be detoured.
    DetourSetIgnoreTooSmall(TRUE);

	DETACH(&(PVOID&)Real_connect, Mine_connect);
	DETACH(&(PVOID&)Real_WSAIoctl, Mine_WSAIoctl);
	DETACH(&(PVOID&)Real_WSASocket, Mine_WSASocket);
	DETACH(&(PVOID&)Real_WSAConnect, Mine_WSAConnect);
	
    if (DetourTransactionCommit() != 0) {
        PVOID *ppbFailedPointer = NULL;
        LONG error = DetourTransactionCommitEx(&ppbFailedPointer);
        
		OutputDebugString(L"Detach detours failed\n");

        return error;
    }
    return 0;
}