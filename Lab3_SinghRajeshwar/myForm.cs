// Project : Lab 3 - Pointy Pixel Penetration
// Mar 09 2021
// By Rajeshwar Singh
//
// Submission Code : 1202_CMPE2800_L03
// ///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace Lab3_SinghRajeshwar
{
    public partial class myForm : Form
    {
        List<ShapeBase> allshapes = new List<ShapeBase>();                                //List of all the shapes
        private BufferedGraphicsContext _bgc = new BufferedGraphicsContext();             //Buffered Graphics Context
        private BufferedGraphics _bg = null;                                              //Buffered Graphics
        private Random rand = new Random();                                               //rng
        List<myRegs> allRegions = new List<myRegs>();                                     //List of all the intersected regions

        public myForm()
        {
            InitializeComponent();

            MouseDown += MyForm_MouseDown;                                  //MouseDown Event Subscription
            timerClick.Enabled = true;                                      //turning on the timer
            timerClick.Tick += TimerClick_Tick;                             //Timer tick event subscription
            Resize += MyForm_Resize;                                        //Resize event subscription
            _bg = _bgc.Allocate(CreateGraphics(), this.DisplayRectangle);   //Allocation buffered graphics
            ShapeBase._cfRadius = 50;                                        //intial radius of shapes
            MouseWheel += MyForm_MouseWheel;                                //Mouse Wheel event handler
            ShapeBase.baseColor = Color.Green;                              //basecolor for all shapes
        }

        //When Mousewheel is scrolled
        private void MyForm_MouseWheel(object sender, MouseEventArgs e)
        {
            //if no shapes are drawn yet
            if(allshapes.Count < 1)
            {
                //Change the radius for shapes(never let it be less than 5)
                if (ShapeBase._cfRadius + (e.Delta / 100) > 4)
                {
                    //Show it on top
                    ShapeBase._cfRadius += e.Delta / 100;
                    Text = "Pointy Pixel Size : " +ShapeBase._cfRadius.ToString();
                    
                }
            }
            
        }

        //If Window Resize change the drawing area too
        private void MyForm_Resize(object sender, EventArgs e)
        {
            _bg = _bgc.Allocate(CreateGraphics(), this.DisplayRectangle);
        }

        //Every time timer ticks
        private void TimerClick_Tick(object sender, EventArgs e)
        {
            Graphics gr = _bg.Graphics;

            //clear the back buffer
            gr.Clear(Color.FromKnownColor(KnownColor.Control));

            //all shapes ticks and not set to compare
            allshapes.ForEach(x => x.Tick(this.ClientRectangle));
            allshapes.ForEach(x => x.IsMarkedToCompare = false);

            for(int i = 0; i < allshapes.Count; i++)
            {
                //Region of first shape
                Region A = new Region(allshapes[i].GetPath());
                for(int j = i; j < allshapes.Count;j++)
                {
                    //Dont process if both shapes are same
                    if (!ReferenceEquals(allshapes[i], allshapes[j]))
                    {
                        //if they are pretty close
                        allshapes[i].Dist(allshapes[j]);
                        allshapes[j].Dist(allshapes[i]);
                        if (allshapes[i].IsMarkedToCompare)
                        {
                            //Create Second Region and see if the collide
                            Region B = new Region(allshapes[j].GetPath());
                            Region inter = A.Clone();
                            inter.Intersect(B);

                            //if they collide
                            if (!inter.IsEmpty(gr))
                            {
                                //Create instance of new class and add it to the list
                                myRegs myRegion = new myRegs(inter);
                                allRegions.Add(myRegion);

                                //Both shapes will be removed from our shape list
                                allshapes[i].IsMarkedForDeath = true;
                                allshapes[j].IsMarkedForDeath = true;
                            }

                        }
                    }
                }
            }

            //Draw each region in our region list
            allRegions.ForEach(x => gr.FillRegion(new SolidBrush(Color.Black), x.intRegion));
            
            //If Region is shown for more than 2 seconds remove the region
            allRegions.RemoveAll(x => x.stop.ElapsedMilliseconds > 2000);
            
            //Remove All the collided shapes
            allshapes.RemoveAll(x => x.IsMarkedForDeath);

            //show the number of shapes
            gr.DrawString($"{allshapes.Count}", new Font("Ariel", 30), new SolidBrush(Color.Pink), ClientRectangle.Width / 2, ClientRectangle.Height / 2);
           
            //Render every shape
            allshapes.ForEach(x => x.Render(gr));
            
            //Flip the back buffer to the primary surfce
            _bg.Render();
        }

        //Event handler for mouseDown
        private void MyForm_MouseDown(object sender, MouseEventArgs e)
        {
           //If mouse was left clicked without Shift pressed
           if (e.Button == MouseButtons.Left && Control.ModifierKeys != Keys.Shift)
           {
                //Create a a single triangle and add it to our collection
                Triangle newTri = new Triangle(e.Location);              
                allshapes.Add(newTri);
           }

           //If mouse was left clicked with Shift pressed
           if(e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift)
           {
                //Create Thousand triangles and add them to our collection
                for(int i = 0; i < 1000; i++)
                {
                    PointF newPoint = new PointF(rand.Next(0, ClientRectangle.Width), rand.Next(0, ClientRectangle.Height));
                    Triangle newTri = new Triangle(newPoint);                    
                    allshapes.Add(newTri);
                }
           }

            //If mouse was right clicked without Shift pressed
            if (e.Button == MouseButtons.Right && Control.ModifierKeys != Keys.Shift)
            {
                //Create a a single rock and add it to our collection
                Rock newRock = new Rock(e.Location);
                allshapes.Add(newRock);
            }

            //If mouse was right clicked with Shift pressed
            if (e.Button == MouseButtons.Right && Control.ModifierKeys == Keys.Shift)
            {
                //Create Thousand rocks and add them to our collection
                for (int i = 0; i < 1000; i++)
                {
                    PointF newPoint = new PointF(rand.Next(0, ClientRectangle.Width), rand.Next(0, ClientRectangle.Height));
                    Rock newRock = new Rock(newPoint);
                    allshapes.Add(newRock);
                }
            }

            //If mouse was left clicked and Control is pressed
            if(e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Control)
            {
                //Clear the screen
                allshapes.Clear();
            }
        }       
    }
}
