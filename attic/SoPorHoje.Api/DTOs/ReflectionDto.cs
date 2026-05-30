namespace SoPorHoje.Api.DTOs;

public record ReflectionDto(
    string DateKey,
    string Title,
    string Quote,
    string Text,
    string Reference
);

public record PagedReflectionsResponse(
    List<ReflectionDto> Items,
    int Page,
    int PageSize,
    int TotalCount
);
