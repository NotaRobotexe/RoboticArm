import Arm as arm
print("pred in")
arm.InitCom()
print("za in")
msg = arm.ReadInput()
print(msg)
arm.SendMessage(msg)
print("za s")
