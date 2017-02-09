﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Fx.Portability.ObjectModel;
using Microsoft.Fx.Portability.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.Fx.Portability
{
    internal class CompressedHttpClient : HttpClient
    {
        /// <param name="productName">Product name that will be displayed in the User Agent string of requests</param>
        /// <param name="productVersion">Product version that will be displayed in the User Agent string of requests</param>
        public CompressedHttpClient(ProductInformation info)
            : this(info, new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
        {
        }

        public CompressedHttpClient(ProductInformation info, HttpMessageHandler handler)
            : base(handler)
        {
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DefaultRequestHeaders.AcceptLanguage.TryParseAdd(CultureInfo.CurrentCulture.ToString());

            DefaultRequestHeaders.Add("Client-Type", info.Name);
            DefaultRequestHeaders.Add("Client-Version", info.Version);
        }

        public async Task<ServiceResponse<TResponse>> CallAsync<TResponse>(HttpMethod method, string requestUri)
        {
            using (var request = new HttpRequestMessage(method, requestUri))
            {
                return await CallInternalAsync<TResponse>(request);
            }
        }

        public async Task<ServiceResponse<byte[]>> CallAsync(HttpMethod method, string requestUri, ResultFormatInformation format)
        {
            var result = await CallAsync(method, requestUri, new[] { format });

            return new ServiceResponse<byte[]>(result.Response.Data, result.Headers);
        }

        public async Task<ServiceResponse<ReportingResultWithFormat>> CallAsync(HttpMethod method, string requestUri, IEnumerable<ResultFormatInformation> format)
        {
            using (var request = new HttpRequestMessage(method, requestUri))
            {
                var response = await CallInternalAsync(request, format);

                return new ServiceResponse<ReportingResultWithFormat>(response.Response.FirstOrDefault(), response.Headers);
            }
        }

        public async Task<ServiceResponse<IEnumerable<ReportingResultWithFormat>>> CallAsync<TRequest>(HttpMethod method, string requestUri, TRequest requestData, IEnumerable<ResultFormatInformation> formats)
        {
            var content = requestData.Serialize().Compress();

            using (var request = new HttpRequestMessage(method, requestUri))
            {
                request.Content = new ByteArrayContent(content);
                request.Content.Headers.ContentEncoding.Add("gzip");

                return await CallInternalAsync(request, formats);
            }
        }

        public async Task<ServiceResponse<TResponse>> CallAsync<TRequest, TResponse>(HttpMethod method, string requestUri, TRequest requestData)
        {
            var content = requestData.Serialize().Compress();

            using (var request = new HttpRequestMessage(method, requestUri))
            {
                request.Content = new ByteArrayContent(content);
                request.Content.Headers.ContentEncoding.Add("gzip");

                return await CallInternalAsync<TResponse>(request);
            }
        }

        private async Task ProcessBadRequestAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (string.Equals(response.ReasonPhrase, typeof(UnknownTargetException).Name, StringComparison.Ordinal))
            {
                throw new UnknownTargetException(content);
            }

            //Trace.TraceError(string.Format("Unknown HttpStatusCode.BadRequest: {0} [{1}]", response.ReasonPhrase, content));

            throw new PortabilityAnalyzerException(LocalizedStrings.UnknownBadRequestMessage);
        }

        private async Task<ServiceResponse<TResponse>> CallInternalAsync<TResponse>(HttpRequestMessage request)
        {
            var json = new ResultFormatInformation
            {
                DisplayName = "Json",
                MimeType = "application/json",
                FileExtension = ".json"
            }; ;

            var response = await CallInternalAsync(request, new[] { json });
            var result = response.Response.Single().Data.Deserialize<TResponse>();

            return new ServiceResponse<TResponse>(result, response.Headers);
        }

        private async Task<byte[]> ReadStreamToEnd(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);

                return ms.ToArray();
            }
        }

        private async Task<ServiceResponse<IEnumerable<ReportingResultWithFormat>>> CallInternalAsync(HttpRequestMessage request, IEnumerable<ResultFormatInformation> formats)
        {
            var formatMap = formats.ToDictionary(f => f.MimeType, f => f.DisplayName);

            try
            {
                if (request.Content != null)
                {
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                foreach (var format in formats)
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(format.MimeType));
                }

                using (var response = await SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var contentType = response.Content.Headers.ContentType;
                        if (string.Equals("multipart/mixed", contentType.MediaType, StringComparison.OrdinalIgnoreCase))
                        {
                            var boundary = contentType.Parameters.FirstOrDefault(p => string.Equals("boundary", p.Name, StringComparison.OrdinalIgnoreCase))?.Value
                                .Trim('\"');

                            Debug.Assert(boundary != null);

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                var reader = new MultipartReader(boundary, stream);
                                var result = new List<ReportingResultWithFormat>();

                                while (true)
                                {
                                    var section = await reader.ReadNextSectionAsync();

                                    if (section == null)
                                    {
                                        break;
                                    }

                                    StringValues contentTypes;
                                    section.Headers.TryGetValue("Content-Type", out contentTypes);

                                    if (contentTypes.Count == 0)
                                    {
                                        continue;
                                    }

                                    var multipartContentType = MediaTypeHeaderValue.Parse(contentTypes[0]);

                                    string formatName = string.Empty;
                                    formatMap.TryGetValue(multipartContentType.MediaType, out formatName);

                                    result.Add(new ReportingResultWithFormat
                                    {
                                        Data = await ReadStreamToEnd(section.Body),
                                        Format = formatName
                                    });
                                }

                                return new ServiceResponse<IEnumerable<ReportingResultWithFormat>>(result, response);
                            }
                        }
                        else
                        {
                            var formatName = string.Empty;
                            formatMap.TryGetValue(response.Content.Headers.ContentType.MediaType, out formatName);

                            var data = new ReportingResultWithFormat
                            {
                                Data = await response.Content.ReadAsByteArrayAsync(),
                                Format = formatName
                            };

                            return new ServiceResponse<IEnumerable<ReportingResultWithFormat>>(new[] { data }, response);
                        }
                    }
                    else
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.BadRequest:
                                await ProcessBadRequestAsync(response);
                                break; // ProcessBadRequestAsync always throws but the compiler does not detect it
                            case HttpStatusCode.MovedPermanently:
                                throw new MovedPermanentlyException();
                            case HttpStatusCode.NotFound:
                                throw new NotFoundException();
                            case HttpStatusCode.Unauthorized:
                                throw new UnauthorizedEndpointException();
                            case HttpStatusCode.ProxyAuthenticationRequired:
                                throw new ProxyAuthenticationRequiredException(request.RequestUri);
                        }

                        throw new PortabilityAnalyzerException(string.Format(CultureInfo.CurrentCulture, LocalizedStrings.UnknownErrorCodeMessage, response.StatusCode));
                    }
                }
            }
            catch (HttpRequestException e)
            {
                throw new PortabilityAnalyzerException(LocalizedStrings.HttpExceptionMessage, e);
            }
        }
    }
}
