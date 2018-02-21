#include "InterProcess.h"
#include <iostream>
#include <string>

#include <winsock2.h>
#include <ws2tcpip.h>
#pragma comment (lib, "Ws2_32.lib")
WSADATA wsaData;

SOCKET sck;
SOCKADDR_IN info;

InterProcess::InterProcess(int port)
{
	if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) { std::cout << "WSAStartup failed" << std::endl; }
	if ((sck = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) == -1) { std::cout << "socker error" << std::endl;}

	std::string address = "127.0.0.1";

	info.sin_family = AF_INET;
	info.sin_port = htons(port);
	inet_pton(AF_INET, address.c_str(), &(info.sin_addr));
	if (connect(sck, (struct sockaddr *)&info, sizeof(info)) == -1) { std::cout<<"connect error" << std::endl;}
}

void InterProcess::SendData(std::string message) {
	int total = 0;
	int len = message.size();
	const char *buf = message.c_str();
	int n;

	n = send(sck, buf, len, 0);
	if (n == -1) {
		std::cout << "probbably not connected" << std::endl;
		std::exit(0); //lets prettend that everything went smothly
	}
}

std::string InterProcess::ReceiveData() {
	int check_byte = 0;
	bool is_empty = false;
	char buff[8];
	int bytes_read;
	bytes_read = recv(sck, buff, sizeof(buff), 0);
	
	if (bytes_read == -1) { std::cout << "probbably not connected" << std::endl; }
	else if (bytes_read == 0) {
		ReleaseAll();
		std::cout << "probably server is offline or I realy dont know because this shit is just not work fuck this fucking broken cunt" << std::endl;
		std::exit(0); //lets prettend that everything went smothly
	}

	std::string  msg = "";
	for each (char a in buff){
		if (a == '\0')
			break;
		msg += a;
	}

	return msg;
}

void InterProcess::ReleaseAll() {
	closesocket(sck);
	WSACleanup();
}

InterProcess::~InterProcess()
{
}
