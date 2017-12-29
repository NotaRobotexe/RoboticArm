#include <iostream>
#include <string>
#include <thread>
#include <vector>
#include <chrono>

#include "GPIO.h"
#include "NetworkCom.h"
#include "PCA9685.h"

#define ARM0a 9
#define ARM0b 11
#define ARM1 10
#define ARM2 13
#define GRIPPERR 15
#define GRIPPER 14

#define MIN_s 102
#define MAX_s 576

#define MANUALMOVE "0"
#define AUTOMOVE "0"
using namespace std;

bool Movement=false;
bool ShutDownStart = false;

void LaunchStream();
void GetTempAndLoad(NetworkCom netData);
void distanceTrigger(NetworkCom netTrigger);
void ShutDown(NetworkCom b, NetworkCom a, NetworkCom c, NetworkCom d, NetworkCom e);
void Movemend(NetworkCom netMove);
void FanSpeed(NetworkCom fanSpeed);
void ManualMovemend(PCA9685 pwm, string msg);
void AutoMovemend(PCA9685 pwm, string msg);
string getTemp();
string getCPULoad();

string TempAndLoad = "26*83";

int main(void)
{
	NetworkCom netCom(6969);
	NetworkCom netData(6968);
	NetworkCom netMove(6967);
	NetworkCom netTrigger(6966);
	NetworkCom netFan(6965);

	bool launched = false;
	string NetComMessage = "";

	GPIO::Init();
	do
	{
		NetComMessage = netCom.Recv();
		cout << "com recive" + NetComMessage << endl;

		if (NetComMessage.substr(0,1)=="1") //launch all thread
		{

			thread th_dataMessaging(GetTempAndLoad, netData);
			th_dataMessaging.detach();

			thread th_triggerMessaaging(distanceTrigger, netTrigger);
			th_triggerMessaaging.detach();

			thread th_movemend(Movemend,netMove);
			th_movemend.detach();

			thread th_fanspeed(FanSpeed, netFan);
			th_fanspeed.detach();
		}
		else if (NetComMessage.substr(0, 1) == "2") //launch or relaunch stream
		{

		}

	} while (NetComMessage.substr(0, 1) != "7");
	
	ShutDown(netCom,netMove,netData,netFan,netTrigger);
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
		fread(buffer.data(), 1, buffer.size(), fp);
		pclose(fp);

		for (size_t i = 5; i < buffer.size(); i++)
		{
			temp += buffer[i];
		}
	}

	return temp;
}

void GetTempAndLoad(NetworkCom netData) {
	do
	{
		string temp = getTemp();
		string load = getCPULoad();

		string NetComMessage = temp + "*" + load;

		netData.Send(NetComMessage);
		
		this_thread::sleep_for(chrono::milliseconds(1000));

	} while (ShutDownStart==false);
}

/*distance trigger*/

void distanceTrigger(NetworkCom netTrigger) {
	string status = "false";
	do
	{
		string trigger = GPIO::CheckTrigger();
		if (trigger != status)
		{
			netTrigger.Send(trigger);
			status = trigger;
		}

		this_thread::sleep_for(chrono::milliseconds(100));

	} while (ShutDownStart==false);
}

void Movemend(NetworkCom netMove) {
	PCA9685 pwm;
	pwm.init(1, 0x40);
	pwm.setPWMFreq(51);

	do
	{
		string msg = netMove.Recv();
		if (msg.substr(1, 1) == MANUALMOVE)
		{
			ManualMovemend(pwm, msg);
		}
		else if (msg.substr(1, 1) == AUTOMOVE)
		{
			AutoMovemend(pwm, msg);
		}

	} while (ShutDownStart == false);
}

void ManualMovemend(PCA9685 pwm, string msg) {
	if (msg.substr(2, 1) == "0")
	{
		GPIO::BaseMovemend(stoi(msg.substr(3, 3)));
		cout << "base " + msg.substr(3, 3) << endl;
	}
	else if (msg.substr(2, 1) == "1") {
		pwm.setPWM(ARM0a, stoi(msg.substr(3, 3)));
		pwm.setPWM(ARM0b, (MAX_s + MIN_s) - stoi(msg.substr(3, 3)));

		cout << "arm0 " + msg.substr(3, 3) << endl;
	}
	else if (msg.substr(2, 1) == "2") {
		pwm.setPWM(ARM1, stoi(msg.substr(3, 3)));
		cout << "arm1 " + msg.substr(3, 3) << endl;
	}
	else if (msg.substr(2, 1) == "3") {
		pwm.setPWM(ARM2, stoi(msg.substr(3, 3)));
		cout << "arm2 " + msg.substr(3, 3) << endl;
	}
	else if (msg.substr(2, 1) == "4") {
		pwm.setPWM(GRIPPERR, stoi(msg.substr(3, 3)));
		cout << "gripper r " + msg.substr(3, 3) << endl;
	}
	else if (msg.substr(2, 1) == "5") {
		pwm.setPWM(GRIPPER, stoi(msg.substr(3, 3)));
		cout << "gripper " + msg.substr(3, 3) << endl;
	}
}

void AutoMovemend(PCA9685 pwm, string msg) {

}

void FanSpeed(NetworkCom fanSpeed) {
	do
	{
		int speed = stoi(fanSpeed.Recv());
		GPIO::FanSpeed(speed);
	} while (ShutDownStart == false);
}

void LaunchStream() {

}

void ShutDown(NetworkCom b, NetworkCom a, NetworkCom c, NetworkCom d, NetworkCom e) {
	ShutDownStart = true;
	b.Quit();
	a.Quit();
	c.Quit();
	d.Quit();
	e.Quit();
}

