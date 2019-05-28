namespace E2EEDRM.REST.Models.Processing
{

	public class CustodianSaveRequest
	{
		public Custodian Custodian { get; set; }
		public int workspaceArtifactId { get; set; }
	}

	public class Custodian
	{
		public string DocumentNumberingPrefix { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CustodianType { get; set; }
	}

}
