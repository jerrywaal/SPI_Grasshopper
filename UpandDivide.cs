using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPI
{
    public class UpAndDivide : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public UpAndDivide()
          : base("Up and Divide", "UaD",
              "Move curve on Z axis and divide it",
              "SPI", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve to Move", GH_ParamAccess.list);
            pManager.AddNumberParameter("Height", "H", "Height for Move", GH_ParamAccess.item);
            pManager.AddBooleanParameter("DivideType", "DT", "0-divide with distance 1-divide with number", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Distance", "Dis", "Divide Distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("CountNumber", "CN", "Divide Curve with specific count", GH_ParamAccess.item, 1);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("TopPoints", "TopPt", "Points in top curve", GH_ParamAccess.tree);
            pManager.AddPointParameter("BotPoints", "BotPt", "Points in bottom curve", GH_ParamAccess.tree);
            pManager.AddCurveParameter("TopCurve", "TopCrv", "Curve on top", GH_ParamAccess.list);
            pManager.AddCurveParameter("BotCurve", "BotCrv", "Curve in bottom", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //基础数据初始化
            double Height = 0;
            double Distance = 0;
            double CountNumber = 1;
            bool DivideType = false;
            List<Curve> OC = new List<Curve>();
            List<Curve> TopCurve = new List<Curve>();
            List<Point3d> TopPtP = new List<Point3d>();
            List<Point3d> DivPtBotList = new List<Point3d>();

            DA.GetDataList(0, OC);
            DA.GetData(1, ref Height);
            DA.GetData(2, ref DivideType);
            DA.GetData(3, ref Distance);
            DA.GetData(4, ref CountNumber);


            //分栏杆点
            foreach (Curve OriginCurve in OC)
            {
                double Length = OriginCurve.GetLength();
                int Count = 0;
                if (DivideType == false)
                {
                    Count = Convert.ToInt32(Length) / Convert.ToInt32(Distance);
                }

                else if (DivideType == true)
                {
                    Count = Convert.ToInt32(CountNumber);
                }
                
                OriginCurve.DivideByCount(Count, true, out Point3d[] DivPt);
                DivPtBotList = DivPt.ToList();
            }

            
            //Point3d CurveMidPoint = OriginCurve.PointAtNormalizedLength(0.5);
            //Point3d ExtrudePoint = new Point3d(CurveMidPoint.X, CurveMidPoint.Y, CurveMidPoint.Z + Height);
            //PlanePt1.Add(CurveMidPoint);
            //PlanePt1.Add(ExtrudePoint);
            //Vector3d PlaneVt1 = ExtrudePoint - CurveMidPoint;
            //Vector3d PlaneVt2 = CurveMidPoint - CurveEndPoint;
            //Plane HandRailTopLineOffsetPlane = new Plane(CurveMidPoint, PlaneVt2, PlaneVt1);

            //树形数据处理
            DataTree<Point3d> DivPtTopTree = new DataTree<Point3d>();
            DataTree<Point3d> DivPtBotTree = new DataTree<Point3d>();

            foreach (Point3d P in DivPtBotList)
            {
                Point3d DPT = new Point3d(P.X, P.Y, P.Z + Height);
                TopPtP.Add(DPT);
                DivPtTopTree.Add(DPT);
            }

            DivPtBotTree.AddRange(DivPtBotList);
            DivPtBotTree.Graft(true);
            DivPtBotTree.SimplifyPaths();
            DivPtTopTree.Graft(true);
            DivPtTopTree.SimplifyPaths();

            //生成扶手线

            foreach (Curve OriginCurve in OC)
            {
                Curve[] HandRailTopLine = OriginCurve.Offset(Plane.WorldZX,Math.Abs(Height), 0.01, CurveOffsetCornerStyle.Sharp);
                TopCurve = HandRailTopLine.ToList();
            }



            //结果生成
            DA.SetDataTree(0, DivPtTopTree);
            DA.SetDataTree(1, DivPtBotTree);
            DA.SetDataList(2, TopCurve);
            DA.SetDataList(3, OC);

            //TopPt = DivPtTopTree;
            //BotPt = DivPtBotTree;
            //TopCurve = HandRailTopLine;
            //BotCurve = OriginCurve;
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
                return Properties.Icon.UpAndDivide;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f7b3ddd0-7151-4521-9573-5334b405d042"); }
        }
    }
}