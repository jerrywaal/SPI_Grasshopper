using System;
using System.Collections.Generic;
using Rhino.Collections;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;

namespace SPI
{
    public class GeometrySplit : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GeometrySplit()
          : base("GeometrySplit", "GS",
              "Split geometry data with types",
              "SPI", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "G", "Geometry to split", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "C", "", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Surface", "S", "", GH_ParamAccess.list);
            pManager.AddBrepParameter("Brep", "B", "", GH_ParamAccess.list);
            pManager.AddMeshParameter("Mesh", "M", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Object> OBJ = new List<object>();
            List<GH_Point> PointList = new List<GH_Point>();
            List<GH_Mesh> MeshList = new List<GH_Mesh>();
            List<Surface> SurfaceList = new List<Surface>();
            List<GH_Brep> BrepList = new List<GH_Brep>();
            List<GH_Curve> CurveList = new List<GH_Curve>();

            DA.GetDataList(0, OBJ);

            
            for (int i = 0; i < OBJ.Count; i++)
            {
                if (OBJ[i] is GH_Point)
                {
                    GH_Point Point = (GH_Point)OBJ[i];
                    PointList.Add(Point);
                }

                else if (OBJ[i] is GH_Curve)
                {
                    GH_Curve Crv = (GH_Curve)OBJ[i];
                    CurveList.Add(Crv);
                }

                else if (OBJ[i] is GH_Brep)
                {
                    GH_Brep brep = (GH_Brep)OBJ[i];
                    Brep B = brep.Value;
                    if(B == null)
                    {
                        return;
                    }
                    B = B.DuplicateBrep();
                    if(B.Faces.Count==1)
                    {
                       Brep brepSrf = B.Faces.ExtractFace(0);
                       Surface srf = brepSrf.Surfaces.First();
                        SurfaceList.Add(srf);
                    }

                    else
                    {
                        BrepList.Add(brep);
                    }

                }

                else if(OBJ[i] is GH_Mesh)
                {
                    GH_Mesh mesh = (GH_Mesh)OBJ[i];
                    MeshList.Add(mesh);
                }

                
            }

            DA.SetDataList(0, PointList);
            DA.SetDataList(1, CurveList);
            DA.SetDataList(2, SurfaceList);
            DA.SetDataList(3, BrepList);
            DA.SetDataList(4, MeshList);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Icon.GeometrySplit;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("BAE2C446-521B-4E1B-857A-8726B2ECDF2E"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}