using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Utils.Constants.Settings;

namespace Google.BLL.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly GoogleSettings googleConfiguration;

        public GoogleService(IOptions<GoogleSettings> googleConfiguration)
        {
            this.googleConfiguration = googleConfiguration.Value;
        }

        public GoogleCredential CreateCredentials()
        {
            using (var stream = new FileStream(googleConfiguration.ServiceAccountKeyPath, FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential.FromStream(stream)
                    .CreateScoped(DriveService.ScopeConstants.Drive);
            }
        }

        public DriveService CreateDriveService(GoogleCredential credential)
        {
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = googleConfiguration.ApplicationName
            });
        }
    }
}
