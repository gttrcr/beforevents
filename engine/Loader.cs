namespace Beforevents
{
    using CG.Web.MegaApiClient;
    using Octokit;
    using Newtonsoft.Json.Linq;

    public class Loader
    {
        private readonly string _megaUser = "";
        private readonly string _megaPassword = "";
        private readonly string _gitProductHeaderValue = "";
        private readonly string _gitCredentials = "";
        private readonly string _gitUser = "";
        private readonly string _gitRepo = "";
        private readonly string _gitBranch = "";
        private readonly string _file = "";

        public Loader()
        {
            string json = File.ReadAllText("proper.json");
            dynamic j = JObject.Parse(json);
            _gitProductHeaderValue = j.gitProductHeaderValue;
            _gitCredentials = j.gitCredentials;
            _gitUser = j.gitUser;
            _gitRepo = j.gitRepo;
            _gitBranch = j.gitBranch;
            _file = j.file;
        }

        public void UploadMega()
        {
            MegaApiClient client = new MegaApiClient();
            client.Login(_megaUser, _megaPassword);
            IEnumerable<INode> nodes = client.GetNodes();
            INode root = nodes.Single(x => x.Type == NodeType.Root);
            INode myFile = client.UploadFile(_file, root);
            Uri downloadLink = client.GetDownloadLink(myFile);
            Console.WriteLine(downloadLink);
            client.Logout();
        }

        public void UploadGitHub(string content)
        {
            File.WriteAllText(_file, content);
            var gitHubClient = new GitHubClient(new ProductHeaderValue(_gitProductHeaderValue));
            gitHubClient.Credentials = new Credentials(_gitCredentials);
            var (owner, repoName, filePath, branch) = (_gitUser, _gitRepo, _file, _gitBranch);
            gitHubClient.Repository.Content.CreateFile(owner, repoName, filePath, new CreateFileRequest($"Events update for {DateTime.Today}", content, branch)).Wait();
            File.Delete(_file);
        }
    }
}