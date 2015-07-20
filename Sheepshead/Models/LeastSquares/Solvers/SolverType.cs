// -----------------------------------------------------------------------
// <copyright file="SolverType.cs" company="ComponentOwl.com">
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
    /// <summary>
    ///   Type of least squares solver.
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        ///   Solver based on normal equations and Cholesky decomposition.
        /// </summary>
        Normal,
        /// <summary>
        ///   Solver based on QR decomposition.
        /// </summary>
        Qr,
        /// <summary>
        ///   Solver based on singular value decomposition (SVD).
        /// </summary>
        Svd,
        /// <summary>
        ///   Solver based on steepest descent iteration.
        /// </summary>
        SteepestDescent,
        /// <summary>
        ///   Solver based on Gauss-Newton iteration.
        /// </summary>
        GaussNewton,
        /// <summary>
        ///   Solver based on Levenberg-Marquardt iteration.
        /// </summary>
        LevenbergMarquardt
    }
}
