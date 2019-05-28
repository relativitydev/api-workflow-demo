using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace E2EEDRM.Helpers.Models.Review
{
	public class CreateResponsiveFieldRequest
	{
		[JsonProperty(PropertyName = "Artifact Type Name")]
		public string ArtifactTypeName { get; set; }
		[JsonProperty(PropertyName = "Parent Artifact")]
		public ParentArtifact ParentArtifact { get; set; }
		public string Name { get; set; }
		[JsonProperty(PropertyName = "Object Type")]
		public ObjectType ObjectType { get; set; }
		[JsonProperty(PropertyName = "Field Type ID")]
		public int FieldTypeID { get; set; }
		[JsonProperty(PropertyName = "Is Required")]
		public bool IsRequired { get; set; }
		[JsonProperty(PropertyName = "Open To Associations")]
		public bool OpenToAssociations { get; set; }
		public bool Linked { get; set; }
		[JsonProperty(PropertyName = "Allow Sort/Tally")]
		public bool AllowSortTally { get; set; }
		public int Width { get; set; }
		public bool Wrapping { get; set; }
		[JsonProperty(PropertyName = "Allow Group By")]
		public bool AllowGroupBy { get; set; }
		[JsonProperty(PropertyName = "Allow Pivot")]
		public bool AllowPivot { get; set; }
		[JsonProperty(PropertyName = "Ignore Warnings")]
		public bool IgnoreWarnings { get; set; }
		[JsonProperty(PropertyName = "No Value")]
		public string NoValue { get; set; }
		[JsonProperty(PropertyName = "Yes Value")]
		public string YesValue { get; set; }
	}

	public class ParentArtifact
	{
		[JsonProperty(PropertyName = "Artifact ID")]
		public int ArtifactID { get; set; }
	}

	public class ObjectType
	{
		[JsonProperty(PropertyName = "Descriptor Artifact Type ID")]
		public int DescriptorArtifactTypeID { get; set; }
	}

}
