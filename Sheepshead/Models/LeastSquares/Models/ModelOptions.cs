// -----------------------------------------------------------------------
// <copyright file="ModelOptions.cs" company="ComponentOwl.com">
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
    ///   Model function options.
    /// </summary>
    internal struct ModelOptions
    {
        /// <summary>
        ///   Model function parameters.
        /// </summary>
        public Vector<double> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        private readonly Vector<double> parameters;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ModelOptions" /> struct.
        /// </summary>
        /// <param name = "parameters">Model function parameters.</param>
        public ModelOptions(Vector<double> parameters)
        {
            this.parameters = parameters;
        }
    }
}
