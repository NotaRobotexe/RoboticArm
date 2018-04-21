import Arm as arm

arm.InitCom()
arm.SendMessage("nadviazane spojenie")
add = arm.GetStreamAddress()
res = arm.GetStreamResolution()
arm.SendMessage(add)
arm.SendMessage(res)
