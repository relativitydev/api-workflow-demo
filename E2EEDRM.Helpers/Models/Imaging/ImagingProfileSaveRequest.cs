using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2EEDRM.REST.Models.Imaging
{
	public class ImagingProfileSaveRequest
	{
		public Imagingprofile imagingProfile { get; set; }
		public int workspaceId { get; set; }
	}

	public class Imagingprofile
	{
		public string ImagingMethod { get; set; }
		public Basicoptions BasicOptions { get; set; }
		public object[] NativeTypes { get; set; }
		public object[] ApplicationFieldCodes { get; set; }
		public string Name { get; set; }
	}

	public class Basicoptions
	{
		public int ImageOutputDpi { get; set; }
		public string BasicImageFormat { get; set; }
		public string ImageSize { get; set; }
	}

}
