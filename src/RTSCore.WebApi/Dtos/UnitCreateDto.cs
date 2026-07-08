using System.ComponentModel.DataAnnotations;

using RTSCore.Domain.ValueObjects;

namespace RTSCore.WebApi.Dtos;

public readonly record struct UnitCreateDto
(
    [Required(ErrorMessage = "Id обязателен для заполнения")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Id должен быть от 3 до 40 символов")]
    string Id,

    [EnumDataType(typeof(UnitType), ErrorMessage = "Неверный тип юнита")]
    UnitType Type,

    [EnumDataType(typeof(FactionType), ErrorMessage = "Неверный тип фракции")]
    FactionType Faction
);