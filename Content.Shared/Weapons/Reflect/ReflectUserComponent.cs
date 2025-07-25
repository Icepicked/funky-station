// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Reflect;

/// <summary>
/// Added to an entity if it equips a reflection item in a hand slot or into its clothing.
/// Reflection events will then be relayed.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ReflectUserComponent : Component;
