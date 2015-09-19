// -----------------------------------------------------------------------
// <copyright file="Model.cs" company="ComponentOwl.com">
//     Copyright © 2010-2012 ComponentOwl.com. All rights reserved.
// </copyright>
// <author>Libor Tinka</author>
// -----------------------------------------------------------------------
// This project uses freeware
// Better ListView and Better SplitButton components.
// Check out http://www.componentowl.com
// -----------------------------------------------------------------------
using System.Collections.Generic;

namespace Sheepshead.Models.LeastSquares
{
    #region Usings

    using System;
    using System.Collections.ObjectModel;

    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Common model function.
    /// </summary>
    public abstract class XyModel
    {
        /// <summary>
        ///   Descriptive name of the model function.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        ///   Model function parameter names.
        /// </summary>
        public ReadOnlyCollection<string> ParameterNames
        {
            get
            {
                return this.parameterNames;
            }
        }

        private readonly ReadOnlyCollection<string> parameterNames;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XyModel" /> class.
        /// </summary>
        /// <param name = "parameterNames">Model function parameter names.</param>
        protected XyModel(string[] parameterNames)
        {
            this.parameterNames = new ReadOnlyCollection<string>(parameterNames);
        }

        /// <summary>
        ///   Create a new Model instance from type.
        /// </summary>
        /// <param name = "modelType">Model function type.</param>
        /// <returns>Model instance.</returns>
        public static XyModel FromType(ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.Line:
                    return (new LineModel());
                case ModelType.Parabola:
                    return (new ParabolaModel());
                case ModelType.Power:
                    return (new PowerModel());
                case ModelType.TrigonometricSum:
                    return (new TrigonometricSumModel());
            }

            throw (new ApplicationException(String.Format("Unknown model type: '{0}'.", modelType)));
        }

        /// <summary>
        ///   Get value of the model function for the specified parameters.
        /// </summary>
        /// <param name = "x">X-coordinate of the function point.</param>
        /// <param name = "parameters">Model function parameters.</param>
        /// <param name = "y">Y-coordinate of the function point.</param>
        public abstract void GetValue(
            double x,
            Vector<double> parameters,
            out double y);

        /// <summary>
        ///   Get value of the model function for the specified parameters in multi-dimensional graph.
        /// </summary>
        /// <param name="control">independant coordinates of the function.</param>
        /// <param name = "parameters">Model function parameters.</param>
        /// <param name="z">dependant coordinate of the function point.</param>
        public abstract void GetValue(
            Vector<double> control,
            Vector<double> parameters,
            out double z);

        /// <summary>
        ///   Get gradient of the model function for the specified parameters.
        /// </summary>
        /// <param name = "x">X-coordinate of the function point.</param>
        /// <param name = "parameters">Model function parameters.</param>
        /// <param name = "gradient">Model function gradient.</param>
        public abstract void GetGradient(
            double x,
            Vector<double> parameters,
            ref Vector<double> gradient);

        /// <summary>
        ///   Get gradient of the model function for the specified parameters in multi-dimensional graph.
        /// </summary>
        /// <param name="control">independant coordinates of the function.</param>
        /// <param name="parameters">Model function parameters.</param>
        /// <param name="gradient">Model function gradient.</param>
        public abstract void GetGradient(
            Vector<double> control,
            Vector<double> parameters,
            ref Vector<double> gradient);

        /// <summary>
        ///   Get vector of residuals for the specified parameters.
        /// </summary>
        /// <param name = "pointCount">Number of data points.</param>
        /// <param name = "dataX">X-coordinates of the data points.</param>
        /// <param name = "dataY">Y-coordinates of the data points.</param>
        /// <param name = "parameters">Model function parameters.</param>
        /// <param name = "residual">Vector of residuals.</param>
        public void GetResidualVector(
            int pointCount,
            Vector<double> dataX,
            Vector<double> dataY,
            Vector<double> parameters,
            ref Vector<double> residual)
        {
            double y;

            for (int j = 0; j < pointCount; j++)
            {
                GetValue(
                    dataX[j],
                    parameters,
                    out y);

                residual[j] = (y - dataY[j]);
            }
        }

        public void GetResidualVector(
            int pointCount,
            List<Vector<double>> dataControl,
            Vector<double> dataDependent,
            Vector<double> parameters,
            ref Vector<double> residual)
        {
            double z;

            for (int i = 0; i < dataControl.Count; i++)
                for (int j = 0; j < pointCount; j++)
                {
                    GetValue(
                        dataControl[i][j],
                        parameters,
                        out z);

                    residual[j] = (z - dataControl[i][j]);
                }
        }
    }
}
