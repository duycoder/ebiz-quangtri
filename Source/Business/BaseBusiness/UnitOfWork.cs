using Model.Entities;
using Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BaseBusiness
{
    public class UnitOfWork : IDisposable
    {
        private readonly DBEntities context;
        private bool isDisposed { get; set; }
        private Dictionary<string, object> repositories;
        public UnitOfWork(DBEntities context)
        {
            this.context = context;
        }

        public UnitOfWork()
        {
            this.context = new DBEntities();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool p)
        {
            if (!isDisposed)
            {
                if (p)
                {
                    context.Dispose();
                }
            }
            isDisposed = true;
        }
        public BaseRepository<T> Get<T>() where T : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<string, object>();
            }

            var type = typeof(T).Name;


            if (!repositories.ContainsKey(type))
            {
                var repositoryType = typeof(BaseRepository<>);
                //Tao mot thuc the repository tuong ung voi kieu T truyen vao
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), context);
                repositories.Add(type, repositoryInstance);
            }
            return (BaseRepository<T>)repositories[type];

        }

    }
}
