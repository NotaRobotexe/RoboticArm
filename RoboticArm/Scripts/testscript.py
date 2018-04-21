import Arm as arm
import cv2
import numpy as np

cap = cv2.VideoCapture("test.mp4")

_, frame = cap.read()
BGR = np.uint8([[[51,91,222]]])
hsv,a = arm.BasicColorRecognition(frame,BGR,20,7000)
arm.DrawTargetsOnVideo(a)
print(a)
cv2.imshow('hsv',hsv)

