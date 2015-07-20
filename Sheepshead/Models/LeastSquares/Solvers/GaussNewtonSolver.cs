// -----------------------------------------------------------------------
// <copyright file="GaussNewtonSolver.cs" company="ComponentOwl.com">
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
    ///   Solves the nonlinear least squares problem using Gauss-Newton iteration.
    /// </summary>
    public sealed class GaussNewtonSolver : NonlinearSolver
    {
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

                Vector<double> step = jacobian.Transpose().Multiply(jacobian).Cholesky().Solve(jacobian.Transpose().Multiply(residual));

                parametersCurrent.Subtract(step, parametersNew);

                GetObjectiveValue(
                    model,
                    pointCount,
                    dataX,
                    dataY,
                    parametersNew,
                    out valueNew);

                iterations.Add(new DenseVector(parametersNew));

                if (ShouldTerminate(
                    valueCurrent,
                    valueNew,
                    iterations.Count,
                    parametersCurrent,
                    parametersNew,
                    solverOptions))
                {
                    break;
                }

                parametersNew.CopyTo(parametersCurrent);
                valueCurrent = valueNew;
            }
        }
    }
}
