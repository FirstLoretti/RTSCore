namespace RTSCore.Domain.Interfaces;

public interface ICatalogOption<TemplateType> where TemplateType : Enum
{
    TemplateType Type { get; }
    string DisplayName { get; }
    int Cost { get; }
}