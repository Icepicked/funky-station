// SPDX-FileCopyrightText: 2024 Skubman <ba.fallaria@gmail.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 Icepick <122653407+Icepicked@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 corresp0nd <46357632+corresp0nd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared._DV.CartridgeLoader.Cartridges;
using Content.Shared.Examine;
using Robust.Shared.Timing;

namespace Content.Shared._DV.NanoChat;

/// <summary>
///     Base system for NanoChat functionality shared between client and server.
/// </summary>
public abstract class SharedNanoChatSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<NanoChatCardComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<NanoChatCardComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        if (ent.Comp.Number == null)
        {
            args.PushMarkup(Loc.GetString("nanochat-card-examine-no-number"));
            return;
        }

        args.PushMarkup(Loc.GetString("nanochat-card-examine-number", ("number", $"{ent.Comp.Number:D4}")));
    }

    /// <summary>
    ///     Helper Method for truncating a string to maximum length
    /// </summary>
    public static string Truncate(string text, int maxLength, string overflowText = "...")
    {
        return text.Length > maxLength
            ? text[..(maxLength - overflowText.Length)] + overflowText
            : text;
    }

    #region Public API Methods

    /// <summary>
    ///     Gets the NanoChat number for a card.
    /// </summary>
    public uint? GetNumber(Entity<NanoChatCardComponent?> card)
    {
        if (!Resolve(card, ref card.Comp))
            return null;

        return card.Comp.Number;
    }

    /// <summary>
    ///     Sets the NanoChat number for a card.
    /// </summary>
    public void SetNumber(Entity<NanoChatCardComponent?> card, uint number)
    {
        if (!Resolve(card, ref card.Comp) || card.Comp.Number == number)
            return;

        card.Comp.Number = number;
        Dirty(card);
    }

    /// <summary>
    ///     Sets IsClosed for a card.
    /// </summary>
    public void SetClosed(Entity<NanoChatCardComponent?> card, bool closed)
    {
        if (!Resolve(card, ref card.Comp))
            return;

        card.Comp.IsClosed = closed;
    }

    /// <summary>
    ///     Gets the recipients dictionary from a card.
    /// </summary>
    public IReadOnlyDictionary<uint, NanoChatRecipient> GetRecipients(Entity<NanoChatCardComponent?> card)
    {
        if (!Resolve(card, ref card.Comp))
            return new Dictionary<uint, NanoChatRecipient>();

        return card.Comp.Recipients;
    }

    /// <summary>
    ///     Gets the messages dictionary from a card.
    /// </summary>
    public IReadOnlyDictionary<uint, List<NanoChatMessage>> GetMessages(Entity<NanoChatCardComponent?> card)
    {
        if (!Resolve(card, ref card.Comp))
            return new Dictionary<uint, List<NanoChatMessage>>();

        return card.Comp.Messages;
    }

    /// <summary>
    ///     Sets a specific recipient in the card.
    /// </summary>
    public void SetRecipient(Entity<NanoChatCardComponent?> card, uint number, NanoChatRecipient recipient)
    {
        if (!Resolve(card, ref card.Comp))
            return;

        card.Comp.Recipients[number] = recipient;
        Dirty(card);
    }

    /// <summary>
    ///     Gets a specific recipient from the card.
    /// </summary>
    public NanoChatRecipient? GetRecipient(Entity<NanoChatCardComponent?> card, uint number)
    {
        if (!Resolve(card, ref card.Comp) || !card.Comp.Recipients.TryGetValue(number, out var recipient))
            return null;

        return recipient;
    }

    /// <summary>
    ///     Gets all messages for a specific recipient.
    /// </summary>
    public List<NanoChatMessage>? GetMessagesForRecipient(Entity<NanoChatCardComponent?> card, uint recipientNumber)
    {
        if (!Resolve(card, ref card.Comp) || !card.Comp.Messages.TryGetValue(recipientNumber, out var messages))
            return null;

        return new List<NanoChatMessage>(messages);
    }

    /// <summary>
    ///     Adds a message to a recipient's conversation.
    /// </summary>
    public void AddMessage(Entity<NanoChatCardComponent?> card, uint recipientNumber, NanoChatMessage message)
    {
        if (!Resolve(card, ref card.Comp))
            return;

        if (!card.Comp.Messages.TryGetValue(recipientNumber, out var messages))
        {
            messages = new List<NanoChatMessage>();
            card.Comp.Messages[recipientNumber] = messages;
        }

        messages.Add(message);
        card.Comp.LastMessageTime = _timing.CurTime;
        Dirty(card);
    }

    /// <summary>
    ///     Gets the currently selected chat recipient.
    /// </summary>
    public uint? GetCurrentChat(Entity<NanoChatCardComponent?> card)
    {
        if (!Resolve(card, ref card.Comp))
            return null;

        return card.Comp.CurrentChat;
    }

    /// <summary>
    ///     Sets the currently selected chat recipient.
    /// </summary>
    public void SetCurrentChat(Entity<NanoChatCardComponent?> card, uint? recipient)
    {
        if (!Resolve(card, ref card.Comp))
            return;

        card.Comp.CurrentChat = recipient;
        Dirty(card);
    }

    /// <summary>
    ///     Gets whether notifications are muted.
    /// </summary>
    public bool GetNotificationsMuted(Entity<NanoChatCardComponent?> card)
    {
        if (!Resolve(card, ref card.Comp))
            return false;

        return card.Comp.NotificationsMuted;
    }

    /// <summary>
    ///     Sets whether notifications are muted.
    /// </summary>
    public void SetNotificationsMuted(Entity<NanoChatCardComponent?> card, bool muted)
    {
        if (!Resolve(card, ref card.Comp) || card.Comp.NotificationsMuted == muted)
            return;

        card.Comp.NotificationsMuted = muted;
        Dirty(card);
    }

    /// <summary>
    ///     Sets whether notifications are muted for a specific chat.
    /// </summary>
    public void ToggleChatMuted(Entity<NanoChatCardComponent?> card, uint chat)
    {
        if (!Resolve(card, ref card.Comp))
            return;

        if (!card.Comp.MutedChats.Remove(chat))
            card.Comp.MutedChats.Add(chat);
        Dirty(card);
    }

    /// <summary>
    ///     Gets whether NanoChat number is listed.
    /// </summary>
    public bool GetListNumber(Entity<NanoChatCardComponent?> card)
    {
        if (!Resolve(card, ref card.Comp))
            return false;

        return card.Comp.ListNumber;
    }

    /// <summary>
    ///     Sets whether NanoChat number is listed.
    /// </summary>
    public void SetListNumber(Entity<NanoChatCardComponent?> card, bool listNumber)
    {
        if (!Resolve(card, ref card.Comp) || card.Comp.ListNumber == listNumber)
            return;

        card.Comp.ListNumber = listNumber;
        Dirty(card);
    }

    /// <summary>
    ///     Gets the time of the last message.
    /// </summary>
    public TimeSpan? GetLastMessageTime(Entity<NanoChatCardComponent?> card)
    {
        if (!Resolve(card, ref card.Comp))
            return null;

        return card.Comp.LastMessageTime;
    }

    /// <summary>
    ///     Gets if there are unread messages from a recipient.
    /// </summary>
    public bool HasUnreadMessages(Entity<NanoChatCardComponent?> card, uint recipientNumber)
    {
        if (!Resolve(card, ref card.Comp) || !card.Comp.Recipients.TryGetValue(recipientNumber, out var recipient))
            return false;

        return recipient.HasUnread;
    }

    /// <summary>
    ///     Clears all messages and recipients from the card.
    /// </summary>
    public void Clear(Entity<NanoChatCardComponent?> card)
    {
        if (!Resolve(card, ref card.Comp))
            return;

        card.Comp.Messages.Clear();
        card.Comp.Recipients.Clear();
        card.Comp.CurrentChat = null;
        Dirty(card);
    }

    /// <summary>
    ///     Deletes a chat conversation with a recipient from the card.
    ///     Optionally keeps message history while removing from active chats.
    /// </summary>
    /// <returns>True if the chat was deleted successfully</returns>
    public bool TryDeleteChat(Entity<NanoChatCardComponent?> card, uint recipientNumber, bool keepMessages = false)
    {
        if (!Resolve(card, ref card.Comp))
            return false;

        // Remove from recipients list
        var removed = card.Comp.Recipients.Remove(recipientNumber);

        // Clear messages if requested
        if (!keepMessages)
            card.Comp.Messages.Remove(recipientNumber);

        // Clear current chat if we just deleted it
        if (card.Comp.CurrentChat == recipientNumber)
            card.Comp.CurrentChat = null;

        if (removed)
            Dirty(card);

        return removed;
    }

    /// <summary>
    ///     Ensures a recipient exists in the card's contacts and message lists.
    ///     If the recipient doesn't exist, they will be added with the provided info.
    /// </summary>
    /// <returns>True if the recipient was added or already existed</returns>
    public bool EnsureRecipientExists(Entity<NanoChatCardComponent?> card,
        uint recipientNumber,
        NanoChatRecipient? recipientInfo = null)
    {
        if (!Resolve(card, ref card.Comp))
            return false;

        if (!card.Comp.Recipients.ContainsKey(recipientNumber))
        {
            // Only add if we have recipient info
            if (recipientInfo == null)
                return false;

            card.Comp.Recipients[recipientNumber] = recipientInfo.Value;
        }

        // Ensure message list exists for this recipient
        if (!card.Comp.Messages.ContainsKey(recipientNumber))
            card.Comp.Messages[recipientNumber] = new List<NanoChatMessage>();

        Dirty(card);
        return true;
    }

    #endregion
}
