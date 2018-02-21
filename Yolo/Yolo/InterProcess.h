#include <string>
#pragma once

class InterProcess
{
public:
	InterProcess(int port);
	void SendData(std::string message);
	std::string ReceiveData();
	void ReleaseAll();
	~InterProcess();
};

