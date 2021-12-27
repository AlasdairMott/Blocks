using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Blocks.GH
{
	public class BlocksInfo : GH_AssemblyInfo
	{
		public override string Name
		{
			get
			{
				return "Blocks";
			}
		}
		public override Bitmap Icon
		{
			get
			{
				//Return a 24x24 pixel bitmap to represent this GHA library.
				return null;
			}
		}
		public override string Description
		{
			get
			{
				//Return a short string describing the purpose of this GHA library.
				return "";
			}
		}
		public override Guid Id
		{
			get
			{
				return new Guid("b1d7fbe3-e83c-494b-8b64-ec074a23efee");
			}
		}

		public override string AuthorName
		{
			get
			{
				//Return a string identifying you or your company.
				return "";
			}
		}
		public override string AuthorContact
		{
			get
			{
				//Return a string representing your preferred contact details.
				return "";
			}
		}
	}
}
