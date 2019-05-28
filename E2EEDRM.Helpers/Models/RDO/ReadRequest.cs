namespace E2EEDRM.REST.Models.RDO
{
	public class ReadRequest
	{
		public request Request { get; set; }
	}

	public class request
	{
		public Object Object { get; set; }
		public field[] Fields { get; set; }
	}

	public class Object
	{
		public int ArtifactID { get; set; }
	}

	public class field
	{
		public int ArtifactID { get; set; }
		public string Name { get; set; }
		public string Guid { get; set; }
	}
}
