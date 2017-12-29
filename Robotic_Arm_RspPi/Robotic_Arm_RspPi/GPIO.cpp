#include "GPIO.h"
#include <wiringPi.h>
#include <softPwm.h>
#include <iostream>
#include <string>

#define TRIGGER 19
#define FAN 26
#define DIRECTION 21
#define STEP 20

int GPIO::BaseAngle = 180;

std::string GPIO::CheckTrigger()
{
	int i = digitalRead(TRIGGER);
	
	if (i == 0)
	{
		return "true";
	}
	else
	{
		return "false";
	}
}

void GPIO::FanSpeed(int speed) {
	//softPwmWrite(FAN, speed);
}

void GPIO::Init() {
	wiringPiSetupGpio();
	pinMode(TRIGGER, INPUT);
	pinMode(DIRECTION, OUTPUT);
	pinMode(STEP, OUTPUT);

	pinMode(FAN, OUTPUT);

	digitalWrite(FAN, HIGH);
	//softPwmCreate(FAN, 50, 100);
}

void GPIO::BaseMovemend(int position) {
	if (position > BaseAngle && position <= 330)
	{
		digitalWrite(DIRECTION, HIGH);
		do
		{
			digitalWrite(STEP, HIGH);
			delayMicroseconds(100);
			digitalWrite(STEP, LOW);
			BaseAngle++;
		} while (position < BaseAngle);
	}
	else if(position < BaseAngle && position >= 30 )
	{
		digitalWrite(DIRECTION, LOW);
		do
		{
			digitalWrite(STEP, HIGH);
			delayMicroseconds(1000);
			digitalWrite(STEP, LOW);
			delayMicroseconds(1000);
			BaseAngle--;
		} while (position > BaseAngle);
	}
}

GPIO::GPIO()
{
}


GPIO::~GPIO()
{
}
