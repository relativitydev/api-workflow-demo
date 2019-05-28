using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2EEDRM.REST.Models.Searching
{
	public class KeywordSearchCreateRequest
	{
		public int workspaceArtifactID { get; set; }
		public Searchdto searchDTO { get; set; }
	}

	public class Searchdto
	{
		public int ArtifactTypeID { get; set; }
		public string Name { get; set; }
		public Searchcriteria SearchCriteria { get; set; }
		public Field[] Fields { get; set; }
	}

	public class Searchcriteria
	{
		public Condition[] Conditions { get; set; }
	}

	public class Condition
	{
		public Condition1 condition { get; set; }
	}

	public class Condition1
	{
		public string Operator { get; set; }
		public Fieldidentifier FieldIdentifier { get; set; }
		public string Value { get; set; }
		public string ConditionType { get; set; }
	}

	public class Fieldidentifier
	{
		public string Name { get; set; }
	}

	public class Field
	{
		public string Name { get; set; }
	}

}
