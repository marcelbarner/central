using Central.Domain.Correspondents;
using Central.Server.Features.Correspondents;

using Riok.Mapperly.Abstractions;

namespace Central.Server.Mappers;

[Mapper]
public static partial class CorrespondentDtoMapper
{
    public static partial CorrespondentDto ToDto(this Correspondent correspondent);
}