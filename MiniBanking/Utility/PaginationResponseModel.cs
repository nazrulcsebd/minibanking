using MiniBanking.Core.Models.DTOModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeBonds.Utility
{
    public class PaginationResponseModel<TEntity> where TEntity : class
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public IQueryable<TEntity> Lists { get; set; }
        public decimal TotalSum { get; set; }

        public static List<LinkInfo> GetLinks(PagedList<TEntity> list, HttpRequest request)
        {
            var links = new List<LinkInfo>();

            if (list.HasPreviousPage)
                links.Add(CreateLink("Get", list.PreviousPageNumber, list.PageSize, "previousPage", "GET", request));

            links.Add(CreateLink("Get", list.PageNumber, list.PageSize, "self", "GET", request));

            if (list.HasNextPage)
                links.Add(CreateLink("Get", list.NextPageNumber, list.PageSize, "nextPage", "GET", request));

            return links;
        }

        private static LinkInfo CreateLink(string routeName, int pageNumber, int pageSize, string rel, string method, HttpRequest request)
        {
            var href = string.Concat(request.Scheme, "://", request.Host, request.Path, "?pageNumber=", pageNumber, "&pageSize=", pageSize);

            return new LinkInfo
            {
                Href = href,
                Rel = rel,
                Method = method
            };
        }


    }
}
