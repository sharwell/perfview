using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EventSources
{
    /// <summary>
    /// An EventRecord is a abstraction that is returned by the EventSource.Events API.   It represents
    /// a single event and is everything the GUI needs to display the event in the GUI.   
    /// </summary>
    public abstract class EventRecord
    {
        public abstract string EventName { get; }
        public abstract string ProcessName { get; }
        public abstract double TimeStampRelatveMSec { get; }

        // TODO FIX NOW should be abstract, get CSV and ETW subclasses to implement
        /// <summary>
        /// The names of the fields in this record
        /// </summary>
        public virtual string[] FieldNames { get { return null; } }
        // TODO FIX NOW should be abstract, get CSV and ETW subclasses to implement
        /// <summary>
        /// This fetches fields from the record.  The index corresponds to the FieldNames array.
        /// </summary>
        public virtual string Field(int index) { return null; }

        /// <summary>
        /// The current contract is that the array is undefined above the 'non-rest' fields (currently we have a max of 4).  
        /// </summary>
        public string[] DisplayFields { get { return m_displayFields; } }
        //  TODO FIX NOW should not be virtual.  
        /// <summary>
        /// Displays fields as key-value pairs.  
        /// </summary>
        public virtual string Rest { get { return m_displayFields[5]; } set { m_displayFields[5] = value; } }
        // The properties are for binding in the GUI.   
        // set property is a hack to allow selection in the GUI (which wants two way binding for that case)
        public string DisplayField1 { get { return m_displayFields[0]; } set { } }
        public string DisplayField2 { get { return m_displayFields[1]; } set { } }
        public string DisplayField3 { get { return m_displayFields[2]; } set { } }
        public string DisplayField4 { get { return m_displayFields[3]; } set { } }

        // returns true of 'pattern' matches the display fields.  
        public virtual bool Matches(Regex pattern)
        {
            // TODO FIX NOW NOT DONE 
            return true;
        }

        #region private 

        protected void SetDisplayFields(EventSource source)
        {
            if (m_displayFields == null)
            {
                m_displayFields = new string[5];
            }

            Debug.Assert(m_displayFields.Length == 5);
            // TODO FIX NOW NOT DONE 
        }

        /// <summary>
        /// m_displayFields are the fields ordered by how the EventSource.ColumnsToDisplay says they should be ordered.
        /// m_displayField[5] is currently the Rest field.  
        /// </summary>
        protected internal string[] m_displayFields;
        protected EventRecord(int numNonRestFields)
        {
            Debug.Assert(numNonRestFields >= 4 || numNonRestFields == 0);
            m_displayFields = new string[numNonRestFields];
        }
        protected EventRecord() { }
        #endregion
    }
}