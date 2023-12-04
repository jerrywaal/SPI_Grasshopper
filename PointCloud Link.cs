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
    public class PointCloudLink : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PointCloudLink()
          : base("PointCloud Link", "PCK",
              "Find the points'shortest link between different point cloud.无序不同组三维点云一一对应。",
              "SPI", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("OriginPointCloud", "OPC", "The point cloud start from", GH_ParamAccess.list);
            pManager.AddPointParameter("Point Cloud", "PC", "The target point cloud", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "link Curve", GH_ParamAccess.item);
            //pManager.AddIntegerParameter("Index", "I", "Index of target point", GH_ParamAccess.item);
            pManager.AddPointParameter("End Points", "EP", "Reorder end point cloud", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //数据初始化
            Point3dList PCL1 = new Point3dList();
            Point3dList PCL2 = new Point3dList();
            List<Point3d> OPC = new List<Point3d>();
            List<Point3d> PC = new List<Point3d>();
            List<Point3d> EPC = new List<Point3d>();
            //List<int> EPI = new List<int>();
            
            List<Polyline> PCC = new List<Polyline>();
            DA.GetDataList(0, OPC);
            DA.GetDataList(1, PC);
            DataTree<Point3d> Tree1 = new DataTree<Point3d>();
            DataTree<Point3d> Tree2 = new DataTree<Point3d>();


            foreach (Point3d P1 in OPC)
            {
                Tree1.Add(P1);
                PCL1.Add(P1);
            }

            foreach (Point3d P2 in PC)
            {
                PCL2.Add(P2);
                Tree2.Add(P2);
            }

            //找最近点连线并剔除出树形数据里
            foreach (GH_Path path in Tree1.Paths)
            {
                if (Tree1.DataCount < Tree2.DataCount)
                {
                    for (int i = 0; i < Tree1.DataCount; i++)
                    {
                        Point3d PCP = Point3dList.ClosestPointInList(PCL2, Tree1[path, i]);
                        int PCPI = Point3dList.ClosestIndexInList(PCL2, Tree1[path, i]);
                        List<Point3d> FP = new List<Point3d> { OPC[i], PCL2[PCPI] };
                        Polyline FC = new Polyline(FP);
                        PCC.Add(FC);
                        EPC.Add(PCP);
                        // EPI.Add(PCPI);
                        PCL2.RemoveAt(PCPI);
                    }
                }
                else
                {
                    for (int i = 0; i < Tree2.DataCount; i++)
                    {
                        Point3d PCP = Point3dList.ClosestPointInList(PCL1, Tree2[path, i]);
                        int PCPI = Point3dList.ClosestIndexInList(PCL1, Tree2[path, i]);
                        List<Point3d> FP = new List<Point3d> { PC[i], PCL1[PCPI] };
                        Polyline FC = new Polyline(FP);
                        PCC.Add(FC);
                        EPC.Add(PCP);
                        // EPI.Add(PCPI);
                        PCL1.RemoveAt(PCPI);
                    }
                }
            }



            DA.SetDataList(0, PCC);
            DA.SetDataList(1, EPC);



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Icon.PointCloudLink;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D07F5508-696F-4AA4-BEC7-3EB880075C30"); }
        }
    }
}