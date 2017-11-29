#include "Trigger.h"
#include <wiringPi.h>
#include <iostream>

Trigger::Trigger()
{
	wiringPiSetup();
	pinMode(7, INPUT);
}

void Trigger::blink() {
	
	int i = digitalRead(7);
	std::cout << i << std::endl;
}

Trigger::~Trigger()
{
}
