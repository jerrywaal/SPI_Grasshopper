using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace SPI
{
    public class UnifyUv : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public UnifyUv()
          : base("Unify Planar UV", "UPUV",
              "Unify Sufaces'UV which in same plane",
              "SPI", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "S", "Surfaces to unify", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "P", "Plane for reference", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "S", "Output surface", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //初始化数据
            List<Brep> S = new List<Brep>();
            List<Brep> FSL = new List<Brep>();
            List<Plane> P = new List<Plane>();           
            const double pi = 3.14159265358;
            Vector3d[] UV;
            Point3d p;
            //分析曲面中间区间
            DA.GetDataList(0, S);
            DA.GetDataList(1, P);
            foreach(Plane PP in P)
            {
                foreach (Brep B in S)
                {
                    foreach (BrepFace SS in B.Faces)
                    {
                        SS.Transform(Transform.PlaneToPlane(PP, Plane.WorldXY));
                        SS.Evaluate(SS.Domain(0).Mid, SS.Domain(1).Mid, 1, out p, out UV);

                        //U,V,WorldX,WorldY vectors
                        Vector3d U = UV[0];
                        Vector3d V = UV[1];
                        Vector3d X = Plane.WorldXY.XAxis;
                        Vector3d Y = Plane.WorldXY.YAxis;

                        //Unitizing vectors to make tests
                        U.Unitize();
                        V.Unitize();

                        if (Math.Abs(U.X) < Math.Abs(V.X))
                        //判断U和V那个更适合与世界坐标X轴对应，若结果为真，则反转曲面区间
                        {
                            SS.Transpose(true);
                            if (Vector3d.VectorAngle(V, X) > pi / 2)
                            // Flipping new U if angle with X is larger than 90°
                            {
                                SS.Reverse(0, true);
                            }
                            if (Vector3d.VectorAngle(U, Y) > pi / 2)
                            // Flipping new U if angle with X is larger than 90°
                            {
                                SS.Reverse(1, true);
                            }
                        }
                        else
                        {
                            if (Vector3d.VectorAngle(U, X) > pi / 2)
                            {
                                SS.Reverse(0, true);
                            }
                            if (Vector3d.VectorAngle(V, Y) > pi / 2)
                            {
                                SS.Reverse(1, true);
                            }
                        }
                        Brep FinalSurface = SS.DuplicateFace(true);
                        FSL.Add(FinalSurface);
                    }                            
                }
            }
            DA.SetDataList(0, FSL);

            


         
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
                return Properties.Icon.Unify_Planar_UV;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("A082FB5F-BCD7-4D6B-9E32-E15BF8C554EB"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }

    
}