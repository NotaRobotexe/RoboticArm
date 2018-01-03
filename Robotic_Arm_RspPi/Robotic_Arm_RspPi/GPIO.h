#pragma once
#include <string>

class GPIO
{
public:
	static std::string CheckTrigger();
	static void FanSpeed(int speed);
	static void RedLed();
	static void BlueLed();
	static void GreenLed();
	static void Init();
	GPIO();
	~GPIO();
};

