// SPDX-FileCopyrightText: 2024 Morb <14136326+Morb0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Chat.Systems;
using Content.Shared.Chat;
using Robust.Shared.Prototypes;

namespace Content.Server.Speech;

public sealed partial class EmotesMenuSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeAllEvent<PlayEmoteMessage>(OnPlayEmote);
    }

    private void OnPlayEmote(PlayEmoteMessage msg, EntitySessionEventArgs args)
    {
        var player = args.SenderSession.AttachedEntity;
        if (!player.HasValue)
            return;

        if (!_prototypeManager.TryIndex(msg.ProtoId, out var proto) || proto.ChatTriggers.Count == 0)
            return;

        _chat.TryEmoteWithChat(player.Value, msg.ProtoId);
    }
}
