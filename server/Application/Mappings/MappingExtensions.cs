using AutoMapper;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{ 
    public static class MappingExtensions
    {
        public static PagedList<TDestination> MapTo<TSource, TDestination>(this PagedList<TSource> source, IMapper mapper)
        {
            // Convert the IReadOnlyList to a List that we can map
            var items = mapper.Map<List<TDestination>>(source.ToList());

            return new PagedList<TDestination>(
                items,
                source.TotalCount,
                source.CurrentPage,
                source.PageSize
            );
        }

    }
}
