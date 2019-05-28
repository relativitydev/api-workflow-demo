
namespace E2EEDRM.REST.Models.Production
{


	public class ReadProductionRequest
	{
		public int workspaceArtifactID { get; set; }
		public int productionArtifactID { get; set; }
		public int DataSourceReadMode { get; set; }
	}
	public class ProductionRunRequest
	{
		public int workspaceArtifactID { get; set; }

		public int productionArtifactID { get; set; }
		public bool suppressWarnings { get; set; }
		public bool overrideConflicts { get; set; }
	}


	public class ProductionStageRequest
	{
		public int workspaceArtifactId { get; set; }
		public int productionArtifactId { get; set; }

	}
	public class ProductionPlaceholderRef
	{
		public ProductionPlaceholderRef()
		{
			this.ArtifactID = 0;
			this.Name = string.Empty;
		}

		public ProductionPlaceholderRef(int artifactID)
		{
			this.ArtifactID = artifactID;
			this.Name = string.Empty;
		}

		public int ArtifactID { get; set; }

		public string Name { get; set; }
	}

	public class ProductionDataSourceObject
	{
		public int workspaceArtifactID { get; set; }
		public int productionID { get; set; }
		public Datasource dataSource { get; set; }
	}

	public class Datasource
	{
		public int ArtifactTypeID { get; set; }
		public string ProductionType { get; set; }
		public Savedsearch SavedSearch { get; set; }
		public string UseImagePlaceholder { get; set; }
		public Markupset MarkupSet { get; set; }
		public bool BurnRedactions { get; set; }
		public string Name { get; set; }
		public ProductionPlaceholderRef PlaceHolder { get; set; }
	}

	public class Savedsearch
	{
		public int ArtifactID { get; set; }
	}

	public class Markupset
	{
		public int ArtifactID { get; set; }
	}



	public class ProductionObject
	{
		public int workspaceArtifactID { get; set; }
		public Production Production { get; set; }
	}

	public class Production
	{
		public Details Details { get; set; }
		public Numbering Numbering { get; set; }
		public string SortOrder { get; set; }
		public Headers Headers { get; set; }
		public Footers Footers { get; set; }
		public bool ShouldCopyInstanceOnWorkspaceCreate { get; set; }
		public string Name { get; set; }
	}

	public class Details
	{
		public string EmailRecipients { get; set; }
		public int BrandingFontSize { get; set; }
		public bool ScaleBrandingFont { get; set; }
	}

	public class Numbering
	{
		public string NumberingType { get; set; }
		public Attachmentrelationalfield AttachmentRelationalField { get; set; }
		public string BatesPrefix { get; set; }
		public string BatesSuffix { get; set; }
		public int BatesStartNumber { get; set; }
		public int NumberOfDigitsForDocumentNumbering { get; set; }
	}

	public class Attachmentrelationalfield
	{
		public int ArtifactID { get; set; }
		public int ViewFieldID { get; set; }
		public string Name { get; set; }
	}

	public class Headers
	{
		public Leftheader LeftHeader { get; set; }
	}

	public class Leftheader
	{
		public string Type { get; set; }
		public Field Field { get; set; }
		public string FreeText { get; set; }
		public string FriendlyName { get; set; }
	}

	public class Field
	{
		public int ArtifactID { get; set; }
		public int ViewFieldID { get; set; }
		public string Name { get; set; }
	}

	public class Footers
	{
		public Leftfooter LeftFooter { get; set; }
	}

	public class Leftfooter
	{
		public string Type { get; set; }
		public Field1 Field { get; set; }
		public string FreeText { get; set; }
		public string FriendlyName { get; set; }
	}

	public class Field1
	{
		public int ArtifactID { get; set; }
		public int ViewFieldID { get; set; }
		public string Name { get; set; }
	}



}
