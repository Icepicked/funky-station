using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.EntityEffects;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Chemistry;

[UsedImplicitly]
public sealed class ReactiveSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;

    public void DoEntityReaction(EntityUid uid, Solution solution, ReactionMethod method, float scale = 1f) // Funky - allow reactives to specify scale
    {
        foreach (var reagent in solution.Contents.ToArray())
        {
            ReactionEntity(uid, method, reagent, solution, scale);
        }
    }

    public void ReactionEntity(EntityUid uid, ReactionMethod method, ReagentQuantity reagentQuantity, Solution? source, float scale = 1f) // Funky - allow reactives to specify scale
    {
        // We throw if the reagent specified doesn't exist.
        var proto = _prototypeManager.Index<ReagentPrototype>(reagentQuantity.Reagent.Prototype);
        ReactionEntity(uid, method, proto, reagentQuantity, source, scale); // Funky - allow reactives to specify scale
    }

    public void ReactionEntity(EntityUid uid, ReactionMethod method, ReagentPrototype proto,
        ReagentQuantity reagentQuantity, Solution? source, float scale = 1f) // Funky - allow reactives to specify scale
    {
        if (!TryComp(uid, out ReactiveComponent? reactive))
            return;

        // custom event for bypassing reactivecomponent stuff
        var ev = new ReactionEntityEvent(method, proto, reagentQuantity, source);
        RaiseLocalEvent(uid, ref ev);

        // If we have a source solution, use the reagent quantity we have left. Otherwise, use the reaction volume specified.
        var args = new EntityEffectReagentArgs(uid, EntityManager, null, source, source?.GetReagentQuantity(reagentQuantity.Reagent) ?? reagentQuantity.Quantity, proto, method, scale); // Funky - allow reactives to specify scale

        // First, check if the reagent wants to apply any effects.
        if (proto.ReactiveEffects != null && reactive.ReactiveGroups != null)
        {
            foreach (var (key, val) in proto.ReactiveEffects)
            {
                if (!val.Methods.Contains(method))
                    continue;

                if (!reactive.ReactiveGroups.ContainsKey(key))
                    continue;

                if (!reactive.ReactiveGroups[key].Contains(method))
                    continue;

                foreach (var effect in val.Effects)
                {
                    if (!effect.ShouldApply(args, _robustRandom))
                        continue;

                    if (effect.ShouldLog)
                    {
                        var entity = args.TargetEntity;
                        _adminLogger.Add(LogType.ReagentEffect, effect.LogImpact,
                            $"Reactive effect {effect.GetType().Name:effect} of reagent {proto.ID:reagent} with method {method} applied on entity {ToPrettyString(entity):entity} at {Transform(entity).Coordinates:coordinates}");
                    }

                    effect.Effect(args);
                }
            }
        }

        // Then, check if the prototype has any effects it can apply as well.
        if (reactive.Reactions != null)
        {
            foreach (var entry in reactive.Reactions)
            {
                if (!entry.Methods.Contains(method))
                    continue;

                if (entry.Reagents != null && !entry.Reagents.Contains(proto.ID))
                    continue;

                foreach (var effect in entry.Effects)
                {
                    if (!effect.ShouldApply(args, _robustRandom))
                        continue;

                    if (effect.ShouldLog)
                    {
                        var entity = args.TargetEntity;
                        _adminLogger.Add(LogType.ReagentEffect, effect.LogImpact,
                            $"Reactive effect {effect.GetType().Name:effect} of {ToPrettyString(entity):entity} using reagent {proto.ID:reagent} with method {method} at {Transform(entity).Coordinates:coordinates}");
                    }

                    effect.Effect(args);
                }
            }
        }
    }
}
public enum ReactionMethod
{
Touch,
Injection,
Ingestion,
}

[ByRefEvent]
public readonly record struct ReactionEntityEvent(
    ReactionMethod Method,
    ReagentPrototype Reagent,
    ReagentQuantity ReagentQuantity,
    Solution? Source
);
