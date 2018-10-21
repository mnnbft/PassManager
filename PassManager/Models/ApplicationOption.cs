namespace PassManager.Models
{
    public class ApplicationOption
    {
        public ApplicationOption Instance { get; } = new ApplicationOption();
        private ApplicationOption() { }

        public string DefaultPassPath { get; set; }
        public string DefaultKeyPath { get; set; }
    }
}
