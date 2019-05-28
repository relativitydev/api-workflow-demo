namespace E2EEDRM.REST.Models.Processing
{
	public class ProcessingSetSaveRequest
	{
		public int workspaceArtifactId { get; set; }
		public Processingset ProcessingSet { get; set; }
	}

	public class Processingset
	{
		public string[] EmailNotificationRecipients { get; set; }
		public string Name { get; set; }
		public Profile Profile { get; set; }
	}

	public class Profile
	{
		public int ArtifactID { get; set; }
	}
}
