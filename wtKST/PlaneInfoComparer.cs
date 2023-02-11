using System.Collections.Generic;

namespace wtKST
{
    public class PlaneInfoComparer : IComparer<PlaneInfo>
    {
        int IComparer<PlaneInfo>.Compare(PlaneInfo x, PlaneInfo y)
        {
            int xpot = x.Potential;
            int ypot = y.Potential;
            int xqrb = x.IntQRB;
            int yqrb = y.IntQRB;
            int result;
            if (xpot > ypot)
            {
                result = -1;
            }
            else if (xpot < ypot)
            {
                result = 1;
            }
            else if (xqrb > yqrb)
            {
                result = 1;
            }
            else if (xqrb < yqrb)
            {
                result = -1;
            }
            else
            {
                result = 1;
            }
            return result;
        }
    }
}
