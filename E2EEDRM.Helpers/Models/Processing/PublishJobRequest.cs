namespace E2EEDRM.REST.Models.Processing
{
	public class PublishJobRequest
	{
		public Publishjob PublishJob { get; set; }
	}

	public class Publishjob
	{
		public int ProcessingSetId { get; set; }
		public int WorkspaceArtifactId { get; set; }
	}
}
