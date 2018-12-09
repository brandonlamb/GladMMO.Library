﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace Guardians
{
	/// <summary>
	/// Client handler for payload: <see cref="VoiceDataChangeRaiseEventPayload"/>.
	/// </summary>
	public sealed class VoiceDataChangeRaiseEventHandler : BaseZoneClientGameMessageHandler<VoiceDataChangeRaiseEventPayload>
	{
		private IVoiceDataProcessor VoiceProcessingService { get; }

		private ILocalPlayerDetails PlayerDetails { get; }

		/// <inheritdoc />
		public VoiceDataChangeRaiseEventHandler(ILog logger, [NotNull] IVoiceDataProcessor voiceProcessingService, [NotNull] ILocalPlayerDetails playerDetails) 
			: base(logger)
		{
			VoiceProcessingService = voiceProcessingService ?? throw new ArgumentNullException(nameof(voiceProcessingService));
			PlayerDetails = playerDetails ?? throw new ArgumentNullException(nameof(playerDetails));
		}

		//TODO: This is a work in progress, we need a time service.
		/// <inheritdoc />
		public override Task HandleMessage(IPeerMessageContext<GameClientPacketPayload> context, VoiceDataChangeRaiseEventPayload payload)
		{
			//When we recieve voice data, we need to dispatch it to
			//the voice data listener.

			//TODO: We just shouldn't send this, not ignore it.
			if(PlayerDetails.LocalPlayerGuid == payload.EntityVoiceData.EntityGuid)
				return Task.CompletedTask;

			//The implementation of this assumes it's not going to block
			VoiceProcessingService.ProcessIncomingVoiceData(payload.EntityVoiceData.EntityGuid, 
				new ArraySegment<byte>(payload.EntityVoiceData.Data.VoiceSegmentData.ToArrayTryAvoidCopy()), 
				payload.EntityVoiceData.Data.SequenceNumber);

			return Task.CompletedTask;
		}
	}
}
