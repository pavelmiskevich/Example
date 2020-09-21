using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vetis.Classes.Service;

namespace Vetis.Classes
{
    public abstract class baseClass
    {
        public SqlParameter par;
        private int _id;
        /// <summary>
        /// Числовой идентификатор
        /// </summary>
        public int Id { get { return _id; } set { _id = value; } }
        private string _tableName;

        /// <summary>
        /// Название таблицы сущности
        /// </summary>
        public string TableName { get { return _tableName; } set { _tableName = value; } }
        private DateTime _creDate;
        /// <summary>
        /// Дата добавления
        /// </summary>
        public DateTime CreDate { get { return _creDate; } set { _creDate = value; } }
        private bool _isActive;
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get { return _isActive; } set { _isActive = value; } }


        //public abstract object(SqlDataReader reader);
        //TODO: перенести общий метод для сущностей с полем Name
        
        //public abstract void Update();
        //public abstract void GetByName();
        //public abstract List<object> SelectAll();
        //public abstract void Search();

        /// <summary>
        /// Удаление объектов из БД по Id
        /// </summary>
        //TODO: придумать как вынести метод в базовый класс
        public virtual void DeleteFromByIdOrApplicationId()
        {
            ServiceDB.DeleteFromById(_tableName, _id);
        }
        /// <summary>
        /// Удаление всех объектов из БД
        /// </summary>
        //TODO: придумать как вынести метод в базовый класс
        public virtual void DeleteFrom()
        {
            ServiceDB.DeleteFrom(_tableName);
        }
    }
}
