using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Windows.Media;

namespace Robotic_Arm_Desktop
{
    class _3Dmodel
    {
        public Model3DGroup group;
        public Model3D elbow0, elbow1, elbow2, griper, baseHolder, test;

        public double baserotation = 0, elbow0rot = 0, elbow1rot = 15, elbow2rot = -90, gripper0rot = 90;

        RotateTransform3D rotate, rotate2;
        TranslateTransform3D translate;

        public _3Dmodel()   //UNDONE: dorobit aj zatvaranie a otvaranie griperu
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(64,64,64));
            Material material = new DiffuseMaterial(brush);

            ModelImporter importer = new ModelImporter();
            importer.DefaultMaterial = material;
            elbow0 = importer.Load(@"ArmModel/elbow0.stl");
            elbow1 = importer.Load(@"ArmModel/elbow1.stl");
            elbow2 = importer.Load(@"ArmModel/elbow2.stl");
            baseHolder = importer.Load(@"ArmModel/base.obj"); 
            griper = importer.Load(@"ArmModel/graper.stl");
            test = importer.Load(@"ArmModel/graper.stl");

            CallUpdate();

            group = new Model3DGroup();
            group.Children.Add(elbow0);
            group.Children.Add(elbow1);
            group.Children.Add(elbow2);
            group.Children.Add(baseHolder);
            group.Children.Add(griper);
            group.Children.Add(test);
        }

        public void UpdateModel(Movement movement)
        {
            baserotation += movement.baseMovemend.AngleInDegree;
            elbow0rot += movement.elbow0.AngleInDegree;
            elbow1rot += movement.elbow1.AngleInDegree;
            elbow2rot += movement.elbow2.AngleInDegree;
            gripper0rot += movement.griperRotation.AngleInDegree;

            CallUpdate();

            baserotation = 0;
            elbow0rot = 0;
            elbow1rot = 22;
            elbow2rot = -90;
            gripper0rot = 90;
        }

        private void CallUpdate()
        {
            Transform3DGroup tra1 = new Transform3DGroup();
            Transform3DGroup tra2 = new Transform3DGroup();
            Transform3DGroup tra3 = new Transform3DGroup();
            Transform3DGroup tra4 = new Transform3DGroup();
            Transform3DGroup tra5 = new Transform3DGroup();
            Transform3DGroup tra6 = new Transform3DGroup();

            //set position of models
            translate = new TranslateTransform3D(0, -4, 0);
            rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), baserotation), new Point3D(0, -4.5, 0));
            tra1.Children.Add(translate);
            tra1.Children.Add(rotate);

            translate = new TranslateTransform3D(3, -4, -1);
            rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -90), new Point3D(0, 0,0));
            rotate2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), elbow0rot), new Point3D(5, 0, 3));

            tra2.Children.Add(translate);
            tra2.Children.Add(rotate);
            tra2.Children.Add(rotate2);
            tra2.Children.Add(tra1);

            translate = new TranslateTransform3D(4, 3, 0);
            rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), elbow1rot), new Point3D(0,30,0));
            tra3.Children.Add(translate);
            tra3.Children.Add(rotate);
            tra3.Children.Add(tra2);

            translate = new TranslateTransform3D(2, -1.8, 0);
            rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), elbow2rot), new Point3D(-4.05, 39.2, 0));
            tra4.Children.Add(translate);
            tra4.Children.Add(rotate);
            tra4.Children.Add(tra3);

            translate = new TranslateTransform3D(1, 0, 0);
            rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), gripper0rot), new Point3D(0, 41, 0.86));
            tra5.Children.Add(translate);
            tra5.Children.Add(rotate);
            tra5.Children.Add(tra4);


            baseHolder.Transform = tra1;
            elbow0.Transform = tra2;
            elbow1.Transform = tra3;
            elbow2.Transform = tra4;
            griper.Transform = tra5;

            tra6.Children.Add(new TranslateTransform3D(25.4931449890137, -39.7999992370605, 2.23200511932373));
            tra6.Children.Add(new TranslateTransform3D(elbow2.Bounds.Location.X, -4, elbow2.Bounds.Location.Z));
            test.Transform = tra6;

            Console.WriteLine(elbow2.Bounds.Location);

            Console.WriteLine(test.Bounds.Location);
        }
    }
}
