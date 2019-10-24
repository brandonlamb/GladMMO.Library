﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace GladMMO
{
	//TODO: Automate user-agent SDK version headers
	[Headers("User-Agent: SDK 0.0.1")]
	public interface ISocialService
	{
		[RequiresAuthentication]
		[Get("/api/CharacterFriends")]
		Task<CharacterFriendListResponseModel> GetCharacterListAsync();
	}
}