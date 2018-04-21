import math 

def AngToRad(angle):
    return angle*math.pi/180

def float_range(start,stop,step):
    x = start
    my_list = []
    if step > 0:
        while x < stop:
            my_list.append(x)
            x += step
    else: # should really be if step < 0 with an extra check for step == 0 
        while x > stop:
            my_list.append(x)
            x += step
    return my_list 

target = [20,-12]

elb0 = 25.5 
elb1=12.5
elb2=20

elb0R=[0,90]
elb1R=[300,360]
elb2R=[200,300]

for ang0 in float_range(elb0R[0],elb0R[1],1):
    elb0x = elb0*math.cos(AngToRad(ang0))
    elb0y = elb0*math.sin(AngToRad(ang0))
    for ang1 in float_range(elb1R[0],elb1R[1],1):
        elb1x = elb1*math.cos(AngToRad(ang1))+elb0x
        elb1y = elb1*math.sin(AngToRad(ang1))+elb0y
        for ang2 in float_range(elb2R[0],elb2R[1],1):
            elb2x = elb2*math.cos(AngToRad(ang2+14.75))+elb1x
            elb2y = elb2*math.sin(AngToRad(ang2+14.75))+elb1y
            if(round(elb2x,0) == target[0] and round(elb2y,0) == target[1]):
                print("find " + str(ang0) + " " + str(360-(ang1-ang0)) + " "+ str(270-ang2)+ " "+str(elb0x)+" "+str(elb0y) + " "+str(elb1x)+" "+str(elb1y)+ " "+str(elb2x)+" "+str(elb2y))
                #if (round(elb1x,0) == round(elb2x,0)):
            