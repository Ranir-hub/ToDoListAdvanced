using System.Collections.ObjectModel;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Util.Store;

namespace ToDoListAdvanced
{
    public class GoogleDriveSyncService
    {
        private const string FileName = "todo_backup.json";
        private static readonly string[] Scopes = { "https://www.googleapis.com/auth/drive.appdata" };

        private UserCredential _credential;
        private DriveService _driveService;
        private string _fileId;

        public async Task<bool> LoginAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("client_secret.json");
                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("TodoListAuthTokens", true)
                );

                if (_credential.Token.IsExpired(SystemClock.Default))
                {
                    await _credential.RefreshTokenAsync(CancellationToken.None);
                }

                _driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = "ToDoListAdvanced"
                });

                _fileId = await GetOrCreateFileAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Вход не выполнен");
            }
        }

        private async Task<string> GetOrCreateFileAsync()
        {
            if (_fileId != null) return _fileId;
            if (_driveService == null) throw new Exception("Вход не выполнен");

            var listRequest = _driveService.Files.List();
            listRequest.Q = $"name='{FileName}' and trashed=false";
            listRequest.Fields = "files(id, name)";
            listRequest.Spaces = "appDataFolder";

            var result = await listRequest.ExecuteAsync();

            if (result.Files.Count > 0)
            {
                _fileId = result.Files[0].Id;
                return _fileId;
            }

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = FileName,
                Parents = new List<string> { "appDataFolder" }
            };

            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, new List<object>());
            stream.Position = 0;

            var createRequest = _driveService.Files.Create(fileMetadata, stream, "application/json");
            createRequest.Fields = "id";
            await createRequest.UploadAsync();

            _fileId = createRequest.ResponseBody?.Id;
            if (string.IsNullOrEmpty(_fileId))
                throw new Exception("Ошибка создания файла");

            return _fileId;
        }

        public async Task SaveToCloudAsync(ObservableCollection<ToDoTask> tasks)
        {
            if (_driveService == null) return;

            var fileId = await GetOrCreateFileAsync();

            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, tasks, new JsonSerializerOptions { WriteIndented = true });
            stream.Position = 0;
            var updateRequest = _driveService.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId, stream, "application/json");
            await updateRequest.UploadAsync();
        }

        public async Task<ObservableCollection<ToDoTask>> LoadFromCloudAsync()
        {
            if (_driveService == null) return null;

            var fileId = await GetOrCreateFileAsync();

            var request = _driveService.Files.Get(fileId);
            using var stream = new MemoryStream();
            await request.DownloadAsync(stream);

            stream.Position = 0;

            var tasks = await JsonSerializer.DeserializeAsync<ObservableCollection<ToDoTask>>(stream);

            return tasks ?? new ObservableCollection<ToDoTask>();
        }

        public bool IsLoggedIn => _credential != null;
        public async Task LogoutAsync()
        {
            if (_credential != null)
            {
                try
                {
                    await _credential.RevokeTokenAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            try
            {
                var storePath = Path.Combine(FileSystem.AppDataDirectory, "TodoListAuthTokens");
                var dataStore = new FileDataStore(storePath, true);
                await dataStore.ClearAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            _credential = null;
            _driveService = null;
            _fileId = null;
        }
    }
}