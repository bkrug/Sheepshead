// -----------------------------------------------------------------------
// <copyright file="NormalSolver.cs" company="ComponentOwl.com">
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

    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Solves the linear least squares problem using normal equations and Cholesky decomposition.
    /// </summary>
    internal sealed class NormalSolver : LinearSolver
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
        public override void Estimate(
            Model model,
            SolverOptions solverOptions,
            int pointCount,
            Vector<double> dataX,
            Vector<double> dataY,
            ref Vector<double> parameters)
        {
            Matrix<double> jacobian = new DenseMatrix(pointCount, parameters.Count);

            GetObjectiveJacobian(
                model,
                pointCount,
                dataX,
                dataY,
                parameters,
                ref jacobian);

            if (solverOptions.UseInternalSolver)
            {
                // solve normal equations using Cholesky decomposition
                parameters = jacobian.Transpose().Multiply(jacobian).Cholesky().Solve(jacobian.Transpose().Multiply(dataY));
            }
            else
            {
                // solve normal equations directly using matrix inverse
                parameters = jacobian.Transpose().Multiply(jacobian).Inverse().Multiply(jacobian.Transpose().Multiply(dataY));
            }
        }
    }
}
