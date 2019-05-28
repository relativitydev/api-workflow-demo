namespace E2EEDRM.REST.Models.Processing
{

	public class ProcessingDataSourceSaveRequest
	{
		public Processingdatasource processingDataSource { get; set; }
		public int workspaceArtifactId { get; set; }
	}

	public class Processingdatasource
	{
		public int Custodian { get; set; }
		public int DestinationFolder { get; set; }
		public string DocumentNumberingPrefix { get; set; }
		public string InputPath { get; set; }
		public string[] OcrLanguages { get; set; }
		public int Order { get; set; }
		public ProcessingSet ProcessingSet { get; set; }
		public int TimeZone { get; set; }
		public bool IsStartNumberVisible { get; set; }
		public int StartNumber { get; set; }
		public int ArtifactID { get; set; }
		public string Name { get; set; }
	}

	public class ProcessingSet
	{
		public int ArtifactID { get; set; }
	}
}
