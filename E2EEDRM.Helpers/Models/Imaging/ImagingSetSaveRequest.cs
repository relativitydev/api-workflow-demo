using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2EEDRM.REST.Models.Imaging
{
	public class ImagingSetSaveRequest
	{
		public Imagingset imagingSet { get; set; }
		public int workspaceId { get; set; }
	}

	public class Imagingset
	{
		public ImagingProfile ImagingProfile { get; set; }
		public int DataSource { get; set; }
		public string EmailNotificationRecipients { get; set; }
		public string Name { get; set; }
	}

	public class ImagingProfile
	{
		public int ArtifactID { get; set; }
	}

}
