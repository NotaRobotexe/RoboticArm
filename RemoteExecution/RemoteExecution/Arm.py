import socket
import cv2
import numpy as np

#############################################################################################################################
objects = []

def LockObject(hierarchy):
    global objects

    for obj in objects:
        if str(hierarchy) == obj[:1]:
            print(obj[2:])

    objects = []

def BasicColorRecognition(Frame,BGR,smooth,minOnjectSizem,OutputMode): #outputmode 1=mask 2=hsv 3=normal 
    global objects
    font = cv2.FONT_HERSHEY_SIMPLEX

    BGRhsv = cv2.cvtColor(BGR,cv2.COLOR_BGR2HSV)
    lower = np.array([BGRhsv[0,0,0]-20,70,50])
    upper= np.array([BGRhsv[0,0,0]+20,255,255])

    hsv = cv2.cvtColor(Frame, cv2.COLOR_BGR2HSV)
    mask = cv2.inRange(hsv, lower, upper)

    kernel = np.ones((smooth,smooth),np.uint8)
    erosion = cv2.erode(mask,kernel,iterations = 1)
    final = cv2.dilate(erosion,kernel,iterations = 1)
        
    (_,contours,_) = cv2.findContours(final, 1, 2)

    if OutputMode == 1:
        final = hsv
    elif OutputMode == 2:
        final = Frame
    hierarchy = 0
    for cnt in contours:
        area = cv2.contourArea(cnt)
        if area > minOnjectSizem:
            hierarchy = hierarchy+1
            rect = cv2.minAreaRect(cnt)
            angle = int(rect[2])
            box = cv2.boxPoints(rect)
            box = np.int0(box)
            center =(int((box[1,0]+box[3,0])/2),int((box[1,1]+box[3,1])/2))
            cv2.drawContours(final,[box],-1,(255,0,255),5)
            cv2.circle(final,center,1, (0,0,255), 10)
            cv2.putText(final,str(hierarchy),center,font,1,(0,0,255),5,cv2.LINE_AA)
            objects.append(str(hierarchy)+"*"+str(center[0])+"*"+str(center[1])+"*"+str(angle))

    return final


#############################################################################################################################

port = 6972
sck = 0
tcp_count = 128
new_sck = 0

def GetTriggerStatus():
    Send("1")
    return Receive().decode('UTF-8')

def ReadInput():
    Send("2")
    data = Receive().decode('UTF-8')
    return data

def MovingSpeed(time):
    Send("5"+str(time))

def SetPosition(base,elbow0,elbow1,elbow2,gripper_rotatio,gripper):
    pos = "3"+str(base)+"*"+str(elbow0)+"*"+str(elbow1)+"*"+str(elbow2)+"*"+str(gripper_rotatio)+"*"+str(gripper)
    print(pos)
    Send(pos)

def IsMovig():
    Send("4")
    ans = Receive().decode('UTF-8')

    if ans=="0":
        return False
    else:
        return True 

def InitCom(ip):
    tcp_ip = ip
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