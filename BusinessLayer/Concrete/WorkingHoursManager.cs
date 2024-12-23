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
    public class WorkingHoursManager : IWorkingHoursService
    {
        IWorkingHoursDal _workingHoursDal;

        public WorkingHoursManager(IWorkingHoursDal workingHoursDal)
        {
            _workingHoursDal = workingHoursDal;
        }

        public List<WorkingHours> GetWorkingHoursById(int id)
        {
            return _workingHoursDal.GetListAll(x => x.Id == id);
        }
        public WorkingHours GetById(int id)
        {
            return _workingHoursDal.GetById(id);
        }

        public List<WorkingHours> GetList()
        {
            return _workingHoursDal.GetListAll();
        }

        public List<WorkingHours> GetList(int id)
        {
            return _workingHoursDal.GetListAll(x => x.Id == id);
        }

        public void TAdd(WorkingHours t)
        {
            _workingHoursDal.Insert(t);
        }

        public void TDelete(WorkingHours t)
        {
            _workingHoursDal.Delete(t);
        }

        public void TUpdate(WorkingHours t)
        {
            _workingHoursDal.Update(t);
        }
    }
}
