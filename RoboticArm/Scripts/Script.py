import Arm as arm
import numpy as np
import cv2

arm.InitCom()
cap = cv2.VideoCapture("C:\\Users\\mt2si\\Desktop\\projekty\\S.O.C Robotic Arm\\Robotic Arm\\PyScript\\test.mp4")
while(1):
	_, frame = cap.read()
	BGR = np.uint8([[[51,91,222]]])
	_,a = arm.BasicColorRecognition(frame,BGR,20,7000)
	if a == 0:
		break
	arm.DrawTargetsOnVideo(a)
	arm.SendMessage("benis")
	print(a)
	k = cv2.waitKey(30) & 0xFF
	if k == 27:
		break
arm.CloseCom()