using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Misp.Kernel.Task
{
    public class BusyAction : INotifyPropertyChanged
    {

        
        #region Properties

        private bool _isBusy;
        private int _loadingPercentage { get; set; }
        protected readonly BackgroundWorker _worker;

        public string LoadingStep { get; set; }

        public bool IsDeterministic { get; set; }

        public int MaxLoadingPercentage { get; set; }

        public int LoadingPercentage
        {
            get { return _loadingPercentage; }
            private set
            {
                if (_loadingPercentage != value)
                {
                    _loadingPercentage = value;
                    OnPropertyChanged("LoadingPercentage");
                }
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

        #endregion


        public BusyAction() : this(true)
        {
            
        }

        public BusyAction(bool isDeterministic)
        {
            IsDeterministic = isDeterministic;
            _isBusy = false;
            LoadingStep = "";
            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
            _worker.WorkerSupportsCancellation = true;
            _worker.WorkerReportsProgress = true;
        }

        public BusyAction(bool isDeterministic, int maxLoadingPercentage)
            : this(isDeterministic)
        {
            this.MaxLoadingPercentage = maxLoadingPercentage;
        }

        public void Run()
        {
            IsBusy = true;
            _worker.RunWorkerAsync();
        }

        public Func<OperationState> DoWork = () => { return OperationState.CONTINUE; };

        public Func<OperationState> EndWork = () => { return OperationState.CONTINUE; };

        public virtual void ReportProgress(int progress, String text)
        {
            _worker.ReportProgress(progress, text);

        }
        

        #region Asynchronous Loading

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LoadingStep = e.UserState.ToString();
            LoadingPercentage = e.ProgressPercentage;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            DoWork();
        }

        #endregion

        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }


}
