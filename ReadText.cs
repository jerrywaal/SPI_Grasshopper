using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;
using Grasshopper;
using System;
using System.Collections.Generic;
using Rhino.DocObjects;

namespace SPI
{
    public class ReadText : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ReadText()
          : base("ReadText", "RT",
              "Read text from Rhino doc by guid",
              "SPI", "Drawing")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            IGH_Param param = new Grasshopper.Kernel.Parameters.Param_Guid();                      
            pManager.AddParameter(param, "Guid", "ID", "Text Guid", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "Text", GH_ParamAccess.list);
            pManager.AddPointParameter("Location", "L", "Location of text", GH_ParamAccess.list);
            //pManager.AddBrepParameter("Block", "B", "Building Block", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Guid> IDList = new List<Guid>();
            /*foreach (IGH_Param source in Params.Input[0].Sources)
           
                IDList.Add(source.InstanceGuid);*/

            DA.GetDataList(0, IDList);

            List<string> TextContentList = new List<string>();
            List<Point3d> TextLocationList = new List<Point3d>();

            
            for (int i = 0; i < IDList.Count; i++)

            {
                RhinoObject RO = RhinoDoc.ActiveDoc.Objects.FindId(IDList[i]);
                TextObject TO = (TextObject)RO;
                string Text = TO.DisplayText;
                Point3d TextLocation = TO.TextGeometry.Plane.Origin;
                TextContentList.Add(Text);
                TextLocationList.Add(TextLocation);
            }

            
            DA.SetDataList(0, TextContentList);
            DA.SetDataList(1, TextLocationList);
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
                return Properties.Icon.ReadTxt;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("efc1d3ac-489b-4705-be6e-456e292200c1"); }
        }
    }
}