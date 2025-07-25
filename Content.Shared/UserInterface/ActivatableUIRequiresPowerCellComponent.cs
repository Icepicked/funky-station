// SPDX-FileCopyrightText: 2023 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.PowerCell;
using Robust.Shared.GameStates;

namespace Content.Shared.UserInterface;

/// <summary>
/// Specifies that the attached entity requires <see cref="PowerCellDrawComponent"/> power.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActivatableUIRequiresPowerCellComponent : Component
{

}
