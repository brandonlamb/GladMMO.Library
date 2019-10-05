﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GladMMO
{
	public sealed class VivoxClaimsTokenFactory : IFactoryCreatable<VivoxTokenClaims, VivoxTokenClaimsCreationContext>
	{
		//TODO: Make Issuer configurable
		private static string VIVOX_ISSUER = "vrguardian-vrg-dev";

		//TODO: Make Issuer configurable
		private static string VIVOX_DOMAIN = "vdx5.vivox.com";

		public VivoxTokenClaims Create([JetBrains.Annotations.NotNull] VivoxTokenClaimsCreationContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			ConvertActionType(context.Action, out string actionType);

			switch (context.Action)
			{
				case VivoxAction.Login:
					return new VivoxTokenClaims(VIVOX_ISSUER, ComputeExpiryTime(), actionType, 1, $"sip:.{VIVOX_ISSUER}.{context.CharacterId}.@{VIVOX_DOMAIN}", null, null);
				default:
					throw new NotImplementedException($"TODO: Implement token generation for VivoxAction: {context.Action}");
			}
			
		}

		private static int ComputeExpiryTime()
		{
			//90 seconds is the example time found here: https://docs.vivox.com/v5/general/unity/5_1_0/Default.htm#AccessTokenDeveloperGuide/GeneratingTokensOnClientUnity.htm%3FTocPath%3DUnity%7CAccess%2520Token%2520Developer%2520Guide%7C_____6
			//This is basicallt from Vivox GetLoginToken. It's what they do with the provided TimeSpan.
			return (int)TimeSpan.FromSeconds(90).TotalSeconds;
		}

		private static void ConvertActionType(VivoxAction action, out string actionType)
		{
			switch (action)
			{
				case VivoxAction.Login:
					actionType = "login";
					break;
				default:
					throw new NotImplementedException($"TODO: Implement string generation for VivoxAction: {action}");
			}
		}
	}
}