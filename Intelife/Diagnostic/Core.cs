using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Intelife.Diagnostic
{
  internal class Core
  {
    public ProcessEx Process { get; set; }
    public CoreStatus Status { get; set; }

    public event EventHandler Finished;
    public Core(int index, bool enabled)
    {
      this.Status = new CoreStatus(index, enabled);
    }
    public void Run(ProcessEx process, ProcessPriorityClass priority)
    {
      if (this.Status.InUse)
        throw new InvalidOperationException("Current core is running another process");

      if (process == null)
        throw new ArgumentNullException("Process can not be null");

      this.Process = process;
      this.Process.ExecutingCore = this;

      //bind event
      this.Process.Exited += Process_Exited;

      this.Process.Start();
      this.Process.Status = ProcessExecutionStatus.Running;
      this.Process.PriorityClass = priority;
      this.Status.InUse = true;

      //TODO set affinity
      this.Process.ProcessorAffinity = (IntPtr) (int) Math.Pow(2, this.Status.Index);
    }
    void Process_Exited(object sender, EventArgs e)
    {
      this.Status.InUse = false;

      //unbind event
      this.Process.Exited -= this.Process_Exited;
      this.Process.Status = ProcessExecutionStatus.Completed;
      this.Process = null;

      //fire finished event
      if (this.Finished != null)
        this.Finished(this, null);
    }
    internal void Kill()
    {
      if (this.Process == null)
        throw new InvalidOperationException("No process is currently running");

      this.Process.Kill();
      this.Process.Status = ProcessExecutionStatus.Completed;
      this.Process = null;

      this.Status.InUse = false;

      //TODO not sure if killing a process will raise prcessexit event
      if (this.Finished != null)
        this.Finished(this, null);
    }
  }
}
