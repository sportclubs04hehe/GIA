using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Resolver
{
    public class DateTimeToUtcConverter : IValueConverter<DateTime, DateTime>
    {
        public DateTime Convert(DateTime sourceMember, ResolutionContext context)
        {
            return sourceMember.Kind == DateTimeKind.Utc
                ? sourceMember
                : sourceMember.ToUniversalTime();
        }
    }
}
