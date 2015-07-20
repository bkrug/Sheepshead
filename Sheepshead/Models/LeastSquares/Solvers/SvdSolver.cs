// -----------------------------------------------------------------------
// <copyright file="SvdSolver.cs" company="ComponentOwl.com">
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

    using MathNet.Numerics;
    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Numerics.LinearAlgebra.Double.Factorization;
    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Solves the linear least squares problem using Singular Value Decomposition (SVD).
    /// </summary>
    internal sealed class SvdSolver : LinearSolver
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

            Svd svd = jacobian.Svd(true);

            if (solverOptions.UseInternalSolver)
            {
                //
                // solve USVx = b using internal SVD solver
                //
                parameters = svd.Solve(dataY);
            }
            else
            {
                //
                // solve USVx = b inverse (transpose) of U and inverse (reciprocal values on diagonal) of S
                //
                // get matrix of left singular vectors with first n columns of U
                Matrix<double> U1 = svd.U().SubMatrix(0, pointCount, 0, n);
                // get matrix of singular values
                Matrix<double> S = new DiagonalMatrix(n, n, svd.S().ToArray());
                // get matrix of right singular vectors
                Matrix<double> V = svd.VT().Transpose();

                //
                // NOTE:
                // a direct solution would be
                // x = V.Multiply(SInv).Multiply(U1.Transpose().Multiply(dataY));
                // but we handle rank-deficient Jacobian by taking a minimum norm solution
                //
                parameters.Clear();

                for (int i = 0; i < n; i++)
                {
                    double sigmai = S[i, i]; // i-th singular value

                    if (sigmai.AlmostEqual(0.0))
                    {
                        continue;
                    }

                    Matrix<double> ui = U1.SubMatrix(0, pointCount, i, 1).Transpose(); // i-th column of U1 transposed

                    parameters.Add(
                        V.Column(i).Multiply(ui.Multiply(dataY)[0] / sigmai),
                        parameters);
                }
            }
        }
    }
}
