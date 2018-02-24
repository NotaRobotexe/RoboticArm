import Arm as arm
import numpy as np
import cv2

arm.InitCom()
arm.InitYolo("C:\\Users\\mt2si\\Desktop\\yolot\\test.mp4","C:\\Users\\mt2si\\Documents\\RoboticArm\\yolo")
while(1):
	a = arm.GetYoloOutput()
	arm.DrawTargetsOnVideo(a)
arm.CloseCom()