#include "NetworkCom.h"
#include <string>
#include <vector>
#include <fstream>
#include <iostream>

#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <arpa/inet.h>
#include <sys/wait.h>
#include <unistd.h>
#include <errno.h>

sockaddr_in info;

NetworkCom::NetworkCom(int port)
{
	this->port = port;
	if ((sck = socket(AF_INET, SOCK_STREAM, 0)) == -1) { error("socker error"); }

	set_info_s();

	if ((bind(sck, (struct sockaddr*)&info, sizeof(sockaddr))) == -1) { error("bind error"); }
	if (listen(sck, 1) == -1) { error("LISTEN error"); }
	if ((new_sck = accept(sck, NULL, NULL)) == -1) { error("ACCEPT error"); }
}


void NetworkCom::Quit()
{
	close(sck);
	close(new_sck);
}

void NetworkCom::set_info_s()
{
	info.sin_family = AF_INET;
	info.sin_addr.s_addr = INADDR_ANY;
	info.sin_port = htons(port);
}

void NetworkCom::error(std::string s) { //UNDONE: (1) ak bude cas treba sa kuknut na a prerobit to na "errno" (2) datum a cas sa davaju na novy riadok aj to (((treba))) skusim fixnut
	std::ofstream err("error_log.txt", std::ios::app);
	std::cout << "ERROR check error_log.txt for more info - " + s;
	if (err.is_open()) {

		err << s;
		err.close();
	}
	else {
		std::cout << "cannot open error_log file";
	}
	exit(1);
}

void NetworkCom::Send(std::string msg)
{
	int len = msg.size();
	const char *buf = msg.c_str();
	send(new_sck, buf, len, 0);
}

std::string NetworkCom::Recv() 
{
	char buf[100];
	int bytes_read;
	bytes_read = recv(new_sck, buf, sizeof(buf), 0);

	std::string pure_buf = "";
	for (size_t i = 0; i < bytes_read; i++) {
		pure_buf += std::string(1, buf[i]);
	}

	return pure_buf;
}