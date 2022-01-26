using libicma.utils;

namespace icma
{
    public abstract class Model : IModel
    {
        protected  string id;
        public Model()
        {
            id = Utils.RandomFileName(false);
        }
       
       
        public string ID => id;

        public bool IsValid => false==string.IsNullOrEmpty(ID);

        public virtual void Clone(object value)
        {

        }
    }
}
