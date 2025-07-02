using AutoMapper;

namespace Application.Resolver
{
    public class UtcDateTimeResolver : IMemberValueResolver<object, object, DateTime, DateTime>
    {
        public DateTime Resolve(object source, object destination, DateTime sourceMember, DateTime destMember, ResolutionContext context)
        {
            return sourceMember.Kind == DateTimeKind.Utc
                ? sourceMember
                : sourceMember.ToUniversalTime();
        }
    }

    public class NullableUtcDateTimeResolver : IMemberValueResolver<object, object, DateTime?, DateTime?>
    {
        public DateTime? Resolve(object source, object destination, DateTime? sourceMember, DateTime? destMember, ResolutionContext context)
        {
            if (!sourceMember.HasValue) return null;
            return sourceMember.Value.Kind == DateTimeKind.Utc
                ? sourceMember
                : sourceMember.Value.ToUniversalTime();
        }
    }
}
