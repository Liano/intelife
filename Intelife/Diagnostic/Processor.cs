using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Intelife.Diagnostic
{
  internal delegate void ProcessorSlotFreedHandler(int freeSlots, ProcessEx finishedProcess);
  internal class Processor
  {
    int _maxCoresToUse;
    List<Core> _cores;
    static Processor _instance;
    public int MaxUsableCores { 
      get { return this._maxCoresToUse; } 
      set { this.SetMaxCoresToUse(value); } 
    }
    public static Processor O
    {
      get
      {
        if (_instance == null)
        {
          _instance = new Processor();
        }

        return _instance;
      }
    }
    public event ProcessorSlotFreedHandler SlotFreed;
    private Processor()
    {
      this._maxCoresToUse = Environment.ProcessorCount;
      this._cores = new List<Core>(this._maxCoresToUse);
      this.InitCores();
    }
    private void SetMaxCoresToUse(int count)
    {
      if (count <= 0)
        throw new ArgumentException("Cores count can not be 0 or negative");

      this._maxCoresToUse = count > Environment.ProcessorCount ? Environment.ProcessorCount : count;

      this.UpdateWorkingCores(count);
    }
    private void InitCores()
    {
      for (int i = 0; i < this._maxCoresToUse; i++)
      {
        var newCore = new Core(i, true);
        newCore.Finished += newCore_Finished;
        this._cores.Add(newCore);
      }
    }
    private void newCore_Finished(object sender, EventArgs e)
    {
      var core = sender as Core;
      if (this.SlotFreed != null)
        this.SlotFreed(this.GetFreeSlotCount(), core.Process);
      
    }
    private int GetFreeSlotCount()
    {
      var count = 0;
      foreach (var core in this._cores)
      {
        if (core.Status.Enabled && !core.Status.InUse)
          count++;
      }

      return count;
    }
    private void UpdateWorkingCores(int count)
    {
      for (int i = 0; i < this._cores.Count; i++)
      {
        if (i >= count)
          this._cores[i].Status.Enabled = false;
        else
          this._cores[i].Status.Enabled = true;
      }
    }
    public void Run(ProcessEx process, ProcessPriorityClass prc)
    {
      if (process == null)
        throw new ArgumentNullException("process can not be null");

      var core = this.GetFirstFreeCore();
      core.Run(process, prc);
    }
    private Core GetFirstFreeCore()
    {
      foreach (var core in this._cores)
      {
        if (!core.Status.InUse && core.Status.Enabled)
          return core;
      }

      throw new InvalidOperationException("No free slot is available");
    }
    public bool Kill(ProcessEx process)
    {
      if (process == null)
        throw new ArgumentNullException("process can not be null");

      foreach (var core in this._cores)
      {
        if (core.Process == process)
        {
          core.Kill();
          return true;
        }
      }

      return false;
    }
    public void killAll()
    {
      foreach (var core in this._cores)
      {
        core.Kill();
      }
    }
    public void Kill(int index)
    {
      if (index < 0 || index >= this._cores.Count)
        throw new ArgumentException("Index number can not be negative or bigger than available slot count");

      this._cores[index].Kill();
    }
    public List<CoreStatus> GetProcessorUsage()
    {
      var list = new List<CoreStatus>(Environment.ProcessorCount);
      foreach (var core in this._cores)
      {
        list.Add(core.Status);
      }

      return list;
    }
  }
}
