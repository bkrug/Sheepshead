// -----------------------------------------------------------------------
// <copyright file="NonlinearSolver.cs" company="ComponentOwl.com">
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

    using System;
    using System.Collections.Generic;

    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Solves the nonlinear least squares problem.
    /// </summary>
    internal abstract class NonlinearSolver : Solver
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
        public abstract void Estimate(
            Model model,
            SolverOptions solverOptions,
            int pointCount,
            Vector<double> dataX,
            Vector<double> dataY,
            ref List<Vector<double>> iterations);

        /// <summary>
        ///   Check whether the solver should terminate computation in current iteration.
        /// </summary>
        /// <param name = "valueCurrent">Current value of the objective function.</param>
        /// <param name = "valueNew">New value of the objective function.</param>
        /// <param name = "iterationCount">Number of computed iterations.</param>
        /// <param name = "parametersCurrent">Current estimated model parameters.</param>
        /// <param name = "parametersNew">New estimated model parameters.</param>
        /// <param name = "solverOptions">Least squares solver options.</param>
        /// <returns>The solver should terminate computation in current iteration.</returns>
        protected static bool ShouldTerminate(
            double valueCurrent,
            double valueNew,
            int iterationCount,
            Vector<double> parametersCurrent,
            Vector<double> parametersNew,
            SolverOptions solverOptions)
        {
            return (
                       Math.Abs(valueNew - valueCurrent) <= solverOptions.MinimumDeltaValue ||
                       parametersNew.Subtract(parametersCurrent).Norm(2.0) <= solverOptions.MinimumDeltaParameters ||
                       iterationCount >= solverOptions.MaximumIterations);
        }
    }
}
