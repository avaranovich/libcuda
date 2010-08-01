using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Libcuda.Api.DataTypes;
using XenoGears.Assertions;
using System.Linq;
using XenoGears.Functional;

namespace Libcuda.Api.Run
{
    [DebuggerNonUserCode]
    public class KernelResult : IDictionary<int, Object>
    {
        private readonly KernelInvocation _invocation;
        public ElapsedTime WallTime { get; private set; }

        internal KernelResult(KernelInvocation invocation)
        {
            _invocation = invocation;
        }

        internal KernelResult(KernelInvocation invocation, ElapsedTime wallTime)
        {
            _invocation = invocation;
            WallTime = wallTime;
        }

        #region Trivial implementation of mutating operations: just throw since we're readonly

        void ICollection<KeyValuePair<int, Object>>.Add(KeyValuePair<int, Object> item)
        {
            throw new NotSupportedException(String.Format(
                "Cannot add item '{0}' to the dictionary: dictionary is read-only", item));
        }

        void ICollection<KeyValuePair<int, Object>>.Clear()
        {
            throw new NotSupportedException(String.Format(
                "Cannot clear the dictionary: dictionary is read-only"));
        }

        bool ICollection<KeyValuePair<int, Object>>.Remove(KeyValuePair<int, Object> item)
        {
            throw new NotSupportedException(String.Format(
                "Cannot remove item '{0}' from the dictionary: dictionary is read-only", item));
        }

        void IDictionary<int, Object>.Add(int key, Object value)
        {
            throw new NotSupportedException(String.Format(
                "Cannot add item '{0}' to the dictionary: dictionary is read-only",
                new KeyValuePair<int, Object>(key, value)));
        }

        bool IDictionary<int, Object>.Remove(int key)
        {
            throw new NotSupportedException(String.Format(
                "Cannot remove item with key '{0}' from the dictionary: dictionary is read-only", key));
        }

        bool ICollection<KeyValuePair<int, Object>>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Trivial implementation of Keys, ContainsKey and Count: just use the 0..ParamCount range

        public int Count
        {
            get
            {
                return _invocation.Args.Count;
            }
        }

        public ICollection<int> Keys
        {
            get
            {
                return 0.UpTo(Count - 1).ToList();
            }
        }

        public bool ContainsKey(int key)
        {
            return 0 <= key && key < Count;
        }

        #endregion

        #region Trivial implementation of [], Values, Enumerators, Contains and CopyTo: just use TryGetValue

        public Object this[int key]
        {
            get
            {
                Object value;
                TryGetValue(key, out value).AssertTrue();
                return value;
            }
            set
            {
                throw new NotSupportedException(String.Format(
                    "Cannot add item '{0}' to the dictionary: dictionary is read-only",
                    new KeyValuePair<int, Object>(key, value)));
            }
        }

        public ICollection<Object> Values
        {
            get
            {
                return Keys.Select(key => this[key]).ToReadOnly();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<KeyValuePair<int, Object>> GetEnumerator()
        {
            // note. this preserves laziness
            return Keys.Select(key => new KeyValuePair<int, Object>(key, this[key])).GetEnumerator();
        }

        public bool Contains(KeyValuePair<int, Object> item)
        {
            return ((IEnumerable<KeyValuePair<int, Object>>)this).Contains(item);
        }

        public void CopyTo(KeyValuePair<int, Object>[] array, int arrayIndex)
        {
            this.ToList().CopyTo(array, arrayIndex);
        }

        #endregion

        public Object Result
        {
            get
            {
                var singleOut = _invocation.Args.AssertSingle(p => 
                    p.Direction == ParameterDirection.Out || p.Direction == ParameterDirection.InOut);
                return singleOut.Result;
            }
        }

        public bool TryGetValue(int key, out Object value)
        {
            if (!ContainsKey(key))
            {
                value = null;
                return false;
            }
            else
            {
                value = _invocation.Args.Nth(key).Result;
                return true;
            }
        }
    }
}