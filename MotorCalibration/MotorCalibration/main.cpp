#include <wiringPi.h>
#include <string>
#include <iostream>
#include "PCA9685.h"
#include <wiringPi.h>

#define ARM0a 9
#define ARM0b 11
#define ARM1 10
#define ARM2 13
#define GRIPPERR 15
#define GRIPPER 14

#define MIN_s 102
#define MAX_s 576

#define DIRECTION 21
#define STEP 20

using namespace std;


int main(void)
{
	string action = "";
	PCA9685 pwm;
	pwm.init(1, 0x40);
	pwm.setPWMFreq(51);
	wiringPiSetupGpio();

	int last2 = 102, last3 = 102;
	int last1 = 102;
	pinMode(DIRECTION, OUTPUT);
	pinMode(STEP, OUTPUT);

	digitalWrite(DIRECTION, HIGH);
	do
	{
		cin >> action;
		int pos = stoi(action.substr(1, 3));
		if (action.substr(0, 1) == "0")
		{
			for (size_t i = 0; i < stoi(action.substr(1,3)); i++)
			{
				digitalWrite(STEP, HIGH);
				delayMicroseconds(1000 * stoi(action.substr(4,4)));
				digitalWrite(STEP, LOW);
				delayMicroseconds(1000 * stoi(action.substr(4, 4)));

			}
			cout << "base " + action.substr(1, 3) << endl;
		}
		else if (action.substr(0, 1) == "1" && pos >=MIN_s && pos <=MAX_s) {
			pwm.setPWM(ARM0a, stoi(action.substr(1, 3)));//300
			pwm.setPWM(ARM0b, (MAX_s + MIN_s)+1 - stoi(action.substr(1, 3))); //378

			while (pos != last1)
			{
				if (pos>last1)
				{
					last1++;
					pwm.setPWM(ARM0a, last1);
					pwm.setPWM(ARM0b, (MAX_s + MIN_s) + 1 - last1);
				}
				else if (pos<last1)
				{
					last1--;
					pwm.setPWM(ARM0a, last1);
					pwm.setPWM(ARM0b, (MAX_s + MIN_s) + 1 - last1);
				}
				delayMicroseconds(1000 * 5);
			}

			cout << "arm0 " + action.substr(1, 3) << endl;
		}
		else if (action.substr(0, 1) == "2"&& pos >= MIN_s && pos <= MAX_s) {
			while (pos!=last2)
			{
				if (pos>last2)
				{
					last2++;
					pwm.setPWM(ARM1, last2);
				}
				else if (pos<last2)
				{
					last2--;
					pwm.setPWM(ARM1, last2);
				}
				delayMicroseconds(1000 * 2);
			}
			
			cout << "arm1 " + action.substr(1, 3) << endl;
		}
		else if (action.substr(0, 1) == "3"&& pos >= MIN_s && pos <= MAX_s) {
			while (pos != last3)
			{
				if (pos>last3)
				{
					last3++;
					pwm.setPWM(ARM2, last3);
				}
				else if (pos<last3)
				{
					last3--;
					pwm.setPWM(ARM2, last3);
				}
				delayMicroseconds(1000 * 2);
			}
			cout << "arm2 " + action.substr(1, 3) << endl;
		}
		else if (action.substr(0, 1) == "4"&& pos >= MIN_s && pos <= MAX_s) {
			pwm.setPWM(GRIPPERR, stoi(action.substr(1, 3)));
			cout << "gripper r " + action.substr(1, 3) << endl;
		}
		else if (action.substr(0, 1) == "5"&& pos >= MIN_s && pos <= MAX_s) {
			pwm.setPWM(GRIPPER, stoi(action.substr(1, 3)));
			cout << "gripper " + action.substr(1, 3) << endl;
		}

	} while (action != "end");

	return 0;
}