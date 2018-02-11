import socket
import cv2
import numpy as np

port = 6972
sck = 0
tcp_count = 128
new_sck = 0

def TryLockAtObject(Position):
    return 0

def Gripper(open_pos):
    pos ="3"+str(-1)+"*"+str(-1)+"*"+str(-1)+"*"+str(-1)+"*"+str(-1)+"*"+str(open_pos)
    Send(pos)

def DrawTargetsOnVideo(objects):
    Coordiniates = ""
    for x in (0,len(objects)-1):
        Coordiniates = Coordiniates+str(objects[x])

    #Send(("8"+Coordiniates))

def MoveForward():
    Send("2")
    onpos = 0
    while onpos == 0:
        ans = Receive().decode('UTF-8')
        if ans == "finished":
            onpos = 1
    return 0

def GetArmPosition():
    Send("7")
    position_ = Receive().decode('UTF-8')
    position = position_.split("*")
    return position

def BasicColorRecognition(Frame,BGR,smooth,minOnjectSizem):
    BGRhsv = cv2.cvtColor(BGR,cv2.COLOR_BGR2HSV)
    lower = np.array([BGRhsv[0,0,0]-20,70,50])
    upper= np.array([BGRhsv[0,0,0]+20,255,255])

    hsv = cv2.cvtColor(Frame, cv2.COLOR_BGR2HSV)
    mask = cv2.inRange(hsv, lower, upper)

    kernel = np.ones((smooth,smooth),np.uint8)
    erosion = cv2.erode(mask,kernel,iterations = 1)
    final = cv2.dilate(erosion,kernel,iterations = 1)
        
    (_,contours,_) = cv2.findContours(final, 1, 2)

    objects = []

    for cnt in contours:
        area = cv2.contourArea(cnt)
        if area > minOnjectSizem:
            rect = cv2.minAreaRect(cnt)
            box = cv2.boxPoints(rect)
            box = np.int0(box)
            cv2.drawContours(final,[box],-1,(255,0,255),5)
            objects.append(box[0])

    return final , objects

def GetTriggerStatus():
    Send("1")
    return Receive().decode('UTF-8')

def ReadInput():
    data = Receive().decode('UTF-8')
    return data

def SendMessage(messaage):
    Send("6"+messaage)

def MovingSpeed(time):
    Send("5"+str(time))

def SetPosition(base, elbow0 ,elbow1 ,elbow2 ,gripper_rotatio ,gripper): #-1 nothing
    pos = "3"+str(base)+"*"+str(elbow0)+"*"+str(elbow1)+"*"+str(elbow2)+"*"+str(gripper_rotatio)+"*"+str(gripper)
    Send(pos)

def IsMoving():
    Send("4")
    ans = Receive().decode('UTF-8')

    if ans=="0":
        return 0
    else:
        return 1 

#netwrok functions

def InitCom():
    tcp_ip = "0.0.0.0"
    global sck
    global new_sck
    sck = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sck.bind((tcp_ip, port))
    sck.listen(1)
    new_sck,addr = sck.accept()

def CloseCom():
    new_sck.close()
    sck.close()

def Receive():
    data = new_sck.recv(tcp_count)
    return data

def Send(data):
    new_sck.sendall(data.encode())