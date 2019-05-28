using System.Collections.Generic;
using System.Linq.Expressions;
using E2EEDRM.Helpers;
using Newtonsoft.Json;

namespace E2EEDRM.REST.Models.Review
{
	public class TaggingDoc
	{
		[JsonProperty(PropertyName = "Artifact ID")]
		public int ArtifactId { get; set; }
		[JsonProperty(PropertyName = "Artifact Type Name")]
		public string TypeName { get; set; }
		[JsonProperty(PropertyName = "Responsive - E2E")] //we need to set this value to whatever field we want to update
		public bool FieldValue { get; set; }
	}
}
