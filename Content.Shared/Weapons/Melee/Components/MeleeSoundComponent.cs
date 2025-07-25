// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Weapons.Melee.Components;

/// <summary>
/// Plays the specified sound upon receiving damage of the specified type.
/// </summary>
[RegisterComponent]
public sealed partial class MeleeSoundComponent : Component
{
    /// <summary>
    /// Specified sounds to apply when the entity takes damage with the specified group.
    /// Will fallback to defaults if none specified.
    /// </summary>
    [DataField("soundGroups",
        customTypeSerializer: typeof(PrototypeIdDictionarySerializer<SoundSpecifier, DamageGroupPrototype>))]
    public Dictionary<string, SoundSpecifier>? SoundGroups;

    /// <summary>
    /// Specified sounds to apply when the entity takes damage with the specified type.
    /// Will fallback to defaults if none specified.
    /// </summary>
    [DataField("soundTypes",
        customTypeSerializer: typeof(PrototypeIdDictionarySerializer<SoundSpecifier, DamageTypePrototype>))]
    public Dictionary<string, SoundSpecifier>? SoundTypes;

    /// <summary>
    /// Sound that plays if no damage is done.
    /// </summary>
    [DataField("noDamageSound")] public SoundSpecifier? NoDamageSound;
}
