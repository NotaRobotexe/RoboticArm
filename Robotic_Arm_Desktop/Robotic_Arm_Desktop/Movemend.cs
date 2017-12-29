﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Robotic_Arm_Desktop
{
    class Arm
    {
        public const double minF = 102;  //hz or dnk what is it
        public const double maxF = 576;
        public double AngleInHz = 102; //in that hz or what is that shit
        public double AngleInDegree = 0;
        public double maxAngle; // in angle
        public double maxUseAngle; // in angle
        public double startfrom = 0; // in angle
        private double OneDegree;

        public Arm(double maxAngle)
        {
            this.maxAngle = maxAngle;
            OneDegree = (maxF - minF) / maxAngle;
        }

        public void SetAngleFromKeyboadrOrGamepad(double increment,string part, NetworkCom netMove)
        {
            double MaxPosition = maxF;
            if ((minF + AngleToHz(startfrom) + AngleToHz(maxUseAngle))< maxF)
            {
                MaxPosition = (minF + AngleToHz(startfrom) + AngleToHz(maxUseAngle));
            }

            if (AngleInHz+increment > (minF+AngleToHz(startfrom)) && AngleInHz+increment < MaxPosition)
            {
                AngleInHz += increment;
                AngleInDegree = (AngleInHz-minF) / OneDegree;

                if (part != "0")
                {
                    netMove.SendData("00"+part+AngleInHz.ToString());
                }
                else
                {
                    int Angle = (int)Math.Round(AngleInDegree);
                    string sAngle;
                    if (Angle<10)
                    {
                        sAngle = "00" + Angle.ToString();
                    }
                    else if (Angle<100)
                    {
                        sAngle = "0" + Angle.ToString();
                    }
                    else
                    {
                        sAngle = Angle.ToString();
                    }
                    netMove.SendData("00" + part + Angle);
                    Console.WriteLine(sAngle);
                }



            }
        }

        public void SetAngle(double angle) //TODO: nastavit nastavovanie stupnou aj priamo. neskor to bude potrebne
        {

        }

        public double AngleToHz(double angle)
        {
            return angle * OneDegree;
        }

        public void updateAngle()
        {
            AngleInDegree = (AngleInHz-minF) / OneDegree;
        }

        public void updateHz()
        {
            AngleInHz = (AngleInDegree * OneDegree) + minF;
        }

    }

    class Movemend
    {
        public Arm baseMovemend = new Arm(360);
        public Arm elbow0 = new Arm(120);
        public Arm elbow1 = new Arm(120);
        public Arm elbow2 = new Arm(120);
        public Arm griper = new Arm(180);
        public Arm griperRotation = new Arm(180);

        public bool gamepadEnabled = true;
        public bool keyboardenabled = true;
        public bool wrongMode = false;

        public short keyboardMovingArm=0;

        public float valueCountExp = (float)0.001;
        public float valueCount = 1; //on how much will value increment

        public void StartAndQuitPosition() 
        {
            baseMovemend.AngleInDegree = 180;
            elbow0.AngleInDegree = 0;
            elbow1.AngleInDegree = 0;
            elbow2.AngleInDegree = 20;
            griper.AngleInDegree = 0;
            griperRotation.AngleInDegree = 0;

            baseMovemend.updateHz();
            elbow0.updateHz();
            elbow1.updateHz();
            elbow2.updateHz();
            griper.updateHz();
            griperRotation.updateHz();

        }

        public void AnalizeData(GamepadState data,NetworkCom netMove) 
        {
                //value 143yellow 79red 47green 31blue 6left 0up 2right 4down 15nothing|7. 0nothing 4left down 1left up 2right up 8 right down 16 select 32 start 64 right press 128 left press | 8. mode 64/192
            if(data.mode != 64)
            {
                wrongMode = true;
            }
            else
            {
                wrongMode = false;
            }

            if (data.button == 0)
            {
                if (valueCount+valueCountExp < 25)
                {
                    valueCount += valueCountExp;
                }
            }
            else if (data.button == 4)
            {
                if (valueCount-valueCountExp>0)
                {
                    valueCount -= valueCountExp;
                }
            }

            if (data.frontButton==8)
            {
                elbow0.SetAngleFromKeyboadrOrGamepad(valueCount,"1",netMove);
            }
            else if (data.frontButton == 4)
            {
                elbow0.SetAngleFromKeyboadrOrGamepad(-(valueCount),"1",netMove);
            }
            else if (data.frontButton == 1)
            {
                griper.SetAngleFromKeyboadrOrGamepad(-valueCount,"5", netMove);
            }
            else if (data.frontButton == 2)
            {
                griper.SetAngleFromKeyboadrOrGamepad(valueCount, "5", netMove);
            }

            if (data.leftStickVer>0)
            {
                elbow1.SetAngleFromKeyboadrOrGamepad(valueCount, "2", netMove);
            }
            else if (data.leftStickVer < 0)
            {
                elbow1.SetAngleFromKeyboadrOrGamepad(-(valueCount), "2", netMove);
            }

            if (data.rightStickVer > 0)
            {
                elbow2.SetAngleFromKeyboadrOrGamepad(valueCount, "3", netMove);
            }
            else if (data.rightStickVer < 0)
            {
                elbow2.SetAngleFromKeyboadrOrGamepad(-(valueCount), "3", netMove);
            }

            if (data.rightStickHor > 0)
            {
                griperRotation.SetAngleFromKeyboadrOrGamepad(valueCount, "4", netMove);
            }
            else if (data.rightStickHor < 0)
            {
                griperRotation.SetAngleFromKeyboadrOrGamepad(-(valueCount), "4", netMove);
            }

            if (data.leftStickHor > 0)
            {
                baseMovemend.SetAngleFromKeyboadrOrGamepad(valueCount, "0", netMove);
            }
            else if (data.leftStickHor < 0)
            {
                baseMovemend.SetAngleFromKeyboadrOrGamepad(-(valueCount), "0", netMove);
            }

        }

        public void AnalizeData(Key key, NetworkCom netMove) //for keyboard
        {

            if (key == Key.R)
            {
                if (valueCount + valueCountExp < 25)
                {
                    valueCount += (float)0.1;
                }
            }
            else if (key == Key.F)
            {
                if (valueCount - valueCountExp > 0)
                {
                    valueCount -= (float)0.1;
                }
            }

            if (key == Key.W && keyboardMovingArm ==0)
            {
                elbow0.SetAngleFromKeyboadrOrGamepad(valueCount, "1", netMove);
            }
            else if (key == Key.S && keyboardMovingArm == 0)
            {
                elbow0.SetAngleFromKeyboadrOrGamepad(-(valueCount), "1", netMove);
            }


            if (key == Key.W && keyboardMovingArm == 1)
            {
                elbow1.SetAngleFromKeyboadrOrGamepad(valueCount, "2", netMove);
            }
            else if (key == Key.S && keyboardMovingArm == 1)
            {
                elbow1.SetAngleFromKeyboadrOrGamepad(-(valueCount), "2", netMove);
            }

            if (key == Key.W && keyboardMovingArm == 2)
            {
                elbow2.SetAngleFromKeyboadrOrGamepad(valueCount, "3", netMove);
            }
            else if (key == Key.S && keyboardMovingArm == 2)
            {
                elbow2.SetAngleFromKeyboadrOrGamepad(-(valueCount), "3", netMove);
            }

            if (key == Key.LeftShift)
            {
                griper.SetAngleFromKeyboadrOrGamepad(-valueCount, "5", netMove);
            }
            else if (key == Key.CrSel)
            {
                griper.SetAngleFromKeyboadrOrGamepad(valueCount, "5", netMove);
            }

            if (key == Key.E)
            {
                griperRotation.SetAngleFromKeyboadrOrGamepad(valueCount, "4", netMove);
            }
            else if (key == Key.Q)
            {
                griperRotation.SetAngleFromKeyboadrOrGamepad(-(valueCount), "4", netMove);
            }

            if (key == Key.A)
            {
                baseMovemend.SetAngleFromKeyboadrOrGamepad(valueCount, "0", netMove);
            }
            else if (key == Key.D)
            {
                baseMovemend.SetAngleFromKeyboadrOrGamepad(-(valueCount), "0", netMove);
            }
        }

        /*public string dataString()
        {
            string s = " *e0 " + elbow0.value.ToString() + " *e1 " + elbow1.value.ToString() + " *e2 " + elbow2.value.ToString() + " *g " + griper.value.ToString() + " *gr " + griperRotation.value.ToString() + " value: "+ valueCount.ToString();
            return s;
        }*/
    }
}
