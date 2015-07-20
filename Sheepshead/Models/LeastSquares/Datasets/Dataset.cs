// -----------------------------------------------------------------------
// <copyright file="Dataset.cs" company="ComponentOwl.com">
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
    ///   Generates a set of points to be fitted by some model fuction.
    /// </summary>
    internal abstract class Dataset
    {
        public static Dataset FromType(DatasetType datasetType)
        {
            switch (datasetType)
            {
                case DatasetType.Exact:
                    return (new ExactDataset());
                case DatasetType.Perturbed:
                    return (new PerturbedDataset());
            }

            throw (new ApplicationException(String.Format("Unknown dataset type: '{0}'.", datasetType)));
        }

        /// <summary>
        ///   Generate set of points for model fitting.
        /// </summary>
        /// <param name = "model">Model function for which the data points are generated.</param>
        /// <param name = "modelOptions">Model function options.</param>
        /// <param name = "datasetOptions">Dataset generating options.</param>
        /// <param name = "dataX">X-coordinates of the data points.</param>
        /// <param name = "dataY">Y-coordinates of the data points.</param>
        public abstract void GeneratePoints(
            XyModel model,
            ModelOptions modelOptions,
            DatasetOptions datasetOptions,
            ref Vector<double> dataX,
            ref Vector<double> dataY);
    }
}
