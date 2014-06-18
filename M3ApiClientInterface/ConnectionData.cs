using System;


namespace M3ApiClientInterface
{
    public class ConnectionData
    {
        //FIELDS
        protected String password;

        protected Int32 port;

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

        public Int32 Port
        {
            get { return port; }

            set
            {
                if (value < 1)
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

        public ConnectionData(String Server, Int32 Port, String UserName, String Password)
        {
            this.Password = Password;

            this.Port = Port;

            this.Server = Server;

            this.UserName = UserName;
        }
    }
}