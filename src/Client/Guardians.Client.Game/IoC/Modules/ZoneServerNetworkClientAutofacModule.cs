﻿using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Logging;
using GladNet;
using ProtoBuf;

namespace Guardians
{
	public sealed class ZoneServerNetworkClientAutofacModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(context => LogLevel.All)
				.As<LogLevel>()
				.SingleInstance();

			builder.RegisterType<UnityLogger>()
				.As<ILog>()
				.SingleInstance();

			ProtobufNetGladNetSerializerAdapter serializer = new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32);
			Unity3DProtobufPayloadRegister payloadRegister = new Unity3DProtobufPayloadRegister();
			payloadRegister.RegisterDefaults();
			payloadRegister.Register(ZoneServerMetadataMarker.ClientPayloadTypesByOpcode, ZoneServerMetadataMarker.ServerPayloadTypesByOpcode);

			IManagedNetworkClient<GameClientPacketPayload, GameServerPacketPayload> client = new DotNetTcpClientNetworkClient()
				.AddHeaderlessNetworkMessageReading(serializer)
				.For<GameServerPacketPayload, GameClientPacketPayload, IPacketPayload>()
				.Build()
				.AsManaged(new UnityLogger(LogLevel.All)); //TODO: How should we handle log level?

			builder.RegisterInstance(client)
				.As<IManagedNetworkClient<GameClientPacketPayload, GameServerPacketPayload>>()
				.As<IPeerPayloadSendService<GameClientPacketPayload>>()
				.As<IPayloadInterceptable>()
				.As<IConnectionService>();

			builder.RegisterType<DefaultMessageContextFactory>()
				.As<IPeerMessageContextFactory>()
				.SingleInstance();

			builder.RegisterType<PayloadInterceptMessageSendService<GameClientPacketPayload>>()
				.As<IPeerRequestSendService<GameClientPacketPayload>>()
				.SingleInstance();
		}
	}
}
