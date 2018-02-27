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
    arm.SetPosition(92.66,49,105.5,11.39,12.91,41)
    WaitForMove()
    arm.SetPosition(92.66,45.96,105.58,9.49,12.91,-1)
    WaitForMove()
    arm.Gripper(70)
    WaitForMove()
    arm.SetPosition(92.66,113,52,57,13,-1)
    WaitForMove()
    arm.SetPosition(2,113,52,57,13,-1)
    WaitForMove()
    arm.SetPosition(2,61,72,41,13,-1)
    WaitForMove()
    arm.Gripper(45)
    WaitForMove()
    arm.SetPosition(92.66,113,52,57,13,-1)


def SecondGum1():
    print("fg2")

def FirstGum2():
    print("sg1")

def SecondGum2():
    print("sg2")

def Firsttic():
    print("t1")

def Secondtic():
    print("t2")

def WaitForMove():
    while(1):
        time.sleep(0.5)
        a = arm.IsMoving()
        if a == 0:
            break

arm.InitCom()
InitCom()
while(1):
    a = Receive()
    order = a.split("*")
    print(a)

    if order[0] == "1":
        FirstGum1()

    if order[0] == "2": 
        FirstGum1()
        SecondGum1()

    if order[2] == "1":
        FirstGum2()

    if order[2] == "2":
        FirstGum2()
        SecondGum2()

    if order[1] == "1":
        Firsttic()

    if order[1] == "2":
        Firsttic()
        Secondtic()

