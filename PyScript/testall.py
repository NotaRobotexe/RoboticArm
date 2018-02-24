import Arm as arm
import numpy as np
import cv2

arm.InitCom()
res = arm.GetStreamResolution()
add = arm.GetStreamAddress()
cap = cv2.VideoCapture(add)
while(1):
	_, frame = cap.read()
	BGR = np.uint8([[[77,70,20]]])
	_,a = arm.BasicColorRecognition(frame,BGR,20,7000)
	arm.DrawTargetsOnVideo(a)
	k = cv2.waitKey(50) & 0xFF
	if k == 27:
		break
arm.CloseCom()