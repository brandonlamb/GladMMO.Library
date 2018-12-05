﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace Guardians
{
	[JsonObject]
	public sealed class CharacterSessionDataResponse : IResponseModel<CharacterSessionDataResponseCode>, ISucceedable
	{
		/// <inheritdoc />
		[JsonProperty]
		public CharacterSessionDataResponseCode ResultCode { get; private set; }

		/// <inheritdoc />
		[JsonIgnore]
		public bool isSuccessful => ResultCode == CharacterSessionDataResponseCode.Success;

		/// <summary>
		/// The ID of the zone that the character has a session on.
		/// It does NOT mean that the player is in that zone, it's very possible
		/// they aren't online. Only claimed sessions mean they are in that zone.
		/// </summary>
		[JsonProperty]
		public int ZoneId { get; private set; }

		/// <inheritdoc />
		public CharacterSessionDataResponse(int zoneId)
		{
			if(zoneId <= 0) throw new ArgumentOutOfRangeException(nameof(zoneId));
			ZoneId = zoneId;
			ResultCode = CharacterSessionDataResponseCode.Success;
		}

		public CharacterSessionDataResponse(CharacterSessionDataResponseCode code)
		{
			if(!Enum.IsDefined(typeof(CharacterSessionDataResponseCode), code)) throw new InvalidEnumArgumentException(nameof(code), (int)code, typeof(CharacterSessionDataResponseCode));

			//TODO: Validate not success, should be failure.
			ResultCode = code;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		private CharacterSessionDataResponse()
		{
			
		}
	}
}
