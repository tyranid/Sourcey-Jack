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

#pragma once

#include <Windows.h>
#include <detours.h>

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;
using namespace System::IO;

#include <msclr/marshal.h>
using namespace msclr::interop;

namespace DetoursLib {

	public ref class Detours
	{
	public:
		
		static bool CreateProcessWithDll(String^ appName, String^ commandLine, String^ dllPath)
		{
			marshal_context ^ context = gcnew marshal_context();

			bool ret = false;
			const wchar_t* szAppName = nullptr;
			const wchar_t* szCommandLine = nullptr;
			const char* szDllPath = nullptr;
			const char* szDetouredPath = nullptr;

			if(appName != nullptr)
			{
				szAppName = context->marshal_as<const wchar_t*>(appName);
			}
			
			if(commandLine != nullptr)
			{
				szCommandLine = context->marshal_as<const wchar_t*>(commandLine);
			}

			if(dllPath != nullptr)
			{
				szDllPath = context->marshal_as<const char*>(dllPath);
			}

			String^ detouredPath = Path::Combine(AppDomain::CurrentDomain->BaseDirectory, "detoured.dll");
			szDetouredPath = context->marshal_as<const char*>(detouredPath);

			STARTUPINFO startInfo = {0};			
			PROCESS_INFORMATION procInfo = {0};

			// Really shouldn't const_cast but can't be bothered, if CreateProcess wasn't so dumb ;)
			if(DetourCreateProcessWithDllW(szAppName, const_cast<wchar_t*>(szCommandLine), nullptr, nullptr, FALSE, 0, nullptr, nullptr, 
                                        &startInfo, &procInfo, 
                                        szDetouredPath, szDllPath,
                                        nullptr))
			{
				CloseHandle(procInfo.hThread);
				CloseHandle(procInfo.hProcess);
				ret = true;
			}

			delete context;

			return ret;
		}

		static bool RunLoadLibrary(HANDLE hProcess, LPSTR lpDll)
		{
			bool ret = false;
			LPVOID lpLoadLibrary = GetProcAddress(GetModuleHandle(L"kernel32"), "LoadLibraryA");
			if(lpLoadLibrary != nullptr)
			{
				HANDLE hThread = CreateRemoteThread(hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)lpLoadLibrary, lpDll, 0, NULL);
				if(hThread)
				{
					WaitForSingleObject(hThread, 4000);
					ret = true;
				}
				else
				{
					System::Diagnostics::Debug::WriteLine(String::Format("Failed to create remote thread {0}", GetLastError()));
				}
			}
			else
			{
				System::Diagnostics::Debug::WriteLine("Didn't get address for LoadLibrary");
			}

			return ret;
		}

		static bool InjectDll(int pid, String^ dllPath)
		{
			marshal_context ^ context = gcnew marshal_context();
			bool ret = false;

			const char* szDllPath = nullptr;
			const char* szDetouredPath = nullptr;
			
			if(dllPath != nullptr)
			{
				szDllPath = context->marshal_as<const char*>(dllPath);
			}

			String^ detouredPath = Path::Combine(AppDomain::CurrentDomain->BaseDirectory, "detoured.dll");
			szDetouredPath = context->marshal_as<const char*>(detouredPath);			
			SIZE_T totalSize = strlen(szDllPath) + strlen(szDetouredPath) + 2;
			HANDLE hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, FALSE, pid);
			if(hProcess != nullptr)
			{	
				LPVOID lpMem = VirtualAllocEx(hProcess, NULL, totalSize, MEM_COMMIT, PAGE_READWRITE);
				if(lpMem)
				{
					LPSTR lpDetoured = (LPSTR)lpMem;
					LPSTR lpDll = lpDetoured + strlen(szDetouredPath) + 1;

					WriteProcessMemory(hProcess, lpDetoured, szDetouredPath, strlen(szDetouredPath)+1, NULL);
					WriteProcessMemory(hProcess, lpDll, szDllPath, strlen(szDllPath)+1, NULL);

					if(RunLoadLibrary(hProcess, lpDetoured))
					{
						RunLoadLibrary(hProcess, lpDll);
					}
				}
			}

			delete context;

			return ret;
		}
	};
}
