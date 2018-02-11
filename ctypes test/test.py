
from ctypes import *
yolo = cdll.LoadLibrary("c:\\yolo_cpp_dll.dll")
cdll.Detector("c:\\Users\\mt2si\\Desktop\\yolot\\yolo-obj.cfg","c:\\Users\\mt2si\\Desktop\\yolot\\yolo-obj_5000.weights",1)
#print(cdll.detect("C:\\Users\\mt2si\\Desktop\\yolot\\test.jpg",0.2))

