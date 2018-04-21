import Arm as arm
import numpy as np
import cv2

arm.InitCom()
res = arm.GetStreamResolution()
add = arm.GetStreamAddress()
arm.SendMessage(str(res))
arm.SendMessage(str(add))
arm.CloseCom()