import Arm as arm
import time
from threading import Thread
import time


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

def Stream():
    while(1):
        time.sleep(0.05)
        a = arm.GetYoloOutput()
        print(len(a))
        if len(a) > 0:
            arm.DrawTargetsOnVideo(a)
			
def throw():
    WaitForMove()
    arm.Gripper(80)
    WaitForMove()
    arm.MovingSpeed(4)
    thro=[4,46,118,24]
    WaitForMove()
    arm.SetPosition(4,69,66,55,-1,-1)
    WaitForMove()
    arm.SetPosition(thro[0],thro[1],thro[2],thro[3],-1,-1)
    WaitForMove()
    arm.SetPosition(-1,-1,-1,-1,85,20)
    WaitForMove()
    arm.MovingSpeed(5)


time.sleep(1)
stage=[[50,65,82,50],[150,50,48,53],[50,30,1,73]]
print("bintcom")
arm.InitCom()
print("intcom")
arm.SendMessage("nadviazane spojenie")
add = arm.GetStreamAddress()
res = arm.GetStreamResolution()
arm.InitYolo(add,"C:\\Users\mt2si\\Desktop\\ArmDemos\\syringe_obj")
arm.SendMessage("Yolo spustene")
time.sleep(8)

th = Thread( target=Stream, args=() )
th.start()

arm.MovingSpeed(5)
arm.SetPosition(-1,stage[0][1],stage[0][2],stage[0][3],85,20)
WaitForMove()
arm.SetPosition(stage[0][0],-1,-1,-1,-1,-1)
WaitForMove()
arm.SetPosition(64,-1,-1,-1,-1,-1)
WaitForMove()
arm.SetPosition(-1,40,82,35,85,20) #
throw()
arm.SetPosition(-1,stage[0][1],stage[0][2],stage[0][3],85,20)
WaitForMove()
arm.SetPosition(104,-1,-1,-1,-1,-1)
WaitForMove()
arm.SetPosition(-1,43,103,9,85,20)
WaitForMove()
arm.SetPosition(-1,-1,-1,-1,21,20)#
throw()
arm.SetPosition(-1,stage[0][1],stage[0][2],stage[0][3],85,20)
WaitForMove()
arm.SetPosition(140,-1,-1,-1,-1,-1)
WaitForMove()

arm.SetPosition(-1,stage[1][1],stage[1][2],stage[1][3],85,20)
WaitForMove()
arm.SetPosition(100,-1,-1,-1,-1,-1)
WaitForMove()
arm.SetPosition(106.84,-1,-1,-1,-1,-1)
WaitForMove()
arm.SetPosition(-1,30,40,46,85,20)
WaitForMove()
arm.SetPosition(-1,30,40,46,16,20)
throw()


arm.SetPosition(-1,stage[1][1],stage[1][2],stage[1][3],85,20)
WaitForMove()
arm.SetPosition(30,-1,-1,-1,-1,-1)
WaitForMove()
arm.SetPosition(-1,stage[1][1],stage[1][2],stage[1][3],85,20)
WaitForMove()
arm.SetPosition(70,-1,-1,-1,-1,-1)
WaitForMove()
arm.SetPosition(-1,31.16,40,41,85,20) #
throw()

arm.SetPosition(90,69,66,55,-1,-1)

th._stop()
th.join()

a = input()
arm.CloseCom()

