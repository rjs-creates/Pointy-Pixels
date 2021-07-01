// Project : Lab 3 - Pointy Pixel Penetration
// Mar 09 2021
// By Rajeshwar Singh
//
// Submission Code : 1202_CMPE2800_L03
// ///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace Lab3_SinghRajeshwar
{
    public abstract class ShapeBase
    {
        protected float _fRotation;             //store rotation value
        protected float _fRotationIncrement;    //store rotation increment
        protected float _fXSpeed;               //store x-axis velocity
        protected float _fYSpeed;               //store y-axis velocity
        protected static Random _random = new Random();   //rng

        //Represent the radius of unit circle on which basis our shape will be created
        public static float _cfRadius { get; set; }
        
        //Color of our shape
        public static Color baseColor { get; set; }
       
        //Is the shape intersected
        public bool IsMarkedForDeath { get; set; }
        
        //Is the shape close to other shape
        public bool IsMarkedToCompare { get; set; }
        
        //Center point of our shape
        public PointF Location { get; set; }

        //Check for distance with other shapes
        public void Dist(ShapeBase comparison)
        {
            //find distance
            double distance = Math.Sqrt(Math.Pow(Location.X - comparison.Location.X, 2) + Math.Pow(Location.Y - comparison.Location.Y, 2));
            
            //draw unit circle if two shapes are too close
            if (distance < (_cfRadius * 2))
                IsMarkedToCompare = true;
        }

        //Abstract method GetPath
        public abstract GraphicsPath GetPath();

        //Draw Our shape and the circle incompassing it based on the conditions
        public void Render(Graphics gr)
        {
            //if two shapes are very close draw the circle
            if (IsMarkedToCompare)
                gr.DrawEllipse(new Pen(Color.Blue), Location.X - _cfRadius, Location.Y - _cfRadius, _cfRadius*2, _cfRadius*2);
            gr.FillPolygon(new SolidBrush(baseColor), GetPath().PathPoints);
        }

        //Change Location of shpae every timer tick
        public void Tick(Rectangle clientRectangle)
        {
            
            //New location after one tick
            PointF pointy = new PointF(Location.X + _fXSpeed, Location.Y + _fYSpeed);

            //Check if the new location is our of bounds or not
            if(pointy.X >= clientRectangle.Width)
            {
                //If out of bound change the velocity direction
                pointy.X = clientRectangle.Width;
                _fXSpeed *= -1;
            }

            //Change the velocity if shape exceeds the left most bound 
            if(pointy.X < 0)
            {
                //If out of bound change the velocity direction
                pointy.X = 0;
                _fXSpeed *= -1;
            }

            //Change the velocity if shape exceeds the bottom bounds 
            if (pointy.Y >= clientRectangle.Height)
            {
                //If out of bound change the velocity direction
                pointy.Y = clientRectangle.Height;
                _fYSpeed *= -1;
            }

            //Change the velocity if shape exceeds the top bound 
            if (pointy.Y < 0)
            {
                //If out of bound change the velocity direction
                pointy.Y = 0;
                _fYSpeed *= -1;
            }

            //set calculated location and add rotation
            Location = pointy;
            _fRotation += _fRotationIncrement;

        }

        //Making our shape
        public static GraphicsPath MakePolygonPath(int numPoints, double variance)
        {
            //List containing all the points of our shape
            List<PointF> mylist = new List<PointF>();

            //Graphic path to draw
            GraphicsPath grpath = new GraphicsPath();

            //Calculating the least radius
            double minRadius = _cfRadius * (1 - variance);

            //iterate through all the points
            for(double i = 0; i < Math.PI*2; i+= ((Math.PI*2)/numPoints))
            {
                //radius for different points
                double newRadius = _random.NextDouble() * (_cfRadius - minRadius)+minRadius;

                //Add points to the list
                mylist.Add(new PointF((float)(Math.Sin(i) * newRadius),(float) (Math.Cos(i) * newRadius)));
            }

            //Draw the polygon and return the graphics path
            grpath.AddPolygon(mylist.ToArray());
            return grpath;
        }

        //Base Class contructor
        public ShapeBase(PointF pos)
        {
            Location = pos;                                     //intializing position
            _fRotation = 0;                                     //intializing rotation
            _fRotationIncrement = (float)((_random.NextDouble() * 6) - 3);        //random rotation increment value
            _fXSpeed = (float)((_random.NextDouble() * 5) - 2.5);                 //random x velocity
            _fYSpeed = (float)((_random.NextDouble() * 5) - 2.5);                 //random y velocity
        }
    }

    //Derived Triangle class
    public class Triangle : ShapeBase
    {
        //Graphics path for triangle model
        private static GraphicsPath _modelGraphicPath = new GraphicsPath();
        public Triangle(PointF triPoint):base(triPoint)
        {           
        }

        //Abstract GetPath
        public override GraphicsPath GetPath()
        {
            //Create the Graphics path for the shape and clone it
            RebuildModel();
            GraphicsPath grp = (GraphicsPath)_modelGraphicPath.Clone();

            //Put it in its position and rotate it based on its rotation value
            Matrix mat = new Matrix();
            mat.Translate(Location.X, Location.Y);
            mat.Rotate(_fRotation);

            //Add the transformation to graphics path and return it
            grp.Transform(mat);
            return grp;
        }

        //Creating Triangle using 3 points and 0 variance
        public static void RebuildModel()
        {
            _modelGraphicPath = MakePolygonPath(3, 0);
        }
    }

    //Derived Triangle class
    public class Rock : ShapeBase
    {
        //Graphics path for rock model
        private GraphicsPath _modelGraphicPath;

        //Contructor which create shpae with number of vertex between 4 and 12 and has a variance of 0.6
        public Rock(PointF rockPoint) : base(rockPoint)
        {
            _modelGraphicPath = MakePolygonPath(_random.Next(4,13), 0.6);
        }

        //Abstract GetPath
        public override GraphicsPath GetPath()
        {
            //Create the Graphics path for the shape and clone it
            GraphicsPath grp = (GraphicsPath)_modelGraphicPath.Clone();
            Matrix mat = new Matrix();

            //Put it in its position and rotate it based on its rotation value
            mat.Translate(Location.X, Location.Y);
            mat.Rotate(_fRotation);

            //Add the transformation to graphics path and return it
            grp.Transform(mat);
            return grp;
        }
    }


    //Custom class for collision regions
    public class myRegs
    {
        private Region interRegion;          //stores a region and a stopwatch
        private Stopwatch interStop;

        //Constructor to contain a region and start the stopwatch
        public myRegs(Region mRegion)
        {
            interRegion = mRegion;
            interStop = new Stopwatch();
            interStop.Start();
        }

        //property to get the region back
        public Region intRegion { get { return interRegion; } set { interRegion = value; } }
        
        //property to see if the stopwatch time elapsed is more than 2 seconds
        public Stopwatch stop { get { return interStop; } set { interStop = value; } }
    }
}
