// -----------------------------------------------------------------------
// <copyright file="TrigonometricSumModel.cs" company="ComponentOwl.com">
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

    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Trigonometric sum model function.
    /// </summary>
    public sealed class TrigonometricSumModel : NonlinearModel
    {
        /// <summary>
        ///   Descriptive name of the model function.
        /// </summary>
        public override string Name
        {
            get
            {
                return "y = a * cos(b * x) + b * sin(a * x)";
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TrigonometricSumModel" /> class.
        /// </summary>
        public TrigonometricSumModel()
            : base(new[] { "a", "b" })
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
            y = RunEquation(x, parameters);
        }

        public override void GetValue(Vector<double> control, Vector<double> parameters, out double z)
        {
            z = 0;
            for (var i = 0; i < control.Count; ++i)
                z += RunEquation(control[i], parameters.SubVector(i * 2, 2));
        }

        private static double RunEquation(double x, Vector<double> parameters)
        {
            return parameters[0] * Math.Cos(parameters[1] * x) + parameters[1] * Math.Sin(parameters[0] * x);
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
            GetGradient(x, parameters, ref gradient);
        }

        public override void GetGradient(Vector<double> control, Vector<double> parameters, ref Vector<double> gradient)
        {
            for (var i = 0; i < control.Count; ++i)
                GetGradient(control[i], parameters.SubVector(i * 2, 2), ref gradient, i * 2);
        }

        private static void GetGradient(double x, Vector<double> parameters, ref Vector<double> gradient, int gradIndex = 0)
        {
            gradient[0] = (Math.Cos(parameters[1] * x) + parameters[1] * Math.Cos(parameters[0] * x));
            gradient[1] = (-parameters[0] * Math.Sin(parameters[1] * x) * x + Math.Sin(parameters[0] * x));
        }
    }
}
