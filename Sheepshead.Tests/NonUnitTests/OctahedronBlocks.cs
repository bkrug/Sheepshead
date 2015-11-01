using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Sheepshead.Tests.NonUnitTests
{
    [TestClass]
    public class OctahedronBlocks
    {
        private bool[,,,] _blocks = new bool[21, 21, 21, 21];
        private List<List<int>> _nextGen;
        private static int max = 20;
        private static int mid = max / 2;

        [TestMethod]
        public void BuildOctahedron()
        {
            _nextGen = new List<List<int>>();
            SetPoint(mid, mid, mid, mid);
            SetNextGen(0);
            DisplayOctahedron();
        }

        private void SetPoint(int a, int b, int c, int d)
        {
            if (_blocks[a, b, c, d])
                throw new Exception("Point already set.");
            _blocks[a, b, c, d] = true;
            _nextGen.Add(new List<int>() { a, b, c, d });
        }

        private void SetChildren(int a, int b, int c, int d)
        {
            if (c >= mid)
            {
                SetPoint(a, b, c + 1, d);
            }
            if (c <= mid)
            {
                SetPoint(a, b, c - 1, d);
            }
            if (c == mid)
            {
                if (a >= mid && b >= mid)
                    SetPoint(a + 1, b, c, d);
                if (a >= mid && b <= mid)
                    SetPoint(a, b - 1, c, d);
                if (a <= mid && b <= mid)
                    SetPoint(a - 1, b, c, d);
                if (a <= mid && b >= mid)
                    SetPoint(a, b + 1, c, d);
            }
            if (a == mid && b == mid && c == mid)
            {
                if (d <= mid)
                    SetPoint(mid, mid, mid, d - 1);
                if (d >= mid)
                    SetPoint(mid, mid, mid, d + 1);
            }
        }

        private void SetNextGen(int depth)
        {
            if (depth >= mid)
                return;
            var _curGen = _nextGen;
            _nextGen = new List<List<int>>();
            foreach (var point in _curGen)
            {
                SetChildren(point[0], point[1], point[2], point[3]);
            }
            SetNextGen(depth + 1);
        }

        private void DisplayOctahedron()
        {
            using (var writer = new StreamWriter(@"c:\temp\octahedron.txt"))
            {
                for (var d = 0; d <= max; ++d)
                {
                    for (var b = 0; b <= max; ++b)
                    {
                        for (var c = 0; c <= max; ++c)
                        {
                            for (var a = 0; a <= max; ++a)
                            {
                                var symbol = _blocks[a, b, c, d] ? "X" : ".";
                                writer.Write(symbol);
                            }
                            writer.Write("    ");
                        }
                        writer.WriteLine();
                    }
                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine();
                }
            }
        }
    }
}
