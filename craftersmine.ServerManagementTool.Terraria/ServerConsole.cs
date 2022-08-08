using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Wpf.Ui.Common;

namespace craftersmine.ServerManagementTool.Terraria
{
    public sealed class ServerConsole
    {
        private const string ChatPattern = "<(.*)> (.*)";

        private const string ExceptionPattern = "(.*)Exception: (.*)";
        private const string StacktracePattern = "at (.*)";

        private const string KickedOrBannedPattern = "(.*):(.*) was booted: (.*)";

        private bool _hasException;
        private readonly object _lock;
        private ObservableCollection<ConsoleEntry> _entries;

        public event EventHandler EntryAdded;

        public ObservableCollection<ConsoleEntry> ConsoleEntries
        {
            get => _entries;
            // ReSharper disable once PropertyCanBeMadeInitOnly.Local
            private set
            {
                _entries = value;
                BindingOperations.EnableCollectionSynchronization(_entries, _lock);
            } }


#pragma warning disable CS8618
        public ServerConsole()
#pragma warning restore CS8618
        {
            _lock = new object();
            ConsoleEntries = new ObservableCollection<ConsoleEntry>();
        }

        public void Add(string content)
        {
            if (content.StartsWith(": "))
                content = content.Substring(2, content.Length - 2);

            if (isChatMessage(content))
            {
                ConsoleEntries.Add(new ConsoleEntry(content, ConsoleEntrySeverity.Chat));
                return;
            }

            var severity = getSeverity(content);

            ConsoleEntries.Add(new ConsoleEntry(content, severity));
            EntryAdded?.Invoke(this, EventArgs.Empty);
        }

        private ConsoleEntrySeverity getSeverity(string content)
        {
            if (Regex.IsMatch(content, ExceptionPattern))
            {
                _hasException = true;
                return ConsoleEntrySeverity.Error;
            }
            if (_hasException && Regex.IsMatch(content, StacktracePattern))
                return ConsoleEntrySeverity.Error;
            if (Regex.IsMatch(content, KickedOrBannedPattern))
                return ConsoleEntrySeverity.Warning;

            return ConsoleEntrySeverity.Info;
        }

        private bool isChatMessage(string content)
        {
            return Regex.IsMatch(content, ChatPattern);
        }

        public void Clear()
        {
            ConsoleEntries.Clear();
        }
    }

    public struct ConsoleEntry
    {
        public DateTime Occurred { get; set; }
        public string Content { get; set; }
        public ConsoleEntrySeverity Severity { get; set; }

        public SymbolRegular Icon
        {
            get
            {
                switch (Severity)
                {
                    case ConsoleEntrySeverity.Info:
                        return SymbolRegular.Info16;
                    case ConsoleEntrySeverity.Warning:
                        return SymbolRegular.Warning16;
                    case ConsoleEntrySeverity.Error:
                        return SymbolRegular.ErrorCircle16;
                    case ConsoleEntrySeverity.Crash:
                        return SymbolRegular.ErrorCircle16;
                    case ConsoleEntrySeverity.Chat:
                        return SymbolRegular.Chat16;
                }

                return SymbolRegular.Info16;
            }
        }

        public ConsoleEntry(string content, ConsoleEntrySeverity severity)
        {
            Occurred = DateTime.Now;
            Content = content;
            Severity = severity;
        }
    }

    public enum ConsoleEntrySeverity
    {
        Chat,
        Info,
        Warning,
        Error,
        Crash
    }
}
