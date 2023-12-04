using System;
using System.Collections.Generic;
using System.IO;

using Grasshopper.Kernel;
using Rhino.Geometry;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace SPI
{
    public class ExcelReader : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ExcelReader()
          : base("Excel Reader", "ER",
              "Read Excel",
              "SPI", "Tool")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("File Path", "Path", "Excel file path.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "R", "", GH_ParamAccess.item,false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Cell Values", "Values", "Values from the Excel file", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string FilePath = string.Empty;
            bool Run = false;
            if (!DA.GetData(0, ref FilePath))
            {
                return;
            }  

            if(!DA.GetData(1, ref Run)||!Run)
            {
                return;
            }

            try
            {
                List<string> CellValue = ReadExcel(FilePath);
                DA.SetDataList(0, CellValue);
            }
            catch(Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message);
            }


        }

        public List<string> ReadExcel(string FilePath)
        {
            List<string> CellValue = new List<string>();

            using (FileStream ExcelFileStream = new FileStream(FilePath,FileMode.Open,FileAccess.Read))
            {
                IWorkbook WorkBook;

                //For reading xls file.
                if (Path.GetExtension(FilePath).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                    WorkBook = new HSSFWorkbook(ExcelFileStream);
                //For reading xlsx file.
                else if (Path.GetExtension(FilePath).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    WorkBook = new XSSFWorkbook(ExcelFileStream);
                else
                    throw new Exception("Unsupported File Format.");

                // Get the first sheet (index 0)
                ISheet Sheet = WorkBook.GetSheetAt(0);

                for(int RowIndex = 0; RowIndex<= Sheet.LastRowNum;RowIndex++)
                {
                    IRow Row = Sheet.GetRow(RowIndex);
                    if(Row != null)
                    {
                        for(int CellIndex =0;CellIndex<Row.LastCellNum;CellIndex++)
                        {
                            ICell Cell = Row.GetCell(CellIndex);
                            if(Cell != null)
                            {
                                CellValue.Add(Cell.ToString());
                            }
                        }
                    }
                }
            }

            return CellValue;
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
                return Properties.Icon.ExcelReader;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("EFF62FEC-F4A5-4595-9A22-40BCEFF2A338"); }
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}