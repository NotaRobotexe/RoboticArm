import socket
import Arm as arm
import time 

port = 6979
sck = 0
tcp_count = 128
new_sck = 0

def InitCom():
    tcp_ip = "0.0.0.0"
    global sck
    global new_sck
    sck = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sck.bind((tcp_ip, port))
    sck.listen(1)
    new_sck,addr = sck.accept()

def Receive():
    data = new_sck.recv(tcp_count).decode('UTF-8')
    return data

def FirstGum1():
    arm.SetPosition(91,43.3,99.5,16,170,22)
    WaitForMove()
    arm.SetPosition(92.66,45.96,105.58,9.49,12.91,-1)
    WaitForMove()
    arm.Gripper(85)
    WaitForMove()
    arm.SetPosition(91,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,64.81,77,40,93,-1)
    WaitForMove()
    arm.Gripper(34)
    WaitForMove()


def SecondGum1():
    time.sleep(0.5)
    WaitForMove()
    arm.SetPosition(91,43.3,99.5,16,170,22)
    WaitForMove()
    arm.SetPosition(91.43,41,99.5,15.24,171.14,-1)
    WaitForMove()
    arm.Gripper(85)
    WaitForMove()
    arm.SetPosition(91,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,64.81,77,40,93,-1)
    WaitForMove()
    arm.Gripper(34)

def FirstGum2():
    time.sleep(0.5)
    WaitForMove()
    arm.SetPosition(90.19,77.97,65.9,65.64,93.34,30)
    WaitForMove()
    arm.SetPosition(90.88,28.56,30.45,63.08,177.49,35)
    WaitForMove()
    arm.Gripper(85)
    WaitForMove()
    arm.SetPosition(90.88,35.22,30.85,63.08,177.49,-1)
    WaitForMove()
    arm.SetPosition(90.67,38.51,29.02,65.84,171.6,-1)
    WaitForMove()
    arm.SetPosition(91,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,64.81,77,40,93,-1)
    WaitForMove()
    arm.Gripper(34)
    
def SecondGum2():
    time.sleep(0.5)
    WaitForMove()
    arm.SetPosition(90.19,77.97,65.9,65.64,93.34,35)
    WaitForMove()
    arm.SetPosition(90.88,28.56,30.45,63.08,177.49,35)
    WaitForMove()
    arm.SetPosition(90.88,28.0,37.1,52.13,177.49,35)
    WaitForMove()
    arm.Gripper(85)
    WaitForMove()
    arm.SetPosition(90.88,29.22,30.85,64.64,177.49,-1)
    WaitForMove()
    arm.SetPosition(90.67,38.51,29.02,65.84,171.6,-1)
    WaitForMove()
    arm.SetPosition(91,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,64.81,77,40,93,-1)
    WaitForMove()
    arm.Gripper(34)
    

def Firsttic():
    time.sleep(1)
    WaitForMove()
    arm.SetPosition(90.19,77.97,65.9,65.64,93.34,32)
    WaitForMove()
    arm.SetPosition(90.62,42.35,48.6,63.81,171.11,32)
    WaitForMove()
    arm.SetPosition(90.62,36.48,57.99,46.59,171.11,32)
    WaitForMove()
    arm.Gripper(85)
    WaitForMove()
    arm.SetPosition(90.62,36.48,57.99,46.59,171.11,-1)
    WaitForMove()
    arm.SetPosition(90.62,42.35,48.6,63.81,171.11,-1)
    WaitForMove()
    arm.SetPosition(91,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,64.81,77,40,93,-1)
    WaitForMove()
    arm.Gripper(34)

def Secondtic():
    WaitForMove()
    arm.SetPosition(91.19,77.97,65.9,65.64,93.34,32)
    WaitForMove()
    arm.SetPosition(91.62,42.35,48.6,63.81,171.11,32)
    WaitForMove()
    arm.SetPosition(91.62,36.08,63.07,35.64,171.11,32)
    WaitForMove()
    arm.Gripper(85)
    WaitForMove()
    arm.SetPosition(92.62,36.48,57.99,46.59,171.11,-1)
    time.sleep(1)
    WaitForMove()
    arm.SetPosition(91.62,42.35,48.6,63.81,171.11,-1)
    WaitForMove()
    arm.SetPosition(91,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,110,79,45,170,-1)
    WaitForMove()
    arm.SetPosition(15,64.81,77,40,93,-1)
    WaitForMove()
    arm.Gripper(34)

def WaitForMove():
	while(1):
		a = arm.IsMoving()
		if a == 0:
			break
		time.sleep(0.5)


arm.InitCom()
InitCom()
while(1):
    a = Receive()
    order = a.split("*")
    print(a)
    arm.MovingSpeed(2)

    if order[0] == "1":
        FirstGum1()

    if order[0] == "2": 
        FirstGum1()
        SecondGum1()

    if order[1] == "1":
        Firsttic()

    if order[1] == "2":
        Firsttic()
        Secondtic()
        
    if order[2] == "1":
        FirstGum2()

    if order[2] == "2":
        FirstGum2()
        SecondGum2()


