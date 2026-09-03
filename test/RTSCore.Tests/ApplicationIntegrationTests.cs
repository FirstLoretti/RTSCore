using MediatR;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RTSCore.Application.Cities.Commands;
using RTSCore.Application.Campaing.Commands;
using RTSCore.Application.Common.Behaviors;
using RTSCore.Domain.Entities;
using RTSCore.Domain.Exeptions;
using RTSCore.Domain.Interfaces;
using RTSCore.Domain.ValueObjects;
using RTSCore.Domain.ValueObjects.Presets;
using RTSCore.Infrastructure.Persistence;

using Unit = RTSCore.Domain.Entities.Unit;
using RTSCore.Domain.Services;
using RTSCore.Application.Cities.Queries;
using RTSCore.Application.Cities.Queries.Common;
using RTSCore.Application.Units.Commands;
using RTSCore.Application.Campaing.Commands.Diplomacy;
using RTSCore.Application.Campaing.Services.Diplomacy;

namespace RTSCore.Tests;

public class ApplicationIntegrationTests
{
    #region UnitCommands

    [Theory]
    [InlineData(UnitType.EnglandPeasant, 0, true)]
    [InlineData(UnitType.EnglandPeasant, 1, false)]
    [InlineData(UnitType.Invulnerable, 0, false)]
    public async Task Mediator_DisbandUnit_ShouldHandleRulesCorrectly(
        UnitType unitType,
        int turnsToRecruit,
        bool shouldSucceed
    )
    {
        var unitId = new UnitId("test_unit");

        var templates = new UnitTemplate[]
        {
            new (UnitType.EnglandPeasant, "Test Peasant", 1, 1, 1, 1, 1, 1, 1, 1, 1),
            new (UnitType.Invulnerable, "Test Invulnerable", 1, 1, 1, 1, 1, 1, 1, 1, 1)
        };

        var (dbName, serviceProvider) = SetupTestInvironment(service =>
            service.AddSingleton<IReadOnlyCollection<UnitTemplate>>(templates));

        using (var scope = serviceProvider.CreateScope())
        {
            var unit = Unit.CreateWithCustomStatus(
                 unitId, FactionType.England, templates.First(u => u.Type == unitType), turnsToRecruit
            );
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Units.Add(unit);
            await context.SaveChangesAsync();
        }

        var command = new DisbandUnitCommand(unitId);

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (shouldSucceed)
            {
                await mediator.Send(command);
            }
            else
            {
                await Assert.ThrowsAsync<GameRuleException>(async () => await mediator.Send(command));
            }
        }

