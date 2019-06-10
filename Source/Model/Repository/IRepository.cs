using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repository
{
    public interface IRepository<T>
    {
        IQueryable<T> All();
        void Insert(T item);

        void Update(T item);

        void Delete(int id);

        void DeleteRange(List<T> lstItem);

        T Find(object id);

        void Save();

        void ExecuteSQL(string Sql);
    }
}
