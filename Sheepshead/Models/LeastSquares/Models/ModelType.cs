// -----------------------------------------------------------------------
// <copyright file="ModelType.cs" company="ComponentOwl.com">
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
    ///   Model function type.
    /// </summary>
    public enum ModelType
    {
        /// <summary>
        ///   Line model.
        /// </summary>
        Line,
        /// <summary>
        ///   Parabola model.
        /// </summary>
        Parabola,
        /// <summary>
        ///   Power function model.
        /// </summary>
        Power,
        /// <summary>
        ///   Sum of trigonometric functions model.
        /// </summary>
        TrigonometricSum
    }
}
