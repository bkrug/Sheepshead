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
        protected int _dimCount;

        public ResultPredictor(IStatRepository<K, S> repository)
        {
            Repository = repository;
        }

        protected abstract K CreateKey(K originalKey, List<int> keyValues);
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
            _dimCount = MaxRanges.Count();
            var generatedStat = CreateStat();
            EditStat(_midPoints.Select(p => (double)p).ToList(), key, ref generatedStat);
            ExpandStatRange(0, key, ref generatedStat);
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

        private void EditStat(List<double> point, K originalKey, ref S generatedStat)
        {
            bool skipRecording;
            List<int> recordKey;
            GetClosestValidKey(point, out skipRecording, out recordKey);
            if (!skipRecording)
            {
                var newKey = CreateKey(originalKey, recordKey);
                var recordedStat = Repository.GetRecordedResults(newKey);
                generatedStat.AddOtherStat(recordedStat);
            }
            _nextLayerOfKeys.Add(point);
        }

        private void GetClosestValidKey(List<double> point, out bool skipRecording, out List<int> recordKey)
        {
            skipRecording = false;
            recordKey = point.Select(p => (int)Math.Round(p)).ToList();
            for (var i = 0; i < point.Count() && !skipRecording; ++i)
            {
                var range = MaxRanges.ElementAt(i);
                var halfOfValidRange = _steps[i] / 2;
                //Skip anything that is outside the range of valid values.
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
        }

        private void ExpandStatRange(int depth, K originalKey, ref S generatedStat)
        {
            if (ReachedMinimumTries(generatedStat) || depth + _tolerance >= _maxRange / 4)
                return;
            var curLayer = _nextLayerOfKeys;
            _nextLayerOfKeys = new List<List<double>>();
            foreach (var point in curLayer)
            {
                EditChildStats(point, originalKey, ref generatedStat);
            }
            ExpandStatRange(depth + 1, originalKey, ref generatedStat);
        }

        //Expands stat range in n-dimensional space.  If stats could be graphed, they would resemble an n-dimensional diamond or octahedron.
        private void EditChildStats(List<double> point, K originalKey, ref S generatedStat)
        {
            var d = 0;
            while (d == 0 || d < _dimCount && point[d] == _midPoints[d]) 
            {
                if (point[d] - _tolerance <= _midPoints[d])
                {
                    var newPoint = new List<double>(point);
                    newPoint[d] = point[d] - _steps[d];
                    EditStat(newPoint, originalKey, ref generatedStat);
                }
                if (point[d] + _tolerance >= _midPoints[d])
                {
                    var newPoint = new List<double>(point);
                    newPoint[d] = point[d] + _steps[d];
                    EditStat(newPoint, originalKey, ref generatedStat);
                }
                ++d;
            }
        }
    }

    public struct RangeDetail
    {
        public int Min;
        public int Max;
        public List<int> ValidValues;
        public int? Precision;
    }
}