#include <Servo.h>

void base(int angle);
void arm_one(int angle);
void arm_two(int angle);

String readString;
int last_angle_base1 = 0, last_angle_arm1 = 0, last_angle_arm2 = 0;
Servo bs1, bs2, arm1, arm2;
int _delay = 40;

void setup() {
	Serial.begin(9600);
	bs1.attach(6);
	bs2.attach(5);
	arm1.attach(9);
	arm2.attach(10);
}

void loop() {
	while (Serial.available()) {
		char c = Serial.read(); 
		readString += c; 
		delay(2);
	}

	if (readString.length() >0) {
		Serial.println(readString); 

		if (readString.substring(0, 1)=="b"){
			base(readString.substring(1).toInt());
		}
		else if (readString.substring(0, 1)=="o"){
			arm_one(readString.substring(1).toInt());
		}
		else if (readString.substring(0, 1) == "t") {
			arm_two(readString.substring(1).toInt());
		}

		readString = ""; 
	}
}

void base(int angle) {
	Serial.println("base");
	Serial.println(angle);

	if (angle > last_angle_base1) {
		for (int a = last_angle_base1; a <= angle; a++) {
			bs1.write(a);
			bs2.write(180 - a);
			delay(_delay);
		}
	}
	else {
		for (int a = last_angle_base1; a >= angle; a--) {
			bs1.write(a);
			bs2.write(180 - a);
			delay(_delay);
		}
	}

	last_angle_base1 = angle;

}

void arm_one(int angle) {
	Serial.println("arm1");
	Serial.println(angle);

	if (angle > last_angle_arm1) {
		for (int a = last_angle_arm1; a <= angle; a++) {
			arm1.write(a);
			delay(_delay);
		}
	}
	else {
		for (int a = last_angle_arm1; a >= angle; a--) {
			arm1.write(a);
			delay(_delay);
		}
	}

	last_angle_arm1 = angle;
}

void arm_two(int angle) {
	Serial.println("arm2");
	Serial.println(angle);

	if (angle > last_angle_arm2) {
		for (int a = last_angle_arm2; a <= angle; a++) {
			arm2.write(a);
			delay(_delay);
		}
	}
	else {
		for (int a = last_angle_arm2; a >= angle; a--) {
			arm2.write(a);
			delay(_delay);
		}
	}

	last_angle_arm2 = angle;
}
