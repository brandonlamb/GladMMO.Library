﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladMMO;
using UnityEngine.Playables;

namespace GladMMO
{
	public sealed class UnityPlayableDirectorUIPlayableAdapter : BaseUnityUIAdapter<PlayableDirector, IUIPlayable>, IUIPlayable
	{
		/// <inheritdoc />
		public bool isPlaying => this.UnityUIObject.state == PlayState.Playing;

		/// <inheritdoc />
		public void Play()
		{
			this.UnityUIObject.Play();
		}

		/// <inheritdoc />
		public void Stop()
		{
			this.UnityUIObject.Stop();
		}
	}
}
