using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MatlabSimple
{
    public struct Margin
    {
        public double left;
        public double right;
        public double top;
        public double bottom;

        public Margin(double left, double right, double top, double bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public Margin(double leftRight, double topBottom)
        {
            this.left = leftRight;
            this.right = leftRight;
            this.top = topBottom;
            this.bottom = topBottom;
        }
    }

    public struct Point
    {
        public double X;
        public double Y;
    }

    public class Plot
    {
        #region Fields
        private Canvas cv;
        private double biasX;
        private double biasY;
        private double leftMargin;
        private double rightMargin;
        private double topMargin;
        private double bottomMargin;
        private double stepX;
        private double stepY;
        #endregion

        #region Properties
        public double Width
        {
            get { return this.cv.ActualWidth - this.leftMargin - this.rightMargin; }
        }
        public double Height
        {
            get { return this.cv.ActualHeight - this.topMargin - this.bottomMargin; }
        }
        #endregion;

        #region Constructors
        public Plot(Canvas canvas)
        {
            this.InitDefaults();
            this.cv = canvas;
        }

        public Plot(Canvas canvas, Margin margin)
        {
            this.cv = canvas;
            this.leftMargin = margin.left;
            this.rightMargin = margin.right;
            this.topMargin = margin.top;
            this.bottomMargin = margin.bottom;
        }
        #endregion

        #region Methods
        private void InitDefaults()
        {
            leftMargin = 10;
            rightMargin = 10;
            topMargin = 10;
            bottomMargin = 10;
        }
        private Point GetPoint(double x, double y)
        {
            Point newPoint;
            newPoint.X = Math.Round(this.biasX + x * stepX) + this.leftMargin;
            newPoint.Y = this.cv.ActualHeight - (Math.Round(this.biasY + y * stepY) + this.topMargin);
            return newPoint;
        }
        private void drawLine(Point from, Point to)
        {
            Line line = new Line();
            line.Stroke = System.Windows.Media.Brushes.Blue;
            line.X1 = from.X;
            line.Y1 = from.Y;
            line.X2 = to.X;
            line.Y2 = to.Y;
            this.cv.Children.Add(line);
        }
        public void Clean()
        {
            this.cv.Children.Clear();
        }

        public void PlotData(double[] x, double[] y)
        {
            if (x.Length != y.Length)
            {
                throw new Exception("Arrays of input values are not same length"); 
            }
            double maxX = x.Max();
            double minX = x.Min();
            double maxY = y.Max();
            double minY = y.Min();
            if (maxX * maxY < 0)
            {
                biasX = this.Width / 2;
                stepX = this.Width / (maxX + Math.Abs(minX));
            }
            else
            {
                biasX = 0;
                stepX = this.Width / maxX;
            }
            if (maxY * minY < 0)
            {
                biasY = this.Height / 2;
                stepY = this.Height / (maxY + Math.Abs(minY));
            }
            else
            {
                biasY = 0;
                stepY = this.Height / maxY;
            }
            List<Point> points = new List<Point>();
            for (int i = 0; i < x.Length; i++)
            {
                Point newPoint = this.GetPoint(x[i], y[i]);
                points.Add(newPoint);
            }
            for (int i = 0; i < points.Count - 1; i++)
            {
                this.drawLine(points[i], points[i + 1]);
            }
        }
        #endregion;
    }
}
