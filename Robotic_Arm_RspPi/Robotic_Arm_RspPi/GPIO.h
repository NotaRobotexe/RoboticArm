#pragma once
#include <string>

class GPIO
{
public:
	static int BaseAngle;
	static std::string CheckTrigger();
	static void FanSpeed(int speed);
	static void Init();
	static void BaseMovemend(int position);
	GPIO();
	~GPIO();
};

