using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Excel = Microsoft.Office.Interop.Excel;
using Misp.Kernel.Util;
using System.Diagnostics;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using System.IO;

namespace Misp.Kernel.Ui.Office
{
    public class PowerpointLoader
    {
        public static Dictionary<String, PowerpointLoader> PRESENTATIONS = new Dictionary<String, PowerpointLoader>(0);

        public static event TransformationTreeRunInfoEventHandler RunHandler;
        public static Kernel.Service.FileTransferService FileTransfertService { get; set; }
        public static int LoopCount { get; set; }

        public String TemplateFilePath { get; set; }
        public String FilePath { get; set; }
        public PowerPoint.Application PowerPointApplication { get; set; }
        public PowerPoint.Presentation PowerPointPresentation { get; set; }

        private Excel.Workbook workbook;
        private Excel.Worksheet worksheet;

        public static bool stop = false;

        public PowerpointLoader() 
        {
            if (PowerPointApplication == null) PowerPointApplication = new PowerPoint.Application();
        }

        public bool isVisible() 
        {
            return PowerPointApplication.Visible == MsoTriState.msoTrue;
        }

        public PowerpointLoader(String FilePath)
        {
            this.FilePath = FilePath;
            MsoTriState ofalse = MsoTriState.msoFalse;
            //MsoTriState otrue = MsoTriState.msoTrue;
            if (PowerPointApplication == null) PowerPointApplication = new PowerPoint.Application();
            PowerPointApplication.Presentations.Open(@FilePath, ofalse, ofalse, ofalse);
            int currrent =  PowerPointApplication.Presentations.Count;
            PowerPointPresentation = PowerPointApplication.Presentations[currrent];
        }

        public void setTextData(int sileIndex, int shapeIndex, String data)
        {
            PowerPoint.Shape shape = getShape(sileIndex, shapeIndex);
            if (shape == null || shape.TextFrame == null || shape.TextFrame.TextRange == null) return;
            shape.TextFrame.TextRange.Text = data;
        }

        public void setExcelData(int slideIndex, int shapeIndex, string sheetName,int row ,int column ,Object data)
        {
            PowerPoint.Shape shape = getShape(slideIndex, shapeIndex);
            if (workbook == null) return;
            Excel.Worksheet worksheet = null;
            
            foreach (Object sheetItem in workbook.Sheets)
            {
                try
                {
                    Excel.Worksheet sheet = (Excel.Worksheet)sheetItem;
                    if (sheet.Name == sheetName)
                    {
                        worksheet = sheet;
                        break;
                    }
                }
                catch (Exception) {}
            }
            
            if (worksheet == null) return;
            Excel.Range xlRange = worksheet.Cells[row, column];
            Excel.Range cell = xlRange.Cells[1];
            cell.Value = data;           
        }

        public void startExcelEdition(int slideIndex, int shapeIndex)
        {
            PowerPoint.Shape shape = getShape(slideIndex, shapeIndex);
            if (shape == null || shape.Type != MsoShapeType.msoEmbeddedOLEObject || !shape.OLEFormat.ProgID.StartsWith("Excel.Sheet.")) return;
            try
            {
                workbook = null;
                worksheet = null;
                workbook = (Excel.Workbook)shape.OLEFormat.Object;
                if (workbook == null) worksheet = null;
            }
            catch (Exception) 
            {
                worksheet = null;
            }
        }

        public void endExcelEdition()
        {
            if (workbook != null)
            {
                if (worksheet != null) worksheet.Activate();
                workbook.Save();
            }
            worksheet = null;
            workbook = null;
        }

        public void copyFile(String destPath, String name) 
        {
            if (FileTransfertService == null) return;
            FileTransfertService.downloadPresentationTemplate(destPath, name);
        }

        public void duplicateSlide(int sileIndex)
        {
            PowerPoint.Slide slide = getSilde(sileIndex);
            if (slide == null) return;
            slide.Duplicate();        
        }

        public void deleteSlide(int sileIndex)
        {
            PowerPoint.Slide slide = getSilde(sileIndex);
            if (slide == null) return;
            slide.Delete();
        }

        public PowerPoint.Slide getSilde(int sileIndex)
        {
            if (PowerPointPresentation == null) return null;
            if (PowerPointPresentation.Slides.Count < sileIndex) return null;
            if (sileIndex < 0) return null;
            return PowerPointPresentation.Slides[sileIndex];
        }

