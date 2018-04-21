import Arm as arm
import numpy as np
import cv2

arm.InitCom()
add = arm.GetStreamAddress()
print(add)
arm.SendMessage(add)
arm.InitYolo(add,"C:\\Users\\mt2si\\Documents\\RoboticArm\\yolo")
while(1):
	a = arm.GetYoloOutput()
	print(a)
	arm.DrawTargetsOnVideo(a)
arm.CloseCom()