// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 PJBot <pieterjan.briers+bot@gmail.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 QueerCats <jansencheng3@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Systems;
using Content.Server.Station.Systems;
using Content.Shared.Preferences;

namespace Content.IntegrationTests.Tests.Internals;

[TestFixture]
[TestOf(typeof(InternalsSystem))]
public sealed class AutoInternalsTests
{
    [Test]
    public async Task TestInternalsAutoActivateInSpaceForStationSpawn()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;

        var testMap = await pair.CreateTestMap();

        var stationSpawning = server.System<StationSpawningSystem>();
        var atmos = server.System<AtmosphereSystem>();
        var internals = server.System<InternalsSystem>();

        await server.WaitAssertion(() =>
        {
            var profile = new HumanoidCharacterProfile();
            var dummy = stationSpawning.SpawnPlayerMob(testMap.GridCoords, "TestInternalsDummy", profile, station: null);

            Assert.That(atmos.HasAtmosphere(testMap.Grid), Is.False, "Test map has atmosphere - test needs adjustment!");
            Assert.That(internals.AreInternalsWorking(dummy), "Internals did not automatically connect!");

            server.EntMan.DeleteEntity(dummy);
        });

        await pair.CleanReturnAsync();
    }

    [Test]
    public async Task TestInternalsAutoActivateInSpaceForEntitySpawn()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;

        var testMap = await pair.CreateTestMap();

        var atmos = server.System<AtmosphereSystem>();
        var internals = server.System<InternalsSystem>();

        await server.WaitAssertion(() =>
        {
            var dummy = server.EntMan.Spawn("TestInternalsDummyEntity", testMap.MapCoords);

            Assert.That(atmos.HasAtmosphere(testMap.Grid), Is.False, "Test map has atmosphere - test needs adjustment!");
            Assert.That(internals.AreInternalsWorking(dummy), "Internals did not automatically connect!");

            server.EntMan.DeleteEntity(dummy);
        });

        await pair.CleanReturnAsync();
    }

    [TestPrototypes]
    private const string Prototypes = @"
- type: playTimeTracker
  id: PlayTimeInternalsDummy

- type: startingGear
  id: InternalsDummyGear
  equipment:
    mask: ClothingMaskBreath
    tankstorage: OxygenTankFilled

- type: job
  id: TestInternalsDummy
  playTimeTracker: PlayTimeInternalsDummy
  startingGear: InternalsDummyGear

- type: entity
  id: TestInternalsDummyEntity
  parent: MobHuman
  components:
    - type: Loadout
      prototypes: [InternalsDummyGear]
";
}
