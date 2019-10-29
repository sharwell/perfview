using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EventSources
{

    /// <summary>
    /// An EventSource is an abstraction that provides all the functionality that is needed by PerfView's Events viewer GUI.
    /// Anything that implements this interface can be viewed in the Events Viewer.
    /// 
    /// The main functionality that subclasses implement is the 'Events' enumeration.   
    /// </summary>
    public abstract class EventSource
    {
        /* ForEach is the main attraction.   Everything else simply supports this */
        /// <summary>
        /// ForEach is the most important property, calls 'callback' for each event in turn.
        /// If the callback return false, then the foreach does not deliver any more callbacks.  
        /// </summary>
        public abstract void ForEach(Func<EventRecord, bool> callback);

        // filtering.  These should be consulted by the subclasses when returning events in the 'Events' enumeration.  
        /// <summary>
        /// Filter out any events less than StartTimeRelativeMSec
        /// </summary>
        public double StartTimeRelativeMSec;
        /// <summary>
        /// Filter out any events greater than EndTimeRelativeMSec
        /// </summary>
        public double EndTimeRelativeMSec;
        /// <summary>
        /// The maximum number of events to return.    Will return an eventrecord with a null EventName after this (but a valid timestamp).  
        /// </summary>
        public int MaxRet;
        /// <summary>
        /// If set, only display events from a process that matches this regular expression
        /// </summary>
        public string ProcessFilterRegex;
        /// <summary>
        /// The list of all process names (exe names without the .EXE) in the collection (optional, null if not known)) 
        /// </summary>
        public virtual ICollection<string> ProcessNames { get { return null; } }
        /// <summary>
        /// If set, only display events that have this regular expression in their 'ToString' (this is relative expensive)
        /// </summary>
        public string TextFilterRegex;
        /// <summary>
        /// After calling this method with a list of event Names, the Events enumeration will only return
        /// records of events with one of these names.  
        /// </summary>
        public abstract void SetEventFilter(List<string> eventNames);
        /// <summary>
        /// returns the set of names for events in the collection (used to populate the left pane in the display.  
        /// </summary>
        public abstract ICollection<string> EventNames { get; }

        // selecting the columns to return. 
        /// <summary>
        /// Set the columns you wish to show up as Field1, Field2, ...
        /// Use AllColumnNames to get a valid list of names to use here.  
        /// </summary>
        public List<string> ColumnsToDisplay;
        /// <summary>
        /// Optionally you can provide a list of columns available.  
        /// </summary>
        public virtual ICollection<string> AllColumnNames(List<string> eventNames) { return null; }

        /// <summary>
        /// The number of fields that are NOT put in the 'rest' column.  Defaults to 4
        /// </summary>
        public int NonRestFields;

        /// <summary>
        /// Optionally, the implementation can provide the sum of every entry in each column specified in 'ColumnsToDisplay'. 
        /// This will be set by the implementation when the 'Events' enumeration is scanned. 
        /// </summary>
        public virtual double[] ColumnSums { get; protected set; }
        /// <summary>
        /// If the source knows a bound on the times of all the events it sets this.  Should be set to PositiveInfinity if it is unknown.
        /// </summary>
        public double MaxEventTimeRelativeMsec;

        /// <summary>
        /// Clones the EventSource, so you can have multiple windows opened on the same set of data.   
        /// </summary>
        /// <returns></returns>
        public abstract EventSource Clone();

        /// <summary>
        /// Utility function that takes a specification for columns (which can include *) and the raw list of
        /// all possible columns and returns the list that match the specification. 
        /// </summary>
        public static List<string> ParseColumns(string columnSpec, ICollection<string> columnNames)
        {
            if (string.IsNullOrWhiteSpace(columnSpec))
            {
                return null;
            }

            var ret = new List<string>();
            var regex = new Regex(@"\s*(\S+)\s*");
            var index = 0;
            for (; ; )
            {
                var match = regex.Match(columnSpec, index);
                var name = match.Groups[1].Value;
                if (name == "*")
                {
                    var startCount = ret.Count;
                    foreach (var colName in columnNames)
                    {
                        // If it was already specified, leave it out
                        for (int i = 0; i < startCount; i++)
                        {
                            if (ret[i] == colName)
                            {
                                goto Next;
                            }
                        }

                        ret.Add(colName);
                        Next:;
                    }
                }
                else
                {
                    ret.Add(name);
                }

                index += match.Groups[0].Length;
                if (index == columnSpec.Length)
                {
                    break;
                }
            }
            return ret;
        }
    }
}