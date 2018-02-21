#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <iostream>
#include <vector>
#include <fstream>

#include "InterProcess.h"
#include "yolo_v2_class.hpp"

#define OPENCV

using namespace std;
using namespace cv;

std::vector<std::string> objects_names_from_file(std::string const filename) {
	std::ifstream file(filename);
	std::vector<std::string> file_lines;
	if (!file.is_open()) return file_lines;
	for (std::string line; getline(file, line);) file_lines.push_back(line);
	std::cout << "object names loaded \n";
	return file_lines;
}

int main(int argc, char** argv) {
	string cfg = "obj.cfg";
	string weight = "obj.weights";
	string names = "obj.names";
	auto obj_names = objects_names_from_file(names);
	Detector dec(cfg, weight);

	InterProcess ip(6973);

	VideoCapture capture(argv[1]);
	Mat frame;
	ip.SendData("tick dest");
	while (true)
	{
		ip.ReceiveData();
		capture >> frame;
		vector<bbox_t> raw_objects = dec.detect(frame);
		
		string objects = "";
		if (raw_objects.size()>0)
		{
			for (size_t i = 0; i < raw_objects.size(); i++)
			{
				int centerX = raw_objects[i].x + (raw_objects[i].w / 2);
				int centerY = raw_objects[i].y + (raw_objects[i].h / 2);
				string pos = "";
				if (raw_objects[i].w > raw_objects[i].h)
					pos = "w";
				else
					pos = "h";

				objects += to_string(centerX) + "*" + to_string(centerY) + "*" + pos + "*" + obj_names[raw_objects[i].obj_id]+"*"+ to_string(raw_objects[i].prob)+"|";
			}
			ip.SendData(objects);
		}
		else
		{
			ip.SendData("*");
		}
	}

	return 0;
}