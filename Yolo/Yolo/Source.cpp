#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <iostream>
#include <vector>
#include <fstream>
#include <thread>

#include "InterProcess.h"
#include "yolo_v2_class.hpp"

#define OPENCV

using namespace std;
using namespace cv;

Mat frame;
string source = "";

std::vector<std::string> objects_names_from_file(std::string const filename) {
	std::ifstream file(filename);
	std::vector<std::string> file_lines;
	if (!file.is_open()) return file_lines;
	for (std::string line; getline(file, line);) file_lines.push_back(line);
	std::cout << "object names loaded \n";
	return file_lines;
}

void background_stream() {
	VideoCapture capture(source);
	//namedWindow("edges", 1);

	while (true)
	{
		capture.read(frame);
		//imshow("edges", frame);
		if (waitKey(30) >= 0) break;
	}
}

int main(int argc, char** argv) {
	source = argv[1];
	string path = argv[2];
	string cfg = path+"\\obj.cfg";
	string weight = path+"\\obj.weights";
	string names = path+"\\obj.names";
	auto obj_names = objects_names_from_file(names);

	thread([]() { background_stream(); }).detach();
	Detector dec(cfg, weight);

	InterProcess ip(6973);

	ip.SendData("tick dest");
	while (true)
	{
		Mat _frame = frame;
		ip.ReceiveData();

		vector<bbox_t> raw_objects = dec.detect(_frame);

		string objects = "";
		if (raw_objects.size()>0){
			for (size_t i = 0; i < raw_objects.size(); i++)
			{
				int centerX = raw_objects[i].x + (raw_objects[i].w / 2);
				int centerY = raw_objects[i].y + (raw_objects[i].h / 2);
				string pos = "";
				if (raw_objects[i].w > raw_objects[i].h)
					pos = "w";
				else
					pos = "h";
				
				objects += to_string(centerX) + "*" + to_string(centerY) + "*" + obj_names[raw_objects[i].obj_id] + "*" + pos+"*"+ to_string(raw_objects[i].prob)+"|";
				//cout << objects << endl;
			}
			ip.SendData(objects);
		}
		else{
			ip.SendData("*");
		}
	}

	return 0;
}