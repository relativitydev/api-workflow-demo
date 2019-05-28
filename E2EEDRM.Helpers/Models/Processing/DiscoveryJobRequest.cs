namespace E2EEDRM.REST.Models.Processing
{
	public class DiscoveryJobRequest
	{
		public Discoveryjob DiscoveryJob { get; set; }
	}

	public class Discoveryjob
	{
		public int ProcessingSetId { get; set; }
		public int WorkspaceArtifactId { get; set; }
	}
}
