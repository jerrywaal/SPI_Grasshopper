using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SPI
{
    public class EasyExtrude : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public EasyExtrude()
          : base("EasyExtrude", "EE",
              "Extrude Surfaces follow their normal direction",
              "SPI", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Base", "B", "Surface to extrude", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distance", "D", "Extrude distance", GH_ParamAccess.item);
            pManager.AddBooleanParameter("BothSide", "BS", "Extrude to bothside or not", GH_ParamAccess.item,false);
            pManager.AddBooleanParameter("SwitchSide", "SS", "Switch extrude direction", GH_ParamAccess.item,false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Result", "R", "Extrusion Result", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Surface> SL = new List<Surface>();
            List<Brep> ExtrusionList = new List<Brep>();
            bool BothSide = false;
            bool SwitchSide = false;
            double Distance = 0;
            Point3d p;
            Vector3d[] UV;
            Vector3d N;
            
            DA.GetDataList(0, SL);
            DA.GetData(1, ref Distance);
            DA.GetData(2, ref BothSide);
            DA.GetData(3, ref SwitchSide);
            foreach (Surface s in SL)
            {
                s.Evaluate(s.Domain(0).Mid, s.Domain(1).Mid, 1, out p, out UV);
                Plane P = new Plane(p, UV[0], UV[1]);
                N = P.Normal;
                Line L = new Line(p, N, Distance);
                Brep B = Brep.CreateFromSurface(s);
                BrepFace BF = B.Faces[0];
                if(SwitchSide == false)
                {
                    Brep Extrusion = Brep.CreateFromOffsetFace(BF, Distance, 0.01, BothSide,true);
                    ExtrusionList.Add(Extrusion);
                }
                else
                {
                    Brep Extrusion = Brep.CreateFromOffsetFace(BF, -Distance, 0.01, BothSide, true);
                    ExtrusionList.Add(Extrusion);
                }
                
            }
            DA.SetDataList(0, ExtrusionList);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Icon.EasyExtrude;


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("60E466C8-E021-4436-B383-8A801B0C5F22"); }
        }
    }
}