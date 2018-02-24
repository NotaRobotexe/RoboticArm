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

        public static async Task StartTemplateAsync(List<string> commands, Movement movement, TextBox textBox)
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
                                    allMotorsOnPositions = Moving(instructions, movement);
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

        public static async Task AnimationFromTemplate(string command, Movement movement)
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
                    allMotorsOnPositions = Moving(instructions, movement);
                    Thread.Sleep(Convert.ToInt32(sleepTime / instructions[8]));
                } while (allMotorsOnPositions == false);
            });
        }

        public static async Task ScriptDefaultMovemend(string rawcommand, Movement movement)
        {
            Global.IsMoving = true;

            string command = rawcommand;
            List<string> instructionsRaw = command.Split('*').ToList();
            List<double> instructions = new List<double>();

            foreach (var item in instructionsRaw)
            {
                instructions.Add(Convert.ToDouble(item));
            }

            if (instructions[0] == -1){
                instructions[0] = movement.baseMovemend.AngleInPWM;
            }
            else{
                instructions[0] = Arm.DegreeToPwm(instructions[0]);
            }

            if (instructions[1] == -1)
            {
                instructions[1] = movement.elbow0.AngleInPWM;
            }
            else
            {
                instructions[1] = Arm.DegreeToPwm(instructions[1]);
            }

            if (instructions[2] == -1)
            {
                instructions[2] = movement.elbow1.AngleInPWM;
            }
            else
            {
                instructions[2] = Arm.DegreeToPwm(instructions[2]);
            }

            if (instructions[3] == -1)
            {
                instructions[3] = movement.elbow2.AngleInPWM;
            }
            else
            {
                instructions[3] = Arm.DegreeToPwm(instructions[3]);
            }

            if (instructions[4] == -1)
            {
                instructions[4] = movement.griperRotation.AngleInPWM;
            }
            else
            {
                instructions[4] = Arm.DegreeToPwm(instructions[4]);
            }

            if (instructions[5] == -1)
            {
                instructions[5] = movement.griper.AngleInPWM;
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
                    allMotorsOnPositions = Moving(instructions, movement);
                    Thread.Sleep(Global.MovingSpeed);

                    if (Global.ScriptEnabled == false){
                        break;
                    }
                } while (allMotorsOnPositions == false);

                Global.IsMoving = false;
            });
        }

        private static bool Moving(List<double> instruction, Movement movement)
        {
            int onPosition = 0;
            if (instruction[1]>= movement.elbow0.startfrom && instruction[1]<= movement.elbow0.EndAt)
            {
                if (instruction[1] > movement.elbow0.AngleInPWM)
                {
                    if (movement.elbow0.AngleInPWM + incrementation > instruction[1])
                    {
                        movement.elbow0.Update(instruction[1], 1);
                    }
                    else
                    {
                        movement.elbow0.Update(movement.elbow0.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[1] < movement.elbow0.AngleInPWM)
                {
                    if (movement.elbow0.AngleInPWM - incrementation < instruction[1])
                    {
                        movement.elbow0.Update(instruction[1], 1);
                    }
                    else
                    {
                        movement.elbow0.Update(movement.elbow0.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[2] >= movement.elbow1.startfrom && instruction[2] <= movement.elbow1.EndAt)
            {
                if (instruction[2] > movement.elbow1.AngleInPWM)
                {
                    if (movement.elbow1.AngleInPWM + incrementation > instruction[2])
                    {
                        movement.elbow1.Update(instruction[2], 1);
                    }
                    else
                    {
                        movement.elbow1.Update(movement.elbow1.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[2] < movement.elbow1.AngleInPWM)
                {
                    if (movement.elbow1.AngleInPWM - incrementation < instruction[2])
                    {
                        movement.elbow1.Update(instruction[2], 1);
                    }
                    else
                    {
                        movement.elbow1.Update(movement.elbow1.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[3] >= movement.elbow2.startfrom && instruction[3] <= movement.elbow2.EndAt)
            {
                if (instruction[3] > movement.elbow2.AngleInPWM)
                {
                    if (movement.elbow2.AngleInPWM + incrementation > instruction[3])
                    {
                        movement.elbow2.Update(instruction[3], 1);
                    }
                    else
                    {
                        movement.elbow2.Update(movement.elbow2.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[3] < movement.elbow2.AngleInPWM)
                {
                    if (movement.elbow2.AngleInPWM - incrementation < instruction[3])
                    {
                        movement.elbow2.AngleInPWM = instruction[3];
                    }
                    else
                    {
                        movement.elbow2.Update(movement.elbow2.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[4] >= movement.griperRotation.startfrom && instruction[4] <= movement.griperRotation.EndAt)
            {
                if (instruction[4] > movement.griperRotation.AngleInPWM)
                {
                    if (movement.griperRotation.AngleInPWM + incrementation > instruction[4])
                    {
                        movement.griperRotation.AngleInPWM = instruction[4];
                    }
                    else
                    {
                        movement.griperRotation.Update(movement.griperRotation.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[4] < movement.griperRotation.AngleInPWM)
                {
                    if (movement.griperRotation.AngleInPWM - incrementation < instruction[4])
                    {
                        movement.griperRotation.AngleInPWM = instruction[4];
                    }
                    else
                    {
                        movement.griperRotation.Update(movement.griperRotation.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[5] >= movement.griper.startfrom && instruction[5] <= movement.griper.EndAt)
            {
                if (instruction[5] > movement.griper.AngleInPWM)
                {
                    if (movement.griper.AngleInPWM + incrementation > instruction[5])
                    {
                        movement.griper.AngleInPWM = instruction[5];
                    }
                    else
                    {
                        movement.griper.Update(movement.griper.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[5] < movement.griper.AngleInPWM)
                {
                    if (movement.griper.AngleInPWM - incrementation < instruction[5])
                    {
                        movement.griper.AngleInPWM = instruction[5];
                    }
                    else
                    {
                        movement.griper.Update(movement.griper.AngleInPWM - incrementation, 1);
                    }
                }
                else
                {
                    onPosition++;
                }
            }

            if (instruction[0] >= movement.baseMovemend.startfrom && instruction[0] <= movement.baseMovemend.EndAt)
            {
                if (instruction[0] > movement.baseMovemend.AngleInPWM)
                {
                    if (movement.baseMovemend.AngleInPWM + incrementation > instruction[0])
                    {
                        movement.baseMovemend.AngleInPWM = instruction[0];
                    }
                    else
                    {
                        movement.baseMovemend.Update(movement.baseMovemend.AngleInPWM + incrementation, 1);
                    }
                }
                else if (instruction[0] < movement.baseMovemend.AngleInPWM)
                {
                    if (movement.baseMovemend.AngleInPWM - incrementation < instruction[0])
                    {
                        movement.baseMovemend.AngleInPWM = instruction[0];
                    }
                    else
                    {
                        movement.baseMovemend.Update(movement.baseMovemend.AngleInPWM - incrementation, 1);
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