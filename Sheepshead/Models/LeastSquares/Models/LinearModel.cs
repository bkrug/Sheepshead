// -----------------------------------------------------------------------
// <copyright file="LinearModel.cs" company="ComponentOwl.com">
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
    ///   Model function which is linear in parameters.
    /// </summary>
    internal abstract class LinearModel : Model
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "LinearModel" /> class.
        /// </summary>
        /// <param name = "parameterNames">Model function parameter names.</param>
        protected LinearModel(string[] parameterNames)
            : base(parameterNames)
        {
        }
    }
}
