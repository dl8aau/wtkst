namespace wtKST
{
    public class PlaneInfo
    {
        public string Call;

        public string Category;

        public int IntQRB;

        public int Potential;

        public int Mins;

        public PlaneInfo(string call, string category, int intqrb, int potential, int mins)
        {
            Call = call;
            IntQRB = intqrb;
            Mins = mins;
            Potential = potential;
            Category = category;
        }
    }
}
