using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace SPI
{
    public class DataFilter : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent2 class.
        /// </summary>
        public DataFilter()
          : base("DataFilter", "DF",
              "Cooperate with shap in brep component, or can be used ",
              "SPI", "Tool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List", "L", "Geometry to test", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Relation", "R", "Relation", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Mode", "M", "0 - include intersecting object to inside list, 1 - exclude intersecting object from inside list", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("In", "I", "Data inside", GH_ParamAccess.list);
            pManager.AddGenericParameter("Out", "O", "Data Outside", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<int> RelationList = new List<int>();
            List<Object> DataList = new List<Object>();
            List<Object> InList = new List<Object>();
            List<Object> OutList = new List<object>();
            bool Mode = false;
            DA.GetDataList(0, DataList);
            DA.GetDataList(1, RelationList);
            DA.GetData(2, ref Mode);

            for (int i = 0;i<RelationList.Count;i++)
            {
                if(Mode)
                {
                    if(RelationList[i]!=2)
                    {
                        InList.Add(DataList[i]);
                    }

                    else
                    {
                        OutList.Add(DataList[i]);
                    }
                }

                else if(!Mode)
                {
                    if(RelationList[i]==0)
                    {
                        InList.Add(DataList[i]);
                    }

                    else
                    {
                        OutList.Add(DataList[i]);
                    }
                }

               
            }

            DA.SetDataList(0, InList);
            DA.SetDataList(1, OutList);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Icon.ObjectFilter;
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e183f280-787d-41b8-ae31-ddfd11e56b7e"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;





    }
}