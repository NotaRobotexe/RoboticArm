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
        _3Dmodel gripper;
        const double LearningRate = 1;
        const double SamplingDistance = 0.15;
        bool moving = false;
        double[,] angles;
        Vector3D target;
        Vector3D point;
        public bool MidpointReached = true;

        public InverseKinematic(Movement mv,_3Dmodel gripper)
        {
            angles = new double[3,3];
            movement = mv;
            this.gripper = gripper;

            //debug later delete
            target.X = -32.987374785963;
            target.Y = -7.45200511932373;
            target.Z = 22.1720001023346;
        }


        public void InverseKinematics()
        {
            GetAngle();

            //debug later delete
            if (DistanceFromTarget() < 0.1)  //distance treshild je tolerancia 
            {
                MidpointReached = true;
                //Global.InverseKinematicMovement = false;
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
            point.X = gripper.griper.Bounds.Location.X;
            point.Y = gripper.griper.Bounds.Location.Y;
            point.Z = gripper.griper.Bounds.Location.Z;

            return Math.Sqrt(Math.Pow((point.X - target.X), 2.0) + Math.Pow((point.Y - target.Y), 2.0) + Math.Pow((point.Z - target.Z), 2.0));
        }

        public void RealoadTarger()
        {
            //Vector3D arget = new Vector3D();

            target.X = gripper.endpoint.Bounds.Location.X;
            target.Y = gripper.endpoint.Bounds.Location.Y;
            target.Z = gripper.endpoint.Bounds.Location.Z; 


            //TranslateTransform3D translate = new TranslateTransform3D(target.X +xx, target.Y, target.Z -yy);

            //gripper.elbow5.Transform = translate;
        }


        private void GetAngle()
        {

            angles[0, 0] = movement.elbow0.AngleInDegree;
            angles[1, 0] = movement.elbow1.AngleInDegree;
            angles[2, 0] = movement.elbow2.AngleInDegree;

            angles[0, 1] = Arm.PwmToDegree(movement.elbow0.startfrom);
            angles[1, 1] = Arm.PwmToDegree(movement.elbow1.startfrom);
            angles[2, 1] = Arm.PwmToDegree(movement.elbow2.startfrom);

            angles[0, 2] = Arm.PwmToDegree(movement.elbow0.EndAt);
            angles[1, 2] = Arm.PwmToDegree(movement.elbow1.EndAt);
            angles[2, 2] = Arm.PwmToDegree(movement.elbow2.EndAt);
        }

        private void SetAngle()
        {
            movement.elbow0.Update(angles[0, 0], 0);
            movement.elbow1.Update(angles[1, 0], 0);
            movement.elbow2.Update(angles[2, 0], 0);
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
