#include "GPIO.h"
#include <wiringPi.h>
#include <iostream>
#include <string>

std::string GPIO::CheckTrigger()
{
	int i = digitalRead(7);
	
	if (i == 0)
	{
		return "true";
	}
	else
	{
		return "false";
	}
}

GPIO::GPIO()
{
	wiringPiSetup();
	pinMode(7, INPUT);
}

GPIO::~GPIO()
{
}
