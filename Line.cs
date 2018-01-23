using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeChecker
{
    public class Line
    {
        public IntPoint TopPoint { get; set; }
        public IntPoint BottomPoint { get; set; }
        public int Height

        {
            get
            {
                return BottomPoint.Y - TopPoint.Y;
            }

        }

    }
}
