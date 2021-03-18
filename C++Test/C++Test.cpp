
#include <iostream>

#include "Windows.h"
#include <iostream>
#include <tchar.h>
#include<cstdio>
#include <unordered_map>
using namespace std;

typedef const char* (*delegateGetMessageResult)(const char* szGroupID, const char* szQQID, const char* szContent);
typedef const char* (*GetResultCallBack)(delegateGetMessageResult callback);
typedef const char* (*delegateGetLog)(const char* szContent);
typedef const char* (*GetLogCallBack)(delegateGetLog callback);

typedef void* (__cdecl* LoginIn)(const char* userId, const char* password);


const char* SendMessageCallBack(const char* szGroupID, const char* szQQID, const char* szContent)
{
	std::cout << "群:" << szGroupID << "QQ号:" << szQQID << "发送了这样的消息:" << szContent << "\n";
	return szContent;
}

const char* PintLogCallBack(const char* szContent)
{
	std::cout << szContent << "\n";
	return szContent;
}

int main()
{
	HMODULE hModule = LoadLibrary(L"D:\\CSharp\\QQSDK\\QQSDK\\bin\\x86\\Debug\\QQSDK.dll");
	if (hModule != NULL)
	{

		GetResultCallBack pGetResult = (GetResultCallBack)GetProcAddress(hModule, "GetResultCallBack");
		if (pGetResult != NULL)
		{
			const char* ret = pGetResult(&SendMessageCallBack);
		}

		GetLogCallBack pGetLog = (GetLogCallBack)GetProcAddress(hModule, "GetLogCallBack");
		if (pGetLog != NULL)
		{
			const char* ret = pGetLog(&PintLogCallBack);
		}

		LoginIn pLoginIn = (LoginIn)GetProcAddress(hModule, "LoginIn");
		pLoginIn("2773449707", "e0UuvFVFu20");
	
	}
	system("pause");
	return 0;
}
