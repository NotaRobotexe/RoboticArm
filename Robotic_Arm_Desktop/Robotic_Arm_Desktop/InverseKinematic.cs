using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Robotic_Arm_Desktop
{
    class InverseKinematic
    {
        Movement movement;
        _3Dmodel model;
        const double LearningRate = 0.5;
        const double SamplingDistance = 0.15;
        double[,] angles;
        Point3D point;

        int PointsDensity = 15;
        double distance = 50;
        Point3D[] Targets;
        int TargetID = 0;

        public InverseKinematic(Movement mv,_3Dmodel model)
        {
            angles = new double[3,3];
            movement = mv;
            this.model = model;

            Targets = new Point3D[PointsDensity];
            Settings();
        }

        void Settings()
        {
            Point3D startPoint;
            double angle = movement.elbow0.AngleInDegree + movement.elbow1.AngleInDegree + movement.elbow2.AngleInDegree - 22;
            startPoint = model.griper.Bounds.Location;
            startPoint.X = Math.Round(startPoint.X, 3);
            startPoint.Z = Math.Round(startPoint.Z, 3);

            for (int i = 0; i < PointsDensity; i++)
            {
                Targets[i].X = Math.Round(startPoint.X + (Math.Cos(DegreeToRadian(angle)) * ((distance / PointsDensity) * (i+1))), 3);
                Targets[i].Z = Math.Round(startPoint.Z + (Math.Sin(DegreeToRadian(angle)) * ((distance / PointsDensity) * (i+1))), 3);
                Targets[i].Y = startPoint.Y;
            }

            Console.WriteLine(startPoint);
            Console.WriteLine(Targets[0]);
        }

        public void InverseKinematics()
        {
            GetAngle();

            if (DistanceFromTarget() < 0.5)  
            {

                if (TargetID++<PointsDensity)
                {
                    TargetID++;
                }
                else
                {
                    Global.InverseKinematicMovement = false;
                }
                //MidpointReached = true;
                
            }

            /*if (Global.triggered == true)  
            {
                moving = false;
                SetAngle();
            }*/

            for (int i = 0; i <= 2; i++)
            {
                // Gradient descent
                double gradient = PartialGradient(i);
                angles[i,0] -= LearningRate * gradient;

                // hodnota medzi min a max
                angles[i,0] = Clamp(angles[i,0], angles[i,1], angles[i,2]);
            }

            SetAngle();
        }


        private double PartialGradient(int i) //"""""magic""""" dont touch while it work
        {
            // Saves the angle,
            // it will be restored later
            double angle = angles[i,0];
            // Gradient : [F(x+SamplingDistance) - F(x)] / h

            double f_x = DistanceFromTarget();
            angles[i, 0] += SamplingDistance;
            SetAngle();
            double f_x_plus_d = DistanceFromTarget();

            double gradient = (f_x_plus_d - f_x) / SamplingDistance;

            // Restores
            angles[i,0] = angle;
            SetAngle();

            return gradient;
        }

        private double DistanceFromTarget()
        {
            point.X = model.griper.Bounds.Location.X;
            point.Y = model.griper.Bounds.Location.Y;
            point.Z = model.griper.Bounds.Location.Z;

            double distance = Math.Sqrt(Math.Pow((point.X - Targets[TargetID].X), 2.0) + Math.Pow((point.Y - Targets[TargetID].Y), 2.0) + Math.Pow((point.Z - Targets[TargetID].Z), 2.0));
            return distance;
        }


        private void GetAngle()
        {

            angles[2, 0] = movement.elbow0.AngleInDegree;
            angles[1, 0] = movement.elbow1.AngleInDegree;
            angles[0, 0] = movement.elbow2.AngleInDegree;

            angles[2, 1] = Arm.PwmToDegree(movement.elbow0.startfrom);
            angles[1, 1] = Arm.PwmToDegree(movement.elbow1.startfrom);
            angles[0, 1] = Arm.PwmToDegree(movement.elbow2.startfrom);

            angles[2, 2] = Arm.PwmToDegree(movement.elbow0.EndAt);
            angles[1, 2] = Arm.PwmToDegree(movement.elbow1.EndAt);
            angles[0, 2] = Arm.PwmToDegree(movement.elbow2.EndAt);
        }

        private void SetAngle()
        {
            movement.elbow0.Update(angles[2, 0], 0);
            movement.elbow1.Update(angles[1, 0], 0);
            movement.elbow2.Update(angles[0, 0], 0);
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private T Clamp<T>(T value, T min, T max)
            where T : System.IComparable<T>
            {
                T result = value;
                if (value.CompareTo(max) > 0)
                    result = max;
                if (value.CompareTo(min) < 0)
                    result = min;
                return result;
            }
    }
}
