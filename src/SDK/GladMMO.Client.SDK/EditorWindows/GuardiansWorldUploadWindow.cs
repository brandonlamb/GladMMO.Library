﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using Microsoft.Azure.Storage.Blob;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GladMMO.SDK
{
	public sealed class GuardiansWorldUploadWindow : AuthenticatableEditorWindow
	{
		[SerializeField]
		private UnityEngine.Object SceneObject;

		[SerializeField]
		private string AssetBundlePath;

		[MenuItem("VRGuardians/Content/WorldUpload")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(GuardiansWorldUploadWindow));
		}

		protected override void AuthenticatedOnGUI()
		{
			//TODO: Validate scene file
			SceneObject = EditorGUILayout.ObjectField("Scene", SceneObject, typeof(SceneAsset), false);

			if(GUILayout.Button("Build World AssetBundle"))
			{

				//Once authenticated we need to try to build the bundle.
				ProjectVindictiveAssetbundleBuilder builder = new ProjectVindictiveAssetbundleBuilder(SceneObject);

				//TODO: Handle uploading build
				AssetBundleManifest manifest = builder.BuildBundle();

				//TODO: Refactor all this crap
				AssetBundlePath = manifest.GetAllAssetBundles().First();

				Debug.Log($"Generated AssetBundle with Path: {AssetBundlePath}");

				return;
			}

			if(GUILayout.Button("Upload Assetbundle"))
			{
				IDownloadableContentServerServiceClient ucmService = Refit.RestService.For<IDownloadableContentServerServiceClient>("http://72.190.177.214:5005/");

				//Done out here, must be called on the main thread
				string projectPath = Application.dataPath.ToLower().TrimEnd(@"assets".ToCharArray());

				UnityAsyncHelper.InitializeSyncContext();

				UnityAsyncHelper.UnityMainThreadContext.Post(async o =>
				{
					EditorUtility.DisplayProgressBar("Uploading Content", "Please wait until complete.", 0.5f);
					try
					{
						RequestedUrlResponseModel newWorldUpload = await ucmService.GetNewWorldUploadUrl(AuthenticationModelSingleton.Instance.AuthenticationToken)
							.ConfigureAwait(true);

						string uploadUrl = newWorldUpload.UploadUrl;
						Debug.Log($"Uploading to: {uploadUrl}.");

						var cloudBlockBlob = new CloudBlockBlob(new Uri(uploadUrl));
						await cloudBlockBlob.UploadFromFileAsync(Path.Combine(projectPath, "AssetBundles", "temp", AssetBundlePath))
							.ConfigureAwait(true);
					}
					catch (Exception e)
					{
						Debug.LogError($"Failed to upload. Reason: {e.Message}");
						throw;
					}
					finally
					{
						//Important to ALWAYS end the progress bar, even if failed.
						EditorUtility.ClearProgressBar();
					}

				}, null);
			}
		}

		protected override void UnAuthenticatedOnGUI()
		{
			//TODO: Redirect to login.
		}
	}
}