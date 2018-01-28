import Arm as arm

arm.InitCom("127.0.0.1")
mess = arm.ReadInput()
print(mess)
if mess="ACK":
    arm.SetPosition(90,120,40,55,-1,-1)
arm.CloseCom()