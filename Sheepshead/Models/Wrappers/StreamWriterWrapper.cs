using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Sheepshead.Models.Wrappers
{
    public interface IStreamWriterWrapper : IDisposable
    {
        void Write(string text);
        void WriteLine(string text);
        void Close();
    }

    public class StreamWriterWrapper : IStreamWriterWrapper
    {
        private StreamWriter _writer;

        public StreamWriterWrapper(string filename)
        {
            _writer = new StreamWriter(filename);
        }

        public void Write(string text)
        {
            _writer.Write(text);
        }

        public void WriteLine(string text)
        {
            _writer.WriteLine(text);
        }

        public void Close()
        {
            _writer.Close();
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}