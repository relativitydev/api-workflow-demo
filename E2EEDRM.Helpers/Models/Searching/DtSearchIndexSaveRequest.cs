using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2EEDRM.REST.Models.Searching
{
	public class DtSearchIndexSaveRequest
	{
		public DtSearchIndexRequest dtSearchIndexRequest { get; set; }
	}

	public class DtSearchIndexRequest
	{
		public string Name { get; set; }
		public int Order { get; set; }
		public int SearchSearchID { get; set; }
		public bool RecognizeDates { get; set; }
		public bool SkipNumericValues { get; set; }
		public int IndexShareCodeArtifactID { get; set; }
		public string EmailAddress { get; set; }
		public string NoiseWords { get; set; }
		public string AlphabetText { get; set; }
		public string DirtySettings { get; set; }
		public int SubIndexSize { get; set; }
		public int FragmentationThreshold { get; set; }
		public int Priority { get; set; }
	}
}
