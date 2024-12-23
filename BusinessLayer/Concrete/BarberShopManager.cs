using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class BarberShopManager : IBarberShopService
    {
        IBarberShopDal _barberShopDal;

        public BarberShopManager(IBarberShopDal barberShopDal)
        {
            _barberShopDal = barberShopDal;
        }

        public List<BarberShop> GetBarberShopById(int id)
        {
            return _barberShopDal.GetListAll(x => x.Id == id);
        }
        public BarberShop GetById(int id)
        {
            return _barberShopDal.GetById(id);
        }

        public List<BarberShop> GetList()
        {
            return _barberShopDal.GetListAll();
        }

        public void TAdd(BarberShop t)
        {
            _barberShopDal.Insert(t);
        }

        public void TDelete(BarberShop t)
        {
            _barberShopDal.Delete(t);
        }

        public void TUpdate(BarberShop t)
        {
            _barberShopDal.Update(t);
        }

        public BarberShop GetBarberShop(int barberShopId)
        {
            using (var context = new Context())
            {
                return context.BarberShops
                              .Include(bs => bs.WorkingHours) // Çalışma saatlerini dahil et
                              .FirstOrDefault(bs => bs.Id == barberShopId);
            }
        }
    }
}
