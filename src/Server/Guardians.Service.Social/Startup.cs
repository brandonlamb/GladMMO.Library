﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace Guardians
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc(options =>
				{
					//This prevents ASP Core from trying to validate Vector3's children, which contain Vector3 (because Unity3D thanks)
					//so it will cause stack overflows. This will avoid it.
					//options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(Vector3)));
				})
				.RegisterHealthCheckController();

			X509Certificate2 cert = null;
			string certPath = "Certs/TestCert.pfx";

			try
			{
				cert = X509Certificate2Loader.Create(certPath).Load();
			}
			catch(Exception e)
			{
				throw new System.InvalidOperationException($"Failed to load {nameof(X509Certificate2)} from Path: {certPath} \n\n StackTrace: {e.StackTrace}", e);
			}

			//This provides JwtBearer support for Authorize attribute/header
			services.AddJwtAuthorization(cert);
			services.AddResponseCaching();

			services.AddSignalR(options => { }).AddJsonProtocol();

			services.AddSingleton<IUserIdProvider, SignalRPlayerCharacterUserIdProvider>();

			//Registers service discovery client.
			services.AddSingleton<IServiceDiscoveryService>(provider =>
			{
				return Refit.RestService.For<IServiceDiscoveryService>("http://sd.vrguardians.net:5000");
			});

			services.AddSingleton<IAuthenticationService, AsyncEndpointAuthenticationService>(provider =>
			{
				return new AsyncEndpointAuthenticationService(QueryForRemoteServiceEndpoint(provider.GetService<IServiceDiscoveryService>(), "Authentication"));
			});

			services.AddSingleton<ISocialServiceToGameServiceClient, AsyncEndpointISocialServiceToGameServiceClient>(provider =>
			{
				return new AsyncEndpointISocialServiceToGameServiceClient(QueryForRemoteServiceEndpoint(provider.GetService<IServiceDiscoveryService>(), "GameServer"),
					new RefitSettings() { AuthorizationHeaderValueGetter = () => GetSocialServiceAuthorizationToken(provider.GetService<IAuthenticationService>()) });
			});
		}

		private async Task<string> GetSocialServiceAuthorizationToken([JetBrains.Annotations.NotNull] IAuthenticationService authService)
		{
			if(authService == null) throw new ArgumentNullException(nameof(authService));

			//TODO: Don't hardcode the authentication details
			ProjectVersionStage.AssertBeta();
			//TODO: Handle errors
			return (await authService.TryAuthenticate(new AuthenticationRequestModel("SocialService", "Test69!"))).AccessToken;
		}

		private async Task<string> QueryForRemoteServiceEndpoint(IServiceDiscoveryService serviceDiscovery, string serviceType)
		{
			ResolveServiceEndpointResponse endpointResponse = await serviceDiscovery.DiscoverService(new ResolveServiceEndpointRequest(ClientRegionLocale.US, serviceType));

			if(!endpointResponse.isSuccessful)
				throw new InvalidOperationException($"Failed to query for Service: {serviceType} Result: {endpointResponse.ResultCode}");

			//TODO: Logging
			//Debug.Log($"Recieved service discovery response: {endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort} for Type: {serviceType}");

			//TODO: Do we need extra slash?
			return $"{endpointResponse.Endpoint.EndpointAddress}:{endpointResponse.Endpoint.EndpointPort}/";
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
#warning Do not deploy exceptions page into production
			app.UseDeveloperExceptionPage();

			app.UseResponseCaching();
			app.UseAuthentication();

			loggerFactory.RegisterGuardiansLogging(Configuration);
			loggerFactory.AddDebug();

			app.UseMvcWithDefaultRoute();

			app.UseSignalR(routes =>
			{
				routes.MapHub<TextChatHub>("/realtime/textchat");
			});
		}
	}
}
