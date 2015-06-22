namespace MedSet.RESTAPI
{
	using Nancy;
	using Nancy.Bootstrapper;
	using Nancy.Diagnostics;
	using Nancy.SimpleAuthentication;
	using Nancy.TinyIoc;
	using SimpleAuthentication.Core;
	using SimpleAuthentication.Core.Providers;

	public class Bootstrapper : DefaultNancyBootstrapper
	{
		//private const string TwitterConsumerKey = "*key*";
		//private const string TwitterConsumerSecret = "*secret*";
		private const string FacebookAppId = "404935849710103";
		private const string FacebookAppSecret = "61ed4ec44e87daf11ac421895d14e4a0";
		private const string GoogleConsumerKey = "870300371040-242h9v895pnjmq7ps97tacss0nn4ple3.apps.googleusercontent.com";
		private const string GoogleConsumerSecret = "jo8kn0v0kavKEsMmj3zDNZGH";

		// The bootstrapper enables you to reconfigure the composition of the framework,
		// by overriding the various methods and properties.
		// For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper
		protected override DiagnosticsConfiguration DiagnosticsConfiguration
		{
			get { return new DiagnosticsConfiguration { Password = @"medset.restapi" }; }
		}

		protected override void ConfigureApplicationContainer(TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer(container);

			//var twitterProvider = new TwitterProvider(new ProviderParams { PublicApiKey = TwitterConsumerKey, SecretApiKey = TwitterConsumerSecret });
			var facebookProvider = new FacebookProvider(new ProviderParams { PublicApiKey = FacebookAppId, SecretApiKey = FacebookAppSecret });
			var googleProvider = new GoogleProvider(new ProviderParams { PublicApiKey = GoogleConsumerKey, SecretApiKey = GoogleConsumerSecret });

			var authenticationProviderFactory = new AuthenticationProviderFactory();

			//authenticationProviderFactory.AddProvider(twitterProvider);
			authenticationProviderFactory.AddProvider(facebookProvider);
			authenticationProviderFactory.AddProvider(googleProvider);

			container.Register<IAuthenticationCallbackProvider>(new MedSetAuthenticationCallbackProvider());
		}
	}
}