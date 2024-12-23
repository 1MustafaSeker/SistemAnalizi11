using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class TransactionManager : ITransactionService
    {
        ITransactionDal _transactionDal;

        public TransactionManager(ITransactionDal transactionDal)
        {
            _transactionDal = transactionDal;
        }

        public List<Transaction> GetTransactionById(int id)
        {
            return _transactionDal.GetListAll(x => x.Id == id);
        }
        public Transaction GetById(int id)
        {
            return _transactionDal.GetById(id);
        }

        public List<Transaction> GetList()
        {
            return _transactionDal.GetListAll();
        }

        public void TAdd(Transaction t)
        {
            _transactionDal.Insert(t);
        }

        public void TDelete(Transaction t)
        {
            _transactionDal.Delete(t);
        }

        public void TUpdate(Transaction t)
        {
            _transactionDal.Update(t);
        }
    }
}
