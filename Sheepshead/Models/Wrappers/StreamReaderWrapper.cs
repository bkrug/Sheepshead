using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Sheepshead.Models.Wrappers
{
    public interface IStreamReaderWrapper : IDisposable
    {
        string ReadToEnd();
        string ReadLine();
        void Close();
    }

    public class StreamReaderWrapper : IStreamReaderWrapper
    {
        private StreamReader _stream;

        public StreamReaderWrapper(string filename)
        {
            EnsureFileExists(filename);
            _stream = new StreamReader(filename);
        }

        private void EnsureFileExists(string filename)
        {
            if (!File.Exists(filename))
            {
                if (filename.LastIndexOf(@"\") >= 0)
                {
                    var path = filename.Substring(0, filename.LastIndexOf(@"\"));
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                using (var writer = File.CreateText(filename))
                {
                }
            }
        }

        public string ReadLine()
        {
            return _stream.ReadLine();
        }

        public string ReadToEnd()
        {
            return _stream.ReadToEnd();
        }

        public void Close()
        {
            _stream.Close();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}