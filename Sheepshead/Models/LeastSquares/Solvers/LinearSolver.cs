// -----------------------------------------------------------------------
// <copyright file="LinearSolver.cs" company="ComponentOwl.com">
//     Copyright © 2010-2012 ComponentOwl.com. All rights reserved.
// </copyright>
// <author>Libor Tinka</author>
// -----------------------------------------------------------------------
// This project uses freeware
// Better ListView and Better SplitButton components.
// Check out http://www.componentowl.com
// -----------------------------------------------------------------------
using System.Collections.Generic;

namespace Sheepshead.Models.LeastSquares
{
    #region Usings

    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Solves the linear least squares problem.
    /// </summary>
    public abstract class LinearSolver : Solver
    {
        /// <summary>
        ///   Estimate parameters of the model function.
        /// </summary>
        /// <param name = "model">Model function.</param>
        /// <param name = "solverOptions">Least squares solver options.</param>
        /// <param name = "pointCount">Number of data points.</param>
        /// <param name = "dataX">X-coordinates of the data points.</param>
        /// <param name = "dataY">Y-coordinates of the data points.</param>
        /// <param name = "parameters">Estimated model function parameters.</param>
        public abstract void Estimate(
            XyModel model,
            SolverOptions solverOptions,
            int pointCount,
            Vector<double> dataX,
            Vector<double> dataY,
            ref Vector<double> parameters);

        /// <summary>
        ///   Estimate parameters of the model function.
        /// </summary>
        /// <param name = "model">Model function.</param>
        /// <param name = "solverOptions">Least squares solver options.</param>
        /// <param name = "pointCount">Number of data points.</param>
        /// <param name = "control">Control coordinates of the data points.</param>
        /// <param name = "dataZ">Dependent coordinates of the data points.</param>
        /// <param name = "parameters">Estimated model function parameters.</param>
        public abstract void Estimate(
            XyModel model,
            SolverOptions solverOptions,
            int pointCount,
            List<Vector<double>> control,
            Vector<double> dataZ,
            ref Vector<double> parameters);
    }
}
