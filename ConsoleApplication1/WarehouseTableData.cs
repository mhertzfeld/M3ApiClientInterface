using MyClassLibrary;
using System;


namespace ConsoleApplication1
{
    public class WarehouseTableData
        : MyClassLibrary.Data.DataObjectBase
    {
        //FIELDS
        protected Boolean? exclude;

        protected String filterField;

        protected Int32 fromWarehouseId;

        protected Int32 toWarehouseId;

        protected String warehouseTableId;

        
        //PROPERTIES
        public virtual Boolean? Exclude
        {
            get { return exclude; }

            set
            {
                if (value == default(Boolean?))
                { throw new PropertySetToDefaultException("Exclude"); }

                exclude = value;
            }
        }

        public virtual String FilterField
        {
            get { return filterField; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("FilterField"); }

                filterField = value;
            }
        }

        public virtual Int32 FromWarehouseId
        {
            get { return fromWarehouseId; }

            set
            {
                if (value < 1)
                { throw new PropertySetToOutOfRangeValueException("FromWarehouseId"); }

                fromWarehouseId = value;
            }
        }

        public virtual Int32 ToWarehouseId
        {
            get { return toWarehouseId; }

            set
            {
                if (value < 1)
                { throw new PropertySetToOutOfRangeValueException("ToWarehouseId"); }

                toWarehouseId = value;
            }
        }
        
        public virtual String WarehouseTableId
        {
            get { return warehouseTableId; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("WarehouseTableId"); }

                warehouseTableId = value;
            }
        }


        //INITIALIZE
        public WarehouseTableData()
        {
            exclude = null;

            filterField = null;

            fromWarehouseId = 0;

            toWarehouseId = 0;

            warehouseTableId = null;
        }
    }
}