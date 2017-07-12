using System;


namespace M3ApiClientInterface
{
    public class ConnectionData
    {
        //FIELDS
        protected String password;

        protected UInt16 port;

        protected String server;        

        protected String userName;


        //PROPERTIES
        public String Password
        {
            get { return password; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("Password"); }

                password = value;
            }
        }

        public UInt16 Port
        {
            get { return port; }

            set
            {
                if (value == 0)
                { throw new PropertySetToOutOfRangeValueException("Port"); }

                port = value;
            }
        }

        public String Server
        {
            get { return server; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("Server"); }

                server = value;
            }
        }

        public String UserName
        {
            get { return userName; }

            set
            {
                if (value == default(String))
                { throw new PropertySetToDefaultException("UserName"); }

                userName = value;
            }
        }


        //INITIALIZE
        public ConnectionData()
        {
            password = null;

            port = 0;

            server = null;

            userName = null;
        }
    }
}