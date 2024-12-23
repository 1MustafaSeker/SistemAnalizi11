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
    public class OperationManager : IOperationService
    {
        IOperationDal _operationDal;

        public OperationManager(IOperationDal operationDal)
        {
            _operationDal = operationDal;
        }

        public List<Operation> GetOperationById(int id)
        {
            return _operationDal.GetListAll(x => x.Id == id);
        }
        public Operation GetById(int id)
        {
            return _operationDal.GetById(id);
        }

        public List<Operation> GetList()
        {
            return _operationDal.GetListAll();
        }

        public void TAdd(Operation t)
        {
            _operationDal.Insert(t);
        }

        public void TDelete(Operation t)
        {
            _operationDal.Delete(t);
        }

        public void TUpdate(Operation t)
        {
            _operationDal.Update(t);
        }
    }
}
