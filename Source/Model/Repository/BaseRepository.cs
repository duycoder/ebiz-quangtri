using Model.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repository
{
    public class BaseRepository<T> where T : class
    {
        public DBEntities Context;
        private IDbSet<T> entities;
        public string messageError { get; set; }
        public BaseRepository(DBEntities context)
        {
            this.Context = context;
            entities = Context.Set<T>();
        }
        public virtual IEnumerable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }


        public IDbSet<T> Entities
        {
            get
            {
                if (entities == null)
                {
                    entities = Context.Set<T>();
                }
                return entities;
            }
        }
        public IQueryable<T> All()
        {
            return this.Entities as IQueryable<T>;
        }

        public void Insert(T item)
        {
            messageError = string.Empty;
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException("item");
                }
                this.Entities.Add(item);
                this.Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var EntityValidError in ex.EntityValidationErrors)
                {
                    foreach (var error in EntityValidError.ValidationErrors)
                    {
                        messageError += "\n" + string.Format("Property: {0} - Error: {1}", error.PropertyName, error.ErrorMessage);
                    }
                }

            }
        }

        public void InsertRange(IEnumerable<T> items)
        {
            this.Context.Set<T>().AddRange(items);
        }

        public void ExecuteSQL(string Sql)
        {
            ((IObjectContextAdapter)Context).ObjectContext.ExecuteStoreCommand(Sql);
        }

        public void Update(T item)
        {
            messageError = string.Empty;
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.entities.Attach(item);
                Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var EntityValidError in ex.EntityValidationErrors)
                {
                    foreach (var error in EntityValidError.ValidationErrors)
                    {
                        messageError += "\n" + string.Format("Property: {0} - Error: {1}", error.PropertyName, error.ErrorMessage);
                    }
                }

            }
        }

        public void Delete(object id)
        {
            messageError = string.Empty;
            try
            {
                var item = this.Entities.Find(id);
                if (item == null)
                {
                    throw new ArgumentNullException("item");
                }
                this.Entities.Remove(item);
                this.Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var EntityValidError in ex.EntityValidationErrors)
                {
                    foreach (var error in EntityValidError.ValidationErrors)
                    {
                        messageError += "\n" + string.Format("Property: {0} - Error: {1}", error.PropertyName, error.ErrorMessage);
                    }
                }

            }
        }

        public void Delete(T entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == System.Data.Entity.EntityState.Detached)
            {
                this.entities.Attach(entityToDelete);
            }
            this.entities.Remove(entityToDelete);
            this.Context.SaveChanges();
        }

        public  void DeleteRange(List<T> lstItem)
        {
            messageError = string.Empty;
            try
            {

                if (lstItem == null || !lstItem.Any())
                {
                    throw new ArgumentNullException("item");
                }
                foreach (var item in lstItem)
                {
                    this.Entities.Remove(item);
                    this.Context.SaveChanges();    
                }
                
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var EntityValidError in ex.EntityValidationErrors)
                {
                    foreach (var error in EntityValidError.ValidationErrors)
                    {
                        messageError += "\n" + string.Format("Property: {0} - Error: {1}", error.PropertyName, error.ErrorMessage);
                    }
                }

            }
        }


        public T Find(object id)
        {
            return this.Entities.Find(id);
        }
        public IQueryable<T> AllNoTracking
        {
            get { return this.entities.AsNoTracking(); }
        }

        public void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                string error = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        error += string.Format("{0}:{1}; ", ve.PropertyName, ve.ErrorMessage);
                    }

                }
                throw new Exception(error);
            }
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
        }
    }
}
