using AForge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeChecker
{
    public class SheetRecognition
    {
        private Bitmap _bitmap;
        private UnsafeBitmap _lockBitmap;
        public List<List<Shape>> RowsAndShapes { get; private set; }
        public List<Line> AngleLines { get; private set; }
       public SheetRecognition(Bitmap bitmap)
        {
            _bitmap = bitmap;
            RowsAndShapes = new List<List<Shape>>();
            AngleLines = new List<Line>();
            _lockBitmap = new UnsafeBitmap(_bitmap);
            _lockBitmap.LockBitmap();
        }
      public void ProcessImage ()
        {
            List<Shape> result = new List<Shape>();
            IntPoint horizontalLinePoint = new IntPoint(0, 0);
            List<Line> lines = new List<Line>();
         
            double times = 0;
            int count = 0; 
            int minLineHeight = (int)(_bitmap.Height * 0.017);
            int maxWidth = (int)(_bitmap.Width * 0.04884);
            int minWidth = (int)(_bitmap.Width * 0.020007);
            int startingHeight = 0;
            int consequentWhitePixel = 0;
            for (int x = 100; x < _bitmap.Width; x++)
            {
                for (int y = startingHeight; y < _bitmap.Height; y++)
                {
                  
                    var pixel = _lockBitmap.GetPixel(x, y);
                  

                    if (pixel.R < 230 && pixel.G < 230 && pixel.B < 230 && !(pixel.R<100 && pixel.G<100 && pixel.B>150))
                    {
                        
                        if (horizontalLinePoint.X == 0 && horizontalLinePoint.Y == 0)
                        {
                            horizontalLinePoint.X = x;
                            horizontalLinePoint.Y = y;
                        }
                    }
                    else
                    {
                        consequentWhitePixel++;
                        if ((horizontalLinePoint.X > 0 || horizontalLinePoint.Y > 0)&& consequentWhitePixel>5)
                        {
                            consequentWhitePixel = 0;
                           var line = new Line()
                            {
                                BottomPoint = new IntPoint() { X = x, Y = y },
                                TopPoint = horizontalLinePoint
                            };
                            
                            if (line.Height > minLineHeight-10)
                            {
                                if (line.Height > 200)
                                {
                                    AngleLines.Add(line);
                                }
                                else
                                {
                                    var similarLine = lines.Where(listItem => listItem.TopPoint.Y - line.TopPoint.Y > minLineHeight * -1 && listItem.TopPoint.Y - line.TopPoint.Y < minLineHeight)
                                        .Where(listItem => listItem.TopPoint.X - line.TopPoint.X > -20 && listItem.TopPoint.X - line.TopPoint.X < 20).FirstOrDefault();
                                    if (similarLine != null)
                                    {
                                        if (similarLine.Height <= line.Height)
                                        {
                                            lines.Remove(similarLine);
                                            lines.Add(line);
                                        }

                                    }
                                    else
                                    {
                                        lines.Add(line);
                                    }
                                } 
                            }

                            horizontalLinePoint.X = 0;
                            horizontalLinePoint.Y = 0;
                          
                        }
                    }
                  
                }
            }
            var average = times / count;
          while(lines.Count>0)
            {
                var item = lines[0];
                var nearbyItems = lines.Where(x => x.TopPoint.Y - item.TopPoint.Y > -1 * minLineHeight && x.TopPoint.Y - item.TopPoint.Y < minLineHeight )
                    .OrderBy(x=>x.TopPoint.X);

                if (nearbyItems.Count() > 1)
                {   
                  
                    var nearbyItem = nearbyItems.ToList()[1];
                    if (nearbyItem != null)
                    {
                        if (item.TopPoint.DistanceTo(nearbyItem.TopPoint) > minWidth)
                        {
                           result.Add(new Shape(item.TopPoint, nearbyItem.TopPoint, item.BottomPoint, nearbyItem.BottomPoint));
                            lines.Remove(nearbyItem);
                            lines.Remove(item);
                        }
                        else
                        {
                             lines.Remove(nearbyItem);
                           var st = item.TopPoint.DistanceTo(nearbyItem.TopPoint);
                        }
                    }
                }
                else

                {
                    lines.Remove(item);
                }
                
            }
            result = result.OrderBy(x => x.TopLeftPoint.Y).ToList();
            List<List<Shape>> recongizedRowsAndShapes = new List<List<Shape>>();
            while (result.Count > 0)
            {
                var point = result[0].TopLeftPoint;
                var height = result[0].TopLeftPoint.DistanceTo(result[0].BottomLeftPoint);
                var row = result.Where(x => x.TopLeftPoint.Y - point.Y > height * -1 && x.TopLeftPoint.Y - point.Y < height).OrderBy(x=>x.TopLeftPoint.X).ToList();
                foreach (var item in row)
                {
                    result.Remove(item);
                }
                recongizedRowsAndShapes.Add(row);
            }
            foreach (var item in recongizedRowsAndShapes)
            {
              RowsAndShapes.Add(IsChecked(item));
            }

        }

        private List<Shape> IsChecked(List<Shape> row)
        {
            foreach (var item in row)
            {

                Rectangle newRectangle = new Rectangle(item.TopLeftPoint.X, item.TopLeftPoint.Y, (int)item.TopLeftPoint.DistanceTo(item.TopRightPoint), (int)item.TopLeftPoint.DistanceTo(item.BottomLeftPoint));

                double totalPixels = 0;
                double coloredPixels = 0;
                for (int x = newRectangle.X + 10; x < newRectangle.Right - 10; x++)
                {
                    for (int y = newRectangle.Y + 10; y < newRectangle.Bottom - 10; y++)
                    {
                        totalPixels++;
                        var pixel = _bitmap.GetPixel(x, y);
                        if (pixel.R < 100 && pixel.G < 100 && pixel.B > 100)
                        {
                            coloredPixels++;
                        }
                    }
                }
                double result = (coloredPixels / totalPixels) * 100;
                if(result>2 && result < 13)
                {
                    item.IsChecked = true;
                }
                else
                {
                    item.IsChecked = false;
                }
            }
            return row;
        }
        
    }
}
