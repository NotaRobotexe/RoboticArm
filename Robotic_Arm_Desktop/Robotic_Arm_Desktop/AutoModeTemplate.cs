using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Robotic_Arm_Desktop
{
    internal static class AutoModeTemplate
    {
        private const int sleepTime = 15;
        private const int incrementation = 1;

        public static async Task StartTemplateAsync(List<string> commands, Movemend movemend, TextBox textBox)
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

                        await Task.Run(() =>
                        {
                            bool allMotorsOnPositions = false;
                            do
                            {
                                if (Global.stop == false)
                                {
                                    allMotorsOnPositions = Moving(instructions, movemend);
                                    Thread.Sleep(Convert.ToInt32(sleepTime / instructions[8]));
                                }
                                else
                                {
                                    break;
                                }
                            } while (allMotorsOnPositions == false);

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

                textBox.Text = (reps - i).ToString();
            }

            Global.autoModeRunning = false;
        }

        public static async Task AnimationFromTemplate(string command, Movemend movemend)
        {
            List<string> instructionsRaw = command.Split('*').ToList();
            List<double> instructions = new List<double>();

            foreach (var item in instructionsRaw)
            {
                instructions.Add(Convert.ToDouble(item));
            }

            await Task.Run(() =>
            {
                bool allMotorsOnPositions = false;
                do
                {
                    allMotorsOnPositions = Moving(instructions, movemend);
                    Thread.Sleep(Convert.ToInt32(sleepTime / instructions[8]));
                } while (allMotorsOnPositions == false);
            });
        }

        public static async Task ScriptDefaultMovemend(string rawcommand, Movemend movemend)
        {
            Global.IsMoving = true;

            string command = rawcommand.Substring(1);
            List<string> instructionsRaw = command.Split('*').ToList();
            List<double> instructions = new List<double>();

            foreach (var item in instructionsRaw)
            {
                instructions.Add(Convert.ToDouble(item));
            }

            if (instructions[0] == -1){
                instructions[0] = movemend.baseMovemend.AngleInPWM;
            }
            else{
                instructions[0] = Arm.DegreeToPwm(instructions[0]);
            }

            if (instructions[1] == -1)
            {
                instructions[1] = movemend.elbow0.AngleInPWM;
            }
            else
            {
                instructions[1] = Arm.DegreeToPwm(instructions[1]);
            }

            if (instructions[2] == -1)
            {
                instructions[2] = movemend.elbow1.AngleInPWM;
            }
            else
            {
                instructions[2] = Arm.DegreeToPwm(instructions[2]);
            }

            if (instructions[3] == -1)
            {
                instructions[3] = movemend.elbow2.AngleInPWM;
            }
            else
            {
                instructions[3] = Arm.DegreeToPwm(instructions[3]);
            }

            if (instructions[4] == -1)
            {
                instructions[4] = movemend.griperRotation.AngleInPWM;
            }
            else
            {
                instructions[4] = Arm.DegreeToPwm(instructions[4]);
            }

            if (instructions[5] == -1)
            {
                instructions[5] = movemend.griper.AngleInPWM;
            }
            else
            {
                instructions[5] = Arm.DegreeToPwm(instructions[5]);
            }

            await Task.Run(() =>
            {
                bool allMotorsOnPositions = false;
                do
                {
                    allMotorsOnPositions = Moving(instructions, movemend);
                    Thread.Sleep(Global.MovingSpeed);

                    if (Global.ScriptEnabled == false){
                        break;
                    }
                } while (allMotorsOnPositions == false);

                Global.IsMoving = false;
            });
        }

        private static bool Moving(List<double> instruction, Movemend movemend)
        {
            int onPosition = 0;
            if (instruction[1]>= movemend.elbow0.startfrom && instruction[1]<= movemend.elbow0.EndAt)
            {
                if (instruction[1] > movemend.elbow0.AngleInPWM)
                {
                    if (movemend.elbow0.AngleInPWM + incrementation > instruction[1])
                    {
                        movemend.elbow0.Update(instruction[1], 1);
                    }
                    else
                    {
                        movemend.elbow0.Update(movemend.elbow0.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[1] < movemend.elbow0.AngleInPWM)
                {
                    if (movemend.elbow0.AngleInPWM - incrementation < instruction[1])
                    {
                        movemend.elbow0.Update(instruction[1], 1);
                    }
                    else
                    {
                        movemend.elbow0.Update(movemend.elbow0.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[2] >= movemend.elbow1.startfrom && instruction[2] <= movemend.elbow1.EndAt)
            {
                if (instruction[2] > movemend.elbow1.AngleInPWM)
                {
                    if (movemend.elbow1.AngleInPWM + incrementation > instruction[2])
                    {
                        movemend.elbow1.Update(instruction[2], 1);
                    }
                    else
                    {
                        movemend.elbow1.Update(movemend.elbow1.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[2] < movemend.elbow1.AngleInPWM)
                {
                    if (movemend.elbow1.AngleInPWM - incrementation < instruction[2])
                    {
                        movemend.elbow1.Update(instruction[2], 1);
                    }
                    else
                    {
                        movemend.elbow1.Update(movemend.elbow1.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[3] >= movemend.elbow2.startfrom && instruction[3] <= movemend.elbow2.EndAt)
            {
                if (instruction[3] > movemend.elbow2.AngleInPWM)
                {
                    if (movemend.elbow2.AngleInPWM + incrementation > instruction[3])
                    {
                        movemend.elbow2.Update(instruction[3], 1);
                    }
                    else
                    {
                        movemend.elbow2.Update(movemend.elbow2.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[3] < movemend.elbow2.AngleInPWM)
                {
                    if (movemend.elbow2.AngleInPWM - incrementation < instruction[3])
                    {
                        movemend.elbow2.AngleInPWM = instruction[3];
                    }
                    else
                    {
                        movemend.elbow2.Update(movemend.elbow2.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[4] >= movemend.griperRotation.startfrom && instruction[4] <= movemend.griperRotation.EndAt)
            {
                if (instruction[4] > movemend.griperRotation.AngleInPWM)
                {
                    if (movemend.griperRotation.AngleInPWM + incrementation > instruction[4])
                    {
                        movemend.griperRotation.AngleInPWM = instruction[4];
                    }
                    else
                    {
                        movemend.griperRotation.Update(movemend.griperRotation.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[4] < movemend.griperRotation.AngleInPWM)
                {
                    if (movemend.griperRotation.AngleInPWM - incrementation < instruction[4])
                    {
                        movemend.griperRotation.AngleInPWM = instruction[4];
                    }
                    else
                    {
                        movemend.griperRotation.Update(movemend.griperRotation.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[5] >= movemend.griper.startfrom && instruction[5] <= movemend.griper.EndAt)
            {
                if (instruction[5] > movemend.griper.AngleInPWM)
                {
                    if (movemend.griper.AngleInPWM + incrementation > instruction[5])
                    {
                        movemend.griper.AngleInPWM = instruction[5];
                    }
                    else
                    {
                        movemend.griper.Update(movemend.griper.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[5] < movemend.griper.AngleInPWM)
                {
                    if (movemend.griper.AngleInPWM - incrementation < instruction[5])
                    {
                        movemend.griper.AngleInPWM = instruction[5];
                    }
                    else
                    {
                        movemend.griper.Update(movemend.griper.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[0] >= movemend.baseMovemend.startfrom && instruction[0] <= movemend.baseMovemend.EndAt)
            {
                if (instruction[0] > movemend.baseMovemend.AngleInPWM)
                {
                    if (movemend.baseMovemend.AngleInPWM + incrementation > instruction[0])
                    {
                        movemend.baseMovemend.AngleInPWM = instruction[0];
                    }
                    else
                    {
                        movemend.baseMovemend.Update(movemend.baseMovemend.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[0] < movemend.baseMovemend.AngleInPWM)
                {
                    if (movemend.baseMovemend.AngleInPWM - incrementation < instruction[0])
                    {
                        movemend.baseMovemend.AngleInPWM = instruction[0];
                    }
                    else
                    {
                        movemend.baseMovemend.Update(movemend.baseMovemend.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
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

        private static List<double> Deserialization(string command)
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