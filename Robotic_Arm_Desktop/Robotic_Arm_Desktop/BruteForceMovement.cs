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
        Movement movement;
        _3Dmodel model;
        Point3D startPoin,EndPoin;
        double angle;
        double angleoffset = 5;
        double fA, fB; //linear function

        public BruteForceMovement(Movement movement, _3Dmodel model)
        {
            this.movement = movement;
            this.model = model;
        }

        public void InitBRM()
        {
            Settings(100);
            Console.WriteLine(startPoin.X + " " + startPoin.Z);
            Console.WriteLine(EndPoin.X + " " + EndPoin.Z);
        }

        private void FindValidposition()
        {

        }

        private void Settings(int distance)
        {
            angle = Math.Abs(movement.elbow0.AngleInDegree + movement.elbow1.AngleInDegree + movement.elbow2.AngleInDegree+22-360);

            startPoin = model.griper.Bounds.Location;
            startPoin.X = Math.Round(startPoin.X, 3);
            startPoin.Z = Math.Round(startPoin.Z, 3);

            double width = Math.Round(distance * Math.Sin(DegreeToRadian(angle)),3);
            double height = Math.Round(Math.Sqrt((distance * distance) -(width * width)),3);

            EndPoin.X -= width;
            EndPoin.Z -= height;


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
