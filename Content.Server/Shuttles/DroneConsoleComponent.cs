// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Shuttles.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.Shuttles;

// Primo shitcode
/// <summary>
/// Lets you remotely control a shuttle.
/// </summary>
[RegisterComponent]
public sealed partial class DroneConsoleComponent : Component
{
    [DataField("components", required: true)]
    public ComponentRegistry Components = default!;

    /// <summary>
    /// <see cref="ShuttleConsoleComponent"/> that we're proxied into.
    /// </summary>
    [DataField("entity")]
    public EntityUid? Entity;
}
