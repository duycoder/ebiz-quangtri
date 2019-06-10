using Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Business.BaseBusiness
{
    public class BaseBusiness<T> where T : class
    {
        private readonly UnitOfWork UnitOfWork;
        public BaseRepository<T> repository;
        public DBEntities context;
        public BaseBusiness(UnitOfWork unitOfWork)
        {
            //UnitOfWork = unitOfWork;
            repository = unitOfWork.Get<T>();
            context = repository.Context;
            
        }
        public BaseBusiness()
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            repository = unitOfWork.Get<T>();
        }
        public void Save()
        {
            repository.Save();
        }

        public T Find(object id)
        {
            return repository.Find(id);
        }

        public void Save(T item)
        {
            try
            {
                var type = item.GetType();
                var property = type.GetProperty("ID");
                if (property!=null)
                {
                    var getvalue = property.GetValue(item).ToString();
                    if (getvalue == "0")
                    {
                        this.repository.Insert(item);
                    }
                    else
                    {
                        this.repository.Update(item);
                    }
                }
                else
                {
                    throw new Exception("Không tồn tại ID");
                }
                
                this.repository.Save();
            }
            catch (Exception ex)
            {
                //LogHelper.Error(string.Format("UserService.Save: {0}", ex.Message));
                throw new Exception(ex.Message);
            }
        }
    }
}
