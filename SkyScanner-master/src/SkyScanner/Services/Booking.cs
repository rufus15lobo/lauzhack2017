﻿// Copyright (c) 2015-2016 Tamas Vajk. All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using SkyScanner.Booking;
using SkyScanner.Services.Base;
using SkyScanner.Settings;
using System.Threading;

namespace SkyScanner.Services
{
    internal class Booking : RequesterWithPing<BookingResponseBase>
    {
        private readonly BookingQuerySettings _querySettings;

        public Booking(string apiKey, BookingQuerySettings querySettings)
            : base(apiKey, querySettings.BookingRequest)
        {
            _querySettings = querySettings;
        }

        protected override Func<HttpClient, CancellationToken, Task<HttpResponseMessage>> HttpMethod
        {
            get
            {
                return (client, token) =>
                    client.PutAsync(
                        $"http://partners.api.skyscanner.net/apiservices/pricing/v1.0/{_querySettings.BookingRequest.SessionKey}/booking", GetFormContent(), token);
            }
        }

        protected override ResponsePinger<BookingResponseBase> CreatePinger(Uri sessionUri)
        {
            return new BookingResponsePinger(ApiKey, sessionUri, _querySettings.BookingResponse);
        }
    }
}