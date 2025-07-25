// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning
{
    [Serializable, NetSerializable]
    public enum AcceptCloningUiButton
    {
        Deny,
        Accept,
    }

    [Serializable, NetSerializable]
    public sealed class AcceptCloningChoiceMessage : EuiMessageBase
    {
        public readonly AcceptCloningUiButton Button;

        public AcceptCloningChoiceMessage(AcceptCloningUiButton button)
        {
            Button = button;
        }
    }
}
