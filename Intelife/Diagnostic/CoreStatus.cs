using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelife.Diagnostic
{
  public class CoreStatus
  {
    public int Index { get; internal set; }
    public bool InUse { get; set; }
    public bool Enabled { get; set; }
    public string Command { get; internal set; }
    public CoreStatus(int index, bool enabled)
    {
      this.Enabled = enabled;
      this.Index = index;
      this.InUse = false;
    }
  }
}
