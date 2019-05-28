namespace E2EEDRM.REST.Models.Processing
{
	public class InventoryJobRequest
	{
		public Iventoryjob InventoryJob { get; set; }
	}

	public class Iventoryjob
	{
		public int ProcessingSetId { get; set; }
		public int WorkspaceArtifactId { get; set; }
	}
}
