using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Intelife.Diagnostic
{
  public class ProcessManager
  {
    object _linkedQueueLock = new object();
    object _queueLock = new object();
    Queue<KeyValuePair<ProcessEx, ProcessPriorityClass>> _processQueue;
    Queue<LinkedList<KeyValuePair<ProcessEx, ProcessPriorityClass>>> _linkedProcessQueue;
    LinkedListNode<KeyValuePair<ProcessEx, ProcessPriorityClass>> _currentNode;

    public ProcessManager()
    {
      this._linkedProcessQueue = new Queue<LinkedList<KeyValuePair<ProcessEx, ProcessPriorityClass>>>();
      this._processQueue = new Queue<KeyValuePair<ProcessEx, ProcessPriorityClass>>();
    }
    /// <summary>
    /// Gets or sets the maximum concurrent process.
    /// </summary>
    /// <value>
    /// The maximum concurrent process.
    /// </value>
    public int MaxConcurrentProcess
    {
      get { return Processor.O.MaxUsableCores; }
      set { Processor.O.MaxUsableCores = value; }
    }

    /// <summary>
    /// Queues the process.
    /// </summary>
    /// <param name="process">The process to enqueue.</param>
    /// <param name="prc">process priority.</param>
    public void QueueProcess(ProcessEx process, ProcessPriorityClass prc)
    {
      lock (_queueLock)
      {
        this._processQueue.Enqueue(new KeyValuePair<ProcessEx, ProcessPriorityClass>(process, prc));
      }
    }
    /// <summary>
    /// Queues linked processes.
    /// </summary>
    /// <param name="linkedProcess">The linked processes to enqueue</param>
    public void QueueLinkedProcesses(LinkedList<KeyValuePair<ProcessEx, ProcessPriorityClass>> linkedProcess)
    {
      lock (_linkedQueueLock)
      {
        this._linkedProcessQueue.Enqueue(linkedProcess);
      }
    }

    public void Start()
    {

    }
  }
}