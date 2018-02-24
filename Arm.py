import threading
import socket
import cv2
import numpy as np
import os

port = 6972
sck = 0
tcp_count = 128
new_sck = 0

Yport = 6973
Ysck = 0
Ytcp_count = 128
Ynew_sck = 0


def TryLockAtObject(PositionX,positionY,speed,toleration,resolution):
    centerX = resolution[0]/2
    centerY = resolution[1]/2

    command = "w"
    onpos = 0

    if PositionX > centerX+toleration:
        command = command+"1"
    elif PositionX < centerX-toleration:
        command = command+"2"
    else:
        onpos = 1
        command = command+"0"

    if PositionY > centerY+toleration:
        command = command+"1"
    elif PositionY < centerY-toleration:
        command = command+"2"
    else:
        onpos = onpos+1
        command = command+"0"

    command= command+str(speed)
    Send(command)
    acnknowladge()

    if onpos == 2:
        return 1
    else:
        return 0

def GetStreamResolution():
    Send("q")
    res_ = Receive()
    res = res_.split("*")
    return res

def GetStreamAddress():
    Send("9")
    StreamAdd = Receive()
    return StreamAdd

def Gripper(open_pos):
    pos ="3"+str(-1)+"*"+str(-1)+"*"+str(-1)+"*"+str(-1)+"*"+str(-1)+"*"+str(open_pos)
    Send(pos)
    acnknowladge()

def DrawTargetsOnVideo(objects): #structure [x,y,name]
    Coordiniates = ""
    for x in (0,len(objects)-1):
        for i in range(3):
            Coordiniates = Coordiniates+str(objects[x][i])+"*"
        Coordiniates = Coordiniates+"|"

    Send(("8"+Coordiniates))
    acnknowladge()

def MoveForward():
    Send("2")
    acnknowladge()

def GetArmPosition():
    Send("7")
    position_ = Receive()
    position = position_.split("*")
    return position

def BasicColorRecognition(Frame,BGR,smooth,minOnjectSizem):
    if Frame is not None:
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
                target = [str(box[0][0]),str(box[0][1])," "]
                objects.append(target)

        return final , objects #will return something like this [['0', '479', ''], ['609', '351', ''], ['164', '363', '']]
    else:
        print(".")
        return 0 , 0

def GetTriggerStatus():
    Send("1")
    return Receive()

def ReadInput():
    data = Receive()
    return data

def SendMessage(messaage):
    Send("6"+messaage)
    acnknowladge()

def MovingSpeed(time):
    Send("5"+str(time))
    acnknowladge()

def SetPosition(base, elbow0 ,elbow1 ,elbow2 ,gripper_rotatio ,gripper): #-1 nothing
    pos = "3"+str(base)+"*"+str(elbow0)+"*"+str(elbow1)+"*"+str(elbow2)+"*"+str(gripper_rotatio)+"*"+str(gripper)
    Send(pos)
    acnknowladge()

def IsMoving():
    Send("4")
    ans = Receive()
    if ans=="0":
        return 0
    else:
        return 1 

def InitYolo(source):
    tcp_ip = "127.0.0.1"
    global Ysck
    global Ynew_sck
    Ysck = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    Ysck.bind((tcp_ip, Yport))

    thread = threading.Thread(target=launchYolo, args=(source,))
    thread.daemon = True
    thread.start()

    Ysck.listen(1)
    Ynew_sck,addr = Ysck.accept()
    data = Ynew_sck.recv(Ytcp_count).decode('UTF-8')
    return data

def GetYoloOutput():
    data = "k"
    Ynew_sck.sendall(data.encode()+b"\0")
    objects = Ynew_sck.recv(Ytcp_count).decode('UTF-8')
    
    if objects == "*":
        return 0

    objects_ = objects.split("|")
    num_of_obj = (len(objects_)-1)

    final_object=[]
    for i in range(num_of_obj):
        final_object.append([])
        parameters = objects_[i].split("*")
        for a in range(5):      
            final_object[i].append(parameters[a])

    return final_object    

def CloseYolo():
    Ynew_sck.close()
    Ysck.close()
    
def launchYolo(source): #do not call from script!!!!!
    dirpath = os.getcwd()    
    usrname = os.path.basename(dirpath)
    path = "C:\\Users\\"+usrname+"\\Documents\\RoboticArm\\yolo\\"
    os.system("cd " + path + " & Yolo.exe "+ source)


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
    data = new_sck.recv(tcp_count).decode('UTF-8')
    return data

def Send(data):
    new_sck.sendall(data.encode())

def acnknowladge():
    while 1==1:
        ack = Receive()
        print(ack)
        if ack == "1":
            break
