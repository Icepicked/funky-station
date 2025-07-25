// SPDX-FileCopyrightText: 2024 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects;

[ImplicitDataDefinitionForInheritors]
[MeansImplicitUse]
public abstract partial class EntityEffectCondition
{
    [JsonPropertyName("id")] private protected string _id => this.GetType().Name;

    public abstract bool Condition(EntityEffectBaseArgs args);

    /// <summary>
    /// Effect explanations are of the form "[chance to] [action] when [condition] and [condition]"
    /// </summary>
    /// <param name="prototype"></param>
    /// <returns></returns>
    public abstract string GuidebookExplanation(IPrototypeManager prototype);
}

