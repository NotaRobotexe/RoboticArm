using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Robotic_Arm_Desktop
{
    public class Arm
    {
        public static event EventHandler PositonChange;

        private const double PwmPerDegree = 2.633333333333333333333333333333333;
        public const double min_Pwm = 102;
        public const double max_Pwm = 576;

        public double AngleInPWM = 102; 
        public double AngleInDegree = 0;

        public double EndAt; 
        public double startfrom = 102;

        public void SetPostionFromKeyboadrOrGamepad(double increment)
        {
            if (AngleInPWM+increment >= startfrom && AngleInPWM+increment <= EndAt)
            {
                Update(AngleInPWM + increment, 1);
                OnPositionChange(EventArgs.Empty);
            }
        }

        public void Update(double pos,int mode) //mode: 0 angle 1 pwm
        {
            if (mode == 0)
            {
                AngleInDegree = pos;
                AngleInPWM = (pos * PwmPerDegree) + min_Pwm;
            }
            else
            {
                AngleInPWM = pos;
                AngleInDegree = (pos - min_Pwm) / PwmPerDegree;
            }
            OnPositionChange(EventArgs.Empty);
        }

        public static double DegreeToPwm(double angle)
        {
            return min_Pwm + (angle * PwmPerDegree);
        }

        public static double PwmToDegree(double pwm)
        {
            return (pwm - min_Pwm) / PwmPerDegree;
        }

        protected virtual void OnPositionChange(EventArgs e)
        {
            EventHandler eventHandler = PositonChange;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

    }

    public class Movement
    {
        public event EventHandler IncrementationChange;

        public Arm baseMovemend = new Arm();
        public Arm elbow0 = new Arm();
        public Arm elbow1 = new Arm();
        public Arm elbow2 = new Arm();
        public Arm griper = new Arm();
        public Arm griperRotation = new Arm();

        public bool gamepadEnabled = true;
        public bool keyboardenabled = true;

        public short keyboardMovingArm=0; // moving with witch part of arm 0=el0 1=el1 2=el2 

        public float valueCountExp = 0.01f;
        public float valueCount = 2.5f; //on how much will value increment


        public void AnalizeData(GamepadState data) 
        {
            if (data.button10 > 0)
            {
                if (valueCount+valueCountExp < 25)
                {
                    valueCount += valueCountExp;
                    OnIncrementationChange(EventArgs.Empty);
                }
            }
            else if (data.button11 > 0)
            {
                if (valueCount-valueCountExp>0)
                {
                    valueCount -= valueCountExp;
                    OnIncrementationChange(EventArgs.Empty);
                }
            }

            if (data.button7 > 0)
            {
                elbow0.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }
            else if (data.button6 > 0)
            {
                elbow0.SetPostionFromKeyboadrOrGamepad((valueCount));
            }

            if (data.button4 > 0)
            {
                griper.SetPostionFromKeyboadrOrGamepad(-valueCount);
            }
            else if (data.button5 > 0)
            {
                griper.SetPostionFromKeyboadrOrGamepad(valueCount);
            }

            if (data.y > 32511)
            {
                elbow1.SetPostionFromKeyboadrOrGamepad(valueCount);
            }
            else if (data.y < 32511)
            {
                elbow1.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }

            if (data.button2 > 0)
            {
                elbow2.SetPostionFromKeyboadrOrGamepad(valueCount);
            }
            else if (data.button0 > 0)
            {
                elbow2.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }

            if (data.button3 > 0)
            {
                griperRotation.SetPostionFromKeyboadrOrGamepad(valueCount);
            }
            else if (data.button1 > 0)
            {
                griperRotation.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }

            if (data.x > 32511)
            {
                baseMovemend.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }
            else if (data.x < 32511)
            {
                baseMovemend.SetPostionFromKeyboadrOrGamepad((valueCount));
            }

        }

        public void AnalizeData(Key key) //for keyboard
        {
            if (key == Key.R)
            {
                if (valueCount + valueCountExp < 25)
                {
                    valueCount += valueCountExp;
                    OnIncrementationChange(EventArgs.Empty);
                }
            }
            else if (key == Key.F)
            {
                if (valueCount - valueCountExp > 0)
                {
                    valueCount -= valueCountExp;
                    OnIncrementationChange(EventArgs.Empty);
                }
            }

            if (key == Key.W && keyboardMovingArm ==0)
            {
                elbow0.SetPostionFromKeyboadrOrGamepad(valueCount);
            }
            else if (key == Key.S && keyboardMovingArm == 0)
            {
                elbow0.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }


            if (key == Key.W && keyboardMovingArm == 1)
            {
                elbow1.SetPostionFromKeyboadrOrGamepad(valueCount);
            }
            else if (key == Key.S && keyboardMovingArm == 1)
            {
                elbow1.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }

            if (key == Key.W && keyboardMovingArm == 2)
            {
                elbow2.SetPostionFromKeyboadrOrGamepad(valueCount);
            }
            else if (key == Key.S && keyboardMovingArm == 2)
            {
                elbow2.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }

            if (key == Key.LeftShift)
            {
                griper.SetPostionFromKeyboadrOrGamepad(-valueCount);
            }
            else if (key == Key.Z)
            {
                griper.SetPostionFromKeyboadrOrGamepad(valueCount);
            }

            if (key == Key.E)
            {
                griperRotation.SetPostionFromKeyboadrOrGamepad(valueCount);
            }
            else if (key == Key.Q)
            {
                griperRotation.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }

            if (key == Key.A)
            {
                baseMovemend.SetPostionFromKeyboadrOrGamepad(valueCount);
            }
            else if (key == Key.D)
            {
                baseMovemend.SetPostionFromKeyboadrOrGamepad(-(valueCount));
            }
        }

        protected virtual void OnIncrementationChange(EventArgs e)
        {
            EventHandler eventHandler = IncrementationChange;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

    }
}
