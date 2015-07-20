// -----------------------------------------------------------------------
// <copyright file="SolverOptions.cs" company="ComponentOwl.com">
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

    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Least squares solver options.
    /// </summary>
    public struct SolverOptions
    {
        /// <summary>
        ///   Use linear system solver provided by Math.NET.
        /// </summary>
        public bool UseInternalSolver
        {
            get
            {
                return this.useInternalSolver;
            }
        }

        /// <summary>
        ///   Change in objective function value to stop iteration.
        /// </summary>
        public double MinimumDeltaValue
        {
            get
            {
                return this.minimumDeltaValue;
            }
        }

        /// <summary>
        ///   Change in model function parameters to stop iteration.
        /// </summary>
        public double MinimumDeltaParameters
        {
            get
            {
                return this.minimumDeltaParameters;
            }
        }

        /// <summary>
        ///   Number of iterations to stop processing.
        /// </summary>
        public int MaximumIterations
        {
            get
            {
                return this.maximumIterations;
            }
        }

        /// <summary>
        ///   Initial guess for nonlinear least squares solvers.
        /// </summary>
        public Vector<double> Guess
        {
            get
            {
                return this.guess;
            }
        }

        private readonly bool useInternalSolver;
        private readonly double minimumDeltaValue;
        private readonly double minimumDeltaParameters;
        private readonly int maximumIterations;
        private readonly Vector<double> guess;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SolverOptions" /> struct.
        /// </summary>
        /// <param name = "useInternalSolver">Use linear system solver provided by Math.NET.</param>
        /// <param name = "minimumDeltaValue">Change in objective function value to stop iteration.</param>
        /// <param name = "minimumDeltaParameters">Change in model function parameters to stop iteration.</param>
        /// <param name = "maximumIterations">Number of iterations to stop processing.</param>
        /// <param name = "guess">Initial guess for nonlinear least squares solvers.</param>
        public SolverOptions(
            bool useInternalSolver,
            double minimumDeltaValue,
            double minimumDeltaParameters,
            int maximumIterations,
            Vector<double> guess)
        {
            this.useInternalSolver = useInternalSolver;
            this.minimumDeltaValue = minimumDeltaValue;
            this.minimumDeltaParameters = minimumDeltaParameters;
            this.maximumIterations = maximumIterations;
            this.guess = guess;
        }
    }
}
