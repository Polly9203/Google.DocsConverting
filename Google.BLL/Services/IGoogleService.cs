using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;

namespace Google.BLL.Services
{
    public interface IGoogleService
    {
        public GoogleCredential CreateCredentials();
        public DriveService CreateDriveService(GoogleCredential credential);
    }
}
