// Simple Shape Checker sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System.Linq;

namespace ShapeChecker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // Exit from application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // On loading of the form
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadDemo("coins.jpg");
        }

        // Open file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var startTime = DateTime.Now;
                foreach (var item in openFileDialog.FileNames)
                {
                    
                        ProcessImage((Bitmap)Bitmap.FromFile(item), item);
                   
                }
                var finishTime =DateTime.Now;
                TimeSpan totalTime = finishTime - startTime;
                MessageBox.Show("TotalTime: " + totalTime.TotalSeconds + "s. Files: " + openFileDialog.FileNames.Length);
            }
        }

        // Load first demo image
        private void loadDemoImage1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDemo("coins.jpg");
        }

        // Load second demo image
        private void loadDemoImage2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDemo("demo.png");
        }

        // Load third demo image
        private void loadDemoImage3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDemo("demo1.png");
        }

        // Load fourth demo image
        private void loadDemoImage4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDemo("demo2.png");
        }

        // Load one of the embedded demo image
        private void LoadDemo(string embeddedFileName)
        {
        }

        // Process image
        private void ProcessImage(Bitmap bitmap, string fileLocation)
        {
            
            

            // From this bitmap, the graphics can be obtained, because it has the right PixelFormat
            Graphics g = Graphics.FromImage(bitmap); 
            Pen redPen = new Pen(Color.Red, 4);
            Pen greenPen = new Pen(Color.Green, 4);
            Pen yellowPen = new Pen(Color.Yellow, 4);
            var startDateTime = DateTime.Now;
            var sheetRecognition = new SheetRecognition(bitmap);
            sheetRecognition.ProcessImage();
            int[] trueAnswers = new int[19]
            { 2,0,2,1,0,0,3,3,2,3,0,3,1,1,0,2,3,0,2};

            int trueAnswersCount = 0;
            List<List<Shape>> RowsAndShapes = sheetRecognition.RowsAndShapes;

            //List<Rectangle> sortedRectangles = recognizedRectangles.OrderBy(x => x.Top).ToList();
            for (int r = 0; r < RowsAndShapes.Count; r++)
            {
                for (int i = 0; i < RowsAndShapes[r].Count; i++)
                {
                    
                List<IntPoint> corners = new List<IntPoint>();
                corners.Add(new IntPoint()
                {
                    X = RowsAndShapes[r][i].TopLeftPoint.X,
                    Y = RowsAndShapes[r][i].TopLeftPoint.Y
                });
                corners.Add(new IntPoint()
                {
                    X = RowsAndShapes[r][i].BottomLeftPoint.X,
                    Y = RowsAndShapes[r][i].BottomLeftPoint.Y
                });

                corners.Add(new IntPoint()
                {
                    X = RowsAndShapes[r][i].BottomRightPoint.X,
                    Y = RowsAndShapes[r][i].BottomRightPoint.Y
                });
                    corners.Add(new IntPoint()
                {
                    X = RowsAndShapes[r][i].TopRightPoint.X,
                    Y = RowsAndShapes[r][i].TopRightPoint.Y
                });
                    if (RowsAndShapes[r][i].IsChecked && trueAnswers[r] == i)
                    {
                        g.DrawPolygon(greenPen, ToPointsArray(corners));
                    }else if (RowsAndShapes[r][i].IsChecked)
                    {
                        g.DrawPolygon(yellowPen, ToPointsArray(corners));
                    }
                    else
                    {
                        g.DrawPolygon(redPen, ToPointsArray(corners));
                    }
                }
              
                //if (r > 1)
                //{
                //    var trueIndex = trueAnswers[r - 2];
                //    if (RowsAndShapes[r][trueIndex].IsChecked)
                //    {
                //        trueAnswersCount++;
                //        List<IntPoint> corners = new List<IntPoint>();
                //        corners.Add(new IntPoint()
                //        {
                //            X = RowsAndShapes[r][trueAnswers[r - 2]].TopLeftPoint.X,
                //            Y = RowsAndShapes[r][trueAnswers[r - 2]].TopLeftPoint.Y
                //        });
                //        corners.Add(new IntPoint()
                //        {
                //            X = RowsAndShapes[r][trueAnswers[r - 2]].BottomLeftPoint.X,
                //            Y = RowsAndShapes[r][trueAnswers[r - 2]].BottomLeftPoint.Y
                //        });

                //        corners.Add(new IntPoint()
                //        {
                //            X = RowsAndShapes[r][trueAnswers[r - 2]].BottomRightPoint.X,
                //            Y = RowsAndShapes[r][trueAnswers[r - 2]].BottomRightPoint.Y
                //        }); corners.Add(new IntPoint()
                //        {
                //            X = RowsAndShapes[r][trueAnswers[r - 2]].TopRightPoint.X,
                //            Y = RowsAndShapes[r][trueAnswers[r - 2]].TopRightPoint.Y
                //        });
                //        g.DrawPolygon(greenPen, ToPointsArray(corners));
                //    }
                //    else
                //    {
                //        var isChecked = RowsAndShapes[r].Where(x => x.IsChecked == true).FirstOrDefault();
                //        var trueAnswer = trueAnswers[r - 2];
                //        if (isChecked != null)
                //        {
                            
                //            List<IntPoint> corners = new List<IntPoint>();
                //            corners.Add(new IntPoint()
                //            {
                //                X = isChecked.TopLeftPoint.X,
                //                Y = isChecked.TopLeftPoint.Y
                //            });
                //            corners.Add(new IntPoint()
                //            {
                //                X = isChecked.BottomLeftPoint.X,
                //                Y = isChecked.BottomLeftPoint.Y
                //            });

                //            corners.Add(new IntPoint()
                //            {
                //                X = isChecked.BottomRightPoint.X,
                //                Y = isChecked.BottomRightPoint.Y
                //            }); corners.Add(new IntPoint()
                //            {
                //                X = isChecked.TopRightPoint.X,
                //                Y = isChecked.TopRightPoint.Y
                //            });
                //            g.DrawPolygon(yellowPen, ToPointsArray(corners));
                //        }
                //        g.DrawLine(redPen, RowsAndShapes[r][trueAnswer].TopLeftPoint.X, RowsAndShapes[r][trueAnswer].TopLeftPoint.Y, RowsAndShapes[r][trueAnswer].BottomRightPoint.X, RowsAndShapes[r][trueAnswer].BottomRightPoint.Y);
                //        g.DrawLine(redPen, RowsAndShapes[r][trueAnswer].TopRightPoint.X, RowsAndShapes[r][trueAnswer].TopRightPoint.Y, RowsAndShapes[r][trueAnswer].BottomLeftPoint.X, RowsAndShapes[r][trueAnswer].BottomLeftPoint.Y);

                //    }
                //}
                
            }

              
                redPen.Dispose();
                greenPen.Dispose();
            yellowPen.Dispose();
                g.Dispose();

                // put new image to clipboard
                Clipboard.SetDataObject(bitmap);
                // and to picture box
                pictureBox.Image = bitmap;
                var fileName = fileLocation.Remove(fileLocation.Length - 4) + "-checked.jpg";
                bitmap.Save(fileName, ImageFormat.Jpeg);

                UpdatePictureBoxPosition();
            
        }

        // Size of main panel has changed
        private void splitContainer_Panel2_Resize(object sender, EventArgs e)
        {
            UpdatePictureBoxPosition();
        }

        // Update size and position of picture box control
        private void UpdatePictureBoxPosition()
        {
            int imageWidth;
            int imageHeight;

            if (pictureBox.Image == null)
            {
                imageWidth = 320;
                imageHeight = 240;
            }
            else
            {
                imageWidth = pictureBox.Image.Width;
                imageHeight = pictureBox.Image.Height;
            }

            Rectangle rc = splitContainer.Panel2.ClientRectangle;

            pictureBox.SuspendLayout();
            pictureBox.Location = new Point((rc.Width - imageWidth - 2) / 2, (rc.Height - imageHeight - 2) / 2);
            pictureBox.Size = new Size(imageWidth + 2, imageHeight + 2);
            pictureBox.ResumeLayout();
        }

        // Conver list of AForge.NET's points to array of .NET points
        private Point[] ToPointsArray(List<IntPoint> points)
        {
            Point[] array = new Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new Point(points[i].X, points[i].Y);
            }

            return array;
        }

        // Show about form
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm form = new AboutForm();

            form.ShowDialog();
        }

       
        private IntPoint GetCenter(Rectangle rectangle)
        {
            int halfWidth = rectangle.Width / 2;
            int halfHeight = rectangle.Height / 2;

            IntPoint center = new IntPoint()
            {
                X = rectangle.X + halfWidth,
                Y = rectangle.Y + halfHeight
            };
            return center;
        }

        private void splitContainer_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
