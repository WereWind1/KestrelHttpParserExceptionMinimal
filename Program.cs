
using System;
using System.IO;
using System.Buffers;
using System.Collections.Generic;

using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.AspNetCore.Builder;


namespace Minimal
{
    public class Segment<T> : ReadOnlySequenceSegment<T>
    {
        public Segment(ReadOnlyMemory<T> memory)
        {
            Memory = memory;
        }

        public void SetNext(Segment<T> next)
        {
            Next = next;
            next.RunningIndex = RunningIndex + Memory.Length;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var first = "Head\r";
            var second = "va\r";
            var segment1 = new Segment<byte>(Encoding.ASCII.GetBytes(first));
            var segment2 = new Segment<byte>(Encoding.ASCII.GetBytes(second));
            segment1.SetNext(segment2);

            var sequence = new ReadOnlySequence<byte>(segment1, 0, segment2, second.Length);
            var requestHandler = new RequestHandler();
            var parser = new HttpParser<RequestHandler>(false);
            var reader = new SequenceReader<byte>(sequence);
            var res = parser.ParseHeaders(requestHandler, ref reader);
        }
    }

    /*
    Helper class for explicit definition ond oberload of  publicmethods of IHttpRequestLineHandler, IHttpHeadersHandler.
    ParseHeaders() calls OnStartLine to fill the requestHandler. We don't need the parsed data so the methods are empty
    */
    public class RequestHandler : IHttpRequestLineHandler, IHttpHeadersHandler
    {
        public string Method { get; set; }

        public string Version { get; set; }

        public string RawTarget { get; set; }

        public string RawPath { get; set; }

        public string Query { get; set; }

        public bool PathEncoded { get; set; }

        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public void OnHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
        {
            ;
        }

        void IHttpHeadersHandler.OnHeadersComplete(bool endStream) { }

        public void OnStartLine(HttpMethod method, HttpVersion version, Span<byte> target, Span<byte> path, Span<byte> query, Span<byte> customMethod, bool pathEncoded)
        {
            ;
        }

        public void OnStartLine(HttpVersionAndMethod versionAndMethod, TargetOffsetPathLength targetPath, Span<byte> startLine)
        {
            ;
        }

        public void OnStaticIndexedHeader(int index)
        {
            throw new NotImplementedException();
        }

        public void OnStaticIndexedHeader(int index, ReadOnlySpan<byte> value)
        {
            throw new NotImplementedException();
        }
    }
 }

