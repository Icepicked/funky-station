// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;
using Robust.Shared.Prototypes;

namespace Content.Shared.Abilities.Goliath;

public sealed partial class GoliathSummonTentacleAction : EntityWorldTargetActionEvent
{
    /// <summary>
    /// The ID of the entity that is spawned.
    /// </summary>
    [DataField]
    public EntProtoId EntityId = "EffectGoliathTentacleSpawn";

    /// <summary>
    /// Directions determining where the entities will spawn.
    /// </summary>
    [DataField]
    public List<Direction> OffsetDirections = new()
    {
        Direction.North,
        Direction.South,
        Direction.East,
        Direction.West,
    };

    /// <summary>
    /// How many entities will spawn beyond the original one at the target location?
    /// </summary>
    [DataField]
    public int ExtraSpawns = 3;
};
