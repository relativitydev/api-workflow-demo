using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2EEDRM.REST.Models.Searching
{
	public class DtSearchSaveRequest
	{
		public int workspaceArtifactID { get; set; }
		public searchdto searchDTO { get; set; }
	}

	public class searchdto
	{
		public int ArtifactTypeID { get; set; }
		public string Name { get; set; }
		public searchcriteria SearchCriteria { get; set; }
		public searchcontainer SearchContainer { get; set; }
		public searchindex SearchIndex { get; set; }
		public Fields[] Fields { get; set; }
	}

	public class searchcriteria
	{
		public condition[] Conditions { get; set; }
	}

	public class condition
	{
		public condition1 condition1 { get; set; }
	}

	public class condition1
	{
		public string Operator { get; set; }
		public fieldidentifier FieldIdentifier { get; set; }
		public string Value { get; set; }
		public string ConditionType { get; set; }
	}

	public class fieldidentifier
	{
		public string Name { get; set; }
	}

	public class searchcontainer
	{
		public int ArtifactID { get; set; }
		public string Name { get; set; }
	}

	public class searchindex
	{
		public int ArtifactID { get; set; }
		public string Name { get; set; }
	}

	public class Fields
	{
		public string Name { get; set; }
	}
}
