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
    KeyValuePair<ProcessEx, ProcessPriorityClass> _currentProcess;

    public ProcessManager()
    {
      this._linkedProcessQueue = new Queue<LinkedList<KeyValuePair<ProcessEx, ProcessPriorityClass>>>();
      this._processQueue = new Queue<KeyValuePair<ProcessEx, ProcessPriorityClass>>();

      Processor.O.SlotFreed += Processor_SlotFreed;
    }

    private void Processor_SlotFreed(int freeSlots, ProcessEx finishedProcess)
    {
      this.LaunchQueue();
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
        process.Status = ProcessExecutionStatus.Queued;
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
        foreach (var item in linkedProcess)
        {
          item.Key.Status = ProcessExecutionStatus.Queued;
        }
      }
    }
    public void Start()
    {

    }
    private void LaunchQueue()
    {
      //LinkedListNode<KeyValuePair<ProcessEx, ProcessPriorityClass>> linkedListNode = null;

      try
      {
        lock (this)
        {
          LaunchLinkedProcess();
          LaunchQueuedProcess(); 
        }
      }
      catch (InvalidOperationException) // no processing slot available
      {
        // exit the process execution queuing loop
        return;
      }
      catch (IndexOutOfRangeException)
      {
        //no more process queue set current process empty
        this._currentProcess = new KeyValuePair<ProcessEx, ProcessPriorityClass>(null, ProcessPriorityClass.Normal);

        //exit from process execution queuing loop
        return;
      }
      catch (Exception)
      {

        throw;
      }
    }
    private void LaunchQueuedProcess()
    {
      //no currently process is waiting, get one from queue.
      if (this._currentProcess.Key == null)
        this._currentProcess = this.DequeueProcess();

      //keep running new processes
      while (true)
      {
        //when there is no more slot the following will throw an InvalidOperationException
        Processor.O.Run(this._currentProcess.Key, this._currentProcess.Value);
        this._currentProcess.Key.Status = ProcessExecutionStatus.Running;

        //get next waiting process
        this._currentProcess = this.DequeueProcess();
      }
    }
    private void LaunchLinkedProcess()
    {
      LinkedList<KeyValuePair<ProcessEx, ProcessPriorityClass>> linkedProcesses = null;
      try
      {
        while (true)
        {
          //no current node no linked list
          if (this._currentNode == null)
          {
            if (linkedProcesses == null)
              linkedProcesses = this.DequeueChain();

            this._currentNode = linkedProcesses.First;
          }

          //first item is null
          if (this._currentNode == null)
          {
            linkedProcesses = null;

            //loop again to get next linked processes if available
            continue;
          }

          switch (this._currentNode.Value.Key.Status)
          {
            case ProcessExecutionStatus.Completed://get next linked process if current is completed
              this._currentNode = this._currentNode.Next; //next node might be null
              goto case ProcessExecutionStatus.Queued;//
            case ProcessExecutionStatus.Queued:
              if (this._currentNode != null)
              {
                Processor.O.Run(this._currentNode.Value.Key, this._currentNode.Value.Value);
                this._currentNode.Value.Key.Status = ProcessExecutionStatus.Running;
              }
              else //beyond last linked processes item
              {
                linkedProcesses = null;

                //loop again to get next linked processes if available
                continue;
              }
              break;
            case ProcessExecutionStatus.Created:
              throw new InvalidOperationException("Invalid process status \"Created\"");
          }

          break;
        }

      }
      catch (InvalidOperationException) //no processing slot available
      {

      }
      catch (IndexOutOfRangeException) //no more linked processes in the queue.
      {
        linkedProcesses = null;
      }
      catch (Exception)
      {

        throw;
      }
    }
    private KeyValuePair<ProcessEx, ProcessPriorityClass> DequeueProcess()
    {
      lock (this._queueLock)
      {
        if (this._processQueue.Count > 0)
          return this._processQueue.Dequeue();
        else
          throw new IndexOutOfRangeException("No more items in the process queue");
      }
    }
    private LinkedList<KeyValuePair<ProcessEx, ProcessPriorityClass>> DequeueChain()
    {
      lock (this._linkedQueueLock)
      {
        if (this._linkedProcessQueue.Count > 0)
          return this._linkedProcessQueue.Dequeue();
        else
          throw new IndexOutOfRangeException("No more items in linked processes queue");
      }
    }
  }
}