#pragma once
#include <string>

class NetworkCom
{
private:
	void set_info_s();
	void error(std::string s);
	int port;
	int sck, new_sck;
	std::string addres;

public:
	void Quit();
	std::string Recv();
	void Send(std::string msg);
	NetworkCom(int port);
};

