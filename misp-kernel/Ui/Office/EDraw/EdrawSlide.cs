using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows;
using EDOfficeLib;
using Misp.Kernel.Domain;
using System.IO;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Excel = Microsoft.Office.Interop.Excel;
using Core = Microsoft.Office.Core;
using OfficeProperties = Microsoft.Office.Core;
using Microsoft.Office.Core;
using Misp.Kernel.Util;
using System.Web.Script.Serialization;
using Microsoft.Office.Interop.PowerPoint;

namespace Misp.Kernel.Ui.Office.EDraw
{
    public partial class EdrawSlide : UserControl
    {

        public struct shapePosition
        {
            public float top;
            public float left;
            public float width;
            public float height;
        }

        #region Events

        public event ChangeShapeEventHandler ShapeAdded;
        public event ChangeShapeEventHandler ShapeUpdated;
        public event ChangeShapeEventHandler ShapeDeleted;
        public event ChangeEventHandler SlideFileChanged;
        public event ChangeEventHandler Imported;
        
        #endregion


        #region Properties

        public bool ThrowEvent = true;

        public static shapePosition shapeposition;
        public static float shapeShiftPosition;

        public static string POWER_POINT_ID = "PowerPoint.Application";
        public static string POWER_POINT_EXT = ".pptx";
        public static String POWERPOINT_FILTER = "Powerpoint files (*.ppt)|*.pptx";

        PowerPoint.Application PowerpointApplication;
        PowerPoint.Presentation PowerpointPresentation;

       

        /// <summary>
        /// Assigne ou retourne l'url du document courant
        /// </summary>
        public string DocumentUrl { get; set; }


        private string documentName;

        /// <summary>
        /// Assigne ou retourne le titre du fichier courant
        /// </summary>
        public string DocumentName
        {
            get
            {
                return this.axEDOffice1.GetDocumentName().Split('.')[0];
            }
            set { documentName = value; }
        }

        /// <summary>
        /// Retourne le composant office
        /// </summary>
        public AxEDOfficeLib.AxEDOffice Office
        {
            get { return this.axEDOffice1; }
        }



        #endregion


        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de EdrawOffice.
        /// </summary>
        public EdrawSlide()
        {
            InitializeComponent();
            this.Office.ClearTempFiles();           
        }

        #endregion


        #region Initializations

        /// <summary>
        /// Initialize handler
        /// </summary>
        protected void initHandlers()
        {
            PowerpointApplication.WindowSelectionChange += OnFileChanged;
            PowerpointApplication.WindowSelectionChange += OnWindowSelectionChanged;
        }
        
        private void OnFileChanged(Selection Sel)
        {
            try
            {
                if (SlideFileChanged != null)
                {
                    SlideFileChanged();
                    PowerpointApplication.WindowSelectionChange -= OnFileChanged;
                }
            }
            catch { }
        }

        
        List<int> selectedShapePositions = new List<int>(0);

        private void OnWindowSelectionChanged(Selection Sel)
        {            
            if (Sel != null)
            {
                try
                {
                    PowerPoint.PpSelectionType type = Sel.Type;                    
                    if (type == PowerPoint.PpSelectionType.ppSelectionShapes)
                    {
                        selectedShapePositions = new List<int>(0);
                        PowerPoint.ShapeRange range = Sel.ShapeRange;
                        if (range == null) return;                        
                        foreach (PowerPoint.Shape shape in range)
                        {
                            selectedShapePositions.Add(shape.Id);
                        }
                    }                    
                    else if (type == PowerPoint.PpSelectionType.ppSelectionNone)
                    {
                        if (selectedShapePositions != null && selectedShapePositions.Count > 0)
                        {
                            PowerPoint._Slide slide = GetActiveSlide();
                            if (slide == null) return;
                            foreach (int position in selectedShapePositions)
                            {
                                PowerPoint.Shape shape = getShape(slide.SlideIndex, position);
                                if (shape != null) continue;
                                if (ShapeDeleted != null) ShapeDeleted(slide.SlideNumber, slide.Name, position, "", SlideItemType.NULL, "");
                            }
                            selectedShapePositions = new List<int>(0);
                        }                        
                    }
                }
                catch
                {
                    
                    //PowerPoint._Slide slide = GetActiveSlide();
                    //if (slide == null) return;
                    //foreach (int position in selectedShapePositions)
                    //{
                    //    if (ShapeDeleted != null) ShapeDeleted(slide.SlideNumber, slide.Name, position, "", SlideItemType.NULL, "");
                    //}
                    selectedShapePositions = new List<int>(0);
                }
            }
            else selectedShapePositions = new List<int>(0);
        }

        #endregion


        #region File Actions

