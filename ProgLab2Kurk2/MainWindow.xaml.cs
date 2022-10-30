using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AutoMapper;
using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Series;

namespace ProgLab2Kurk2
{
    public partial class MainWindow
    {
        private PlotModel _plotModel;
        private int polinomes = 1;

        public MainWindow()
        {
            InitializeComponent();
            _plotModel = new PlotModel();

            var col1 = new DataGridTextColumn();
            col1.Binding = new Binding("x");
            col1.Header = "X";

            var col2 = new DataGridTextColumn();
            col2.Binding = new Binding("y");
            col2.Header = "Y";

            DataGrid.Columns.Add(col1);
            DataGrid.Columns.Add(col2);
        }

        private void SetProperty_OnClick(object sender, RoutedEventArgs e)
        {
            var x = double.Parse(XBlock.Text);
            var y = double.Parse(YBlock.Text);
            DataGrid.Items.Add(new XY { x = x, y = y });

            var point = new LineSeries
            {
                Color = OxyColors.Black,
                MarkerStroke = OxyColors.Black,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle
            };
            point.Points.Add(new DataPoint(x, y));
            _plotModel.Series.Add(point);
            PlotView.Model = _plotModel;
        }

        private void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            var x = new List<double>();
            var y = new List<double>();
            foreach (XY xy in DataGrid.Items)
            {
                x.Add(xy.x);
                y.Add(xy.y);
            }

            var myReg = new LSM(x.ToArray(), y.ToArray());
            myReg.Polynomial(polinomes);
            foreach (var coeff in myReg.Coeff)
            {
                Console.WriteLine(coeff);
            }

            Console.WriteLine(myReg.Delta);
            Func<double, double> func;

            double sumY = 0;
            foreach (XY xy in DataGrid.Items)
            {
                sumY += xy.y;
            }

            double notY = sumY / DataGrid.Items.Count;
            double SStot = 0;
            double SSres = 0;
            if (polinomes == 1)
            {
                func = (x1) =>
                {
                    ABlock.Text = $"a = {myReg.Coeff[1].ToString()}";
                    BBlock.Text = $"b = {myReg.Coeff[0].ToString()}";
                    CBlock.Text = String.Empty;

                    return myReg.Coeff.Last() * x1 + myReg.Coeff.First();
                };
            }
            else
            {
                func = (x1) =>
                {
                    ABlock.Text = $"a = {myReg.Coeff[2].ToString()}";
                    BBlock.Text = $"b = {myReg.Coeff[1].ToString()}";
                    CBlock.Text = $"c = {myReg.Coeff[0].ToString()}";
                    
                    return myReg.Coeff[2] * x1 * x1 + myReg.Coeff[1] * x1 + myReg.Coeff[0];
                };
            }

            foreach (XY xy in DataGrid.Items)
            {
                SStot += (xy.y - notY) * (xy.y - notY);

                SSres += (xy.y - func(xy.x)) *
                         (xy.y - func(xy.x));
            }

            var r = $"determination = {Math.Sqrt(1 - (SSres / SStot))}";
            CoeffBlock.Text = r;
            if (_plotModel.Series.Any(series => series is FunctionSeries))
            {
                _plotModel.Series.Remove(_plotModel.Series.First(series => series is FunctionSeries));
            }

            var functionSeries = new FunctionSeries(func, 0, 100, 0.001);
            _plotModel.Series.Add(functionSeries);
            PlotView.Model = _plotModel;
        }

        private void RadioButton1_OnChecked(object sender, RoutedEventArgs e)
        {
            polinomes = 1;
            RadioButton2.IsChecked = false;
        }

        private void RadioButton2_OnChecked(object sender, RoutedEventArgs e)
        {
            polinomes = 2;
            RadioButton1.IsChecked = false;
        }
    }

    public struct XY
    {
        public double x { get; set; }
        public double y { get; set; }
    }
}