// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Repo <47093363+Titian3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Managers;
using Content.Shared.CCVar;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.Afk
{
    /// <summary>
    /// Tracks AFK (away from keyboard) status for players.
    /// </summary>
    /// <seealso cref="CCVars.AfkTime"/>
    public interface IAfkManager
    {
        /// <summary>
        /// Check whether this player is currently AFK.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>True if the player is AFK, false otherwise.</returns>
        bool IsAfk(ICommonSession player);

        /// <summary>
        /// Resets AFK status for the player as if they just did an action and are definitely not AFK.
        /// </summary>
        /// <param name="player">The player to set AFK status for.</param>
        void PlayerDidAction(ICommonSession player);

        void Initialize();
    }

    [UsedImplicitly]
    public sealed class AfkManager : IAfkManager
    {
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly IGameTiming _gameTiming = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        [Dependency] private readonly IConsoleHost _consoleHost = default!;
        [Dependency] private readonly IAdminManager _adminManager = default!;

        private readonly Dictionary<ICommonSession, TimeSpan> _lastActionTimes = new();

        public void Initialize()
        {
            // Connecting, console commands and input commands all reset AFK status.

            _playerManager.PlayerStatusChanged += PlayerStatusChanged;
            _consoleHost.AnyCommandExecuted += ConsoleHostOnAnyCommandExecuted;
        }

        public void PlayerDidAction(ICommonSession player)
        {
            if (player.Status == SessionStatus.Disconnected)
                // Make sure we don't re-add to the dictionary if the player is disconnected now.
                return;

            _lastActionTimes[player] = _gameTiming.RealTime;
        }

        public bool IsAfk(ICommonSession player)
        {
            if (!_lastActionTimes.TryGetValue(player, out var time))
            {
                // Some weird edge case like disconnected clients. Just say true I guess.
                return true;
            }

            var timeOut = _adminManager.IsAdmin(player)
                ? TimeSpan.FromSeconds(_cfg.GetCVar(CCVars.AdminAfkTime))
                : TimeSpan.FromSeconds(_cfg.GetCVar(CCVars.AfkTime));

            return _gameTiming.RealTime - time > timeOut;
        }

        private void PlayerStatusChanged(object? sender, SessionStatusEventArgs e)
        {
            if (e.NewStatus == SessionStatus.Disconnected)
            {
                _lastActionTimes.Remove(e.Session);
                return;
            }

            PlayerDidAction(e.Session);
        }

        private void ConsoleHostOnAnyCommandExecuted(IConsoleShell shell, string commandname, string argstr, string[] args)
        {
            if (shell.Player is { } player)
                PlayerDidAction(player);
        }
    }
}
