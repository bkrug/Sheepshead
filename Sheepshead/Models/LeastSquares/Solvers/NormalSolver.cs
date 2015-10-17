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
    public sealed class NormalSolver : LinearSolver
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
            XyModel model,
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

            Estimate(ref solverOptions, dataY, ref parameters, jacobian);
        }

        public override void Estimate(XyModel model, SolverOptions solverOptions, int pointCount, System.Collections.Generic.List<Vector<double>> control, Vector<double> dataZ, ref Vector<double> parameters)
        {
            Matrix<double> jacobian = new DenseMatrix(pointCount, parameters.Count);

            GetObjectiveJacobian(
                model,
                pointCount,
                control,
                dataZ,
                parameters,
                ref jacobian);

            Estimate(ref solverOptions, dataZ, ref parameters, jacobian);
        }

        private static void Estimate(ref SolverOptions solverOptions, Vector<double> dependant, ref Vector<double> parameters, Matrix<double> jacobian)
        {
            if (solverOptions.UseInternalSolver)
            {
                // solve normal equations using Cholesky decomposition
                parameters = jacobian.Transpose().Multiply(jacobian).Cholesky().Solve(jacobian.Transpose().Multiply(dependant));
            }
            else
            {
                // solve normal equations directly using matrix inverse
                parameters = jacobian.Transpose().Multiply(jacobian).Inverse().Multiply(jacobian.Transpose().Multiply(dependant));
            }
        }
    }
}
