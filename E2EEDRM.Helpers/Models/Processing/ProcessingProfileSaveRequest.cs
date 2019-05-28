namespace E2EEDRM.REST.Models.Processing
{
	public class ProcessingProfileSaveRequestStructure
	{
		public ProcessingProfileSaveRequest ProcessingProfileSaveRequest { get; set; }
	}
	public class ProcessingProfileSaveRequest
	{
		public ProcessingProfile ProcessingProfile { get; set; }
		public int WorkspaceId { get; set; }
	}

	public class ProcessingProfile
	{
		public string Name { get; set; }
		public int ArtifactId { get; set; }
		public NumberingSettings NumberingSettings { get; set; }
		public InventoryDiscoverSettings InventoryDiscoverSettings { get; set; }
		public ExtractionSettings ExtractionSettings { get; set; }
		public DeduplicationSettings DeduplicationSettings { get; set; }
		public PublishSettings PublishSettings { get; set; }
	}

	public class NumberingSettings
	{
		public string DefaultDocumentNumberingPrefix { get; set; }
		public string NumberingType { get; set; }
		public int DefaultStartNumber { get; set; }
		public string Delimiter { get; set; }
		public string NumberofDigits { get; set; }
		public string ParentChildNumbering { get; set; }
	}

	public class InventoryDiscoverSettings
	{
		public bool DeNIST { get; set; }
		public string DeNISTMode { get; set; }
		public string[] DefaultOCRlanguages { get; set; }
		public int DefaultTimeZoneID { get; set; }
	}

	public class ExtractionSettings
	{
		public bool Extractchildren { get; set; }
		public string[] ChildExtractionMethod { get; set; }
		public string ExcelTextExtractionMethod { get; set; }
		public string ExcelHeaderFooterExtraction { get; set; }
		public string WordTextExtractionMethod { get; set; }
		public string PowerPointTextExtractionMethod { get; set; }
		public bool OCR { get; set; }
		public string OCRAccuracy { get; set; }
		public bool OCRTextSeparator { get; set; }
		public string EmailOutput { get; set; }
	}

	public class DeduplicationSettings
	{
		public string DeduplicationMethod { get; set; }
		public bool PropagateDeduplicationData { get; set; }
	}

	public class PublishSettings
	{
		public bool UseSourceFolderStructure { get; set; }
		public bool AutopublishSet { get; set; }
		public DefaultDestinationFolder DefaultDestinationFolder { get; set; }
	}

	public class DefaultDestinationFolder
	{
		public int ArtifactID { get; set; }
	}
}
