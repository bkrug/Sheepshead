// -----------------------------------------------------------------------
// <copyright file="DatasetOptions.cs" company="ComponentOwl.com">
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
    ///   Options for generating point set.
    /// </summary>
    public struct DatasetOptions
    {
        /// <summary>
        ///   Number of points to generate.
        /// </summary>
        public int PointCount
        {
            get
            {
                return this.pointCount;
            }
        }

        /// <summary>
        ///   X-coordinate of the first generated point.
        /// </summary>
        public double DataXStart
        {
            get
            {
                return this.dataXStart;
            }
        }

        /// <summary>
        ///   Minimum increment to compute X-coordinate of the next point.
        /// </summary>
        public double DataXIntervalMinimum
        {
            get
            {
                return this.dataXIntervalMinimum;
            }
        }

        /// <summary>
        ///   Maximum increment to compute X-coordinate of the next point.
        /// </summary>
        public double DataXIntervalMaximum
        {
            get
            {
                return this.dataXIntervalMaximum;
            }
        }

        /// <summary>
        ///   Mean of the point Y-coordinate error distribution.
        /// </summary>
        public double ErrorMean
        {
            get
            {
                return this.errorMean;
            }
        }

        /// <summary>
        ///   Standard deviation of the point Y-coordinate error distribution.
        /// </summary>
        public double ErrorStandardDeviation
        {
            get
            {
                return this.errorStandardDeviation;
            }
        }

        private readonly int pointCount;
        private readonly double dataXStart;
        private readonly double dataXIntervalMinimum;
        private readonly double dataXIntervalMaximum;
        private readonly double errorMean;
        private readonly double errorStandardDeviation;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DatasetOptions" /> struct.
        /// </summary>
        /// <param name = "pointCount">Number of points to generate.</param>
        /// <param name = "dataXStart">X-coordinate of the first generate point.</param>
        /// <param name = "dataXIntervalMinimum">Minimum increment to compute X-coordinate of the next point.</param>
        /// <param name = "dataXIntervalMaximum">Maximum increment to compute X-coordinate of the next point.</param>
        /// <param name = "errorMean">Mean of the point Y-coordinate error distribution.</param>
        /// <param name = "errorStandardDeviation">Standard deviation of the point Y-coordinate error distribution.</param>
        public DatasetOptions(
            int pointCount,
            double dataXStart,
            double dataXIntervalMinimum,
            double dataXIntervalMaximum,
            double errorMean,
            double errorStandardDeviation)
        {
            this.pointCount = pointCount;
            this.dataXStart = dataXStart;
            this.dataXIntervalMinimum = dataXIntervalMinimum;
            this.dataXIntervalMaximum = dataXIntervalMaximum;
            this.errorMean = errorMean;
            this.errorStandardDeviation = errorStandardDeviation;
        }
    }
}
