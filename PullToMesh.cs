using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;

using Rhino.DocObjects;
using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;

namespace SPI
{
    public class PullToMesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PullToMesh()
          : base("PullToMesh", "PuToM",
              "Pull Curve to Mesh",
              "SPI", "Tool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve to project", GH_ParamAccess.list);
            pManager.AddMeshParameter("Mesh", "M", "Mesh to project on to", GH_ParamAccess.list);
            pManager.AddVectorParameter("Direction", "D", "Direction to project", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Projected Curve", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> OC = new List<Curve>();
            List<Mesh> OM = new List<Mesh>();
            List<Vector3d> OV = new List<Vector3d>();

            if (!DA.GetDataList(0, OC)) { return; }
            if (!DA.GetDataList(1, OM)) { return; }
            if (!DA.GetDataList(2, OV)) { return; }

            foreach (Curve C in OC)
            {
              foreach(Mesh M in OM)
                {
                    foreach(Vector3d V in OV)
                    {
                        Polyline Curve.PullToMesh
                        DA.SetDataList(0, PC);
                    }
                    
                }
            }
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D8F1E4DD-E6F0-4CDD-A29D-0CA06D815821"); }
        }
    }
}