        /// <summary>
        /// Opent the given file.
        /// Create a new file if filePath = null
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public OperationState Open(String filePath = null)
        {
            bool result = false;
            if(String.IsNullOrEmpty(filePath)) result = this.Office.CreateNew(POWER_POINT_ID);
            else if (System.IO.File.Exists(filePath))result = this.Office.Open(filePath, POWER_POINT_ID);
            if (result)
            {
                PowerpointApplication = this.Office.GetApplication() as PowerPoint.Application;
                DocumentUrl = this.Office.GetDocumentFullName();
                int count = PowerpointApplication.Presentations.Count;
                PowerpointPresentation = PowerpointApplication.Presentations[count];
                initHandlers();
                return OperationState.CONTINUE;
            }
            return OperationState.STOP;
        }

        /// <summary>
        /// Close the opened file.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Close()
        {
            if (PowerpointPresentation != null)
            {
                PowerpointPresentation.Close();
                PowerpointPresentation = null;
            }
            this.Office.ExitOfficeApp();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Sauve le fichier ouvert sous un autre nom.
        /// </summary>
        /// <param name="filePath">L'url du nouveau fichier</param>
        /// <param name="overwrite"></param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState SaveAs(String filePath)
        {
            bool result = false;
            try
            {
                if (System.IO.File.Exists(filePath) && DocumentUrl != null && DocumentUrl.Equals(filePath)) result = this.Office.Save();
                else result = this.Office.SaveAs(filePath);
                if (result)
                {
                    DocumentUrl = this.Office.GetDocumentFullName();
                    return OperationState.CONTINUE;
                }
            }
            catch (Exception ex)
            {
                return OperationState.STOP;
            }
            return OperationState.STOP;
        }

        #endregion


        #region Slide Actions

        /// <summary>
        /// Return the slide at the given position
        /// </summary>
        /// <param name="slidePosition"></param>
        /// <returns></returns>
        public PowerPoint._Slide GetSlide(int slidePosition){
            if (PowerpointPresentation == null) return null;            
            int count = PowerpointPresentation.Slides.Count;
            if(slidePosition <= 0 || slidePosition > count) return null;
            return PowerpointPresentation.Slides[slidePosition];
        }

        public PowerPoint._Slide GetActiveSlide(){
            if (PowerpointPresentation == null) return null;            
            int count = PowerpointPresentation.Slides.Count;
            if(count <= 0) return null;
            return PowerpointPresentation.Slides[PowerpointApplication.ActiveWindow.Selection.SlideRange.SlideNumber];
        }

        #endregion


        #region Shape Actions

        /// <summary>
        /// Get the shape at given coodinates
        /// </summary>
        /// <param name="slidePosition"></param>
        /// <param name="shapePosition"></param>
        /// <returns></returns>
        public PowerPoint.Shape getShape(int slidePosition, int shapeId)
        {
            PowerPoint._Slide slide = GetSlide(slidePosition);
            if (slide == null) return null;
            int count = PowerpointPresentation.Slides.Count;
            if (count <= 0) return null;
            foreach (PowerPoint.Shape shape in slide.Shapes)
            {
                if (shape.Id == shapeId) return shape;
            }
            return null;
        }

        public PowerPoint.Shape GetActiveShape() 
        {
            PowerPoint._Slide slide = GetActiveSlide();
            if (slide == null) return null;
            if (slide.Shapes.Count < 1) return null;
            PowerPoint.Shape shape = null;
            try
            {
                shape = PowerpointApplication.ActiveWindow.Selection.ShapeRange[1];
            }
            catch 
            {
                shape = null;
            }            
            return shape;
        }

        public void CloseShapeWorkbook(PowerPoint.Shape shape = null)
        {
            if (shape == null) shape = GetActiveShape();
            if (shape == null || shape.Type != MsoShapeType.msoEmbeddedOLEObject || !shape.OLEFormat.ProgID.StartsWith("Excel.Sheet.")) return;
            Excel.Workbook workbook = (Excel.Workbook)shape.OLEFormat.Object;
            if (workbook == null) return;
            workbook.Close();
        }

        public void AddOrUpdateTextShape(SlideItemType type, String text, int loopOid, PowerPoint.Shape shape = null){
            PowerPoint.Slide activeSlide = (PowerPoint.Slide)GetActiveSlide();
            if(activeSlide == null) return;
            if(shape == null) shape = GetActiveShape();
            String value = type == SlideItemType.INCREMENTAL ? text : "" + loopOid;
            bool added = false;
            if (shape == null)
            {
                shapeShiftPosition += 5F;
                float left = 15F + shapeShiftPosition + 2F;
                float top = 25F + shapeShiftPosition + 5F;
                shape = activeSlide.Shapes.AddShape(MsoAutoShapeType.msoShapeRectangle, left, top, 150F, 50F);
                added = true;
            }
            try
            {
                PowerPoint.TextRange range = shape.TextFrame.TextRange;
                range.Text = text;
                if (added && ShapeAdded != null) ShapeAdded(activeSlide.SlideNumber, activeSlide.Name, shape.Id, value, type, text);
                if (!added && ShapeUpdated != null) ShapeUpdated(activeSlide.SlideNumber, activeSlide.Name, shape.Id, value, type, text);
            }
            catch (Exception)  {
                shapeShiftPosition += 5F;
                float left = 15F + shapeShiftPosition + 2F;
                float top = 25F + shapeShiftPosition + 5F;
                shape = activeSlide.Shapes.AddShape(MsoAutoShapeType.msoShapeRectangle, left, top, 150F, 50F);
                if (ShapeAdded != null) ShapeAdded(activeSlide.SlideNumber, activeSlide.Name, shape.Id, value, type, text);
            }
        }

        public void AddOrUpdateExcelShape(String excelFilePath, int reportOid, PowerPoint.Shape shape = null){
            PowerPoint.Slide activeSlide = (PowerPoint.Slide)GetActiveSlide();
            if(activeSlide == null) return;
            if(shape == null) shape = GetActiveShape();
            if (shape == null || shape.Type != MsoShapeType.msoEmbeddedOLEObject || !shape.OLEFormat.ProgID.StartsWith("Excel.Sheet."))
            {
                try
                {
                    shape = activeSlide.Shapes.AddOLEObject(100, 100, -1f, -1f,
                        "", excelFilePath, Core.MsoTriState.msoFalse, "", 0,
                        "", Core.MsoTriState.msoFalse);
                    shape.Name = "" + reportOid;
                    if (ShapeAdded != null) ShapeAdded(activeSlide.SlideNumber, activeSlide.Name, shape.Id, "" + reportOid, SlideItemType.REPORT, "");
                }
                catch (Exception ex) 
                {
                    MessageDisplayer.DisplayError("Insert Report", "Problem while adding report");
                }
                
            }            
            else{                
                float left = shape.Left;
                float top = shape.Top;
                float width = -1f;
                float height = -1f;
                String name = shape.Name;
                int position = shape.Id;
                                
                shape.Delete();
                if (ShapeDeleted != null) ShapeDeleted(activeSlide.SlideNumber, name, position, "" + reportOid, SlideItemType.REPORT,"");
                
                shape = activeSlide.Shapes.AddOLEObject(left, top, width, height,
                    "", excelFilePath, Core.MsoTriState.msoFalse, "", 0,
                    "", Core.MsoTriState.msoFalse);
                shape.Name = "" + reportOid;
                if (ShapeAdded != null) ShapeAdded(activeSlide.SlideNumber, activeSlide.Name, shape.Id, "" + reportOid, SlideItemType.REPORT, "");
            
            }
        }

        #endregion


        #region Import / Export

        /// <summary>
        /// Ouvre le dialogue permettant de choisir le document à importer.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Import()
        {
            bool result = this.Office.OpenFileDialog(POWERPOINT_FILTER);
            if (result)
            {
                this.DocumentUrl = this.Office.GetDocumentFullName();
                this.DocumentName = this.Office.GetDocumentName();
                if (Imported != null)
                {
                    Imported();
                    if (result)
                    {
                        PowerpointApplication = this.Office.GetApplication() as PowerPoint.Application;
                        DocumentUrl = this.Office.GetDocumentFullName();
                        int count = PowerpointApplication.Presentations.Count;
                        PowerpointPresentation = PowerpointApplication.Presentations[count];
                        initHandlers();
                        return OperationState.CONTINUE;
                    }
                }
                return OperationState.CONTINUE;
            }
            return OperationState.STOP;
        }

        /// <summary>
        /// Sauve le fichier ouvert sous un autre nom.
        /// </summary>
        /// <param name="filePath">L'url du nouveau fichier</param>
        /// <param name="overwrite"></param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Export(string filePath)
        {
            bool result = false;
            try
            {
                result = this.Office.SaveAs(filePath);
                if (result) return OperationState.CONTINUE;
            }
            catch (Exception ex)
            {
                return OperationState.STOP;
            }
            return OperationState.STOP;
        }

        #endregion

        public void ChangeTitleBarCaption(string newName)
        {
            //if (PowerpointPresentation != null) PowerpointPresentation.Application.ActiveWindow.Caption = newName;
        }

        public void RemoveTempFiles()
        {
            this.Office.ClearTempFiles();
        }
    }
}
