// SPDX-FileCopyrightText: 2021 ike709 <ike709@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 Kot <1192090+koteq@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.IdentityManagement;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.Atmos.UI;

/// <summary>
/// Initializes a <see cref="GasPressurePumpWindow"/> and updates it when new server messages are received.
/// </summary>
[UsedImplicitly]
public sealed class GasPressurePumpBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private const float MaxPressure = Atmospherics.MaxOutputPressure;

    [ViewVariables]
    private GasPressurePumpWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<GasPressurePumpWindow>();

        _window.ToggleStatusButtonPressed += OnToggleStatusButtonPressed;
        _window.PumpOutputPressureChanged += OnPumpOutputPressurePressed;
        Update();
    }

    public override void Update()
    {
        if (_window == null)
            return;

        _window.Title = Identity.Name(Owner, EntMan);

        if (!EntMan.TryGetComponent(Owner, out GasPressurePumpComponent? pump))
            return;

        _window.SetPumpStatus(pump.Enabled);
        _window.MaxPressure = pump.MaxTargetPressure;
        _window.SetOutputPressure(pump.TargetPressure);
    }

    private void OnToggleStatusButtonPressed()
    {
        if (_window is null)
            return;

        SendPredictedMessage(new GasPressurePumpToggleStatusMessage(_window.PumpStatus));
    }

    private void OnPumpOutputPressurePressed(float value)
    {
        SendPredictedMessage(new GasPressurePumpChangeOutputPressureMessage(value));
    }
}
