// -----------------------------------------------------------------------
// <copyright file="Solver.cs" company="ComponentOwl.com">
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

    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Numerics.LinearAlgebra.Generic;

    #endregion

    /// <summary>
    ///   Solves the least squares problem.
    /// </summary>
    public abstract class Solver
    {
        /// <summary>
        ///   Create a new Solver instance from type.
        /// </summary>
        /// <param name = "solverType">Least squares solver type.</param>
        /// <returns>Solver instance.</returns>
        public static Solver FromType(SolverType solverType)
        {
            switch (solverType)
            {
                case SolverType.Normal:
                    return (new NormalSolver());
                case SolverType.Qr:
                    return (new QrSolver());
                case SolverType.Svd:
                    return (new SvdSolver());
                case SolverType.SteepestDescent:
                    return (new SteepestDescentSolver());
                case SolverType.GaussNewton:
                    return (new GaussNewtonSolver());
                case SolverType.LevenbergMarquardt:
                    return (new LevenbergMarquardtSolver());
            }

            throw (new ApplicationException(String.Format("Unknown solver type: '{0}'.", solverType)));
        }

        /// <summary>
        ///   Get value of the objective function.
        /// </summary>
        /// <param name = "model">Model function.</param>
        /// <param name = "pointCount">Number of data points.</param>
        /// <param name = "dataX">X-coordinates of the data points.</param>
        /// <param name = "dataY">Y-coordinates of the data points.</param>
        /// <param name = "parameters">Model function parameters.</param>
        /// <param name = "value">Objective function value.</param>
        protected static void GetObjectiveValue(
            XyModel model,
            int pointCount,
            Vector<double> dataX,
            Vector<double> dataY,
            Vector<double> parameters,
            out double value)
        {
            value = 0.0;

            double y = 0.0;

            for (int j = 0; j < pointCount; j++)
            {
                model.GetValue(
                    dataX[j],
                    parameters,
                    out y);

                value += Math.Pow(
                    y - dataY[j],
                    2.0);
            }

            value *= 0.5;
        }

        //TODO: Call this from overload of NonlinearSolver.Estimate
        protected static void GetObjectiveValue(
            XyModel model,
            int pointCount,
            List<Vector<double>> control,
            Vector<double> dataY,
            Vector<double> parameters,
            out double value)
        {
            value = 0.0;

            double z = 0.0;

            for (int j = 0; j < pointCount; j++)
            {
                model.GetValue(
                    control[j],
                    parameters,
                    out z);

                value += Math.Pow(
                    z - dataY[j],
                    2.0);
            }

            value *= 0.5;
        }

        /// <summary>
        ///   Get Jacobian matrix of the objective function.
        /// </summary>
        /// <param name = "model">Model function.</param>
        /// <param name = "pointCount">Number of data points.</param>
        /// <param name = "dataX">X-coordinates of the data points.</param>
        /// <param name = "dataY">Y-coordinates of the data points.</param>
        /// <param name = "parameters">Model function parameters.</param>
        /// <param name = "jacobian">Jacobian matrix of the objective function.</param>
        protected void GetObjectiveJacobian(
            XyModel model,
            int pointCount,
            Vector<double> dataX,
            Vector<double> dataY,
            Vector<double> parameters,
            ref Matrix<double> jacobian)
        {
            int parameterCount = parameters.Count;

            // fill rows of the Jacobian matrix
            // j-th row of a Jacobian is the gradient of model function in j-th measurement
            for (int j = 0; j < pointCount; j++)
            {
                Vector<double> gradient = new DenseVector(parameterCount);

                model.GetGradient(
                    dataX[j],
                    parameters,
                    ref gradient);

                jacobian.SetRow(j, gradient);
            }
        }

        //TODO: Call this from overload of NonlinearSolver.Estimate
        protected void GetObjectiveJacobian(
            XyModel model,
            int pointCount,
            List<Vector<double>> control,
            Vector<double> dataZ,
            Vector<double> parameters,
            ref Matrix<double> jacobian)
        {
            int parameterCount = parameters.Count;

            // fill rows of the Jacobian matrix
            // j-th row of a Jacobian is the gradient of model function in j-th measurement
            for (int j = 0; j < pointCount; j++)
            {
                Vector<double> gradient = new DenseVector(parameterCount);

                model.GetGradient(
                    control[j],
                    parameters,
                    ref gradient);

                jacobian.SetRow(j, gradient);
            }
        }
    }
}
