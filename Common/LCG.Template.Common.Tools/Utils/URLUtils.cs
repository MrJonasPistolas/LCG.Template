using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace LCG.Template.Common.Tools.Utils
{
    public interface IUrlUtils
    {
        public Uri GenerateHostPathQueryStringUrl(Dictionary<string, string> values, string path = "");
    }

    public class URLUtils : IUrlUtils
    {
        private readonly IConfiguration _configuration;
        public URLUtils(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public Uri GenerateHostPathQueryStringUrl(Dictionary<string, string> values, string path = "")
        {
            var host = _configuration.GetValue<string>("CurrentHost");
            if (host == null)
                throw new InvalidOperationException("CurrentHost isn't defined in appsettings... Aborting...");

            var uri = new Uri(host);

            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Path += path;
            var uriWithPath = uriBuilder.Uri.ToString();

            return new Uri(QueryHelpers.AddQueryString(uriWithPath, values));
        }
    }
}
