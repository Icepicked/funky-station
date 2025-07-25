// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameStates;

namespace Content.Shared.CartridgeLoader;

[RegisterComponent, NetworkedComponent]
public sealed partial class CartridgeLoaderComponent : Component
{
    public const string CartridgeSlotId = "Cartridge-Slot";

    [DataField]
    public ItemSlot CartridgeSlot = new();

    /// <summary>
    /// List of programs that come preinstalled with this cartridge loader
    /// </summary>
    [DataField("preinstalled")] // TODO remove this and use container fill.
    public List<string> PreinstalledPrograms = new();

    /// <summary>
    /// The currently running program that has its ui showing
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public EntityUid? ActiveProgram = default;

    /// <summary>
    /// The list of programs running in the background, listening to certain events
    /// </summary>
    [ViewVariables]
    public readonly List<EntityUid> BackgroundPrograms = new();

    /// <summary>
    /// The maximum amount of programs that can be installed on the cartridge loader entity
    /// </summary>
    [DataField]
    public int DiskSpace = 8;

    /// <summary>
    /// Controls whether the cartridge loader will play notifications if it supports it at all
    /// TODO: Add an option for this to the PDA
    /// </summary>
    [DataField]
    public bool NotificationsEnabled = true;

    [DataField(required: true)]
    public Enum UiKey = default!;
}
