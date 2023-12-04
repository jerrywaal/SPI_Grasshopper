using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace SPI
{
    public class PairItSelf : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PairItSelf()
          : base("GroupItSelf", "GItS",
              "Group items in list with pairs",
              "SPI", "Tool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List", "L", "List to Input", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("List", "L", "Pairs", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Object> OL = new List<object>();
            List<Object[]> OAL = new List<object[]>();

            DA.GetDataList(0, OL);

            for (int i = 0; i < OL.Count; i++)
            {
                for (int j = i + 1; j < OL.Count; j++)
                {
                    Object[] OA = new Object[] { OL[i], OL[j] };
                    OAL.Add(OA);
                }
            }


            //List<Object> OL2 = new List<object>();
            DataTree<Object> Tree = new DataTree<object>();

            for (int i = 0; i < OAL.Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    //OL2.Add(OAL[i][j]);
                    Tree.Add(OAL[i][j], new GH_Path(i));
                }
            }

            DA.SetDataTree(0, Tree);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Icon.PairItSelf;


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7D1506C0-2E1B-475A-B005-BBE249BA8449"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
    
}