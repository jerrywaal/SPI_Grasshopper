using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Grasshopper;

namespace SPI
{
    public class ExportToLayersComponent : GH_Component
    {
        public ExportToLayersComponent()
          : base("LayerBaker", "LB",
              "将几何体分别导出到不同图层",
              "SPI", "Tool")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "Geo", "要导出的东西", GH_ParamAccess.list);
            pManager.AddTextParameter("Layers' Name", "L", "子图层名称列表", GH_ParamAccess.list,"默认");
            pManager.AddColourParameter("Colours", "C", "图层颜色", GH_ParamAccess.list,System.Drawing.Color.Black);
            pManager.AddBooleanParameter("Run", "R", "是否运行程序", GH_ParamAccess.item, false);
            //pManager[0].DataMapping = GH_DataMapping.Graft;
            pManager[1].DataMapping = GH_DataMapping.Graft;
            pManager[2].DataMapping = GH_DataMapping.Graft;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Result", "R", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool run = false;
            if (!DA.GetData(3, ref run) || !run)
            {
                return;
            }

            List<GeometryBase> GeometryList = new List<GeometryBase>();
            List<GeometryBase> GhGeoList = new List<GeometryBase>();
            List<string> LayerNames = new List<string>();
            List<Color> LayerColours = new List<Color>();

            if (!DA.GetDataList(0, GeometryList)) return;
            if (!DA.GetDataList(1, LayerNames)) return;
            DA.GetDataList(2, LayerColours);


            var Doc = RhinoDoc.ActiveDoc;
  

            if (Doc == null)
            {
                //DA.SetData(0, "无法获取Rhino文档。");
                return;
            }



            //var bakeutil = new Grasshopper.Kernel.GH_BakeUtility(GhDoc);

            for (int i = 0; i < GeometryList.Count; i++)
            {
                ObjectAttributes Attributes = new ObjectAttributes();
                string layerName = LayerNames[0];
                Color layerColour = LayerColours[0];
                

                if (string.IsNullOrEmpty(layerName))
                {
                    // 如果图层名称为空，则添加到默认图层
                    Attributes.LayerIndex = Doc.Layers.Add("默认",System.Drawing.Color.Black);
                }
                else

                {
                    int layerIndex = Doc.Layers.FindByFullPath(layerName, -1);
                    if (layerIndex == -1)
                    {
                        layerIndex = Doc.Layers.Add(layerName, layerColour);
                    }
                    Attributes.LayerIndex = layerIndex;
                }

                //GhGeoList.Add(GeometryList[i]);
                Rhino.RhinoDoc.ActiveDoc.Objects.Add(GeometryList[i], Attributes);
                //bakeutil.BakeObjects(GhGeoList, Attributes, Doc);
            }

            if(GeometryList.Count != 0)
            {
                DA.SetData(0, true);
            }
            else
            {
                DA.SetData(0, false);
            }

            

            
    }



        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Icon.Autobake;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("808CD828-26C7-4F5E-95D1-54D2DF48F3F4"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}
