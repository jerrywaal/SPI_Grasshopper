using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace SPI
{
    public class DoubleOffset : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public DoubleOffset()
          : base("Double Offset", "DO", "Offset Curve on bothside in given Plane", "SPI", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Input Curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distance", "Dis", "Distance to offset", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "P", "Plane for offset operation", GH_ParamAccess.list,Plane.WorldXY);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("OffsetCurve", "OffCrv", "Output Curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("SideCurve1", "SCrv1", "One Side Curve", GH_ParamAccess.list);
            pManager.AddCurveParameter("SideCurve2", "SCrv2", "another Side Curve", GH_ParamAccess.list);
            //pManager.AddCurveParameter("LoftSurface", "LoftSrf", "Surface", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> Crv1List = new List<Curve>();
            List<Curve> Crv2List = new List<Curve>();
            List<Curve> C = new List<Curve>();
            List<Plane> P = new List<Plane>();
            Double Distance = 0;

            Curve[] Crv1;
            Curve[] Crv2;

            /* DA.GetData(0, C);*/;
            DA.GetDataList(0, C);
            DA.GetData(1, ref Distance);
            DA.GetDataList(2, P);

            foreach (Curve CC in C)
            {
                foreach(Plane PP in P)
                {
                    Crv1 = CC.Offset(PP, Distance, 0.01, CurveOffsetCornerStyle.Sharp);
                    Crv2 = CC.Offset(PP, -Distance, 0.01, CurveOffsetCornerStyle.Sharp);
                    Crv1List = Crv1.ToList();
                    Crv2List = Crv2.ToList();
                }
               
            }


            List<Curve> CrvList = new List<Curve>();
            foreach (Curve C1 in Crv1List)
            {
                CrvList.Add(C1);
            }

            foreach (Curve C2 in Crv2List)
            {
                CrvList.Add(C2);
            }
            
            //Brep BaseBrep = Brep.CreateEdgeSurface(CrvList);


            DA.SetDataList(0, CrvList);
            DA.SetDataList(1, Crv1List);
            DA.SetDataList(2, Crv2List);
            //DA.SetDataList(3, BaseBrep);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Icon.Double_Offset;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e3adb75e-b693-4f71-96af-7f7282ce8434"); }
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