        Unit? dbUnit;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbUnit = await context.Units.FirstOrDefaultAsync();
        }

        DeleteDatabase(dbName);

        if (shouldSucceed)
        {
            Assert.Null(dbUnit);
        }
        else
        {
            Assert.NotNull(dbUnit);
        }
    }

    #endregion

    #region CampaingCommands

    #region Main

    [Theory]
    [InlineData(new[] { FactionType.England }, PlayerType.Human, PlayerType.Ai)]
    [InlineData(new[] { FactionType.England, FactionType.France }, PlayerType.Human, PlayerType.Human)]
    public async Task Mediator_StartCampaingCommand_ShouldSaveCorrectEntitiesToDatabase(
        FactionType[] selectedFactions,
        PlayerType englandPlayerType,
        PlayerType francePlayerType
    )
    {
        var (dbName, serviceProvider) = SetupTestInvironment(services =>
        {
            var factionPresets = new FactionPreset[]
            {
                new(
                    Type: FactionType.England,
                    Gold: 5000,
                    Cities:
                    [
                        new CityPreset("test_london","Test London",CityType.Settlement,1000, [BuildingType.ReqruitBarrack])
                    ]
                ),
                new(
                    Type: FactionType.France,
                    Gold: 7500,
                    Cities:
                    [
                        new CityPreset("test_paris", "Test Paris", CityType.Village, 500, [BuildingType.Market])
                    ]
                )
            };

            services.AddSingleton(factionPresets);
        });

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new StartCampaignCommand(selectedFactions);
            await mediator.Send(command);
        }

        List<Faction> players;
        List<Building> buildings;
        List<City> cities;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            players = await context.Factions.ToListAsync();
            buildings = await context.Buildings.ToListAsync();
            cities = await context.Cities.ToListAsync();
        }

        DeleteDatabase(dbName);

        Assert.Equal(2, players.Count);
        Assert.Equal(englandPlayerType, players.First(p => p.Type == FactionType.England).PlayerType);
        Assert.Equal(francePlayerType, players.First(p => p.Type == FactionType.France).PlayerType);

        var london = cities.First(c => c.Id == "test_london");
        Assert.Equal(1000, london.Population);
        Assert.Contains(buildings, b => b.Type == BuildingType.ReqruitBarrack && b.CityId == london.Id);
    }

    [Fact]
    public async Task Mediator_EndTurn_ShouldAdvanceConstruction_CollectTaxes_IncreasePopulation_CollectBuildingIncome()
    {
        var (dbName, serviceProvider) = SetupTestInvironment();

        var barrackId = new BuildingId("test_barrack");
        var cityId = new CityId("test_london");
        var startingPopulation = 1000;
        var startingGold = 1000;
        var factionType = FactionType.England;

        using (var scope = serviceProvider.CreateScope())
        {
            var faction = new Faction(factionType, startingGold, PlayerType.Human);
            var cityPreset = new CityPreset(cityId, "Test_London", CityType.Settlement, startingPopulation, []);
            var city = new City(cityPreset, factionType);

            var barrack = Building.CreateWithCustomStatus(
                barrackId, BuildingType.ReqruitBarrack, factionType, city.Id,
                isConstructed: false,
                turnsToConstruct: 1
            );

            var field = Building.CreateWithCustomStatus(
                "test_field", BuildingType.CultivatedField, factionType, city.Id,
                isConstructed: true,
                turnsToConstruct: 0
            );

            var market = Building.CreateWithCustomStatus(
                "test_market", BuildingType.Market, factionType, cityId,
                isConstructed: true,
                turnsToConstruct: 0
            );

            city.RegisterBuilding(barrack);
            city.RegisterBuilding(field);
            city.RegisterBuilding(market);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Cities.Add(city);
            context.Factions.Add(faction);

            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new EndTurnCommand());
        }

        Building dbBarrack;
        City dbCity;
        Faction dbFaction;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbBarrack = await context.Buildings.FirstAsync(b => b.Id == barrackId);
            dbCity = await context.Cities.FirstAsync(c => c.Id == cityId);
            dbFaction = await context.Factions.FirstAsync(f => f.Type == factionType);
        }

        DeleteDatabase(dbName);

        // Assert: продвижение строительства
        Assert.True(dbBarrack.IsConstructed);
        Assert.Equal(0, dbBarrack.TurnsToConstruct);

        // Assert: увеличение населения
        var template = GameBalance.Buildings.GetTemplate(BuildingType.CultivatedField);
        var fieldGrowthBonus = template.Effects
            .Where(e => e.Type == BuildingEffectType.PopulationGrowth)
            .Sum(e => e.Value);

        var populationGrowth = (int)(startingPopulation * (GameBalance.Population.BaseGrowthRate + fieldGrowthBonus));
        var expectedPopulation = startingPopulation + populationGrowth;

        Assert.Equal(expectedPopulation, dbCity.Population);

        // Assert: начисление дохода от налогов и здания
        BuildingType[] testBuildingsType = [BuildingType.CultivatedField, BuildingType.ReqruitBarrack, BuildingType.Market];
        var expectedTaxIncome = startingPopulation * GameBalance.Economy.TaxRatePerCitizen;
        var expectedBuildingsIncome = (int)testBuildingsType
            .Select(t => GameBalance.Buildings.GetTemplate(t))
            .SelectMany(t => t.Effects)
            .Where(e => e.Type == BuildingEffectType.GoldIncome)
            .Sum(e => e.Value);
        var expectedGold = startingGold + expectedTaxIncome + expectedBuildingsIncome;

        Assert.Equal(expectedGold, dbFaction.Gold);
    }

    #endregion

    #region Diplomacy

    [Theory]
    [InlineData(false, false, false, true)]
    [InlineData(true, false, false, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    public async Task Mediator_SendTradeOffer_ShouldHandleRulesCorrectly(
        bool hasDuplicate,
        bool isRelationNull,
        bool isAlreadyTraded,
        bool shouldSucceed
    )
    {
        FactionType initiator = FactionType.England;
        FactionType target = FactionType.France;
        var (dbName, serviceProvider) = SetupTestInvironment();

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var engand = new Faction(FactionType.England, 0, PlayerType.Ai);
            var france = new Faction(FactionType.France, 0, PlayerType.Human);

            if (!isRelationNull)
            {
                var relation = new DiplomacyRelation(initiator, target, GameBalance.Diplomacy.MinStandingForTrade);

                if (isAlreadyTraded)
                {
                    relation.OpenTrade();
                }

                context.DiplomacyRelations.Add(relation);
            }

            if (hasDuplicate)
            {
                var dublicateOffer = new DiplomacyOffer(initiator, target, OfferType.TradeAgreement);
                context.DiplomacyOffers.Add(dublicateOffer);
            }

            await context.SaveChangesAsync();
        }

        var command = new SendTradeOfferCommand(initiator, target);

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (shouldSucceed)
            {
                var resultId = await mediator.Send(command);
                Assert.NotEqual(Guid.Empty, resultId);
            }
            else
            {
                await Assert.ThrowsAnyAsync<Exception>(async () => await mediator.Send(command));
            }
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var offers = await context.DiplomacyOffers.AsNoTracking().ToListAsync();

            if (shouldSucceed)
            {
                var createdOffer = Assert.Single(offers);
                Assert.Equal(OfferStatus.Pending, createdOffer.Status);
            }
        }

        DeleteDatabase(dbName);
    }

    [Fact]
    public async Task Mediator_AcceptOffer_ShouldSuccessfullyTranslateDomainChangesToDatabase()
    {
        var (dbName, serviceProvider) = SetupTestInvironment();

        var initiator = FactionType.England;
        var target = FactionType.France;
        Guid offerId;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var england = new Faction(initiator, 0, PlayerType.Human);
            var france = new Faction(target, 0, PlayerType.Ai);
            var relation = new DiplomacyRelation(initiator, target, GameBalance.Diplomacy.MinStandingForTrade);
            var tradeOffer = new DiplomacyOffer(initiator, target, OfferType.TradeAgreement);
            offerId = tradeOffer.Id;

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(tradeOffer);
            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new AcceptOfferCommand(offerId));
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbOffer = await context.DiplomacyOffers.FirstOrDefaultAsync();
            var dbRelation = await context.DiplomacyRelations.FirstOrDefaultAsync();

            var expectedStanding =
                GameBalance.Diplomacy.AcceptTradeOfferBonus + GameBalance.Diplomacy.MinStandingForTrade;

            Assert.NotNull(dbOffer);
            Assert.Equal(OfferStatus.Accepted, dbOffer.Status);
            Assert.NotNull(dbRelation);
            Assert.Equal(expectedStanding, dbRelation.Standing);
            Assert.True(dbRelation.HasTradeAgreement);
        }

        DeleteDatabase(dbName);
    }

    [Fact]
    public async Task Mediator_RejectOffer_ShouldSuccessfullyTranslateDomainChangesToDatabase()
    {
        var (dbName, serviceProvider) = SetupTestInvironment();

        var initiator = FactionType.England;
        var target = FactionType.France;
        Guid offerId;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var england = new Faction(initiator, 0, PlayerType.Ai);
            var france = new Faction(target, 0, PlayerType.Human);
            var relation = new DiplomacyRelation(initiator, target, GameBalance.Diplomacy.StartingStanding);
            var offer = new DiplomacyOffer(initiator, target, OfferType.TradeAgreement);
            offerId = offer.Id;

            context.Factions.Add(england);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(offer);
            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new RejectOfferCommand(offerId));
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var relation = await context.DiplomacyRelations.FirstOrDefaultAsync();
            var offer = await context.DiplomacyOffers.FirstOrDefaultAsync();
            var expectedStanding = GameBalance.Diplomacy.StartingStanding + GameBalance.Diplomacy.RejectOfferPenalty;

            Assert.NotNull(relation);
            Assert.False(relation.HasTradeAgreement);
            Assert.Equal(expectedStanding, relation.Standing);

            Assert.NotNull(offer);
            Assert.Equal(OfferStatus.Rejeсted, offer.Status);
        }

        DeleteDatabase(dbName);
    }

    #endregion

    #endregion

    #region CityCommands

    [Theory]
    [InlineData(CityType.Village, BuildingType.ReqruitBarrack, 1000, false, true)]
    [InlineData(CityType.Village, BuildingType.MilitiaBarrack, 1000, false, false)]
    [InlineData(CityType.Settlement, BuildingType.MilitiaBarrack, 1000, false, false)]
    [InlineData(CityType.Settlement, BuildingType.MilitiaBarrack, 1000, true, true)]
    public async Task Mediator_ConstructBuilding_ShouldValidateGold_CityType_TierChain(
        CityType cityType,
        BuildingType buildingToConstruct,
        int factionGold,
        bool setupPreviousTier,
        bool shouldSucceed
    )
    {
        var mockTemplates = CreateMockTemplates();

        var (dbName, serviceProvider) = SetupTestInvironment(services =>
            services.AddSingleton<IReadOnlyCollection<BuildingTemplate>>(mockTemplates));

        var targetTemplate = mockTemplates.First(t => t.Type == buildingToConstruct);

        var cityId = new CityId("test_london");
        var factionType = FactionType.England;

        await SeedTestWorldAsync(serviceProvider, cityId, cityType, factionType, factionGold, setupPreviousTier);

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new ConstructBuildingCommand(cityId, buildingToConstruct);

            if (shouldSucceed)
            {
                await mediator.Send(command);
            }
            else
            {
                await Assert.ThrowsAsync<GameRuleException>(async () => await mediator.Send(command));
            }
        }

        Faction dbFaction;
        List<Building> dbBuildings;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbFaction = await context.Factions.FirstAsync(p => p.Type == factionType);
            dbBuildings = await context.Buildings.Where(b => b.Type == buildingToConstruct).ToListAsync();
        }

        DeleteDatabase(dbName);

        if (shouldSucceed)
        {
            Assert.Equal(factionGold - targetTemplate.Cost, dbFaction.Gold);
            Assert.Single(dbBuildings);
            Assert.Equal($"building_{cityId}_{targetTemplate.Type.ToString().ToLower()}", dbBuildings.First().Id);
            Assert.False(dbBuildings.First().IsConstructed);
        }
        else
        {
            Assert.Equal(factionGold, dbFaction.Gold);
            Assert.Empty(dbBuildings);
        }
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Mediator_CancelTrain_ShouldHandleRulesCorrectly(bool isTrained, bool shouldSucceed)
    {
        var cityId = new CityId("test_london");
        var cityType = CityType.Village;
        var ownerFaction = FactionType.England;
        var initialGold = 1000;
        var unitId = new UnitId("test_unit");

        var unitTemplates = new UnitTemplate[]
        {
            new(
                UnitType.EnglandPeasant, "Test Peasant", 1000, 1, 1, 1, 1, 1, 1, 1,
                TurnsToRecruit: isTrained ? 0 : 1, RequiredBuilding: BuildingType.ReqruitBarrack
            )
        };

        var (dbName, serviceProvider) = SetupTestInvironment(service =>
            service.AddSingleton<IReadOnlyCollection<UnitTemplate>>(unitTemplates)
        );

        using (var scope = serviceProvider.CreateScope())
        {
            var faction = new Faction(ownerFaction, initialGold, PlayerType.Human);
            var cityPreset = new CityPreset(cityId, "Test London", cityType, 1, [BuildingType.ReqruitBarrack]);
            var city = new City(cityPreset, ownerFaction);
            var unit = Unit.CreateWithCustomStatus(
                unitId, ownerFaction, unitTemplates[0],
                turnsToRecruit: unitTemplates[0].TurnsToRecruit, currentCityId: cityId
            );

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Factions.Add(faction);
            context.Cities.Add(city);
            context.Units.Add(unit);

            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new CancelRecruitUnitCommand(unitId);

            if (shouldSucceed)
            {
                await mediator.Send(command);
            }
            else
            {
                await Assert.ThrowsAsync<GameRuleException>(async () => await mediator.Send(command));
            }
        }

        Unit? dbUnit;
        Faction? dbFaction;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbFaction = await context.Factions.FirstOrDefaultAsync();
            dbUnit = await context.Units.FirstOrDefaultAsync();
        }

        DeleteDatabase(dbName);

        Assert.NotNull(dbFaction);
        if (shouldSucceed)
        {
            Assert.Null(dbUnit);
            Assert.Equal(initialGold + unitTemplates[0].Cost, dbFaction.Gold);
        }
        else
        {
            Assert.NotNull(dbUnit);
            Assert.True(dbUnit.IsRecruited);
            Assert.Equal(initialGold, dbFaction.Gold);
        }
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Mediator_CancelConstruction_ShouldHandleRulesCorrectly(bool isConstructed, bool shouldSucceed)
    {
        var buildingId = new BuildingId("test_building");
        var factionType = FactionType.England;
        var factionGold = 1000;
        var cityId = "test_london";

        var mockTemplates = CreateMockTemplates();
        var buildingTemplate = mockTemplates.First();
        var building = Building.CreateWithCustomStatus(
            buildingId,
            buildingTemplate.Type,
            factionType,
            cityId,
            isConstructed,
            turnsToConstruct: isConstructed ? 0 : 1
        );

        var (dbName, serviceProvider) = SetupTestInvironment(services =>
            services.AddSingleton<IReadOnlyCollection<BuildingTemplate>>(mockTemplates));

        var dbGold = factionGold - buildingTemplate.Cost;

        await SeedTestWorldAsync(serviceProvider, cityId, CityType.Settlement, factionType, dbGold,
            setupPreviousTier: false,
            previousBuildingType: default,
            registerBuilding: building
        );

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = new CancelConstructBuildingCommand(buildingId);

            if (shouldSucceed)
            {
                await mediator.Send(command);
            }
            else
            {
                await Assert.ThrowsAsync<GameRuleException>(async () => await mediator.Send(command));
            }
        }

        Faction dbFaction;
        List<Building> dbBuildings;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbFaction = await context.Factions.FirstAsync(f => f.Type == factionType);
            dbBuildings = await context.Buildings.ToListAsync();
        }

        DeleteDatabase(dbName);

        if (shouldSucceed)
        {
            Assert.Equal(factionGold, dbFaction.Gold);
            Assert.Empty(dbBuildings);
        }
        else
        {
            Assert.Equal(dbGold, dbFaction.Gold);
            Assert.Single(dbBuildings);
            Assert.True(dbBuildings.First().IsConstructed);
        }
    }

    #endregion

    #region CityQueries

    [Theory]
    [InlineData(CityType.Village, BuildingType.ReqruitBarrack, 1000, false, CityCatalogOptionAvailability.Available, true)]
    [InlineData(CityType.Village, BuildingType.MilitiaBarrack, 1000, true, null, false)]
    [InlineData(CityType.Village, BuildingType.ReqruitBarrack, 0, false, CityCatalogOptionAvailability.Locked, true)]
    [InlineData(CityType.Settlement, BuildingType.MilitiaBarrack, 1000, false, null, false)]
    [InlineData(CityType.Settlement, BuildingType.MilitiaBarrack, 1000, true, CityCatalogOptionAvailability.Available, true)]
    public async Task Mediator_GetAvailableToConstructBuildings_ShouldReturnCorrectedCatalog(
        CityType cityType,
        BuildingType buildingTypeToConstruct,
        int factionGold,
        bool setupPreviousTier,
        CityCatalogOptionAvailability? expectedAvailability,
        bool shouldBeInCatalog
    )
    {
        var mockTemplates = CreateMockTemplates();
        var (dbName, serviceProvider) = SetupTestInvironment(services =>
            services.AddSingleton<IReadOnlyCollection<BuildingTemplate>>(mockTemplates)
        );

        var cityId = new CityId("Test London");
        var factionType = FactionType.England;

        await SeedTestWorldAsync(serviceProvider, cityId, cityType, factionType, factionGold, setupPreviousTier);

        IEnumerable<CityCatalogOptionDto<BuildingType>> catalog;

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            catalog = await mediator.Send(new GetCityConstructionOptionsQuery(cityId));
        }

        DeleteDatabase(dbName);

        var catalogBuilding = catalog.FirstOrDefault(b => b.Type == buildingTypeToConstruct);

        if (shouldBeInCatalog)
        {
            Assert.NotNull(catalogBuilding);
            Assert.Equal(expectedAvailability!.Value, catalogBuilding.Availability);

            if (expectedAvailability == CityCatalogOptionAvailability.Locked)
            {
                Assert.Equal("Недостаточно средств", catalogBuilding.LockReason);
            }
            else
            {
                Assert.Null(catalogBuilding.LockReason);
            }
        }
        else
        {
            Assert.Null(catalogBuilding);
        }
    }

    [Theory]
    [InlineData(false, 100, null, false)]
    [InlineData(true, 100, CityCatalogOptionAvailability.Available, true)]
    [InlineData(true, 100, CityCatalogOptionAvailability.Locked, true)]
    public async Task Mediator_GetCityRecruitOptions_ShouldReturnCorrectCatalog(
        bool isBarrackConstructed,
        int startingGold,
        CityCatalogOptionAvailability? expectedAvailability,
        bool shouldBeInCatalog
    )
    {
        var templateCost = expectedAvailability == CityCatalogOptionAvailability.Available
        ? startingGold
        : startingGold * 2;

        var cityId = new CityId("test_london");
        var factionType = FactionType.England;

        var unitTemplates = new UnitTemplate[]
        {
            new(
                UnitType.EnglandPeasant, "Test Unit", templateCost, 1,1,1,1,1,1,1,1,
                RequiredBuilding: BuildingType.ReqruitBarrack
            )
        };
        var barrack = Building.CreateWithCustomStatus(
            "test_reqruit_barrack", BuildingType.ReqruitBarrack, factionType, cityId, isBarrackConstructed,
            turnsToConstruct: isBarrackConstructed ? 0 : 1
        );

        var (dbName, serviceProvider) = SetupTestInvironment(services =>
            services.AddSingleton<IReadOnlyCollection<UnitTemplate>>(unitTemplates)
        );

        await SeedTestWorldAsync(
            serviceProvider, cityId, CityType.Village, factionType, startingGold, false, default, barrack
        );

        IEnumerable<CityCatalogOptionDto<UnitType>> catalog;

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            catalog = await mediator.Send(new GetCityRecruitOptionsQuery(cityId));
        }

        DeleteDatabase(dbName);

        var unitOption = catalog.FirstOrDefault(u => u.Type == UnitType.EnglandPeasant);

        if (shouldBeInCatalog)
        {
            Assert.NotNull(unitOption);
            Assert.Equal(expectedAvailability, unitOption.Availability);
        }
        else
        {
            Assert.Null(unitOption);
        }
    }

    [Theory]
    [InlineData(true, 1000, true)]
    [InlineData(false, 1000, false)]
    [InlineData(true, 0, false)]
    public async Task Mediator_CreateUnit_ChouldHandleRulesCorrectly(
        bool isBarrackConstructed, int initialGold, bool shouldSucceed)
    {
        var cityId = new CityId("test_london");
        var ownerFaction = FactionType.England;
        var cityType = CityType.Village;
        var buildingType = BuildingType.ReqruitBarrack;
        var unitType = UnitType.EnglandPeasant;

        var unitTemplates = new UnitTemplate[]
        {
            new(
                unitType, "Test Peasant", 1000, 1, 1, 1, 1, 1, 1, 1, 1,
                RequiredBuilding: BuildingType.ReqruitBarrack
            )
        };

        var buildingTemplates = new BuildingTemplate[]
        {
            new(
                buildingType, "Test Barrack", 1000, 1, [cityType]
            )
        };

        var (dbName, serviceProvider) = SetupTestInvironment(services =>
        {
            services.AddSingleton<IReadOnlyCollection<UnitTemplate>>(unitTemplates);
            services.AddSingleton<IReadOnlyCollection<BuildingTemplate>>(buildingTemplates);
        });

        using (var scope = serviceProvider.CreateScope())
        {
            var faction = new Faction(FactionType.England, initialGold, PlayerType.Human);
            var cityPreset = new CityPreset(cityId, "Test London", cityType, 1, []);
            var city = new City(cityPreset, ownerFaction);
            var barrack = Building.CreateWithCustomStatus(
                "test_barrack", buildingType, ownerFaction, cityId,
                isConstructed: isBarrackConstructed,
                turnsToConstruct: isBarrackConstructed ? 0 : 1
            );

            city.RegisterBuilding(barrack);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Factions.Add(faction);
            context.Cities.Add(city);

            await context.SaveChangesAsync();
        }

        var command = new RecruitUnitCommand(cityId, unitType, ownerFaction);

        using (var scope = serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (shouldSucceed)
            {
                await mediator.Send(command);
            }
            else
            {
                await Assert.ThrowsAsync<GameRuleException>(async () => await mediator.Send(command));
            }
        }

        Faction dbFaction;
        Unit? dbUnit;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbFaction = await context.Factions.FirstAsync(f => f.Type == ownerFaction);
            dbUnit = await context.Units.FirstOrDefaultAsync(u => u.Type == unitType);
        }

        DeleteDatabase(dbName);

        if (shouldSucceed)
        {
            Assert.NotNull(dbUnit);
            Assert.Equal(cityId, dbUnit.CurrentCityId);
            Assert.Equal(initialGold - unitTemplates.First().Cost, dbFaction.Gold);
        }
        else
        {
            Assert.Null(dbUnit);
            Assert.Equal(initialGold, dbFaction.Gold);
        }
    }

    #endregion

    #region Ai

    [Theory]
    [InlineData(true, DiplomacyRelation.MaxStanding, 1, 1, OfferStatus.Rejeсted)]
    [InlineData(false, GameBalance.Diplomacy.MinStandingForTrade - 1, 15, 1, OfferStatus.Rejeсted)]
    [InlineData(false, GameBalance.Diplomacy.MinStandingForTrade + 1, 15, 1, OfferStatus.Accepted)]
    public async Task AiDiplomacy_ShouldMakeCorrectDecisions_BasedOnUtilityMath(
        bool isAlreadyTraded,
        int initialStandings,
        int initiatorCityCount,
        int expectedOffersCount,
        OfferStatus expectedStatus
    )
    {
        var (dbName, serviceProvider) = SetupTestInvironment();

        var aiFaction = FactionType.England;
        var initiator = FactionType.France;
        Guid offerId;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var englandAi = new Faction(aiFaction, 0, PlayerType.Ai);
            var france = new Faction(initiator, 0, PlayerType.Human);
            var relation = new DiplomacyRelation(aiFaction, initiator, initialStandings);
            var offer = new DiplomacyOffer(initiator, aiFaction, OfferType.TradeAgreement);
            offerId = offer.Id;

            if (isAlreadyTraded)
            {
                relation.OpenTrade();
            }

            for (int i = 0; i < initiatorCityCount; i++)
            {
                context.Cities.Add(new City(new CityPreset($"test_city{i}", "TC", CityType.Village, 1, []), initiator));
            }
            context.Factions.Add(englandAi);
            context.Factions.Add(france);
            context.DiplomacyRelations.Add(relation);
            context.DiplomacyOffers.Add(offer);
            await context.SaveChangesAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var diplomacyAi = scope.ServiceProvider.GetRequiredService<DiplomacyAi>();
            await diplomacyAi.ProcessTurnAsync(aiFaction, CancellationToken.None);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dbOffers = await context.DiplomacyOffers.ToListAsync();
            var incomingOffer = dbOffers.First(o => o.Target == aiFaction);

            Assert.Equal(expectedStatus, incomingOffer.Status);
            Assert.Equal(expectedOffersCount, dbOffers.Count);
        }

        DeleteDatabase(dbName);
    }

    #endregion

    #region Common

    private static (string, ServiceProvider) SetupTestInvironment(Action<IServiceCollection>? configure = null)
    {
        string dbName = $"app_test_{Guid.NewGuid():N}.db";

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddDbContext<AppDbContext>(
            options => options.UseSqlite($"Data Source={dbName}")
        );
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IUnitRepository, SqlUnitRepository>();
        services.AddScoped<ICityRepository, SqlCityRepository>();
        services.AddScoped<IFactionRepository, SqlFactionRepository>();
        services.AddScoped<IBuildingRepository, SqlBuildingRepository>();
        services.AddScoped<DiplomacyAi>();
        services.AddSingleton(GameBalance.Buildings.GetAllTemplates);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            typeof(RecruitUnitCommand).Assembly
        ));

        configure?.Invoke(services);

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        return (dbName, serviceProvider);
    }

    private static BuildingTemplate[] CreateMockTemplates()
    {
        var mockTemplates = new BuildingTemplate[]
        {
            new(
                Type: BuildingType.ReqruitBarrack,
                DisplayName: "Test Reqruit Barrack",
                Cost: 500,
                TurnsToConstruct: 1,
                AllowedCityTypes:[CityType.Village, CityType.Settlement]
            ),
            new(
                Type: BuildingType.MilitiaBarrack,
                DisplayName: "Test Militia Barrack",
                Cost: 1000,
                TurnsToConstruct: 2,
                AllowedCityTypes:[CityType.Settlement],
                RequiredPreviousTier: BuildingType.ReqruitBarrack
            ),
        };

        return mockTemplates;
    }

    private static async Task SeedTestWorldAsync(
        IServiceProvider serviceProvider,
        CityId cityId,
        CityType cityType,
        FactionType factionType,
        int factionGold,
        bool setupPreviousTier,
        BuildingType previousBuildingType = BuildingType.ReqruitBarrack,
        Building? registerBuilding = null
    )
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var faction = new Faction(factionType, factionGold, PlayerType.Human);
        var cityPreset = new CityPreset(cityId, "Test London", cityType, 1500, []);
        var city = new City(cityPreset, factionType);

        if (setupPreviousTier)
        {
            var previousBuilding = Building.CreateWithCustomStatus(
                id: $"building_{cityId.Value}_{previousBuildingType.ToString().ToLower()}",
                type: previousBuildingType,
                ownerFaction: factionType,
                cityId: cityId,
                isConstructed: true,
                turnsToConstruct: 0
            );

            city.RegisterBuilding(previousBuilding);
        }
        if (registerBuilding != null)
        {
            city.RegisterBuilding(registerBuilding);
        }

        context.Factions.Add(faction);
        context.Cities.Add(city);

        await context.SaveChangesAsync();
    }

    private static void DeleteDatabase(string name)
    {
        SqliteConnection.ClearAllPools();
        File.Delete(name);
    }

    #endregion
}