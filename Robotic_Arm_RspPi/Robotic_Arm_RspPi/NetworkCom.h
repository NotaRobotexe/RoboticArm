#pragma once
#include <string>

class NetworkCom
{
private:
	void set_info_s();
	void error(std::string s);
	int port;
	std::string addres;

public:
	void Quit();
	std::string Recv();
	void Send(std::string msg);
	NetworkCom(int port);
};

