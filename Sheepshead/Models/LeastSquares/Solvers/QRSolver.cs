// -----------------------------------------------------------------------
// <copyright file="QRSolver.cs" company="ComponentOwl.com">
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
    using MathNet.Numerics.LinearAlgebra.Double.Factorization;
    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Solves the linear least squares problem using QR decomposition.
    /// </summary>
    internal sealed class QrSolver : LinearSolver
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
            int n = parameters.Count;

            Matrix<double> jacobian = new DenseMatrix(pointCount, n);

            GetObjectiveJacobian(
                model,
                pointCount,
                dataX,
                dataY,
                parameters,
                ref jacobian);

            // compute QR decomposition of the Jacobian
            QR qr = jacobian.QR();

            if (solverOptions.UseInternalSolver)
            {
                //
                // solve QRx = b using internal QR solver
                //
                parameters = qr.Solve(dataY);
            }
            else
            {
                //
                // solve QRx = b directly using inverse (transpose) of Q and inverse of R
                //
                // get orthogonal matrix with first n columns of Q
                Matrix<double> Q1 = qr.Q.SubMatrix(0, pointCount, 0, n);
                // get upper-triangular matrix of size n x n
                Matrix<double> R = qr.R.SubMatrix(0, n, 0, n);

                parameters = R.Inverse().Multiply(Q1.Transpose().Multiply(dataY));
            }
        }
    }
}
