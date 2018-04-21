import Arm as arm
import numpy as np
import cv2

arm.InitCom()
add = arm.GetStreamAddress()
res = arm.GetStreamResolution()
arm.SendMessage(add)
arm.InitYolo(add,"C:\\Users\\mt2si\\Documents\\RoboticArm\\yolo")
while(1):
	a = arm.GetYoloOutput()
	arm.DrawTargetsOnVideo(a)
	if len(a) > 0:
		b = arm.TryLockAtObject(a[0],1,5,res)
arm.CloseCom()