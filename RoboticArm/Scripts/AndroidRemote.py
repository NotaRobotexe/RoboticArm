import socket
import Arm as arm

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

arm.InitCom()
arm.SendMessage("skript spusteny")
InitCom()
arm.SendMessage("clien pripojeny")

while(1):
    a = Receive()
    pos = arm.GetArmPosition()
    b = arm.IsMoving()
    print(b)
    if a == "0u":
        if b == 0:
            print("asd")
            arm.SetPosition(pos[0],(float(pos[1])+15),pos[2],pos[3],pos[4],pos[5])
    elif a == "0d":
        if b == 0:
            arm.SetPosition(pos[0],(float(pos[1])-15),pos[2],pos[3],pos[4],pos[5])
    elif a == "1d":
        if b == 0:
            arm.SetPosition(pos[0],pos[1],(float(pos[2])-20),pos[3],pos[4],pos[5])
    elif a == "1u":
        if b == 0:
            arm.SetPosition(pos[0],pos[1],(float(pos[2])+20),pos[3],pos[4],pos[5]) 
    elif a == "2d":
        if b == 0:
            arm.SetPosition(pos[0],pos[1],pos[2],(float(pos[3])-20),pos[4],pos[5])
    elif a == "2u":
        if b == 0:
            arm.SetPosition(pos[0],pos[1],pos[2],(float(pos[3])+20),pos[4],pos[5])      
    elif a == "b1":
        if b == 0:
            arm.SetPosition((float(pos[0])-20),pos[1],pos[2],pos[3],pos[4],pos[5])           
    elif a == "b2":
        if b == 0:
            arm.SetPosition((float(pos[0])+20),pos[1],pos[2],pos[3],pos[4],pos[5])               
    elif a == "go":
        if b == 0:
            arm.SetPosition(pos[0],pos[1],pos[2],pos[3],(float(pos[4])-10),pos[5]) 
    elif a == "gc":
        if b == 0:
            arm.SetPosition(pos[0],pos[1],pos[2],pos[3],(float(pos[4])+10),pos[5]) 
    elif a == "gr":
        if b == 0:
            arm.SetPosition(pos[0],pos[1],pos[2],pos[3],pos[4],(float(pos[5])+10)) 
    elif a == "gt":
        if b == 0:
            arm.SetPosition(pos[0],pos[1],pos[2],pos[3],pos[4],(float(pos[5])-10)) 

    

