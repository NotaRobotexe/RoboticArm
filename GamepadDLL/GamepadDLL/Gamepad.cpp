#include <Windows.h>
#include <string>

using namespace std;

struct GamepadState
{
	 int leftStickHor;
	 int leftStickVer;
	 int rightStickHor;
	 int rightStickVer;
	 int button;
	 int frontButton;
	 int mode;
};

extern "C" __declspec(dllexport) GamepadState GamepadProcesing(LPARAM lParam) {
	UINT cbSize;
	HRAWINPUT hRawInput = (HRAWINPUT)lParam;
	GetRawInputData(hRawInput, RID_INPUT, 0, &cbSize, sizeof(RAWINPUTHEADER));
	LPBYTE lpbBuffer = new BYTE[cbSize];
	GetRawInputData(hRawInput, RID_INPUT, lpbBuffer, &cbSize, sizeof(RAWINPUTHEADER));
	RAWINPUT* raw = (RAWINPUT*)lpbBuffer;
	
	GamepadState gpState;
	if (lpbBuffer != NULL) {
		if (raw->header.dwType == RIM_TYPEHID)
		{
			//1. left horizontal |2. left ver| 4 right horizontal| 5 right ver| 6. value 143yellow 79red 47green 31blue 6left 0up 2right 4down 15nothing|7. 0nothing 4left down 1left up 2right up 8 right down 16 select 32 start 64 right press 128 left press | 8. mode 64/192
			gpState.leftStickHor = raw->data.hid.bRawData[1]-128;
			gpState.leftStickVer = raw->data.hid.bRawData[2] - 128;
			gpState.rightStickHor = raw->data.hid.bRawData[4] - 128;
			gpState.rightStickVer = raw->data.hid.bRawData[5] - 128;
			gpState.button = raw->data.hid.bRawData[6];
			gpState.frontButton = raw->data.hid.bRawData[7];
			gpState.mode = raw->data.hid.bRawData[8];
		}
	}
	return gpState;
}

