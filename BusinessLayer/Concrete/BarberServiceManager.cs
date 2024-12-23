using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class BarberServiceManager : IBarberServiceService
    {
        IBarberServiceDal _barberServiceDal;

        public BarberServiceManager(IBarberServiceDal barberServiceDal)
        {
            _barberServiceDal = barberServiceDal;
        }

        public List<BarberService> GetBarberServiceById(int id)
        {
            return _barberServiceDal.GetListAll(x => x.Id == id);
        }
        public BarberService GetById(int id)
        {
            return _barberServiceDal.GetById(id);
        }

        public List<BarberService> GetList()
        {
            return _barberServiceDal.GetListAll();
        }

        public List<BarberService> GetList(int id)
        {
            using (var context = new Context()) // YourDbContext yerine kendi DbContext adınızı yazın
            {
                return context.BarberServices
                              .Include(bs => bs.Operation) // Operation ile ilişkiyi dahil et
                              .Where(bs => bs.Id == id)
                              .ToList();
            }
        }

        public List<BarberService> GetServicesByBarberShopId(int barberShopId)
        {
            using (var context = new Context())
            {
                return context.BarberServices
                              .Where(service => service.BarberShopId == barberShopId)
                              .ToList();
            }
        }
        public void TAdd(BarberService t)
        {
            _barberServiceDal.Insert(t);
        }

        public void TDelete(BarberService t)
        {
            _barberServiceDal.Delete(t);
        }

        public void TUpdate(BarberService t)
        {
            _barberServiceDal.Update(t);
        }
    }
}
