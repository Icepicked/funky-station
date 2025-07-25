// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.UserInterface.Controls;
using Content.Shared.Research;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Research.UI;

[GenerateTypedNameReferences]
public sealed partial class DiskConsoleMenu : FancyWindow
{
    public event Action? OnServerButtonPressed;
    public event Action? OnPrintButtonPressed;

    public DiskConsoleMenu()
    {
        RobustXamlLoader.Load(this);

        ServerButton.OnPressed += _ => OnServerButtonPressed?.Invoke();
        PrintButton.OnPressed += _ => OnPrintButtonPressed?.Invoke();
    }

    public void Update(DiskConsoleBoundUserInterfaceState state)
    {
        PrintButton.Disabled = !state.CanPrint;
        TotalLabel.Text = Loc.GetString("tech-disk-ui-total-label", ("amount", state.ServerPoints));
        CostLabel.Text = Loc.GetString("tech-disk-ui-cost-label", ("amount", state.PointCost));
    }
}

