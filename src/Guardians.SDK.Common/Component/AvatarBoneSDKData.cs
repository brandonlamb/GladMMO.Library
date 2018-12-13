﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Guardians.FinalIK;
using FinalIK;

namespace Guardians.SDK
{
	/// <summary>
	/// Component that is similar to VR-IK component
	/// that contains references to bones that can be automatically initialized.
	/// </summary>
	public sealed class AvatarBoneSDKData : MonoBehaviour, IAvatarIKReferenceContainer<CustomVRIKReferences>
	{
		/// <summary>
		/// Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character. Chest, neck, shoulder and toe bones are optional. VRIK also supports legless characters. If you do not wish to use legs, leave all leg references empty.
		/// </summary>
		[ContextMenuItem("Auto-detect References", "AutoDetectReferences")]
		[Tooltip("Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character. Chest, neck, shoulder and toe bones are optional. Also supports legless characters. If you do not wish to use legs, leave all leg references empty.")]
		public CustomVRIKReferences _references = new CustomVRIKReferences();

		/// <summary>
		/// Auto-detects bone references for this Avatar. Works with a Humanoid Animator on the gameobject only.
		/// </summary>
		[ContextMenu("Auto-detect References")]
		public void AutoDetectReferences()
		{
			CustomVRIKReferences.AutoDetectReferences(transform, out _references);
		}

		/// <inheritdoc />
		public CustomVRIKReferences references => _references;
	}
}