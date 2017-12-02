#include <iostream>
#include "NetworkCom.h"
#include <string>
#include <thread>
#include  <vector>
#include "GPIO.h"
using namespace std;

bool Movement=false;
bool ShutDownStart = false;

void LaunchStream();
void GetData(); //for temperature and procces and fan
void GetTempAndLoad();

void ShutDown(NetworkCom b);

string getTemp();
string getCPULoad();

string TempAndLoad = "26*83";

int main(void)
{
	NetworkCom netCom(6969);
	GPIO tr;

	bool launched = false;
	string NetComMessage = "";

	do
	{
		NetComMessage = netCom.Recv();

		if (NetComMessage.substr(0,1) == "6")
		{
		}
		else if(NetComMessage.substr(0, 1) == "5") //data cpu and temp
		{
			netCom.Send(TempAndLoad);
			GetData();
		}
		else if (NetComMessage.substr(0, 1) == "4")
		{
			Movement = false;
		}
		else if (NetComMessage.substr(0, 1) == "3")
		{
			Movement = true;
		}
		else if (NetComMessage.substr(0, 1) == "2") //GPIO
		{
			netCom.Send(tr.CheckTrigger());
		}
		else if (NetComMessage.substr(0, 1) == "1")
		{

		}
		else if (NetComMessage.substr(0, 1) == "8") //TODO: fan speed
		{

		}


	} while (NetComMessage.substr(0, 1) != "7");
	
	ShutDown(netCom);
	cin.get();
	return 0;
}

/* temperature and cpu load*/
string getCPULoad() {
	string load = "";
	FILE* fp = popen("echo $(vmstat 1 2|tail -1|awk '{print $15}')", "r");
	if (fp) {
		std::vector<char> buffer(4);
		fread(buffer.data(), 1, buffer.size(), fp);
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

void GetData() {
	thread th_messaging(GetTempAndLoad);
	th_messaging.detach();
}

void GetTempAndLoad() {
	string temp = getTemp();
	string load = getCPULoad();

	TempAndLoad = temp + "*" + load;
}


void LaunchStream() {

}

void ShutDown(NetworkCom b) {
	ShutDownStart = true;
	b.Quit();
}

void StartMovemend() {

}