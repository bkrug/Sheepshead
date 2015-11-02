using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sheepshead.Models.Players.Stats
{
    public abstract class ResultPredictor<K, S> where S : IStat<S> where K : IStatUniqueKey
    {
        protected readonly IStatRepository<K, S> Repository;
        protected Dictionary<string, RangeDetail> MaxRanges;
        protected List<List<double>> _nextLayerOfKeys;
        protected List<double> _steps;
        protected List<int> _midPoints;
        protected double _maxRange;
        private const double _tolerance = 0.0001;

        public ResultPredictor(IStatRepository<K, S> repository)
        {
            Repository = repository;
        }

        protected abstract Dictionary<K, bool> CreateKeyList();
        protected abstract S CreateStat();
        protected abstract bool ReachedMinimumTries(S generatedStat);

        public virtual S GetWeightedStat(K key)
        {
            var realStat = Repository.GetRecordedResults(key);
            if (ReachedMinimumTries(realStat))
                return realStat;
            return GenerateStat(key);
        }

        private S GenerateStat(K key)
        {
            _maxRange = MaxRanges.Max(r => (r.Value.Max - r.Value.Min) / (r.Value.Precision ?? 1));
            _steps = MaxRanges.Select(r => (r.Value.Max - r.Value.Min) / _maxRange).ToList();
            _midPoints = GetStartingPoint(key);
            _nextLayerOfKeys = new List<List<double>>();
            var generatedStat = CreateStat();
            SetPoint(_midPoints.Select(p => (double)p).ToList(), key, ref generatedStat);
            SetNextGen(0, key, ref generatedStat);
            return generatedStat;
        }

        private List<int> GetStartingPoint(K originalKey)
        {
            var type = originalKey.GetType();
            var startingPoint = new List<int>();
            foreach (var range in MaxRanges)
                startingPoint.Add((int)type.GetField(range.Key).GetValue(originalKey));
            return startingPoint;
        }

        private void SetPoint(List<double> point, K originalKey, ref S generatedStat)
        {
            var skipRecording = false;
            var recordKey = point.Select(p => (int)Math.Round(p)).ToList();
            for (var i = 0; i < point.Count() && !skipRecording; ++i) {
                var range = MaxRanges.ElementAt(i);
                var halfOfValidRange = _steps[i] / 2;
                if (_midPoints[i] - _tolerance > range.Value.Max || _midPoints[i] + _tolerance < range.Value.Min)
                    skipRecording = true;
                else if (range.Value.ValidValues != null && range.Value.ValidValues.Any())
                {
                    int? newValue = null;
                    foreach (var match in range.Value.ValidValues.Where(v => v + halfOfValidRange >= point[i] && point[i] > v - halfOfValidRange))
                        newValue = match;
                    //If the current value is not within half a step of one valid value, skip recording this.
                    skipRecording = newValue == null;
                    recordKey[i] = newValue ?? 0;
                }
                else
                {
                    //If the current value is not within half a step of an integer, skip recording this.
                    var roundedValue = (int)Math.Round(point[i]);
                    skipRecording = point[i] - roundedValue > halfOfValidRange
                                 || roundedValue - point[i] >= halfOfValidRange;
                }
            }
            if (!skipRecording)
            {
                var newKey = CreateKey(originalKey, recordKey);
                var recordedStat = Repository.GetRecordedResults(newKey);
                generatedStat.AddOtherStat(recordedStat);
            }
            _nextLayerOfKeys.Add(point);
        }

        private void SetNextGen(int depth, K originalKey, ref S generatedStat)
        {
            if (ReachedMinimumTries(generatedStat) || depth + _tolerance >= _maxRange / 2)
                return;
            var curLayer = _nextLayerOfKeys;
            _nextLayerOfKeys = new List<List<double>>();
            foreach (var point in curLayer)
            {
                SetChildren(point, originalKey, ref generatedStat);
            }
            SetNextGen(depth + 1, originalKey, ref generatedStat);
        }

        private void SetChildren(List<double> point, K originalKey, ref S generatedStat)
        {
            var a = point[0];
            var b = point[1];
            var c = point[2];
            var midPointA = _midPoints[0];
            var midPointB = _midPoints[1];
            var midPointC = _midPoints[2];
            var stepA = _steps[0];
            var stepB = _steps[1];
            var stepC = _steps[2];
            if (c + _tolerance >= midPointC)
            {
                var newPoint = new List<double>(point);
                newPoint[2] = c + stepC;
                SetPoint(newPoint, originalKey, ref generatedStat);
            }
            if (c - _tolerance <= midPointC)
            {
                var newPoint = new List<double>(point);
                newPoint[2] = c - stepC;
                SetPoint(newPoint, originalKey, ref generatedStat);
            }
            if (Math.Abs(c - midPointC) < _tolerance)
            {
                if (a + _tolerance >= midPointA && b + _tolerance >= midPointB)
                {
                    var newPoint = new List<double>(point);
                    newPoint[0] = a + stepA;
                    SetPoint(newPoint, originalKey, ref generatedStat);
                }
                if (a + _tolerance >= midPointA && b - _tolerance <= midPointB)
                {
                    var newPoint = new List<double>(point);
                    newPoint[1] = b - stepB;
                    SetPoint(newPoint, originalKey, ref generatedStat);
                }
                if (a - _tolerance <= midPointA && b - _tolerance <= midPointB)
                {
                    var newPoint = new List<double>(point);
                    newPoint[0] = a - stepA;
                    SetPoint(newPoint, originalKey, ref generatedStat);
                }
                if (a - _tolerance <= midPointA && b + _tolerance >= midPointB)
                {
                    var newPoint = new List<double>(point);
                    newPoint[1] = b + stepB;
                    SetPoint(newPoint, originalKey, ref generatedStat);
                }
            }
            for (var d = 3; d < point.Count(); ++d)
            {
                var allEarlierParmsEqual = true;
                for (var i = 0; i < d && allEarlierParmsEqual; ++i)
                    allEarlierParmsEqual = Math.Abs(point[i] - _midPoints[i]) < _tolerance;
                if (allEarlierParmsEqual)
                {
                    if (point[d] - _tolerance <= _midPoints[d])
                    {
                        var newPoint = new List<double>(point);
                        newPoint[d] = point[d] - _steps[d];
                        SetPoint(newPoint, originalKey, ref generatedStat);
                    }
                    if (point[d] + _tolerance >= _midPoints[d])
                    {
                        var newPoint = new List<double>(point);
                        newPoint[d] = point[d] + _steps[d];
                        SetPoint(newPoint, originalKey, ref generatedStat);
                    }
                }
            }
        }

        protected abstract K CreateKey(K originalKey, List<int> keyValues);
    }

    public struct RangeDetail
    {
        public int Min;
        public int Max;
        public List<int> ValidValues;
        public int? Precision;
    }
}