namespace WebApplication1.Services
{
    public class GroupOrders 
    {
        public int group100;
        public int group99;
        public int group98;

        public GroupOrders()
        {
            this.group100 = 0;
            this.group99 = 0;
            this.group98 = 0;
        }

        public int Total() 
        {
            return this.group98 * 98 + this.group99 * 99 + this.group100 * 100;
        }
    }
}
