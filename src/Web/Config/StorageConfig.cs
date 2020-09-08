namespace Web.Config
{
    public class StorageConfig
    {
        public string ConnectionString { get; set; }
        public string Url { get; set; }
        public string PublicContainerName { get; set; }
        public string PrivateContainerName { get; set; }
    }
}