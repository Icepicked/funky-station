// SPDX-FileCopyrightText: 2022 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Disposal.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Disposal;

[Serializable, NetSerializable]
public sealed class MailingUnitBoundUserInterfaceState : BoundUserInterfaceState, IEquatable<MailingUnitBoundUserInterfaceState>
{
    public string? Target;
    public List<string> TargetList;
    public string? Tag;
    public SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState DisposalState;

    public MailingUnitBoundUserInterfaceState(SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState disposalState, string? target, List<string> targetList, string? tag)
    {
        DisposalState = disposalState;
        Target = target;
        TargetList = targetList;
        Tag = tag;
    }

    public bool Equals(MailingUnitBoundUserInterfaceState? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return DisposalState.Equals(other.DisposalState)
               && Target == other.Target
               && TargetList.Equals(other.TargetList)
               && Tag == other.Tag;
    }

    public override bool Equals(object? other)
    {
        if (other is MailingUnitBoundUserInterfaceState otherState)
            return Equals(otherState);
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
