using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Intelife.Diagnostic
{
  public enum ProcessExecutionStatus
  {
    Running,
    Queued,
    Completed,
    Created
  }
  public class ProcessEx : Process
  {
    public string Name { get; set; }
    internal Core ExecutingCore { get; set; }
    public ProcessExecutionStatus Status { get; internal set; }
    public ProcessEx()
      : this(string.Empty)
    {
    }
    public ProcessEx(string name)
      : base()
    {
      this.Status = ProcessExecutionStatus.Created;
      this.Name = name;
    }
  }
}
