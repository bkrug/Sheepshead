// -----------------------------------------------------------------------
// <copyright file="SteepestDescentSolver.cs" company="ComponentOwl.com">
//     Copyright © 2010-2012 ComponentOwl.com. All rights reserved.
// </copyright>
// <author>Libor Tinka</author>
// -----------------------------------------------------------------------
// This project uses freeware
// Better ListView and Better SplitButton components.
// Check out http://www.componentowl.com
// -----------------------------------------------------------------------

namespace Sheepshead.Models.LeastSquares
{
    #region Usings

    using System.Collections.Generic;

    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Solves the nonlinear least squares problem using steepest descent iteration.
    /// </summary>
    public sealed class SteepestDescentSolver : NonlinearSolver
    {
        private const double MinimumStepSize = 0.000001;

        private const double StepSizeInitial = 0.1;
        private const double StepSizeFactor = 2.0;

        /// <summary>
        ///   Estimates the specified model.
        /// </summary>
        /// <param name = "model">Model function.</param>
        /// <param name = "solverOptions">Least squares solver options.</param>
        /// <param name = "pointCount">Number of data points.</param>
        /// <param name = "dataX">X-coordinates of the data points.</param>
        /// <param name = "dataY">Y-coordinates of the data points.</param>
        /// <param name = "iterations">Estimated model function parameters.</param>
        public override void Estimate(
            XyModel model,
            SolverOptions solverOptions,
            int pointCount,
            Vector<double> dataX,
            Vector<double> dataY,
            ref List<Vector<double>> iterations)
        {
            int n = solverOptions.Guess.Count;

            Vector<double> parametersCurrent = new DenseVector(solverOptions.Guess);
            Vector<double> parametersNew = new DenseVector(n);

            double valueCurrent;
            double valueNew;

            GetObjectiveValue(
                model,
                pointCount,
                dataX,
                dataY,
                parametersCurrent,
                out valueCurrent);

            double stepSize = StepSizeInitial;

            while (true)
            {
                Matrix<double> jacobian = new DenseMatrix(pointCount, n);
                Vector<double> residual = new DenseVector(pointCount);

                GetObjectiveJacobian(
                    model,
                    pointCount,
                    dataX,
                    dataY,
                    parametersCurrent,
                    ref jacobian);

                model.GetResidualVector(
                    pointCount,
                    dataX,
                    dataY,
                    parametersCurrent,
                    ref residual);

                // compute steepest descent orientation
                Vector<double> step = jacobian.Transpose().Multiply(residual).Normalize(2.0);

                do
                {
                    // update estimated model parameters using steepest descent step elongated by stepSize
                    parametersNew = parametersCurrent.Subtract(step.Multiply(stepSize));

                    GetObjectiveValue(
                        model,
                        pointCount,
                        dataX,
                        dataY,
                        parametersNew,
                        out valueNew);

                    if (valueNew >= valueCurrent)
                    {
                        // the step was too long to decrease function value - shorten the step
                        stepSize /= StepSizeFactor;
                    }
                }
                while (
                    // iterate until function value is decreased or the step is too show
                    valueNew >= valueCurrent &&
                    stepSize >= MinimumStepSize);

                iterations.Add(new DenseVector(parametersNew));

                if (stepSize <= MinimumStepSize ||
                    ShouldTerminate(
                        valueCurrent,
                        valueNew,
                        iterations.Count,
                        parametersCurrent,
                        parametersNew,
                        solverOptions))
                {
                    break;
                }

                if (valueNew < valueCurrent)
                {
                    // the was short enough to decreate function value - elongate the step
                    stepSize *= StepSizeFactor;
                }

                parametersNew.CopyTo(parametersCurrent);
                valueCurrent = valueNew;
            }
        }

        public override void Estimate(XyModel model, SolverOptions solverOptions, int pointCount, List<Vector<double>> control, Vector<double> dataZ, ref List<Vector<double>> iterations)
        {
            throw new System.NotImplementedException();
        }
    }
}
