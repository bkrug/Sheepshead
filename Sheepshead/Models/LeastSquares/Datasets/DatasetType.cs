// -----------------------------------------------------------------------
// <copyright file="DatasetType.cs" company="ComponentOwl.com">
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
    ///   Type of point set to generate.
    /// </summary>
    internal enum DatasetType
    {
        /// <summary>
        ///   The points lays exactly on the model function.
        /// </summary>
        Exact,
        /// <summary>
        ///   The points can deviate from the model function.
        /// </summary>
        Perturbed
    }
}
