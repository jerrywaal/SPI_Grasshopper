using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SPI
{
    public class RemapPack : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public RemapPack()
          : base("Remap Pack", "RP",
              "Remap Function Pack",
              "SPI", "Tool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Value", "V", "Value to remap", GH_ParamAccess.list);
            pManager.AddNumberParameter("Domain Start", "S", "Start number for remap domain", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Domain End", "E", "End number for remap domain", GH_ParamAccess.item, 1);
            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Mapped", "M", "Remap Result", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //数据初始化
            double DomainStart = 0;
            double DomainEnd = 1;
            List<double> ValueList = new List<double>();
            List<double> RemapList = new List<double>();

            if (!DA.GetDataList(0, ValueList)) { return; }
            DA.GetData(1, ref DomainStart);
            DA.GetData(2, ref DomainEnd);

            double FromMin = ValueList.AsQueryable().Min();
            double FromMax = ValueList.AsQueryable().Max();

            //Remap
            foreach (double x in ValueList)
            {
                if (ValueList.Count > 1)
                {
                    double Remap = (x - FromMin) / (FromMax - FromMin) * (DomainEnd - DomainStart) + DomainStart;
                    RemapList.Add(Remap);
                }
                else if (ValueList.Count == 1)
                {
                    double Remap = x / x * (DomainEnd - DomainStart) + DomainStart;
                    RemapList.Add(Remap);
                }
            }
            DA.SetDataList(0, RemapList);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Icon.Remap_Pack;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("07F73B87-46AB-402B-BA19-6328B2F1858E"); }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }
    }
}