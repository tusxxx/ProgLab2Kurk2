using System;
using OxyPlot;
using OxyPlot.Series;

namespace ProgLab2Kurk2
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            MyModel = new PlotModel { Title = "Example 1" };
            MyModel.Series.Add(new FunctionSeries(GetFunction, -1, 1, 0.1, "cos(x)"));
        }

        public PlotModel MyModel { get; private set; }

        public double GetFunction(double x)
        {
            return 1;
        }
    }
}