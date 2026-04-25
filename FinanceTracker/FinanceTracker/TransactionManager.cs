using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace FinanceTracker
{
    internal class TransactionManager
    {
        private List<Transaction> _transactions=new();
        private int _nextId=1;

        public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

        public void Add(Transaction t)
        {
            t.Id=_nextId++;
            _transactions.Add(t);
        }
        public void Remove(int Id) {
            _transactions.RemoveAll(t => t.Id==Id);
        }
        public Transaction? GetById(int Id)
        {
            return _transactions.FirstOrDefault(t=>t.Id==Id);
        }
        public void Load(List<Transaction> data)
        {
            _transactions= data;
            _nextId=data.Any()? data.Max(t=>t.Id)+1 : 1;
        }
    }
}
