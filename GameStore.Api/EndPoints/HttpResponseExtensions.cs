using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;

namespace GameStore.Api.EndPoints
{
    public static class HttpResponseExtensions
    {
        public static void AddPaginationHeaders(
            this HttpContext response,
            int totalCount,
            int pageSize)
        {
            var paginationHeader = new
            {
                totalPages = (int)Math.Ceiling(totalCount/(double)pageSize)
            };

            response.Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationHeader));
        }
    }
}