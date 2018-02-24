import Arm as arm
import numpy as np
import cv2
import sys

arm.InitCom()
try:
	res = arm.GetStreamResolution()
	cap = cv2.VideoCapture("C:\\Users\\mt2si\\Desktop\\projekty\\S.O.C Robotic Arm\\Robotic Arm\\PyScript\\test.mp4")
	while(1):
		_, frame = cap.read()
		BGR = np.uint8([[[51,91,222]]])
		_,a = arm.BasicColorRecognition(frame,BGR,20,7000)
		arm.DrawTargetsOnVideo(a)
		if a != []:
			while(1):
				b = arm.TryLockAtObject(a[0][0],a[0][1],5,10,res)
				if b== 1:
					break

except: # catch *all* exceptions
	e = sys.exc_info()[0]
	arm.SendMessage(e)
arm.CloseCom()