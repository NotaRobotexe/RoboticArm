import cv2
import numpy as np
from pyzbar.pyzbar import decode
from threading import Thread
import Arm as arm
import time as time
import math

lastpos = []
ismove = 0
res = 0
frame = None

def NewestFrame(add):
    global frame
    cap = cv2.VideoCapture(add)
    while(True):
        ret, frame = cap.read()

def FindQr():
    qr = decode(frame)
    targets=[]
    empty = 0
    center = [-1,-1]
    if(len(qr)>0):
        empty = 1
        center = [qr[0][2][0] + (int)(qr[0][2][2]/2),qr[0][2][1] + (int)(qr[0][2][3]/2)]
        drawTar = [str(center[0]),str(center[1]),qr[0][0].decode("utf-8")]
        targets.append(drawTar)
    return empty,targets,center

def WaitForMove():
	while(1):
		a = arm.IsMoving()
		if a == 0:
			break
		time.sleep(0.1)

def GetDistance(posx,posy):
    disMap=[[32,25],
        [30,23],
        [32,25]]

    posx_ = posx

    if (posx_ > 320):
        posx_ = posx_ /2

    x = (3/320)*posx_ + 30
    x_ = (2/320)*posx_ + 23

    distance = ((x-x_) / 480)*(480-posy)+x_
    return distance

def float_range(start,stop,step):
    x = start
    my_list = []
    if step > 0:
        while x < stop:
            my_list.append(x)
            x += step
    else:
        while x > stop:
            my_list.append(x)
            x += step
    return my_list 

def AngToRad(angle):
    return angle*math.pi/180

def BruteForcePos(distance):
    target = [distance,-12]

    elb0 = 25.5 
    elb1=12.5
    elb2=20

    elb0R=[25,50]
    elb1R=[310,330]
    elb2R=[210,290]

    angs = [0,0,0]

    for ang0 in float_range(elb0R[0],elb0R[1],1):
        elb0x = elb0*math.cos(AngToRad(ang0))
        elb0y = elb0*math.sin(AngToRad(ang0))
        for ang1 in float_range(elb1R[0],elb1R[1],1):
            elb1x = elb1*math.cos(AngToRad(ang1))+elb0x
            elb1y = elb1*math.sin(AngToRad(ang1))+elb0y
            for ang2 in float_range(elb2R[0],elb2R[1],1):
                elb2x = elb2*math.cos(AngToRad(ang2+14.75))+elb1x
                elb2y = elb2*math.sin(AngToRad(ang2+14.75))+elb1y
                if(round(elb2x,0) == round(target[0],0) and round(elb2y,0) == round(target[1],0)):
                    if (round(elb1x,0) == round(elb2x,0)):
                        angs[0] = ang0
                        angs[1] = 360-(ang1-ang0)
                        angs[2] = (270-ang2)+90-((360-(ang1-ang0))-14.75)
                        #print("find " + str(ang0) + " " + str(360-(ang1-ang0)) + " "+ str((270-ang2)+90-((360-(ang1-ang0))-14.75))+ " "+str(elb0x)+" "+str(elb0y) + " "+str(elb1x)+" "+str(elb1y)+ " "+str(elb2x)+" "+str(elb2y))
    
    return angs

arm.InitCom()
arm.SendMessage("nadviazane spojenie")
add = arm.GetStreamAddress()
res = arm.GetStreamResolution()
arm.SendMessage(add)

arm.SetPosition(90,60,62,58,85,20)
th = Thread( target=NewestFrame, args=(add,) )
th.isDaemon
th.start()

while(frame is None):
    time.sleep(0.1)

conf = 0
last_center = [0,0]
arm.MovingSpeed(10)
distance = 0
WaitForMove()
time.sleep(0.500)

id = ""

while(True):
    time.sleep(0.1)
    qr,targets,center = FindQr()

    if(qr==1):
        id = targets[0][2]
        arm.DrawTargetsOnVideo(targets)
        if distance == 0:
            distance = GetDistance(center[0],center[1]) 
            arm.SendMessage("distance = " + str(distance))

        arm.TryLockAtObject(center,2,25,res)

        if(last_center[0] >= center[0]-2 and last_center[0] <= center[0]+2 and last_center[1] >= center[1]-2 and last_center[1] <= center[1]+2):
            conf = conf+1
        else:
            conf = 0
        if(conf == 3):
            act_pos = arm.GetArmPosition()
            f_pos = BruteForcePos(distance)
            conf = 0
            arm.SetPosition(float(act_pos[0])-3,-1,f_pos[1]-14.75,f_pos[2],-1,-1)
            WaitForMove()
            arm.SetPosition(-1,f_pos[0],-1,-1,-1,-1)
            WaitForMove()
            time.sleep(0.500)
            trigger = arm.GetTriggerStatus()
            
            if(trigger == "0"):
                arm.SetPosition(-1,-1,-1,f_pos[2]+10,-1,-1)
                WaitForMove()
                time.sleep(0.500)
                trigger = arm.GetTriggerStatus()
                if(trigger == "0"):
                    arm.SetPosition(-1,-1,-1,f_pos[2]-10,-1,-1)
                    WaitForMove()
                    time.sleep(0.500)
                    arm.Gripper(70)
                    WaitForMove()
                else:
                    arm.Gripper(70)
                    WaitForMove()
            else:
                act_pos = arm.GetArmPosition()
                arm.SetPosition(-1,float(act_pos[1])-3,-1,-1,-1,-1)
                WaitForMove()
                arm.Gripper(70)
                WaitForMove()

            arm.MovingSpeed(3)
            if(id == "alpha"):
                arm.SetPosition(-1,70,-1,-1,-1,-1)
                WaitForMove()
                arm.SetPosition(170,70,85,45,-1,-1)
                WaitForMove()
                arm.SetPosition(-1,-1,-1,-1,-1,20)
                WaitForMove()
            else:
                arm.SetPosition(-1,70,-1,-1,-1,-1)
                WaitForMove()
                arm.SetPosition(10,70,85,45,-1,-1)
                WaitForMove()
                arm.SetPosition(-1,-1,-1,-1,-1,20)
                WaitForMove()

            arm.SetPosition(90,60,62,58,85,20)
            WaitForMove()
            arm.MovingSpeed(10)
        last_center = center






