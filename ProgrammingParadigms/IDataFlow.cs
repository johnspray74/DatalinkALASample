﻿using System.Collections.Generic;
using Libraries;

namespace ProgrammingParadigms
{
    /// <summary>
    /// A dataflow with a single scaler value with a primitive data type and a "OnChanged' event.
    /// OR think of it as an event with data, the receivers are able to read the data at any time.
    /// Or think of it as an implementation of a global variable and an observer pattern, with access to the variable and observer pattern restricted to the line connections on the diagram.
    /// Unidirectional - every line is one direction implying sender(s) and receiver(s).
    /// You can have multiple senders and receivers.
    /// The data is stored in the wire so receivers that don't act on the event can read its value at any time. Receivers cant change the data or send the event. 
    /// </summary>
    /// <typeparam name="T">Generic data type</typeparam>
    public interface IDataFlow<T>
    {
        T Data { set; }
    }

    public delegate void DataChangedDelegate();

    /// <summary>
    /// A reversed IDataFlow, the IDataFlow pushes data to the destination whereas IDataFlow_B pulls data from source.
    /// However, the DataChanged event will notify the destination when change happens.
    /// </summary>
    /// <typeparam name="T">Generic data type</typeparam>
    public interface IDataFlow_B<T>
    {
        T Data { get; }
        event DataChangedDelegate DataChanged;
    }

    /// <summary>
    /// Acts as a pipe to fan out incoming by creating a list and assign the data to the element in the list.
    /// Moreover, any IDataFlow and IDataFlow_B (listens for when data changed within this DataFlowConnector or acts as an IDataFlow<B> adapter) can be transferred bidirectionally.
    /// ----------------------------------------------------------------------------------------------
    /// Ports:
    /// 1. IDataFlow<T> IDataFlow<T>: incoming data which wants to be fanned out
    /// 2. IDataFlow_B<T> IDataFlow_B<T>: the 'b port' which listens for when data changed within this DataFlowConnector or acts as an IDataFlow<B> adapter. Note: this happens after the fanoutList
    /// 3. List<IDataFlow<T>> fanoutList: output port that fans out to every abstraction connected in order of wiring
    /// 4. IDataFlow<T> last: output port that will output after the fanoutList and IDataFlow_B data changed event. This enables chaining of data flow and explicit execution of last to push the data flow.
    /// </summary>
    /// <typeparam name="T">Generic data type</typeparam>
    public class DataFlowConnector<T> : IDataFlow<T>, IDataFlow_B<T> // IDataFlow<T>, IDataFlow_B<T>
    {
        // properties
        public string InstanceName = "Default";

        // ports
        private List<IDataFlow<T>> fanoutList = new List<IDataFlow<T>>();
        private IDataFlow<T> last;

        /// <summary>
        /// Fans out a data flow to mutiple data flows, or connect IDataFlow and IDataFlow_B
        /// </summary>
        public DataFlowConnector() { }

        /// <summary>
        /// Handles data provided to the connector.
        /// </summary>
        /// <param name="value">The data provided.</param>
        private void HandleData(T value)
        {
            data = value;
            foreach (var f in fanoutList) f.Data = value;
            DataChanged?.Invoke();
            if(last != null) last.Data = value;
        }

        // IDataFlow<T> implementation ---------------------------------
        private T data = default;

        T IDataFlow<T>.Data { set => HandleData(value);  }

        // IDataFlow_B<T> implementation ---------------------------------
        public event DataChangedDelegate DataChanged;
        T IDataFlow_B<T>.Data { get => data; }
    }
}
