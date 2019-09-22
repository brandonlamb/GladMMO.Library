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
using Refit;
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

		[MenuItem("VRGuardians/Content/WorldUpload")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(GuardiansWorldUploadWindow));
		}

		protected override void AuthenticatedOnGUI()
		{
			SceneObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);

			//TODO: Validate scene file
			EditorGUILayout.ObjectField("Scene", SceneObject, typeof(SceneAsset), false);

			//If there is a world definition in the scene we should utilize it.
			//TODO: We should do more than just render it.
			WorldDefinitionData definitionData = FindObjectOfType<WorldDefinitionData>();

			if (definitionData != null)
			{
				EditorGUILayout.LabelField($"World Id: {definitionData.WorldId}");
				EditorGUILayout.LabelField($"World GUID: {definitionData.WorldGuid}");
			}
			else
			{
				//If there is no world data definition then we should allow users
				//to upload a new world.
				if(GUILayout.Button("Upload World"))
				{
					string assetBundlePath = GenerateWorldBundle();

					UploadWorldAssetBundle(assetBundlePath);

					return;
				}
			}
		}

		private static void UploadWorldAssetBundle(string assetBundlePath)
		{
			IDownloadableContentServerServiceClient ucmService = 
				new DownloadableContentServiceClientFactory()
					.Create(EmptyFactoryContext.Instance);

			//Done out here, must be called on the main thread
			string projectPath = Application.dataPath.ToLower().TrimEnd(@"assets".ToCharArray());

			UnityAsyncHelper.InitializeSyncContext();

			UnityAsyncHelper.UnityMainThreadContext.Post(async o =>
			{
				EditorUtility.DisplayProgressBar("Uploading Content", "Please wait until complete.", 0.5f);
				try
				{
					ResponseModel<ContentUploadToken, ContentUploadResponseCode> contentUploadToken = await ucmService.GetNewWorldUploadUrl()
						.ConfigureAwait(true);

					//TODO: Handle failure.

					string uploadUrl = contentUploadToken.Result.UploadUrl;
					Debug.Log($"Uploading to: {uploadUrl}.");

					var cloudBlockBlob = new CloudBlockBlob(new Uri(uploadUrl));
					await cloudBlockBlob.UploadFromFileAsync(Path.Combine(projectPath, "AssetBundles", "temp", assetBundlePath))
						.ConfigureAwait(true);

					Debug.Log("Successfully uploaded.");

					//If it's sucessfully uploaded we should create a world definition
					//object in the scene.
					WorldDefinitionData definitionData = new GameObject("World Definition").AddComponent<WorldDefinitionData>();

					definitionData.WorldGuid = contentUploadToken.Result.ContentGuid.ToString();
					definitionData.WorldId = contentUploadToken.Result.ContentId;
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

		private string GenerateWorldBundle()
		{
			//Once authenticated we need to try to build the bundle.
			ProjectVindictiveAssetbundleBuilder builder = new ProjectVindictiveAssetbundleBuilder(SceneObject);

			//TODO: Handle uploading build
			AssetBundleManifest manifest = builder.BuildBundle();
			string bundlePath = manifest.GetAllAssetBundles().First();

			Debug.Log($"Generated AssetBundle with Path: {bundlePath}");

			//TODO: Refactor all this crap
			return bundlePath;
		}

		protected override void UnAuthenticatedOnGUI()
		{
			//TODO: Redirect to login.
		}
	}
}