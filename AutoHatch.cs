using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;

namespace SPI
{
    public class AutoHatch : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public AutoHatch()
          : base("AutoHatch", "AutoHatch",
              "Hatch curve from rhino",
              "SPI", "Tool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curves needed to be hatched", GH_ParamAccess.item);
            pManager.AddTextParameter("PatternName", "PN", "Patterns' name.", GH_ParamAccess.item,"HatchDash");
            pManager.AddNumberParameter("Scale", "S", "Pattern scale", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Rotation", "R", "Rotation angle", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Run", "R", "Run the component", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Bake", "B", "Bake the hatch", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Preview", "P", "Preview the hatch", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve C = null;
            string PatternName = "方块";
            double Scale = 1.0;
            double Rotation = 0.0;
            bool Run = false;
            bool Bake = false;

            DA.GetData(0, ref C);
            DA.GetData(1, ref PatternName);
            DA.GetData(2, ref Scale);
            DA.GetData(3, ref Rotation);
            DA.GetData(4, ref Run);
            DA.GetData(5, ref Bake);

            RhinoDoc Doc = Rhino.RhinoDoc.ActiveDoc;
           
            List<GeometryBase> GBL = new List<GeometryBase>();
            List<Hatch> HL = new List<Hatch>();

            if (Run)
            {
                HatchPattern HP = Doc.HatchPatterns.FindName(PatternName);
                int PatternIndex = HP.Index;
                Hatch[] HA = Hatch.Create(C, PatternIndex, Rotation * (Math.PI / 180), Scale, 0.01);
                foreach (Hatch h in HA)
                {
                    GeometryBase[] GB = h.Explode();
                    HL.Add(h);
                    GBL.AddRange(GB);

                }
                if (Bake)
                {
                    for (int i = 0; i < HL.Count; i++)
                    {
                        Doc.Objects.AddHatch(HL[i]);
                        Doc.Views.Redraw();
                    }
                }
            }

            DA.SetDataList(0, GBL);
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
                return Properties.Icon.Hatch;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D007E98E-2CB0-4EC8-805B-EDE30517D141"); }
        }
    }
}