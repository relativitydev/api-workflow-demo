using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2EEDRM.REST.Models.Imaging
{
	public class RunImagingJobRequest
	{
		public Imagingjob imagingJob { get; set; }
	}

	public class Imagingjob
	{
		public int imagingSetId { get; set; }
		public int workspaceId { get; set; }
	}

}
