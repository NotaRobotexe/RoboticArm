import Arm as arm
import time

def WaitForMove():
	while(1):
		a = arm.IsMoving()
		if a == 0:
			break
		time.sleep(0.1)

arm.InitCom()

pos = arm.GetArmPosition()
elb2 = 0.5
for i in range(0,60,1):
    arm.SetPosition(-1,float(pos[1])-i,float(pos[2])+elb2,float(pos[3])-elb2,-1,-1)
    elb2 = elb2 + 0.5
    WaitForMove()
    pass