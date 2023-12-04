using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace SPI
{
    public class SPIInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "SPI";
            }
        }
        public override Bitmap Icon => Properties.Icon.SPI_LOGO;
      
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
                return new Guid("43180f93-a5f1-43b3-a459-70d4b3bfadfb");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "JerryCen";
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
