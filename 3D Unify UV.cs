using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace SPI
{
    public class Unify3d : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Unify3d()
          : base("Unify 3D Surface", "U3S",
              "Unify surfaces'UV which are in different planes",
              "SPI", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "Breps to unify UV", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("GuideSurface", "GS", "GuideSurface for brep. Surface should bound the brep", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Output surface", GH_ParamAccess.list);
            pManager.AddPointParameter("CenterPoints", "CPT", "Surface center points", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //初始化数据
            List<Brep> Breps = new List<Brep>();
            List<Surface> GuideSurface = new List<Surface>();
            List<Point3d> CenterPtL = new List<Point3d>();
            /*List<Point3d[]> SurfaceUVs = new List<Point3d[]>();
            List<Vector3d> SurfaceUs = new List<Vector3d>();
            List<Vector3d> SurfaceVs = new List<Vector3d>();*/
            List<Surface> Surfaces = new List<Surface>();
            double iTol = 0.5;

         /*   List<Vector3d> TSUs = new List<Vector3d>();
            List<Vector3d> TSVs = new List<Vector3d>();*/

            DA.GetDataList(0, Breps);
            DA.GetDataList(1, GuideSurface);

            // 曲面对比角度设置
            double angle180Max = Math.PI + iTol;
            double angle180Min = Math.PI - iTol;
            double angle0Max = 0 + iTol;
            double angle0Min = 0 - iTol;

            for (int i = 0; i < Breps.Count; i++)
            {
                for (int j = 0; j < Breps[i].Faces.Count; j++)
                {
                    //抽取第一个面
                    Brep brepSrf = Breps[i].Faces.ExtractFace(j);
                    Surface Srf = brepSrf.Surfaces.First();

                    //获得面UV
                    Point3d CenterPt = new Point3d();
                    Vector3d[] SrfUV;
                    Srf.Evaluate(Srf.Domain(0).Mid, Srf.Domain(1).Mid, 1, out CenterPt, out SrfUV);
                    Vector3d SrfU = SrfUV[0];
                    Vector3d SrfV = SrfUV[1];

                    //和输入参考曲面对比
                    double testUParam;
                    double testVParam;
                    Vector3d[] TestUV;
                    Point3d TestPt = new Point3d();
                    GuideSurface[i].ClosestPoint(CenterPt, out testUParam, out testVParam);
                    GuideSurface[i].Evaluate(testUParam, testVParam, 1,out TestPt, out TestUV);

                    //得到要对比的面UV
                    Vector3d TestU = TestUV[0];
                    Vector3d TestV = TestUV[1];


                    //对比要反转的向量
                    //如果不是0或者180度，那就要反转
                    //这样保证输入面和参考面能匹配
                    double VectorAngleT = Vector3d.VectorAngle(SrfU, TestU);//要对比的翻转角度
                    if((angle0Max<=VectorAngleT||VectorAngleT<=angle0Min)&&(angle180Max<=VectorAngleT||VectorAngleT <=angle180Min))
                    {
                        Srf.Transpose(true);
                    }

                    //从翻转后的曲面得到新的UV
                    Point3d newCenterPt = new Point3d();
                    Vector3d[] newSrfUV;
                    Srf.Evaluate(Srf.Domain(0).Mid, Srf.Domain(1).Mid, 1, out newCenterPt, out newSrfUV);
                    Vector3d newSrfU = newSrfUV[0];
                    Vector3d newSrfV = newSrfUV[1];

                    //测试新的UV方向是否正确
                    //如果比0小，那就要翻转方向
                    double VecAngleU = Vector3d.VectorAngle(newSrfU,TestU);
                    double VecAngleV = Vector3d.VectorAngle(newSrfV, TestV);

                    if(angle0Max<=VecAngleU||VecAngleU<=angle0Min)
                    {
                        Srf.Reverse(0, true);
                    }
                    if(angle0Max<=VecAngleV||VecAngleV<=angle0Min)
                    {
                        Srf.Reverse(1, true);
                    }

                    CenterPtL.Add(CenterPt);
                    Surfaces.Add(Srf);
                }
            }
            DA.SetDataList(0, Surfaces);
            DA.SetDataList(1, CenterPtL);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Icon._3D_Unify_Surface;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("704304F2-2C01-4352-B42B-2926521E8DED"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}