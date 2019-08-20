/*
*---------------------------------
*|		All rights reserved.
*|		author: lizhanping
*|		version:1.0
*|		File: Converter.cs
*|		Summary: 
*|		Date: 2019/8/19 14:11:09
*---------------------------------
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UpdateService
{
    public class num2TextBlockVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;
            int num = (int)value;
            return num == 0 ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class num2ProgressBarVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;
            int num = (int)value;
            return num == 1 ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class value2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            double p = (double)value;
            if (p < 100)
            {
                return "waiting...";
            }
            else
            {
                return "done";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Value2DataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            double percentValue = double.Parse(value.ToString());
            double angel = percentValue / 100 * 360; //角度
            double radius = 47; //环形半径

            //起始点
            double leftStart = 50;
            double topStart = 3;

            //结束点
            double endLeft = 0;
            double endTop = 0;

            bool isLagreCircle = false; //是否优势弧，即大于180度的弧形

            //小于90度
            if (angel <= 90)
            {
                double ra = (90 - angel) * Math.PI / 180; //弧度
                endLeft = leftStart + Math.Cos(ra) * radius; //余弦横坐标
                endTop = topStart + radius - Math.Sin(ra) * radius; //正弦纵坐标
            }

            else if (angel <= 180)
            {
                double ra = (angel - 90) * Math.PI / 180; //弧度
                endLeft = leftStart + Math.Cos(ra) * radius; //余弦横坐标
                endTop = topStart + radius + Math.Sin(ra) * radius;//正弦纵坐标
            }

            else if (angel <= 270)
            {
                isLagreCircle = true; //优势弧
                double ra = (angel - 180) * Math.PI / 180;
                endLeft = leftStart - Math.Sin(ra) * radius;
                endTop = topStart + radius + Math.Cos(ra) * radius;
            }

            else if (angel < 360)
            {
                isLagreCircle = true; //优势弧
                double ra = (angel - 270) * Math.PI / 180;
                endLeft = leftStart - Math.Cos(ra) * radius;
                endTop = topStart + radius - Math.Sin(ra) * radius;
            }
            else
            {
                isLagreCircle = true; //优势弧
                endLeft = leftStart - 0.001; //不与起点在同一点，避免重叠绘制出非环形
                endTop = topStart;
            }

            Point arcEndPt = new Point(endLeft, endTop); //结束点
            Size arcSize = new Size(radius, radius);
            SweepDirection direction = SweepDirection.Clockwise; //顺时针弧形
            //弧形
            ArcSegment arcsegment = new ArcSegment(arcEndPt, arcSize, 0, isLagreCircle, direction, true);

            //形状集合
            PathSegmentCollection pathsegmentCollection = new PathSegmentCollection();
            pathsegmentCollection.Add(arcsegment);

            //路径描述
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(leftStart, topStart); //起始地址
            pathFigure.Segments = pathsegmentCollection;

            //路径描述集合
            PathFigureCollection pathFigureCollection = new PathFigureCollection();
            pathFigureCollection.Add(pathFigure);

            //复杂形状
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = pathFigureCollection;

            //Data赋值
            return pathGeometry;

            //达到100 % 则闭合整个
            //if (angel == 360)
            //    return Geometry.Parse(myCycleProcessBar1.Data.ToString() + " z");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Value2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "#507090";
            double percent = double.Parse(value.ToString());
            if (percent <= 25)
                return "#5080A5";
            if (percent <= 50)
                return "#5090BF";
            if (percent <= 75)
                return "#50A0CF";
            if (percent <= 100)
                return "#50B0FF";
            return "#50B0FF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Num2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            int num = (int)value;
            return num == 0 ? "立即更新" : (num == 2) ? "完成" : "正在更新";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Bool2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value==null)
            {
                return string.Empty;
            }
            bool rst = (bool)value;
            return rst ? "更新成功" : "更新失败";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Num2CompleteTextBlockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;
            int num = (int)value;
            return num == 2 ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