        public PowerPoint.Shape getShape(int sileIndex, int shapeId)
        {
            PowerPoint.Slide slide = getSilde(sileIndex);
            if (slide == null) return null;
            if (slide.Shapes.Count <= 0) return null;
            foreach (PowerPoint.Shape shape in slide.Shapes)
            {
                if (shape.Id == shapeId) return shape;
            }
            return null;
        }

        public void saveAndClose()
        {
            save();
            close();
        }

        public void save()
        {
            if (PowerPointPresentation == null) return;
            PowerPointPresentation.Save();
        }

        public void close()
        {            
            if (PowerPointPresentation != null) PowerPointPresentation.Close();
            PowerPointPresentation = null;
            if (PowerPointApplication == null) return;
            if (PowerPointApplication.Presentations.Count == 0) PowerPointApplication.Quit();
            PowerPointApplication = null;
        }
        
        protected void loadInfo(PowerpointLoadInfo info)
        {
            
            if (info == null || string.IsNullOrEmpty(info.action)) return;

            //if(info.action.Equals(PowerpointLoadInfoActions.COPY))
            //{
            //    copyFile(info.destPath, info.name);
            //}
            if (info.action.Equals(PowerpointLoadInfoActions.DUPLICATE_SLIDE))
            {
                duplicateSlide(info.slideIndex + 1);
            }
            if (info.action.Equals(PowerpointLoadInfoActions.DELETE_SLIDE))
            {
                deleteSlide(info.slideIndex + 1);
            }
            else if (info.action.Equals(PowerpointLoadInfoActions.DISPLAY_TEXT))
            {
                setTextData(info.slideIndex + 1, info.shapeIndex + 1, info.text);
            }
            else if (info.action.Equals(PowerpointLoadInfoActions.DISPLAY_EXCEL_CELL))
            {
                setExcelData(info.slideIndex + 1, info.shapeIndex + 1, info.sheetName, info.row, info.col, info.value);
            }            
            else if (info.action.Equals(PowerpointLoadInfoActions.START_EXCEL_EDITION)) startExcelEdition(info.slideIndex + 1, info.shapeIndex + 1);
            else if (info.action.Equals(PowerpointLoadInfoActions.END_EXCEL_EDITION)) endExcelEdition();
            else if (info.action.Equals(PowerpointLoadInfoActions.SAVE_AND_CLOSE)) saveAndClose();
            else if (info.action.Equals(PowerpointLoadInfoActions.SAVE)) save();
            else if (info.action.Equals(PowerpointLoadInfoActions.CLOSE)) close();
        }

        public static void Stop()
        {
            stop = true;
            PowerpointLoader.PRESENTATIONS.Clear();
        }
        
        public static void Load(PowerpointLoadInfo info)
        {
            if (stop) return;
           
            bool condition1 = (info == null || string.IsNullOrEmpty(info.destPath));
            bool condition2 = (info == null || string.IsNullOrEmpty(info.filePath));
                        
            if (condition1 && condition2) return;

            if (info.items != null && info.items.Count > 0)
            {
                int runed = 0;
                foreach (PowerpointLoadInfo item in info.items)
                {
                    if (stop)
                    {
                        sendProgress(++runed, item);
                        return;
                    }
                    Load(item);
                    sendProgress(++runed, item);
                }
                return;
            }
            if (info.action.Equals(PowerpointLoadInfoActions.SLIDE_SHOW))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => ShowPresentationToUser(info), System.Windows.Threading.DispatcherPriority.Background);
                
