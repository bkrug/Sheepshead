// -----------------------------------------------------------------------
// <copyright file="ParabolaModel.cs" company="ComponentOwl.com">
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
    ///   Parabola model function.
    /// </summary>
    public sealed class ParabolaModel : LinearModel
    {
        /// <summary>
        ///   Descriptive name of the model function.
        /// </summary>
        public override string Name
        {
            get
            {
                return "y = a * x^2 + b * x + c";
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ParabolaModel" /> class.
        /// </summary>
        public ParabolaModel()
            : base(new[] { "a", "b", "c" })
        {
        }

        /// <summary>
        ///   Get value of the model function for the specified parameters.
        /// </summary>
        /// <param name = "x">X-coordinate of the function point.</param>
        /// <param name = "parameters">Model function parameters.</param>
        /// <param name = "y">Y-coordinate of the function point.</param>
        public override void GetValue(
            double x,
            Vector<double> parameters,
            out double y)
        {
            y = (parameters[0] * x * x + parameters[1] * x + parameters[2]);
        }

        /// <summary>
        ///   Get gradient of the model function for the specified parameters.
        /// </summary>
        /// <param name = "x">X-coordinate of the function point.</param>
        /// <param name = "parameters">Model function parameters.</param>
        /// <param name = "gradient">Model function gradient.</param>
        public override void GetGradient(
            double x,
            Vector<double> parameters,
            ref Vector<double> gradient)
        {
            gradient[0] = (x * x);
            gradient[1] = x;
            gradient[2] = 1.0;
        }
    }
}
