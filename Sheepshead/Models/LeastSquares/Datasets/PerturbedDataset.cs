// -----------------------------------------------------------------------
// <copyright file="PerturbedDataset.cs" company="ComponentOwl.com">
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

    using MathNet.Numerics.Distributions;
    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Generates set of points normally distributed around model curve.
    /// </summary>
    internal sealed class PerturbedDataset : Dataset
    {
        /// <summary>
        ///   Generate set of points for model fitting.
        /// </summary>
        /// <param name = "model">Model function for which the data points are generated.</param>
        /// <param name = "modelOptions">Model function options.</param>
        /// <param name = "datasetOptions">Dataset generating options.</param>
        /// <param name = "dataX">X-coordinates of the data points.</param>
        /// <param name = "dataY">Y-coordinates of the data points.</param>
        public override void GeneratePoints(
            XyModel model,
            ModelOptions modelOptions,
            DatasetOptions datasetOptions,
            ref Vector<double> dataX,
            ref Vector<double> dataY)
        {
            double dataXCurrent = datasetOptions.DataXStart;
            double dataYCurrent = 0.0;

            Normal normal = new Normal(
                datasetOptions.ErrorMean,
                datasetOptions.ErrorStandardDeviation);

            Random random = new Random();

            for (
                int j = 0;
                j < datasetOptions.PointCount;
                j++, dataXCurrent += (datasetOptions.DataXIntervalMinimum + random.NextDouble() * (datasetOptions.DataXIntervalMaximum - datasetOptions.DataXIntervalMinimum)))
            {
                model.GetValue(dataXCurrent, modelOptions.Parameters, out dataYCurrent);

                dataX[j] = dataXCurrent;
                dataY[j] = (dataYCurrent + normal.Sample());
            }
        }
    }
}
