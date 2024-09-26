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
            this HttpResponse response,
            int totalCount,
            int pageSize)
        {
            var paginationHeader = new
            {
                totalPages = (int)Math.Ceiling(totalCount/(double)pageSize)
            };

            response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationHeader));
            //response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));
            //Use IHeaderDictionary.Append or the indexer to append or set headers. IDictionary.
            //Add will throw an ArgumentException when attempting to add a duplicate key.ASP0019
        }
    }
}