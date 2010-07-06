using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using XenoGears.Functional;
using XenoGears.Traits.Disposable;

namespace Libcuda.Run
{
    [DebuggerNonUserCode]
    public class KernelArguments : Disposable, IList<KernelArgument>
    {
        private readonly List<KernelArgument> _impl = new List<KernelArgument>();
        public KernelArguments(params KernelArgument[] args) { _impl = new List<KernelArgument>(args ?? Seq.Empty<KernelArgument>()); }
        public KernelArguments(IEnumerable<KernelArgument> args) { _impl = new List<KernelArgument>(args ?? Seq.Empty<KernelArgument>()); }

        #region Boilerplate implementation of IList<KernelArgument>

        public int IndexOf(KernelArgument item) { return _impl.IndexOf(item); }
        public void Insert(int index, KernelArgument item) { _impl.Insert(index, item); }
        public void RemoveAt(int index) { _impl.RemoveAt(index); }
        public KernelArgument this[int index] { get { return _impl[index]; } set { _impl[index] = value; } }
        public void Add(KernelArgument item) { _impl.Add(item); }
        public void Clear() { _impl.Clear(); }
        public bool Contains(KernelArgument item) { return _impl.Contains(item); }
        public void CopyTo(KernelArgument[] array, int arrayIndex) { _impl.CopyTo(array, arrayIndex); }
        public bool Remove(KernelArgument item) { return _impl.Remove(item); }
        public int Count { get { return _impl.Count; } }
        bool ICollection<KernelArgument>.IsReadOnly { get { return false; } }
        public IEnumerator<KernelArgument> GetEnumerator() { return _impl.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion

        protected override void DisposeManagedResources()
        {
            _impl.ForEach(p => p.Dispose());
        }
    }
}