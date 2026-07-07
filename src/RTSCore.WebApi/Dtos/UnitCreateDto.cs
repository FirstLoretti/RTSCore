using System.ComponentModel.DataAnnotations;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.WebApi.Dtos;

public record struct UnitCreateDto
{
    [Required(ErrorMessage = "Id обязателен для заполнения")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Id должен быть от 3 до 40 символов")]
    public string Id { get; set; }

    [EnumDataType(typeof(UnitType), ErrorMessage = "Неверный тип юнита")]
    public UnitType Type { get; set; }

    [EnumDataType(typeof(FactionType), ErrorMessage = "Неверный тип фракции")]
    public FactionType Faction { get; set; }
}