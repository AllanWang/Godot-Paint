#!/usr/bin/env bash

cd "$(dirname "$0")" || exit;

protoc --csharp_out=. --csharp_opt=file_extension=.g.cs "PlayerData.proto"