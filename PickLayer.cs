using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using System.Linq;
using Rhino.Geometry;
using Grasshopper;

namespace SPI
{
    public class PickLayer : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent4 class.
        /// </summary>
        public PickLayer()
          : base("PickinLayer", "PL",
              "抓取对应的数据所在的list的全部数据并输出",
              "SPI", "Tool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Data", "Data", "Input data as a list", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Size", "Size", "Size for partitioning the list", GH_ParamAccess.item);
            pManager.AddTextParameter("Layers", "Layers", "Text data for comparison", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("List", "List", "Output tree structure", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> Data = new List<string>();
            int Size = 0;
            List<string> Layers = new List<string>();

            if (!DA.GetDataList(0, Data)) return;
            if (!DA.GetData(1, ref Size)) return;
            if (!DA.GetDataList(2, Layers)) return;

            int Remainder = Data.Count % Size;
            int Padding = Remainder == 0 ? 0 : Size - Remainder;
            for(int i =0;i<Padding;i++)
            {
                Data.Add(string.Empty);
            }

            GH_Structure<IGH_Goo> OutPutTree = new GH_Structure<IGH_Goo>();
            //Partition the data into Lists
            int Index = 0;
            for (int i = 0; i < Data.Count; i += Size)
            {
                List<IGH_Goo> SubList = Data.GetRange(i, Math.Min(Size, Data.Count - 1)).Select(item => GH_Convert.ToGoo(item)).ToList();
                GH_Path Path = new GH_Path(Index);
                OutPutTree.AppendRange(SubList, Path);
                Index++;
            }

            //Find extract lists with mathcing items in layers
            GH_Structure<IGH_Goo> ExtractedTree = new GH_Structure<IGH_Goo>();
            for (int i = 0; i < Layers.Count; i++)
            {
                string Layer = Layers[i];
                for (int j = 0; j < OutPutTree.PathCount; j++)
                {
                    List<IGH_Goo> Sublist = new List<IGH_Goo>((IEnumerable<IGH_Goo>)OutPutTree.get_Branch(j));
                    if (Sublist.Any(item => item.ToString() == Layer))
                    {
                        ExtractedTree.AppendRange(Sublist, new GH_Path(j));
                    }
                }
            }

            DA.SetDataTree(0, ExtractedTree);
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
                return Properties.Icon.Picklayer;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("DD6EA8D7-956C-4443-AAE7-5449ED4843F7"); }
        }
    }
}