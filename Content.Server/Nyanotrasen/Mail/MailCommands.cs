// SPDX-FileCopyrightText: 2024 BombasterDS <115770678+BombasterDS@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Content.Shared.Administration;
using Content.Server.Administration;
using Content.Server.Mail.Components;

namespace Content.Server.Mail;

[AdminCommand(AdminFlags.Fun)]
public sealed class MailToCommand : IConsoleCommand
{
    public string Command => "mailto";
    public string Description => Loc.GetString("command-mailto-description", ("requiredComponent", nameof(MailReceiverComponent)));
    public string Help => Loc.GetString("command-mailto-help", ("command", Command));

    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;

    private readonly string _blankMailPrototype = "MailAdminFun";
    private readonly string _blankLargeMailPrototype = "MailLargeAdminFun"; // Frontier: large mail
    private readonly string _container = "storagebase";
    private readonly string _mailContainer = "contents";


    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 4)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
            return;
        }

        if (!EntityUid.TryParse(args[0], out var recipientUid))
        {
            shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
            return;
        }

        if (!EntityUid.TryParse(args[1], out var containerUid))
        {
            shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
            return;
        }

        if (!Boolean.TryParse(args[2], out var isFragile))
        {
            shell.WriteError(Loc.GetString("shell-invalid-bool"));
            return;
        }

        if (!Boolean.TryParse(args[3], out var isPriority))
        {
            shell.WriteError(Loc.GetString("shell-invalid-bool"));
            return;
        }

        // Frontier: Large Mail
        bool isLarge = false;
        if (args.Length > 4 && !Boolean.TryParse(args[4], out isLarge))
        {
            shell.WriteError(Loc.GetString("shell-invalid-bool"));
            return;
        }
        var mailPrototype = isLarge ? _blankLargeMailPrototype : _blankMailPrototype;
        // End Frontier


        var _mailSystem = _entitySystemManager.GetEntitySystem<MailSystem>();
        var _containerSystem = _entitySystemManager.GetEntitySystem<SharedContainerSystem>();

        if (!_entityManager.TryGetComponent(recipientUid, out MailReceiverComponent? mailReceiver))
        {
            shell.WriteLine(Loc.GetString("command-mailto-no-mailreceiver", ("requiredComponent", nameof(MailReceiverComponent))));
            return;
        }

        if (!_prototypeManager.HasIndex<EntityPrototype>(mailPrototype)) // Frontier: _blankMailPrototype<mailPrototype
        {
        shell.WriteLine(Loc.GetString("command-mailto-no-blankmail", ("blankMail", mailPrototype))); // Frontier: _blankMailPrototype<mailPrototype
            return;
        }

        if (!_containerSystem.TryGetContainer(containerUid, _container, out var targetContainer))
        {
            shell.WriteLine(Loc.GetString("command-mailto-invalid-container", ("requiredContainer", _container)));
            return;
        }

        if (!_mailSystem.TryGetMailRecipientForReceiver(mailReceiver, out MailRecipient? recipient))
        {
            shell.WriteLine(Loc.GetString("command-mailto-unable-to-receive"));
            return;
        }

        if (!_mailSystem.TryGetMailTeleporterForReceiver(mailReceiver, out MailTeleporterComponent? teleporterComponent))
        {
            shell.WriteLine(Loc.GetString("command-mailto-no-teleporter-found"));
            return;
        }

        var mailUid = _entityManager.SpawnEntity(mailPrototype, _entityManager.GetComponent<TransformComponent>(containerUid).Coordinates); // Frontier: _blankMailPrototype<mailPrototype
        var mailContents = _containerSystem.EnsureContainer<Container>(mailUid, _mailContainer);

        if (!_entityManager.TryGetComponent(mailUid, out MailComponent? mailComponent))
        {
            shell.WriteLine(Loc.GetString("command-mailto-bogus-mail", ("blankMail", mailPrototype), ("requiredMailComponent", nameof(MailComponent)))); // Frontier: _blankMailPrototype<mailPrototype
            return;
        }

        foreach (var entity in targetContainer.ContainedEntities.ToArray())
            _containerSystem.Insert(entity, mailContents);

        mailComponent.IsFragile = isFragile;
        mailComponent.IsPriority = isPriority;
        mailComponent.IsLarge = isLarge; //Frontier Mail

        _mailSystem.SetupMail(mailUid, teleporterComponent, recipient.Value);

        var teleporterQueue = _containerSystem.EnsureContainer<Container>(teleporterComponent.Owner, "queued");
        _containerSystem.Insert(mailUid, teleporterQueue);
        shell.WriteLine(Loc.GetString("command-mailto-success", ("timeToTeleport", teleporterComponent.TeleportInterval.TotalSeconds - teleporterComponent.Accumulator)));
    }
}

[AdminCommand(AdminFlags.Fun)]
public sealed class MailNowCommand : IConsoleCommand
{
    public string Command => "mailnow";
    public string Description => Loc.GetString("command-mailnow");
    public string Help => Loc.GetString("command-mailnow-help", ("command", Command));

    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;

    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var _mailSystem = _entitySystemManager.GetEntitySystem<MailSystem>();

        foreach (var mailTeleporter in _entityManager.EntityQuery<MailTeleporterComponent>())
        {
            mailTeleporter.Accumulator += (float) mailTeleporter.TeleportInterval.TotalSeconds - mailTeleporter.Accumulator;
        }

        shell.WriteLine(Loc.GetString("command-mailnow-success"));
    }
}
