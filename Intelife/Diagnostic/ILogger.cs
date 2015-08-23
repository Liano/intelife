using System;

namespace Intelife.Diagnostic
{
  public enum LogLevel
  {
    Trace,
    Debug,
    Info,
    Warning,
    Error,
    Fatal
  }

  public interface ILogger
  {
    /// <summary>
    /// Log message
    /// </summary>
    /// <param name="level">message level</param>
    /// <param name="message">message to log</param>
    void Log(LogLevel level, string message);

    /// <summary>
    /// Log formatted message
    /// </summary>
    /// <param name="level">message level</param>
    /// <param name="message">message to log</param>
    /// <param name="parameters">parameters of formatted message</param>
    void Log(LogLevel level, string message, params object[] parameters);

    /// <summary>
    /// Log an exception
    /// </summary>
    /// <param name="level">Exception level</param>
    /// <param name="message">message to log</param>
    /// <param name="exception">Exception to log</param>
    void Log(LogLevel level, string message, Exception exception);
  }
}
