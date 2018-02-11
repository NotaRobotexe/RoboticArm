import Arm as arm

arm.InitCom()
arm.MovingSpeed(5)
print("speed")
pos = "test"
arm.SendMessage(pos)
print("pos")
arm.SetPosition(20,50,100,80,30,80)

pos = "sada"
arm.SendMessage(pos)