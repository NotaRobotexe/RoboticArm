using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Robotic_Arm_Desktop
{
    class BruteForceMovement
    {
        struct FKproperties
        {
            public Point3D arm0P;
            public Point3D arm1P;
            public Point3D arm2P;

            public double arm0l;
            public double arm1l;
            public double arm2l;

            public double arm0A;
            public double arm1A;
            public double arm2A;
        }

        FKproperties prop;
        Movement movement;
        _3Dmodel model;
        Point3D startPoin,EndPoin;
        double angle;
        double angleoffset = 5;
        double fA, fB; //linear function
        double res = 0.1;
        bool PositionFound = false;

        //z = y ,x=x

        public BruteForceMovement(Movement movement, _3Dmodel model)
        {
            this.movement = movement;
            this.model = model;
        }

        public void InitBRM()
        {
            prop = new FKproperties();
            Settings(100);
            ForwardKinematic();
            arm2testing(prop.arm2P);
            Console.WriteLine(angle);
            Console.WriteLine(startPoin.X + " " + startPoin.Z);
            Console.WriteLine(EndPoin.X + " " + EndPoin.Z);
        }

        private void FindValidposition()
        {
            //for (int  = 0;  < length; ++)
            {

            }
        }

        private void arm2testing(Point3D arm2pos)
        {
            PositionFound = false;
            Point3D finalPos = new Point3D();
            finalPos.X = Math.Round(arm2pos.X + (Math.Cos(DegreeToRadian(60)) * prop.arm2l), 2);
            finalPos.Z = Math.Round(arm2pos.Z + (Math.Sin(DegreeToRadian(60)) *  prop.arm2l), 2);
            Console.WriteLine(finalPos.X + " " + finalPos.Z );
            /*Task.Run(() =>
            {
                Point3D finalPos = new Point3D();
                for (double i = 0; i < angleoffset; i+=res)
                {
                    double _angle = prop.arm2A;
                    if (PositionFound ==false)
                    {
                        _angle += i;
                        finalPos.X = Math.Round(arm2pos.X + (Math.Cos(DegreeToRadian(_angle)) * prop.arm2l),2);
                        finalPos.Z = Math.Round(arm2pos.Z + (Math.Sin(DegreeToRadian(_angle)) * prop.arm2l),2);

                        Console.WriteLine(finalPos.X + " " + finalPos.Z + " " + _angle);
                        if (IsItOnStraightLine(finalPos))
                        {
                            Console.WriteLine("***" + finalPos.X + " " + finalPos.Z + " " + _angle);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            });

            Task.Run(() =>
            {
                Point3D finalPos = new Point3D();
                for (double i = angleoffset; i > 0; i -= res)
                {
                    double _angle = prop.arm2A;
                    if (PositionFound == false)
                    {
                        _angle -= i;
                        finalPos.X = Math.Round(arm2pos.X + (Math.Cos(DegreeToRadian(_angle)) * prop.arm2l),2);
                        finalPos.Z = Math.Round(arm2pos.Z + (Math.Sin(DegreeToRadian(_angle)) * prop.arm2l),2);
                        //Console.WriteLine(finalPos.X + " " + finalPos.Z + " " + _angle);

                        if (IsItOnStraightLine(finalPos))
                        {
                            Console.WriteLine("***"+finalPos.X + " " + finalPos.Z + " " + _angle);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            });*/
        }

        private void ForwardKinematic()
        {
            prop.arm0P = model.elbow0.Bounds.Location;
            prop.arm1P = model.elbow1.Bounds.Location;
            prop.arm2P = model.elbow2.Bounds.Location;

            prop.arm0A = movement.elbow0.AngleInDegree;
            prop.arm1A = prop.arm0A+movement.elbow1.AngleInDegree-22;
            prop.arm2A = prop.arm1A+movement.elbow2.AngleInDegree;

            prop.arm0l = Math.Sqrt(Math.Pow((prop.arm0P.X- prop.arm1P.X),2) + Math.Pow((prop.arm0P.Z - prop.arm1P.Z),2));
            prop.arm1l = Math.Sqrt(Math.Pow((prop.arm1P.X - prop.arm2P.X), 2) + Math.Pow((prop.arm1P.Z - prop.arm2P.Z), 2));
            prop.arm2l = Math.Sqrt(Math.Pow((prop.arm2P.X - startPoin.X), 2) + Math.Pow((startPoin.Z - prop.arm2P.Z), 2));
            Console.WriteLine("0");
        }

        private void Settings(int distance)
        {
            angle = movement.elbow0.AngleInDegree + movement.elbow1.AngleInDegree + movement.elbow2.AngleInDegree-22;

            startPoin = model.griper.Bounds.Location;
            startPoin.X = Math.Round(startPoin.X, 2);
            startPoin.Z = Math.Round(startPoin.Z, 2);

            EndPoin.X =Math.Round( startPoin.X + (Math.Cos(DegreeToRadian(angle)) *distance),2);
            EndPoin.Z = Math.Round( startPoin.Z + (Math.Sin(DegreeToRadian(angle)) * distance),2);

            fA = (EndPoin.Z - startPoin.Z) / (EndPoin.X - startPoin.X);
            fB = startPoin.Z - (startPoin.X * fA);
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private bool IsItOnStraightLine(Point3D point)
        {
            if (((fA * point.X) + fB)==point.Y){
                return true;
            }
            return false;
        }
    }
}
