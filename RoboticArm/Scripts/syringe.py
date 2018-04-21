import Arm as arm
import time
from threading import Thread

lastpos = []
ismove = 0
res = 0

def WaitForMove():
	global ismove
	ismove = 0
	while(1):
		a = arm.IsMoving()
		if a == 0:
			ismove = 1
			break
		time.sleep(0.5)

def LookForObject():
	global ismove
	global lastpos
	while(ismove == 0):
		a = arm.GetYoloOutput()
		if len(a) > 0:
			arm.DrawTargetsOnVideo(a)
			arm.StopMovement()
			lastpos = arm.GetArmPosition()
			GetObject()	

def GetObject():
	global lastpos
	base = float(lastpos[1])
	compensation = float(lastpos[3])
	pos = "w"
	while(1):
		a = arm.GetYoloOutput()
		if len(a) > 0:
			pos = a[0][3]
			arm.DrawTargetsOnVideo(a)
			lock = arm.TryLockAtObject(a[0],3,40,res)
			if(lock == 1):
				base = base-3
				compensation = compensation-3
				arm.SetPosition(-1,base,-1,compensation,-1,-1)
				WaitForMove()
				trigger = arm.GetTriggerStatus()
				if(trigger == "1"):
					break

	GetAndThrowiT(pos)


def GetAndThrowiT(pos):
	if(pos == "h"):
		arm.SetPosition(-1,-1,-1,-1,85,-1)
		WaitForMove()

	else:
		pos = arm.GetArmPosition()
		arm.SetPosition(-1,float(pos[1])+5,-1,-1,160,-1)
		WaitForMove()
		arm.SetPosition(-1,-1,-1,-1,160,-1)
		WaitForMove()
		arm.SetPosition(-1,float(pos[1]),-1,-1,160,-1)
		WaitForMove()
		if(arm.GetTriggerStatus != "1"):
			arm.SetPosition(-1,float(pos[1])-2,-1,-1,160,-1)
			WaitForMove()


	arm.Gripper(90)
	WaitForMove()
	arm.MovingSpeed(2)
	arm.SetPosition(-1,80,-1,-1,-1,-1)
	WaitForMove()
	arm.SetPosition(6,50,80,27,85,-1)
	WaitForMove()
	arm.Gripper(20)
	WaitForMove()
	arm.SetPosition(lastpos[0],lastpos[1],lastpos[2],lastpos[3],lastpos[4],lastpos[5])
	arm.MovingSpeed(5) 


time.sleep(1)
stage=[[40,65,82,50],[135,50,48,53],[50,30,1,73]]
arm.InitCom()
arm.SendMessage("nadviazane spojenie")
add = arm.GetStreamAddress()
res = arm.GetStreamResolution()
arm.InitYolo(add,"C:\\Users\mt2si\\Desktop\\ArmDemos\\syringe_obj")
arm.SendMessage("Yolo spustene")

for st in range(3):
	arm.SendMessage(st)
	arm.MovingSpeed(5)
	arm.SetPosition(stage[st][0],stage[st][1],stage[st][2],stage[st][3],85,20)
	WaitForMove()
	interupted = 0
	while(interupted == 0):
		arm.SetPosition(180-stage[st][0],stage[st][1],stage[st][2],stage[st][3],85,20)
		th = Thread( target=WaitForMove, args=() )
		th.isDaemon
		th.start()
		LookForObject()
		stat = arm.IsMoving()
		print("asdasdasdasd"+str(stat))
		

a = input()
arm.CloseCom()