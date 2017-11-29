using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Robotic_Arm_Desktop
{
    static class AutoModeTemplate
    {
        const int sleepTime = 15;
        const int incrementation = 1;

        public static async Task StartTemplateAsync(List<string> commands,Movemend movemend, _3Dmodel model,TextBox textBox)
        {
            int reps;
            if (textBox.Text == "inf")
            {
                reps = int.MaxValue;
            }
            else
            {
                reps = Convert.ToInt32(textBox.Text);
            }

            for (int i = 0; i < reps; i++)
            {
                foreach (var command in commands)
                {
                    if (Global.stop == false)
                    {
                        List<double> instructions = Deserialization(command);

                        await Task.Run(() => {

                            if (instructions[9] == 1) //fastmode 
                            {
                                MovingFastMode(instructions, movemend);
                            }
                            else //normal speed
                            {
                                bool allMotorsOnPositions = false;
                                do
                                {
                                    if (Global.stop == false)
                                    {
                                        allMotorsOnPositions = Moving(instructions, movemend);
                                        Thread.Sleep(sleepTime / Convert.ToInt16(instructions[8]));
                                    }
                                    else
                                    {
                                        break;
                                    }

                                } while (allMotorsOnPositions == false);

                            }

                            if (instructions[6] == 1) //wait for trigger
                            {
                                while (true)
                                {
                                    if (Global.stop == false)
                                    {
                                        if (Global.triggered == false)
                                        {
                                            Thread.Sleep(300);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            Thread.Sleep(Convert.ToInt16(instructions[7]));
                        });
                    }
                    else
                    {
                        break;
                    }
                }
                if (Global.stop == true)
                {
                    break;
                }

                textBox.Text = (reps-i).ToString();
            }
            
            Global.autoModeRunning = false;
        }

        static void MovingFastMode(List<double> instruction, Movemend movemend)
        {
            movemend.elbow0.AngleInHz = instruction[1];
            movemend.elbow1.AngleInHz = instruction[2];
            movemend.elbow2.AngleInHz = instruction[3];
            movemend.griperRotation.AngleInHz = instruction[4];
            movemend.griper.AngleInHz = instruction[5];
            movemend.baseMovemend.AngleInHz = instruction[0];

            movemend.elbow0.updateAngle();
            movemend.elbow1.updateAngle();
            movemend.elbow2.updateAngle();
            movemend.griperRotation.updateAngle();
            movemend.griper.updateAngle();
            movemend.baseMovemend.updateAngle();
        }

        static bool Moving(List<double> instruction, Movemend movemend)
        {
            int onPosition = 0;

            if (instruction[1] > movemend.elbow0.AngleInHz)
            {
                if (movemend.elbow0.AngleInHz+incrementation > instruction[1])
                {
                    movemend.elbow0.AngleInHz = instruction[1];
                    movemend.elbow0.updateAngle();
                }
                else
                {
                    movemend.elbow0.AngleInHz += incrementation;
                    movemend.elbow0.updateAngle();
                }

            }
            else if (instruction[1] < movemend.elbow0.AngleInHz)
            {
                if (movemend.elbow0.AngleInHz - incrementation < instruction[1])
                {
                    movemend.elbow0.AngleInHz = instruction[1];
                    movemend.elbow0.updateAngle();

                }
                else
                {
                    movemend.elbow0.AngleInHz -= incrementation;
                    movemend.elbow0.updateAngle();
                }
            }
            else
            {
                onPosition++;
            }


            if (instruction[2] > movemend.elbow1.AngleInHz)
            {
                if (movemend.elbow1.AngleInHz + incrementation > instruction[2])
                {
                    movemend.elbow1.AngleInHz = instruction[2];
                    movemend.elbow1.updateAngle();
                }
                else
                {
                    movemend.elbow1.AngleInHz += incrementation;
                    movemend.elbow1.updateAngle();

                }

            }
            else if (instruction[2] < movemend.elbow1.AngleInHz)
            {
                if (movemend.elbow1.AngleInHz - incrementation < instruction[2])
                {
                    movemend.elbow1.AngleInHz = instruction[2];
                    movemend.elbow1.updateAngle();

                }
                else
                {
                    movemend.elbow1.AngleInHz -= incrementation;
                    movemend.elbow1.updateAngle();

                }
            }
            else
            {
                onPosition++;
            }

            if (instruction[3] > movemend.elbow2.AngleInHz)
            {
                if (movemend.elbow2.AngleInHz + incrementation > instruction[3])
                {
                    movemend.elbow2.AngleInHz = instruction[3];
                    movemend.elbow2.updateAngle();
                }
                else
                {
                    movemend.elbow2.AngleInHz += incrementation;
                    movemend.elbow2.updateAngle();
                }

            }
            else if (instruction[3] < movemend.elbow2.AngleInHz)
            {
                if (movemend.elbow2.AngleInHz - incrementation < instruction[3])
                {
                    movemend.elbow2.AngleInHz = instruction[3];
                    movemend.elbow2.updateAngle();
                }
                else
                {
                    movemend.elbow2.AngleInHz -= incrementation;
                    movemend.elbow2.updateAngle();
                }
            }
            else
            {
                onPosition++;
            }

            if (instruction[4] > movemend.griperRotation.AngleInHz)
            {
                if (movemend.griperRotation.AngleInHz + incrementation > instruction[4])
                {
                    movemend.griperRotation.AngleInHz = instruction[4];
                    movemend.griperRotation.updateAngle();
                }
                else
                {
                    movemend.griperRotation.AngleInHz += incrementation;
                    movemend.griperRotation.updateAngle();
                }

            }
            else if (instruction[4] < movemend.griperRotation.AngleInHz)
            {
                if (movemend.griperRotation.AngleInHz - incrementation < instruction[4])
                {
                    movemend.griperRotation.AngleInHz = instruction[4];
                    movemend.griperRotation.updateAngle();
                }
                else
                {
                    movemend.griperRotation.AngleInHz -= incrementation;
                    movemend.griperRotation.updateAngle();
                }
            }
            else
            {
                onPosition++;
            }

            if (instruction[5] > movemend.griper.AngleInHz)
            {
                if (movemend.griper.AngleInHz + incrementation > instruction[5])
                {
                    movemend.griper.AngleInHz = instruction[5];
                    movemend.griper.updateAngle();
                }
                else
                {
                    movemend.griper.AngleInHz += incrementation;
                    movemend.griper.updateAngle();
                }

            }
            else if (instruction[5] < movemend.griper.AngleInHz)
            {
                if (movemend.griper.AngleInHz - incrementation < instruction[5])
                {
                    movemend.griper.AngleInHz = instruction[5];
                    movemend.griper.updateAngle();
                }
                else
                {
                    movemend.griper.AngleInHz -= incrementation;
                    movemend.griper.updateAngle();
                }
            }
            else
            {
                onPosition++;
            }

            if (instruction[0] > movemend.baseMovemend.AngleInHz)
            {
                if (movemend.baseMovemend.AngleInHz + incrementation > instruction[0])
                {
                    movemend.baseMovemend.AngleInHz = instruction[0];
                    movemend.baseMovemend.updateAngle();
                }
                else
                {
                    movemend.baseMovemend.AngleInHz += incrementation;
                    movemend.baseMovemend.updateAngle();
                }

            }
            else if (instruction[0] < movemend.baseMovemend.AngleInHz)
            {
                if (movemend.baseMovemend.AngleInHz - incrementation < instruction[0])
                {
                    movemend.baseMovemend.AngleInHz = instruction[0];
                    movemend.baseMovemend.updateAngle();
                }
                else
                {
                    movemend.baseMovemend.AngleInHz -= incrementation;
                    movemend.baseMovemend.updateAngle();
                }
            }
            else
            {
                onPosition++;
            }

            if (onPosition == 6)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static List<double> Deserialization(string command)
        {
            List<string> instructionsRaw = command.Split('*').ToList();
            List<double> instructions = new List<double>();

            foreach (var item in instructionsRaw)
            {
                instructions.Add(Convert.ToDouble(item));
            }

            return instructions;
        }

    }
}
