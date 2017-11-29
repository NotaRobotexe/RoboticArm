#include <iostream>
#include "NetworkCom.h"
#include <string>
#include <thread>
#include  <vector>
#include "Trigger.h"
using namespace std;

bool Movement=false;
bool ShutDownStart = false;

void LaunchStream();
void LaunchStatMessaging(); //for temperature and procces and fan
void Messaging();
void ShutDown(NetworkCom b);

string getTemp();
string getCPULoad();

int main(void)
{
	Trigger trigger;
	cout << "kurvavvaaa" << std::endl;
	while (true)
	{
		trigger.blink();
	}

	//cout << getCPULoad();

	/*bool launched = false;

	NetworkCom netCom(6969);

	string NetComMessage = "";
	do
	{
		NetComMessage = netCom.Recv();

		if (NetComMessage.substr(0,1) == "6")
		{
		}
		else if(NetComMessage.substr(0, 1) == "5")
		{
		}
		else if (NetComMessage.substr(0, 1) == "4")
		{
			Movement = false;
			cout << "q" << endl;
		}
		else if (NetComMessage.substr(0, 1) == "3")
		{
			Movement = true;
			cout << "q" << endl;
		}
		else if (NetComMessage.substr(0, 1) == "2")
		{
		}
		else if (NetComMessage.substr(0, 1) == "1")
		{
			cout << "q" << endl;
		}


	} while (NetComMessage.substr(0, 1) != "7");
	
	ShutDown(netCom);*/
	cin.get();
	return 0;
}

string getCPULoad() {
	string load = "";
	FILE* fp = popen("echo $(vmstat 1 2|tail -1|awk '{print $15}')", "r");
	if (fp) {
		std::vector<char> buffer(4);
		std::size_t n = fread(buffer.data(), 1, buffer.size(), fp);
		pclose(fp);

		for (size_t i = 0; i < buffer.size(); i++)
		{
			load += buffer[i];
		}
	}
	return load;
}

string getTemp() {
	string temp = "";
	FILE* fp = popen("/opt/vc/bin/vcgencmd measure_temp", "r");
	if (fp) {
		std::vector<char> buffer(9);
		std::size_t n = fread(buffer.data(), 1, buffer.size(), fp);
		pclose(fp);

		for (size_t i = 5; i < buffer.size(); i++)
		{
			temp += buffer[i];
		}
	}

	return temp;
}


void LaunchStatMessaging() {
	//thread th_messaging(Messaging);
	//th_messaging.detach();
}

void Messaging() {
	NetworkCom netStat(6968);
	string msg;
	int FanSpeed = 50;

	while (true)
	{
		msg = netStat.Recv(); // if message return end turn off else message return fan speed

		if (msg != "end") //TODO fan 
		{
			FanSpeed = stoi(msg);

		}
		else 
		{
			break;
		}
	}

	netStat.Quit();
}

void LaunchStream() {

}

void ShutDown(NetworkCom b) {
	ShutDownStart = true;
	b.Quit();
}

void StartMovemend() {

}