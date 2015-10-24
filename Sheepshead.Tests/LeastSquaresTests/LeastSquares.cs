using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sheepshead.Models.LeastSquares;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sheepshead.Tests.LeastSquaresTests
{
    [TestClass]
    public class LeastSquares
    {
        const int midwayPoint = 3;

        //[TestMethod]
        public void Test1()
        {
            var nonlinearSolver = (LevenbergMarquardtSolver)Solver.FromType(SolverType.LevenbergMarquardt);
            List<Vector<double>> iterations = new List<Vector<double>>();
            var dataX = GetXData();
            var dataY = GetYData(dataX);
            var lineModel = new PowerModel();
            var dataSetOpts = new DatasetOptions(dataX.Count(), 0, .5, 1.0, 0.0, 0.1);
            nonlinearSolver.Estimate(
                lineModel,
                new SolverOptions(true, 0.0001, 0.0001, 50, new DenseVector(new[] { 1.0, 1.0 })),
                dataSetOpts.PointCount,
                dataX,
                dataY,
                ref iterations);
            using (var sw = new StreamWriter(@"C:\Temp\GraphPoints.csv"))
            {
                for (var i = 0; i < dataX.Count(); ++i) {
                    var x = dataX[i];
                    double y1;
                    lineModel.GetValue(x, iterations[iterations.Count - 1], out y1);
                    sw.WriteLine(dataX[i] + "," + dataY[i] + "," + y1);
                }
            }
        }

        private DenseVector GetXData()
        {
            var array = new List<double>();
            for (double x = 0; x <= 10; x += (x < midwayPoint ? 1 : 0.1))
                array.Add(x > 0 ? x : 0.001);
            var vector = new DenseVector(array.Count());
            for (var i = 0; i < array.Count(); ++i)
                vector[i] = array[i];
            return vector;
        }

        private DenseVector GetYData(DenseVector xVector)
        {
            //var array = new double[] {0.01, 4, 6, 7, 7.5, 7.75, 7.875, 7.9, 7.92, 7.93};
            var vector = new DenseVector(xVector.Count());
            for (var i = 0; i < xVector.Count; ++i)
                vector[i] = xVector[i] < midwayPoint ? xVector[i] : 3.0 * Math.Pow(xVector[i] + 37.0, 0.5) - 47.0;
            return vector;
        }

        //[TestMethod]
        public void ThreeDimTest()
        {
            var nonlinearSolver = (LevenbergMarquardtSolver)Solver.FromType(SolverType.LevenbergMarquardt);
            List<Vector<double>> iterations = new List<Vector<double>>();
            List<Vector<double>> control;
            Vector<double> dataZ;
            GetControl(out control, out dataZ);
            var lineModel = new PowerModel();
            var dataSetOpts = new DatasetOptions(control.Count(), 0, .5, 1.0, 0.0, 0.1);
            nonlinearSolver.Estimate(
                lineModel,
                new SolverOptions(true, 0.0001, 0.0001, 50, new DenseVector(new[] { 5, 2, 3, 3.5 })),
                dataSetOpts.PointCount,
                control,
                dataZ,
                ref iterations);
            using (var sw = new StreamWriter(@"C:\Temp\3dGraphPoints.csv"))
            {
                for (var x = 0; x <= 10; ++x)
                {
                    for (var y = 0; y <= 10; ++y)
                    {
                        double realZ = RunEquation(x, y);
                        double predictedZ;
                        var xAndY = new DenseVector(new [] {(double) x, y});
                        lineModel.GetValue(xAndY, iterations[iterations.Count - 1], out predictedZ);
                        sw.Write(realZ.ToString("F1") + " : " + predictedZ.ToString("F1") + ",");
                    }
                    sw.WriteLine("");
                }
                sw.WriteLine("");
                sw.Write("Parameters: ,");
                foreach (var eqParam in iterations[iterations.Count - 1])
                    sw.Write(eqParam.ToString("F3") + ",");
                sw.WriteLine("");
            }
        }

        public void GetControl(out List<Vector<double>> control, out Vector<double> dependant) {
            var xList = new List<double>();
            var yList = new List<double>();
            var zList = new List<double>();
            var random = new Random();
            for (var x = 5; x <= 10; ++x)
            {
                for (var y = 5; y <= 10; ++y)
                {
                    xList.Add(x);
                    yList.Add(y);
                    var alteration = random.NextDouble() * 20 - 10;
                    var z = RunEquation(x, y);
                    zList.Add(z + alteration);
                }
            }
            control = new List<Vector<double>>();
            control.Add(new DenseVector(xList.ToArray()));
            control.Add(new DenseVector(yList.ToArray()));
            dependant = new DenseVector(zList.ToArray());
        }

        private static double RunEquation(int x, int y)
        {
            return 5.0 * Math.Pow(x, 2) + 3.0 * Math.Pow((double)y, 3.5);
        }

        //[TestMethod]
        public void Test2()
        {
            var nonlinearSolver = (LevenbergMarquardtSolver)Solver.FromType(SolverType.LevenbergMarquardt);
            List<Vector<double>> iterations = new List<Vector<double>>();
            var dataX = GetXData2();
            var dataY = GetYData2(dataX);
            var lineModel = new PowerModel();
            var dataSetOpts = new DatasetOptions(dataX.Count(), 0, .5, 1.0, 0.0, 0.1);
            nonlinearSolver.Estimate(
                lineModel,
                new SolverOptions(true, 0.0001, 0.0001, 50, new DenseVector(new[] { 1.0, 1.0 })),
                dataSetOpts.PointCount,
                dataX,
                dataY,
                ref iterations);
            using (var sw = new StreamWriter(@"C:\Temp\GraphPoints.csv"))
            {
                for (var i = 0; i < iterations.Last().Count(); ++i)
                    sw.Write(iterations.Last()[i] + "    ");
                sw.WriteLine();
                for (var i = 0; i < dataX.Count(); ++i)
                {
                    var x = dataX[i];
                    double y1;
                    lineModel.GetValue(x, iterations[iterations.Count - 1], out y1);
                    sw.WriteLine(dataX[i] + "," + dataY[i] + "," + y1);
                }
            }
        }

        private DenseVector GetXData2()
        {
            var array = new List<double>();
            for (double x = 0; x <= 10; x += (x < midwayPoint ? 1 : 0.1))
                array.Add(x > 0 ? x : 0.001);
            var vector = new DenseVector(array.Count());
            for (var i = 0; i < array.Count(); ++i)
                vector[i] = array[i];
            return vector;
        }

        private DenseVector GetYData2(DenseVector xVector)
        {
            var vector = new DenseVector(xVector.Count());
            for (var i = 0; i < xVector.Count; ++i)
                vector[i] = -4.0 * Math.Pow(xVector[i], 0.87) + 12;
            return vector;
        }

        //[TestMethod]
        public void LinearEquation()
        {
            //var solver = (NormalSolver)Solver.FromType(SolverType.Normal);
            //List<Vector<double>> iterations = new List<Vector<double>>();
            //List<Vector<double>> control;
            //Vector<double> dataZ;
            //GetControl(out control, out dataZ);
            //var lineModel = new PowerModel();
            //var dataSetOpts = new DatasetOptions(control.Count(), 0, .5, 1.0, 0.0, 0.1);
            //solver.Estimate(
            //    lineModel,
            //    new SolverOptions(true, 0.0001, 0.0001, 50, new DenseVector(new[] { 5, 2, 3, 3.5 })),
            //    dataSetOpts.PointCount,
            //    control,
            //    dataZ,
            //    ref iterations);
        }
    }
}
