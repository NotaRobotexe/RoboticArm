import cv2
import numpy as np
cap = cv2.VideoCapture("test.mp4")

BGR = np.uint8([[[51,91,222]]])
BGRhsv = cv2.cvtColor(BGR,cv2.COLOR_BGR2HSV)
lower = np.array([BGRhsv[0,0,0]-20,70,50])
upper= np.array([BGRhsv[0,0,0]+20,255,255])

while(1):
    _, frame = cap.read()
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
    mask = cv2.inRange(hsv, lower, upper)

    kernel = np.ones((15,15),np.uint8)
    erosion = cv2.erode(mask,kernel,iterations = 1)
    final = cv2.dilate(erosion,kernel,iterations = 1)
    
    (_,contours,_) = cv2.findContours(final, 1, 2)

    for cnt in contours:
        area = cv2.contourArea(cnt)
        if area > 7000:
            rect = cv2.minAreaRect(cnt)
            box = cv2.boxPoints(rect)
            box = np.int0(box)
            cv2.drawContours(hsv,[box],-1,(255,0,255),5)

    cv2.imshow("final",final)
    cv2.imshow('hsv',hsv)

    k = cv2.waitKey(20) & 0xFF
    if k == 27:
        break
cv2.destroyAllWindows()