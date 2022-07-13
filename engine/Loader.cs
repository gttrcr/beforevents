namespace WhaToDo
{
    using CG.Web.MegaApiClient;
    using Octokit;

    public class Loader
    {
        private static readonly string _megaUser = "";
        private static readonly string _megaPassword = "";
        private static readonly string _gitProductHeaderValue = "";
        private static readonly string _gitCredentials = "";
        private static readonly string _gitUser = "";
        private static readonly string _gitRepo = "";
        private static readonly string _gitBranch = "";

        public static void UploadMega(string file)
        {
            MegaApiClient client = new MegaApiClient();
            client.Login(_megaUser, _megaPassword);
            IEnumerable<INode> nodes = client.GetNodes();
            INode root = nodes.Single(x => x.Type == NodeType.Root);
            INode myFile = client.UploadFile(file, root);
            Uri downloadLink = client.GetDownloadLink(myFile);
            Console.WriteLine(downloadLink);
            client.Logout();
        }

        public static void UploadGitHub(string file, string content)
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue(_gitProductHeaderValue));
            gitHubClient.Credentials = new Credentials(_gitCredentials);
            var (owner, repoName, filePath, branch) = (_gitUser, _gitRepo, file, _gitBranch);
            gitHubClient.Repository.Content.CreateFile(owner, repoName, filePath, new CreateFileRequest($"Events update for {DateTime.Today}", content, branch)).Wait();
        }
    }
}