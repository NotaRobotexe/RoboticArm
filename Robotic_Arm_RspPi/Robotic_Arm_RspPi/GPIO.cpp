#include "GPIO.h"
#include <wiringPi.h>
#include <iostream>
#include <string>

#define TRIGGER 21
#define FAN 18
#define RED 17
#define GREEN 22
#define BLUE 27

std::string GPIO::CheckTrigger(){
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
	pwmWrite(FAN, speed);
}

void GPIO::RedLed(){
	digitalWrite(RED, HIGH);
	digitalWrite(BLUE, LOW);
	digitalWrite(GREEN, LOW);
}

void GPIO::GreenLed() {
	digitalWrite(BLUE, HIGH);
	digitalWrite(RED, LOW);
	digitalWrite(GREEN, LOW);
}

void GPIO::BlueLed() {
	digitalWrite(GREEN, HIGH);
	digitalWrite(BLUE, LOW);
	digitalWrite(BLUE, LOW);
}

void GPIO::Init() {
	wiringPiSetupGpio();
	pinMode(TRIGGER, INPUT);
	pinMode(RED, OUTPUT);
	pinMode(GREEN, OUTPUT);
	pinMode(BLUE, OUTPUT);

	pinMode(FAN, PWM_OUTPUT);
	pwmSetMode(PWM_MODE_MS);
}

GPIO::GPIO()
{
}


GPIO::~GPIO()
{
}
