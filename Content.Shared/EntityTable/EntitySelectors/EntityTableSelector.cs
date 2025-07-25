// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable.ValueSelector;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.EntityTable.EntitySelectors;

[ImplicitDataDefinitionForInheritors, UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract partial class EntityTableSelector
{
    /// <summary>
    /// The number of times this selector is run
    /// </summary>
    [DataField]
    public NumberSelector Rolls = new ConstantNumberSelector(1);

    /// <summary>
    /// A weight used to pick between selectors.
    /// </summary>
    [DataField]
    public float Weight = 1;

    /// <summary>
    /// A simple chance that the selector will run.
    /// </summary>
    [DataField]
    public double Prob = 1;

    public IEnumerable<EntProtoId> GetSpawns(System.Random rand,
        IEntityManager entMan,
        IPrototypeManager proto)
    {
        var rolls = Rolls.Get(rand);
        for (var i = 0; i < rolls; i++)
        {
            if (!rand.Prob(Prob))
                continue;

            foreach (var spawn in GetSpawnsImplementation(rand, entMan, proto))
            {
                yield return spawn;
            }
        }
    }

    protected abstract IEnumerable<EntProtoId> GetSpawnsImplementation(System.Random rand,
        IEntityManager entMan,
        IPrototypeManager proto);
}
