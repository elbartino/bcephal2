using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Sourcing.Table
{
    class InputTableRunProcess
    {

        protected DialogAllocationRun allocationRunDialog;
        protected System.Windows.Threading.DispatcherTimer runTimer;
        public InputTableService Service { get; set; }


        public void RunTable(int oid)
        {
            CreateAndShowDialog();
            AllocationRunInfo runInfo = Service.RunAll(oid);
            allocationRunDialog.RunInfo = runInfo;
            CreateAndStartDispatcherTimer();
        }

        protected virtual void OnTimerTick(object sender, EventArgs e)
        {
            AllocationRunInfo runInfo = Service.GetRunInfo(allocationRunDialog.RunInfo.currentPage);
            allocationRunDialog.UpdateGrid(runInfo);
            if (runInfo == null || runInfo.runEnded == true)
            {
                runTimer.Stop();
                allocationRunDialog.CloseButton.IsEnabled = true;
                ApplicationManager.Instance.AllocationCount = this.Service.FileService.GetAllocationCount();
                Service.FileService.SaveCurrentFile();
            }
        }

        private void CreateAndShowDialog()
        {
            allocationRunDialog = new DialogAllocationRun();
            allocationRunDialog.CloseButton.IsEnabled = false;
            allocationRunDialog.Show();
        }

        private void CreateAndStartDispatcherTimer()
        {
            runTimer = new System.Windows.Threading.DispatcherTimer();
            runTimer.Tick += new EventHandler(OnTimerTick);
            runTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            runTimer.Start();
        }

    }
}
