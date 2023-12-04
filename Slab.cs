using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SPI
{
    public class Slab : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Slab()
          : base("Slab", "S",
              "create wall object.",
              "SPI", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to extrude", GH_ParamAccess.list);
            pManager.AddNumberParameter("Height", "H", "Heihgt", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "T", "Thickness", GH_ParamAccess.item);
            pManager.AddBooleanParameter("SwitchSide", "SW", "Switch extrude direction", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Slab", "S", "Outcome", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> OC = new List<Curve>();
            List<Brep> FB = new List<Brep>();
            List<Brep> FB2 = new List<Brep>();

            bool Switch = false;
            double Height = 0.00;
            double Thickness = 0.00;

            if (!DA.GetDataList(0, OC)) { return; }
            if (!DA.GetData(1, ref Height)){ return; }
            if (!DA.GetData(2, ref Thickness)) { return; }


  
                for(int i = 0; i<OC.Count; i++ )
                {
                    Curve C1 = OC[i].Offset(Plane.WorldXY, Thickness / 2, 0.01, CurveOffsetCornerStyle.Sharp)[0];
                    Curve C2 = OC[i].Offset(Plane.WorldXY, -Thickness / 2, 0.01, CurveOffsetCornerStyle.Sharp)[0];

                    List<Curve> GroupC = new List<Curve>() { C1, C2 };


                    Brep Base = Brep.CreateEdgeSurface(GroupC);

                    if (Switch == false)
                    {
                        Brep Final = Brep.CreateFromOffsetFace(Base.Faces[0], -Height, 0.01, false, true);
                        FB.Add(Final);
                    }
                    else
                    {
                        Brep Final = Brep.CreateFromOffsetFace(Base.Faces[0], Height, 0.01, false, true);
                        FB.Add(Final);
                    }

                    Brep[] Slab = Brep.CreateBooleanUnion(FB, 0.01);
                    FB2.AddRange(Slab);

    

                

            }

            DA.SetDataList(0, FB2);


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
                return Properties.Icon.Slab;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C62D6DC2-DF1D-492C-84B3-3DA1C518D1EB"); }
        }
    }
}