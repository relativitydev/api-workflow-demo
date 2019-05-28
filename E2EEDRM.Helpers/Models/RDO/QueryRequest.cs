namespace E2EEDRM.REST.Models.RDO
{
	public class QueryRequest
	{
		public Request request { get; set; }
		public int start { get; set; }
		public int length { get; set; }
	}

	public class Request
	{
		public Objecttype ObjectType { get; set; }
		public Field[] fields { get; set; }
		public string condition { get; set; }
		public object[] sorts { get; set; }
	}

	public class Objecttype
	{
		public string Guid { get; set; }
		public string Name { get; set; }
		public int ArtifactTypeID { get; set; }
	}

	public class Field
	{
		public string Guid { get; set; }
	}
}
