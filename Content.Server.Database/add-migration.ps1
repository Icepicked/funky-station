# SPDX-FileCopyrightText: 2020 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
# SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
#
# SPDX-License-Identifier: MIT

#!/usr/bin/env pwsh

param([String]$name)

if ($name -eq "")
{
    Write-Error "must specify migration name"
    exit
}

dotnet ef migrations add --context SqliteServerDbContext -o Migrations/Sqlite $name
dotnet ef migrations add --context PostgresServerDbContext -o Migrations/Postgres $name