                return;
            }
            PowerpointLoader loader;
            //if (!PRESENTATIONS.TryGetValue(info.filePath, out loader))
            //{
            //    loader = new PowerpointLoader(info.filePath);
            //    PRESENTATIONS.Add(info.filePath, loader);
            //}
            string path = info.destPath != null ? info.destPath : info.filePath;
            if (!PRESENTATIONS.TryGetValue(path, out loader))
            {
                string filePath = path;
                bool isCopyAction = info.action.Equals(PowerpointLoadInfoActions.COPY);
                if (isCopyAction)
                {
                    String dir = path;                    
                    String name = Path.GetFileNameWithoutExtension(info.name);
                    String ext = Path.GetExtension(info.name);
                                        
                    if (!Directory.Exists(path))
                    {
                        try
                        {
                            String root = Directory.GetDirectoryRoot(path);
                            if (Directory.Exists(root)) Directory.CreateDirectory(path);
                            else dir = Presentation.defaultSavingFolder;
                        }
                        catch
                        {
                            dir = Presentation.defaultSavingFolder;
                        }
                    }

                    filePath = dir + info.name;
                    int i = 0;
                    while (System.IO.File.Exists(filePath))
                    {
                        filePath = dir + name + ++i + ext;
                    }
                    loader = new PowerpointLoader();
                    loader.copyFile(filePath, info.name);

                    path = path + info.name;
                }
                
                loader = new PowerpointLoader(filePath);
                PRESENTATIONS.Add(path, loader);
            }
            if (stop) return;
            loader.loadInfo(info);
        }

        public static void ShowPresentationToUser(PowerpointLoadInfo info)
        {
            //if (!System.IO.File.Exists(info.filePath)) return;
            PowerpointLoader loader;
            if (PRESENTATIONS.TryGetValue(info.filePath, out loader)) loader.saveAndClose();

            MsoTriState ofalse = MsoTriState.msoFalse;
            MsoTriState otrue = MsoTriState.msoTrue;
            PowerPoint.Application PowerPointApplication = new PowerPoint.Application();
            PowerPointApplication.Presentations.Open(@loader.FilePath, otrue, otrue, ofalse);

            int currrent = PowerPointApplication.Presentations.Count;
            PowerPoint.Presentation Presentation = PowerPointApplication.Presentations[currrent];
            
            int avanceTime = 2;
            int countSlide = Presentation.Slides.Count;
            int[] SlideIdx = new int[countSlide];
            for (int i = 0; i < countSlide; i++) SlideIdx[i] = i + 1;
            PowerPoint.SlideRange range = Presentation.Slides.Range(SlideIdx);
            range.SlideShowTransition.AdvanceOnTime = MsoTriState.msoTrue;
            range.SlideShowTransition.AdvanceTime = avanceTime;
            range.SlideShowTransition.EntryEffect = PowerPoint.PpEntryEffect.ppEffectBoxOut;
                    
            Presentation.SlideShowSettings.StartingSlide = 1;
            Presentation.SlideShowSettings.EndingSlide = countSlide;
            //Presentation.Save();
            Presentation.SlideShowSettings.Run();

            //long wait = (countSlide * avanceTime) * 1000;

            //Wait for the slide show to end.
            //while (PowerPointApplication.SlideShowWindows.Count >= 1)
            //    System.Threading.Thread.Sleep(1000);

            //if (Presentation != null) Presentation.Close();
            if (PowerPointApplication == null) return;
            if (PowerPointApplication.Presentations.Count == 0) PowerPointApplication.Quit();
        }

        public static void OpenPresentationToUser(PowerpointLoadInfo info)
        {
            if (!System.IO.File.Exists(info.filePath)) return;
            PowerpointLoader loader;
            if (PRESENTATIONS.TryGetValue(info.filePath, out loader)) loader.saveAndClose();

            MsoTriState ofalse = MsoTriState.msoFalse;
            MsoTriState otrue = MsoTriState.msoTrue;
            PowerPoint.Application PowerPointApplication = new PowerPoint.Application();
            PowerPointApplication.Presentations.Open(@info.filePath, ofalse, ofalse, otrue);

            int currrent = PowerPointApplication.Presentations.Count;
            PowerPoint.Presentation Presentation = PowerPointApplication.Presentations[currrent];

            //if (Presentation != null) Presentation.Close();
            if (PowerPointApplication == null) return;
            if (PowerPointApplication.Presentations.Count == 0) PowerPointApplication.Quit();
        }

        public static Process GetRunningPowerPointProcess()
        {
            Process[] prozesse = Process.GetProcessesByName("POWERPNT");
            if (prozesse.Length > 0) return prozesse[0];
            return null;
        }

        public static void KillProcess(Process process)
        {            
            if (process != null && !process.HasExited) process.Kill();
        }

        protected static void sendProgress(int runed, PowerpointLoadInfo item)
        {
            String text = "Presentation : " + System.IO.Path.GetFileNameWithoutExtension(item.filePath) + " - Slide " + (item.slideIndex + 1) + " "; 
            TransformationTreeRunInfo runInfo = new TransformationTreeRunInfo();
            runInfo.errorMessage = text;
            runInfo.runedCount = stop ? LoopCount : runed;
            runInfo.totalCount = LoopCount;
            runInfo.runEnded = runInfo.runedCount == runInfo.totalCount;
            if (RunHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => RunHandler(runInfo), System.Windows.Threading.DispatcherPriority.Background);
        }

    }
}
