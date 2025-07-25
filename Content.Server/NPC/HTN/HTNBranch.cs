// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.NPC.HTN.Preconditions;

namespace Content.Server.NPC.HTN;

/// <summary>
/// AKA Method. This is a branch available for a compound task.
/// </summary>
[DataDefinition]
public sealed partial class HTNBranch
{
    // Made this its own class if we ever need to change it.
    [DataField("preconditions")]
    public List<HTNPrecondition> Preconditions = new();

    /// <summary>
    /// Due to how serv3 works we need to defer getting the actual tasks until after they have all been serialized.
    /// </summary>
    [DataField("tasks", required: true)]
    public List<HTNTask> Tasks = new();
}
