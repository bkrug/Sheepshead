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

        [TestMethod]
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
                vector[i] = xVector[i] < midwayPoint ? xVector[i] : 3.0 * Math.Pow(xVector[i], 0.5);
            return vector;
        }
    }
}
