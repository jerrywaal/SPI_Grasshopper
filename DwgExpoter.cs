using System;
using System.Collections.Generic;
using Rhino;
using Grasshopper.Kernel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace SPI
{
    public class ExportLayerToDWG : GH_Component
    {

        public ExportLayerToDWG()
            : base("ExportToDWG", "ETD",
                "Export geometry on a specified layer to a DWG file",
                "SPI", "Drawing")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Layers'Name", "L", "Name of the layer to export", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Run", "R", "Set to true to run the calculation", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool Run = false;
            List<string> Layers = new List<string>();
            if (!DA.GetDataList(0, Layers)) return;
            if (!DA.GetData(1, ref Run)) return;

            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "DWG export canceled.");

            if (Run)
            {
                //默认参数
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "DWG Files (*.dwg)|*.dwg",
                    Title = "保存DWG文件",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    FileName = "output.dwg"
                };
                //根据图层名字搜索
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string FilePath = saveFileDialog.FileName;

                    var doc = RhinoDoc.ActiveDoc;

                    foreach (string layer in Layers)
                    {
                        var objects = doc.Objects.FindByLayer(layer);
                        if (objects != null)
                        {

                            foreach (var obj in objects)
                            {
                                obj.Select(true);
                            }
                        }
                    }
                    string command = "-_Export " + FilePath + " Enter";
                    RhinoApp.RunScript(command,true);

                    //打开CAD并处理
                    ShowMessageForm("切换成英文输入法并等待CAD自动运行和关闭");
                    Process.Start(FilePath);

                    // 等待一段时间，确保AutoCAD完全加载
                    Thread.Sleep(8000);
                    SendKeys.SendWait("_ZOOM\n");
                    SendKeys.SendWait("{A}");
                    SendKeys.SendWait("{ESC}");
                    SendKeys.SendWait("_QSAVE\n");
                    SendKeys.SendWait("_quit\n");

                }
            }
        }

        Form MessageForm = new Form();
        System.Windows.Forms.Timer FormTimer = new System.Windows.Forms.Timer();

        public void ShowMessageForm(string Message)
        {
            MessageForm.Text = "请等待CAD自动打开和关闭";
            //Measure the text to determine its width.
            int TextWidth = TextRenderer.MeasureText(Message, MessageForm.Font).Width;

            //Set the window size based on the text width.
            MessageForm.Size = new System.Drawing.Size(TextWidth + 40, 100);
            MessageForm.StartPosition = FormStartPosition.CenterScreen;

            //Create label.
            Label label = new Label();
            label.Text = Message;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            label.Dock = DockStyle.Fill;
            MessageForm.Controls.Add(label);

            //Set the time.
            FormTimer.Interval = 2500;// 2.5 second
            FormTimer.Tick += (sender, e) => CloseMessageForm();
            FormTimer.Start();

            MessageForm.ShowDialog();
        }

        public void CloseMessageForm()
        {
            FormTimer.Stop();
            MessageForm.Close();
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Icon.cad_logo2;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("D78C7E2A-7F74-4759-A37B-9A130013C1BD"); }
        }
    }
}
